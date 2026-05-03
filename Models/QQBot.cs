using System.Text.Json.Serialization;

namespace Milimoe.QQBot.Models
{
    public class Payload
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "";

        [JsonPropertyName("op")]
        public int Op { get; set; } = 0;

        [JsonPropertyName("d")]
        public object Data { get; set; } = new();

        [JsonPropertyName("s")]
        public int SequenceNumber { get; set; } = 0;

        [JsonPropertyName("t")]
        public string EventType { get; set; } = "";
    }

    public class ValidationRequest
    {
        [JsonPropertyName("plain_token")]
        public string PlainToken { get; set; } = "";

        [JsonPropertyName("event_ts")]
        public string EventTs { get; set; } = "";
    }

    public class ValidationResponse
    {
        [JsonPropertyName("plain_token")]
        public string PlainToken { get; set; } = "";

        [JsonPropertyName("signature")]
        public string Signature { get; set; } = "";
    }

    public class BotConfig
    {
        public string AppId { get; set; } = "";
        public string Secret { get; set; } = "";
    }

    public class Author
    {
        [JsonPropertyName("user_openid")]
        public string UserOpenId { get; set; } = "";

        [JsonPropertyName("member_openid")]
        public string MemberOpenId { get; set; } = "";
    }

    public class Attachment
    {
        [JsonPropertyName("content_type")]
        public string ContentType { get; set; } = "";

        [JsonPropertyName("filename")]
        public string Filename { get; set; } = "";

        [JsonPropertyName("height")]
        public int Height { get; set; } = 0;

        [JsonPropertyName("width")]
        public int Width { get; set; } = 0;

        [JsonPropertyName("size")]
        public int Size { get; set; } = 0;

        [JsonPropertyName("url")]
        public string Url { get; set; } = "";
    }

    public class C2CMessage
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "";

        [JsonPropertyName("author")]
        public Author Author { get; set; } = new();

        [JsonPropertyName("content")]
        public string Content { get; set; } = "";

        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; } = "";

        [JsonPropertyName("attachments")]
        public Attachment[] Attachments { get; set; } = [];

        [JsonIgnore]
        public string OpenId => Author.UserOpenId;
    }

    public class GroupAtMessage
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "";

        [JsonPropertyName("author")]
        public Author Author { get; set; } = new();

        [JsonPropertyName("content")]
        public string Content { get; set; } = "";

        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; } = "";

        [JsonPropertyName("group_openid")]
        public string GroupOpenId { get; set; } = "";

        [JsonPropertyName("attachments")]
        public Attachment[] Attachments { get; set; } = [];

        [JsonIgnore]
        public string OpenId => GroupOpenId;
    }

    public class MediaResponse
    {
        [JsonPropertyName("file_uuid")]
        public string FileUuid { get; set; } = "";

        [JsonPropertyName("file_info")]
        public string FileInfo { get; set; } = "";

        [JsonPropertyName("ttl")]
        public int Ttl { get; set; }
    }

    public class UploadMediaResult
    {
        /// <summary>
        /// 文件 UUID，成功时有值
        /// </summary>
        public string? FileUuid { get; set; }

        /// <summary>
        /// 文件信息，成功时有值
        /// </summary>
        public string? FileInfo { get; set; }

        /// <summary>
        /// 有效期（秒）
        /// </summary>
        public int Ttl { get; set; }

        /// <summary>
        /// 错误信息，成功时为 null
        /// </summary>
        public string? Error { get; set; }

        /// <summary>
        /// 是否上传成功
        /// </summary>
        [JsonIgnore]
        public bool IsSuccess => string.IsNullOrEmpty(Error);
    }

    public class AccessTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = "";

        [JsonPropertyName("expires_in")]
        public string ExpiresIn { get; set; } = "";
    }

    public class OtherData
    {
        public string RequestUrl { get; set; } = "";
    }
    public class MarkdownMessage
    {
        /// <summary>
        /// 自定义 Markdown（和模板 ID 互斥，只能用一种方式）
        /// </summary>
        [JsonPropertyName("content")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Content { get; set; }

        /// <summary>
        /// 模板 ID
        /// </summary>
        [JsonPropertyName("custom_template_id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? CustomTemplateId { get; set; }

        /// <summary>
        /// 模板参数列表（仅在模板模式下使用）
        /// </summary>
        [JsonPropertyName("params")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<MarkdownParam>? Params { get; set; }
    }

    public class MarkdownParam
    {
        [JsonPropertyName("key")]
        public string Key { get; set; } = "";

        [JsonPropertyName("values")]
        public List<string> Values { get; set; } = [];
    }

    public class KeyboardMessage
    {
        /// <summary>
        /// 模板按钮 ID（与 content 互斥）
        /// </summary>
        [JsonPropertyName("id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Id { get; set; }

        /// <summary>
        /// 自定义按钮内容
        /// </summary>
        [JsonPropertyName("content")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public KeyboardContent? Content { get; set; }
    }

    public class KeyboardContent
    {
        [JsonPropertyName("rows")]
        public List<Row> Rows { get; set; } = [];
    }

    public class Row
    {
        [JsonPropertyName("buttons")]
        public List<Button> Buttons { get; set; } = [];
    }

    public class Button
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "";

        [JsonPropertyName("render_data")]
        public RenderData RenderData { get; set; } = new();

        [JsonPropertyName("action")]
        public Action Action { get; set; } = new();
    }

    public class RenderData
    {
        [JsonPropertyName("label")]
        public string Label { get; set; } = "";

        [JsonPropertyName("visited_label")]
        public string VisitedLabel { get; set; } = "";

        [JsonPropertyName("style")]
        public int Style { get; set; } = 0;
    }

    public class Action
    {
        /// <summary>
        /// 种类：0 - 跳转（HTTP/小程序），1 - 回调（机器人后台会收到 INTERACTION_CREATE 事件，data 会回传），2 - 指令（发送消息或填充输入框，常用）
        /// </summary>
        [JsonPropertyName("type")]
        public int Type { get; set; }

        /// <summary>
        /// 按钮的点击权限，内部的 Type = 0 为指定用户，1 为管理者，2 为所有人。
        /// </summary>
        [JsonPropertyName("permission")]
        public Permission Permission { get; set; } = new();

        /// <summary>
        /// 内容
        /// </summary>
        [JsonPropertyName("data")]
        public string Data { get; set; } = "";

        /// <summary>
        /// 回调数据，机器人后台会收到 INTERACTION_CREATE 事件，data 会回传
        /// </summary>
        [JsonPropertyName("enter")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? Enter { get; set; }

        /// <summary>
        /// 当 Type = 2 时，是否引用当前消息
        /// </summary>
        [JsonPropertyName("reply")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? Reply { get; set; }

        /// <summary>
        /// 打开图片选取器，与 Enter 互斥
        /// </summary>
        [JsonPropertyName("anchor")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Anchor { get; set; }
    }

    public class Permission
    {
        [JsonPropertyName("type")]
        public int Type { get; set; }
    }
}
