using System.Globalization;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Milimoe.QQBot.Models;
using Milimoe.QQBot.Services;
using Rebex.Security.Cryptography;

namespace Milimoe.QQBot.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QQBotController(IOptions<BotConfig> botConfig, ILogger<QQBotController> logger, QQBotService service) : ControllerBase
    {
        private BotConfig BotConfig { get; set; } = botConfig.Value;
        private ILogger<QQBotController> Logger { get; set; } = logger;
        private QQBotService Service { get; set; } = service;

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Payload? payload)
        {
            if (payload is null)
            {
                return BadRequest("Payload 格式无效");
            }

            if (Logger.IsEnabled(LogLevel.Debug)) Logger.LogDebug("收到 Webhook 请求：{payload.Op}", payload.Op);

            try
            {
                if (payload.Op == 13)
                {
                    return HandleValidation(payload);
                }
                else if (payload.Op == 0)
                {
                    // 处理其他事件
                    _ = Task.Run(async () => await HandleEventAsync(payload));
                }
                else
                {
                    if (Logger.IsEnabled(LogLevel.Warning)) Logger.LogWarning("未处理操作码：{payload.Op}", payload.Op);
                }
                return Ok();
            }
            catch (Exception e)
            {
                if (Logger.IsEnabled(LogLevel.Error)) Logger.LogError("Error: {e}", e);
                return StatusCode(500, "服务器内部错误");
            }
        }

        private IActionResult HandleValidation(Payload payload)
        {
            ValidationRequest? validationPayload = JsonSerializer.Deserialize<ValidationRequest>(payload.Data.ToString() ?? "");
            if (validationPayload is null)
            {
                if (Logger.IsEnabled(LogLevel.Error)) Logger.LogError("反序列化验证 Payload 失败");
                return BadRequest("无效的验证 Payload 格式");
            }
            string seed = BotConfig.Secret;
            while (seed.Length < 32)
            {
                seed += seed;
            }
            seed = seed[..32];

            byte[] privateKeyBytes = Encoding.UTF8.GetBytes(seed);

            Ed25519 ed25519 = new();

            ed25519.FromSeed(privateKeyBytes);

            // 将你的消息转换为 byte[]
            byte[] message = Encoding.UTF8.GetBytes(validationPayload.EventTs + validationPayload.PlainToken);

            // 使用 Sign 方法签名消息
            byte[] result = ed25519.SignMessage(message);

            string signature = Convert.ToHexString(result).ToLower(CultureInfo.InvariantCulture);

            ValidationResponse response = new()
            {
                PlainToken = validationPayload.PlainToken,
                Signature = signature
            };
            string responseJson = JsonSerializer.Serialize(response);
            if (Logger.IsEnabled(LogLevel.Debug)) Logger.LogDebug("验证相应：{responseJson}", responseJson);
            return Ok(response);
        }

        private async Task<IActionResult> HandleEventAsync(Payload payload)
        {
            if (Logger.IsEnabled(LogLevel.Debug)) Logger.LogDebug("处理事件：{EventType}, 数据：{Data}", payload.EventType, payload.Data);

            try
            {
                switch (payload.EventType)
                {
                    case "C2C_MESSAGE_CREATE":
                        C2CMessage? c2cMessage = JsonSerializer.Deserialize<C2CMessage>(payload.Data.ToString() ?? "");
                        if (c2cMessage != null)
                        {
                            c2cMessage.Content = c2cMessage.Content.Trim();
                            if (c2cMessage.Content.StartsWith('/'))
                            {
                                c2cMessage.Content = c2cMessage.Content[1..];
                            }
                            // TODO
                            if (Logger.IsEnabled(LogLevel.Information)) Logger.LogInformation("收到来自用户 {c2cMessage.Author.UserOpenId} 的消息：{c2cMessage.Content}", c2cMessage.Author.UserOpenId, c2cMessage.Content);
                            // 上传图片示例
                            //string url = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/images/zi/dj1.png";
                            //UploadMediaResult uploadMediaResult = await Service.UploadC2CMediaAsync(c2cMessage.Author.UserOpenId, 1, url);
                            //if (Logger.IsEnabled(LogLevel.Debug)) Logger.LogDebug("发送的图片地址：{url}", url);
                            //if (string.IsNullOrEmpty(uploadMediaResult.Error))
                            //{
                            //    // 回复消息
                            //    await Service.SendC2CMessageAsync(c2cMessage.Author.UserOpenId, $"你发送的消息是：{c2cMessage.Content}", msgId: c2cMessage.Id);
                            //    // 回复富媒体消息
                            //    await Service.SendC2CMessageAsync(c2cMessage.Author.UserOpenId, "", msgType: 7, media: new { file_info = uploadMediaResult.FileInfo }, msgId: c2cMessage.Id);
                            //}
                            //else
                            //{
                            //    if (Logger.IsEnabled(LogLevel.Error)) Logger.LogError("上传图片失败：{error}", uploadMediaResult.Error);
                            //}
                            // 发 md 示例
                            //// 1. 带蓝字链接
                            //MarkdownMessage mdMsg = new()
                            //{
                            //    Content = "请选择：\n<qqbot-cmd-enter text=\"/签到\"/>"
                            //};
                            //// 2. 带按钮
                            //KeyboardMessage kbMsg = new()
                            //{
                            //    Content = new()
                            //    {
                            //        Rows = [
                            //            new()
                            //            {
                            //                Buttons = [
                            //                    new()
                            //                    {
                            //                        Id = "btn1",
                            //                        RenderData = new RenderData { Label = "同意", VisitedLabel = "已同意", Style = 1 },
                            //                        Action = new Models.Action
                            //                        {
                            //                            Type = 2,
                            //                            Data = "我同意服务条款",
                            //                            Enter = true,
                            //                            Reply = false,
                            //                            Permission = new Permission { Type = 2 }
                            //                        }
                            //                    }
                            //                ]
                            //            }
                            //        ]
                            //    }
                            //};
                            //await Service.SendC2CMarkdownAsync(c2cMessage.AuthorOpenId, mdMsg, kbMsg, c2cMessage.Id);
                        }
                        else
                        {
                            if (Logger.IsEnabled(LogLevel.Error)) Logger.LogError("反序列化 C2C 消息数据失败");
                            return BadRequest("无效的 C2C 消息数据格式");
                        }
                        break;
                    case "GROUP_AT_MESSAGE_CREATE":
                        GroupAtMessage? groupAtMessage = JsonSerializer.Deserialize<GroupAtMessage>(payload.Data.ToString() ?? "");
                        if (groupAtMessage != null)
                        {
                            groupAtMessage.Content = groupAtMessage.Content.Trim();
                            if (groupAtMessage.Content.StartsWith('/'))
                            {
                                groupAtMessage.Content = groupAtMessage.Content[1..];
                            }
                            // TODO
                            if (Logger.IsEnabled(LogLevel.Information)) Logger.LogInformation("收到来自群组 {groupAtMessage.GroupOpenId} 的消息：{groupAtMessage.Content}", groupAtMessage.GroupOpenId, groupAtMessage.Content);
                            // 回复消息，其他参考 C2C，大致相同
                            //await _service.SendGroupMessageAsync(groupAtMessage.GroupOpenId, $"你发送的消息是：{groupAtMessage.Content}", msgId: groupAtMessage.Id);
                        }
                        else
                        {
                            if (Logger.IsEnabled(LogLevel.Error)) Logger.LogError("反序列化群聊消息数据失败");
                            return BadRequest("无效的群聊消息数据格式");
                        }
                        break;
                    default:
                        if (Logger.IsEnabled(LogLevel.Warning)) Logger.LogWarning("未定义事件：{EventType}", payload.EventType);
                        break;
                }
                return Ok();
            }
            catch (JsonException e)
            {
                if (Logger.IsEnabled(LogLevel.Error)) Logger.LogError("反序列化过程遇到错误：{e}", e);
                return BadRequest("Invalid JSON format");
            }
            catch (Exception e)
            {
                if (Logger.IsEnabled(LogLevel.Error)) Logger.LogError("Error: {e}", e);
                return StatusCode(500, "服务器内部错误");
            }
        }
    }
}
