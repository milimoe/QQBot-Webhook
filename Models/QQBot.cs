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

    public class AccessTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = "";

        [JsonPropertyName("expires_in")]
        public string ExpiresIn { get; set; } = "";
    }
}
