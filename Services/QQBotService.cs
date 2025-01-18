using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Milimoe.QQBot.Models;

namespace Milimoe.QQBot.Services
{
    public class QQBotService(IOptions<BotConfig> botConfig, ILogger<QQBotService> logger, IHttpClientFactory httpClientFactory, IMemoryCache memoryCache)
    {
        private readonly BotConfig _botConfig = botConfig.Value;
        private readonly ILogger<QQBotService> _logger = logger;
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient();
        private readonly IMemoryCache _memoryCache = memoryCache;
        private const string AccessTokenCacheKey = "QQBotAccessToken";

        public async Task SendC2CMessageAsync(string openid, string content, int msgType = 0, object? media = null, string? msgId = null, int? msgSeq = null)
        {
            await SendMessageAsync($"/v2/users/{openid}/messages", content, msgType, media, msgId, msgSeq);
        }

        public async Task SendGroupMessageAsync(string groupOpenid, string content, int msgType = 0, object? media = null, string? msgId = null, int? msgSeq = null)
        {
            await SendMessageAsync($"/v2/groups/{groupOpenid}/messages", content, msgType, media, msgId, msgSeq);
        }

        private async Task SendMessageAsync(string url, string content, int msgType = 0, object? media = null, string? msgId = null, int? msgSeq = null)
        {
            string accessToken = await GetAccessTokenAsync();
            HttpRequestMessage request = new(HttpMethod.Post, $"https://api.sgroup.qq.com{url}");
            request.Headers.Authorization = new AuthenticationHeaderValue("QQBot", accessToken);
            _logger.LogDebug("使用的 Access Token：{accessToken}", accessToken);
            Dictionary<string, object> requestBody = new()
            {
                { "content", content },
                { "msg_type", msgType }
            };
            if (media != null)
            {
                requestBody.Add("media", media);
            }
            if (!string.IsNullOrEmpty(msgId))
            {
                requestBody.Add("msg_id", msgId);
            }
            if (msgSeq.HasValue)
            {
                requestBody.Add("msg_seq", msgSeq.Value);
            }
            request.Content = new StringContent(JsonSerializer.Serialize(requestBody), System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                string errorBody = await response.Content.ReadAsStringAsync();
                _logger.LogError("状态码：{response.StatusCode}，错误信息：{errorBody}", response.StatusCode, errorBody);
            }
        }

        public async Task<(string? fileUuid, string? fileInfo, int ttl, string? error)> UploadC2CMediaAsync(string openid, int fileType, string url)
        {
            return await UploadMediaAsync($"/v2/users/{openid}/files", fileType, url);
        }

        public async Task<(string? fileUuid, string? fileInfo, int ttl, string? error)> UploadGroupMediaAsync(string groupOpenid, int fileType, string url)
        {
            return await UploadMediaAsync($"/v2/groups/{groupOpenid}/files", fileType, url);
        }

        private async Task<(string? fileUuid, string? fileInfo, int ttl, string? error)> UploadMediaAsync(string url, int fileType, string fileUrl)
        {
            string accessToken = await GetAccessTokenAsync();
            HttpRequestMessage request = new(HttpMethod.Post, $"https://api.sgroup.qq.com{url}");
            request.Headers.Authorization = new AuthenticationHeaderValue("QQBot", accessToken);
            _logger.LogDebug("使用的 Access Token：{accessToken}", accessToken);
            Dictionary<string, object> requestBody = new()
            {
                { "file_type", fileType },
                { "url", fileUrl },
                { "srv_send_msg", false }
            };
            request.Content = new StringContent(JsonSerializer.Serialize(requestBody), System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                string errorBody = await response.Content.ReadAsStringAsync();
                return (null, null, 0, $"状态码：{response.StatusCode}，错误信息：{errorBody}");
            }
            string responseBody = await response.Content.ReadAsStringAsync();
            MediaResponse? mediaResponse = JsonSerializer.Deserialize<MediaResponse>(responseBody);
            if (mediaResponse == null)
            {
                return (null, null, 0, "反序列化富媒体消息失败。");
            }
            _logger.LogDebug("接收到的富媒体消息：{mediaResponse.FileInfo}，有效时间：{mediaResponse.Ttl}", mediaResponse.FileInfo, mediaResponse.Ttl);
            return (mediaResponse.FileUuid, mediaResponse.FileInfo, mediaResponse.Ttl, null);
        }

        public async Task<string> GetAccessTokenAsync()
        {
            if (_memoryCache.TryGetValue(AccessTokenCacheKey, out string? accessToken) && !string.IsNullOrEmpty(accessToken))
            {
                return accessToken;
            }

            return await RefreshTokenAsync();
        }

        public async Task<string> RefreshTokenAsync()
        {
            HttpRequestMessage request = new(HttpMethod.Post, "https://bots.qq.com/app/getAppAccessToken")
            {
                Content = new StringContent(JsonSerializer.Serialize(new { appId = _botConfig.AppId, clientSecret = _botConfig.Secret }), System.Text.Encoding.UTF8, "application/json")
            };
            HttpResponseMessage response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            AccessTokenResponse? tokenResponse = JsonSerializer.Deserialize<AccessTokenResponse>(responseBody);
            if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.AccessToken) || !int.TryParse(tokenResponse.ExpiresIn, out int expiresIn))
            {
                throw new Exception("获取 Access Token 失败！");
            }
            _memoryCache.Set(AccessTokenCacheKey, tokenResponse.AccessToken, TimeSpan.FromSeconds(expiresIn - 60));
            _logger.LogDebug("获取到 Access Token：{tokenResponse.AccessToken}", tokenResponse.AccessToken);
            return tokenResponse.AccessToken;
        }
    }
}
