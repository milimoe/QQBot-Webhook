using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Milimoe.QQBot.Models;

namespace Milimoe.QQBot.Services
{
    public class QQBotService(IOptions<BotConfig> botConfig, ILogger<QQBotService> logger, IHttpClientFactory httpClientFactory, IMemoryCache memoryCache)
    {
        private BotConfig BotConfig { get; set; } = botConfig.Value;
        private ILogger<QQBotService> Logger { get; set; } = logger;
        private HttpClient HttpClient { get; set; } = httpClientFactory.CreateClient();
        private IMemoryCache MemoryCache { get; set; } = memoryCache;

        private const string AccessTokenCacheKey = "QQBotAccessToken";

        public async Task SendC2CMessageAsync(string openid, string content, int msgType = 0, object? media = null, string? msgId = null, int? msgSeq = null)
        {
            await SendMessageAsync($"/v2/users/{openid}/messages", content, msgType, media, msgId, msgSeq);
        }

        public async Task SendGroupMessageAsync(string groupOpenid, string content, int msgType = 0, object? media = null, string? msgId = null, int? msgSeq = null)
        {
            await SendMessageAsync($"/v2/groups/{groupOpenid}/messages", content, msgType, media, msgId, msgSeq);
        }

        public async Task SendC2CMarkdownAsync(string openid, MarkdownMessage markdown, KeyboardMessage? keyboard = null, string? msgId = null, int? msgSeq = null)
        {
            await SendMarkdownMessageAsync($"/v2/users/{openid}/messages", markdown, keyboard, msgId, msgSeq);
        }

        public async Task SendGroupMarkdownAsync(string groupOpenid, MarkdownMessage markdown, KeyboardMessage? keyboard = null, string? msgId = null, int? msgSeq = null)
        {
            await SendMarkdownMessageAsync($"/v2/groups/{groupOpenid}/messages", markdown, keyboard, msgId, msgSeq);
        }

        private async Task SendMessageAsync(string url, string content, int msgType = 0, object? media = null, string? msgId = null, int? msgSeq = null)
        {
            string accessToken = await GetAccessTokenAsync();
            HttpRequestMessage request = new(HttpMethod.Post, $"https://api.sgroup.qq.com{url}");
            request.Headers.Authorization = new AuthenticationHeaderValue("QQBot", accessToken);
            if (Logger.IsEnabled(LogLevel.Debug)) Logger.LogDebug("使用的 Access Token：{accessToken}", accessToken);
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

            HttpResponseMessage response = await HttpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                string errorBody = await response.Content.ReadAsStringAsync();
                if (Logger.IsEnabled(LogLevel.Error)) Logger.LogError("状态码：{response.StatusCode}，错误信息：{errorBody}", response.StatusCode, errorBody);
            }
        }

        private async Task SendMarkdownMessageAsync(string url, MarkdownMessage markdown, KeyboardMessage? keyboard = null, string? msgId = null, int? msgSeq = null)
        {
            string accessToken = await GetAccessTokenAsync();
            HttpRequestMessage request = new(HttpMethod.Post, $"https://api.sgroup.qq.com{url}");
            request.Headers.Authorization = new AuthenticationHeaderValue("QQBot", accessToken);
            Dictionary<string, object> body = new()
            {
                { "msg_type", 2 },
                { "markdown", markdown }
            };
            if (keyboard != null) body.Add("keyboard", keyboard);
            if (!string.IsNullOrEmpty(msgId)) body.Add("msg_id", msgId);
            if (msgSeq.HasValue) body.Add("msg_seq", msgSeq.Value);
            request.Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await HttpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                string errorBody = await response.Content.ReadAsStringAsync();
                if (Logger.IsEnabled(LogLevel.Error)) Logger.LogError("状态码：{response.StatusCode}，错误信息：{errorBody}", response.StatusCode, errorBody);
            }
        }

        public async Task<UploadMediaResult> UploadC2CMediaAsync(string openid, int fileType, string url)
        {
            return await UploadMediaAsync($"/v2/users/{openid}/files", fileType, url);
        }

        public async Task<UploadMediaResult> UploadGroupMediaAsync(string groupOpenid, int fileType, string url)
        {
            return await UploadMediaAsync($"/v2/groups/{groupOpenid}/files", fileType, url);
        }

        private async Task<UploadMediaResult> UploadMediaAsync(string url, int fileType, string fileUrl)
        {
            string accessToken = await GetAccessTokenAsync();
            HttpRequestMessage request = new(HttpMethod.Post, $"https://api.sgroup.qq.com{url}");
            request.Headers.Authorization = new AuthenticationHeaderValue("QQBot", accessToken);

            Dictionary<string, object> requestBody = new()
            {
                { "file_type", fileType },
                { "url", fileUrl },
                { "srv_send_msg", false }
            };
            request.Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await HttpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                string errorBody = await response.Content.ReadAsStringAsync();
                string error = $"状态码：{response.StatusCode}，错误信息：{errorBody}，多媒体URL地址：{fileUrl}";
                if (Logger.IsEnabled(LogLevel.Error)) Logger.LogError("{Error}", error);
                return new UploadMediaResult { Error = error };
            }

            string responseBody = await response.Content.ReadAsStringAsync();
            MediaResponse? mediaResponse = JsonSerializer.Deserialize<MediaResponse>(responseBody);
            if (mediaResponse == null)
            {
                return new UploadMediaResult { Error = "反序列化富媒体消息失败。" };
            }

            if (Logger.IsEnabled(LogLevel.Debug)) Logger.LogDebug("接收到的富媒体消息：{FileInfo}，有效时间：{Ttl}", mediaResponse.FileInfo, mediaResponse.Ttl);

            return new UploadMediaResult
            {
                FileUuid = mediaResponse.FileUuid,
                FileInfo = mediaResponse.FileInfo,
                Ttl = mediaResponse.Ttl
            };
        }

        public async Task<string> GetAccessTokenAsync()
        {
            if (MemoryCache.TryGetValue(AccessTokenCacheKey, out string? accessToken) && !string.IsNullOrEmpty(accessToken))
            {
                return accessToken;
            }

            return await RefreshTokenAsync();
        }

        public async Task<string> RefreshTokenAsync()
        {
            HttpRequestMessage request = new(HttpMethod.Post, "https://bots.qq.com/app/getAppAccessToken")
            {
                Content = new StringContent(JsonSerializer.Serialize(new { appId = BotConfig.AppId, clientSecret = BotConfig.Secret }), System.Text.Encoding.UTF8, "application/json")
            };
            HttpResponseMessage response = await HttpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            AccessTokenResponse? tokenResponse = JsonSerializer.Deserialize<AccessTokenResponse>(responseBody);
            if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.AccessToken) || !int.TryParse(tokenResponse.ExpiresIn, out int expiresIn))
            {
                throw new Exception("获取 Access Token 失败！");
            }
            MemoryCache.Set(AccessTokenCacheKey, tokenResponse.AccessToken, TimeSpan.FromSeconds(expiresIn - 60));
            if (Logger.IsEnabled(LogLevel.Debug)) Logger.LogDebug("获取到 Access Token：{tokenResponse.AccessToken}", tokenResponse.AccessToken);
            return tokenResponse.AccessToken;
        }
    }
}
