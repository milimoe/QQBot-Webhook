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

        /// <summary>
        /// 创建一个指令按钮（Type = 2），点击后发送指定指令文本。
        /// </summary>
        /// <param name="label">按钮默认显示文本</param>
        /// <param name="data">指令文本（填充或发送的内容）</param>
        /// <param name="enter">true = 点击直接发送消息，false = 仅填充到输入框</param>
        /// <param name="reply">是否引用当前消息（仅在 Type = 2 且 enter = true 时有效）</param>
        /// <param name="visitedLabel">点击后显示的文本，为 null 则与 label 相同</param>
        /// <param name="style">按钮样式：0 = 灰色，1 = 蓝色</param>
        /// <param name="permissionType">权限类型：0 = 指定用户，1 = 管理者，2 = 所有人</param>
        /// <param name="specifyUserIds">当权限类型 = 0 时，需指定用户 OpenID</param>
        /// <returns></returns>
        public static Button CreateCmdButton(string label, string data, bool enter = true, bool reply = false, string? visitedLabel = null, int style = 1, int permissionType = 2, params List<string> specifyUserIds)
        {
            return new Button
            {
                Id = Guid.NewGuid().ToString(),
                RenderData = new RenderData
                {
                    Label = label,
                    VisitedLabel = visitedLabel ?? label,
                    Style = style
                },
                Action = new Action
                {
                    Type = 2,
                    Data = data,
                    Enter = enter,
                    Reply = reply,
                    Permission = new Permission { Type = permissionType, SpecifyUserIds = specifyUserIds }
                }
            };
        }

        /// <summary>
        /// 创建一个指令按钮（Type = 1），点击后向服务器发送 INTERACTION_CREATE 事件。
        /// </summary>
        /// <param name="label">按钮默认显示文本</param>
        /// <param name="data">指令文本（填充或发送的内容）</param>
        /// <param name="enter">true = 点击直接发送消息，false = 仅填充到输入框</param>
        /// <param name="reply">是否引用当前消息（仅在 Type = 2 且 enter = true 时有效）</param>
        /// <param name="visitedLabel">点击后显示的文本，为 null 则与 label 相同</param>
        /// <param name="style">按钮样式：0 = 灰色，1 = 蓝色</param>
        /// <param name="permissionType">权限类型：0 = 指定用户，1 = 管理者，2 = 所有人</param>
        /// <param name="specifyUserIds">当权限类型 = 0 时，需指定用户 OpenID</param>
        /// <returns></returns>
        public static Button CreateInteractionButton(string label, string data, string? visitedLabel = null, int style = 1, int permissionType = 2, params List<string> specifyUserIds)
        {
            return new Button
            {
                Id = Guid.NewGuid().ToString(),
                RenderData = new RenderData
                {
                    Label = label,
                    VisitedLabel = visitedLabel ?? label,
                    Style = style
                },
                Action = new Action
                {
                    Type = 1,
                    Data = data,
                    Permission = new Permission { Type = permissionType, SpecifyUserIds = specifyUserIds }
                }
            };
        }
    }

    public class RenderData
    {
        /// <summary>
        /// 默认显示文本
        /// </summary>
        [JsonPropertyName("label")]
        public string Label { get; set; } = "";

        /// <summary>
        /// 点击后的文本
        /// </summary>
        [JsonPropertyName("visited_label")]
        public string VisitedLabel { get; set; } = "";

        /// <summary>
        /// 0/1：灰/蓝色
        /// </summary>
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
        /// 当 Type = 2 时，true = 直接发送消息，false = 填充输入框
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

        [JsonPropertyName("specify_user_ids")]
        public List<string> SpecifyUserIds { get; set; } = [];
    }

    public class InteractionEvent
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "";

        [JsonPropertyName("type")]
        public int Type { get; set; }

        [JsonPropertyName("chat_type")]
        public int ChatType { get; set; }

        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; } = "";

        [JsonPropertyName("guild_id")]
        public string GuildId { get; set; } = "";

        [JsonPropertyName("channel_id")]
        public string ChannelId { get; set; } = "";

        [JsonPropertyName("group_openid")]
        public string GroupOpenId { get; set; } = "";

        [JsonPropertyName("user_openid")]
        public string UserOpenId { get; set; } = "";

        [JsonPropertyName("version")]
        public int Version { get; set; }

        [JsonPropertyName("application_id")]
        public string ApplicationId { get; set; } = "";

        [JsonPropertyName("data")]
        public InteractionData Data { get; set; } = new();
    }

    public class InteractionData
    {
        [JsonPropertyName("type")]
        public int Type { get; set; }

        [JsonPropertyName("resolved")]
        public InteractionResolved? Resolved { get; set; }

        [JsonPropertyName("feature")]
        public InteractionFeature? Feature { get; set; }
    }

    public class InteractionResolved
    {
        [JsonPropertyName("button_id")]
        public string ButtonId { get; set; } = "";

        [JsonPropertyName("button_data")]
        public string ButtonData { get; set; } = "";

        [JsonPropertyName("message_id")]
        public string MessageId { get; set; } = "";

        [JsonPropertyName("user_id")]
        public string UserId { get; set; } = "";
    }

    public class InteractionFeature
    {
        [JsonPropertyName("type")]
        public int Type { get; set; }

        [JsonPropertyName("data")]
        public string Data { get; set; } = "";
    }

    public static class QQBotModelExtension
    {
        /// <summary>
        /// 增加键盘按钮
        /// </summary>
        /// <param name="kb">键盘消息</param>
        /// <param name="btnCountPerRow">每行按钮数量，自动检查如果超过则新建行</param>
        /// <param name="buttons">按钮</param>
        /// <returns></returns>
        public static KeyboardMessage AppendButtons(this KeyboardMessage kb, int btnCountPerRow, params IEnumerable<Button> buttons)
        {
            if (btnCountPerRow < 1)
                btnCountPerRow = 1;

            kb.Content ??= new KeyboardContent
            {
                Rows = []
            };

            List<Row> rows = kb.Content.Rows;
            // 找到当前可用的最后一个 Row，或新建一个
            Row? currentRow = rows.Count > 0 ? rows[^1] : null;

            foreach (Button button in buttons)
            {
                if (currentRow is null || currentRow.Buttons.Count >= btnCountPerRow)
                {
                    currentRow = new Row { Buttons = [] };
                    rows.Add(currentRow);
                }

                currentRow.Buttons.Add(button);
            }

            return kb;
        }

        /// <summary>
        /// 新增一行，然后在该行增加键盘按钮
        /// </summary>
        /// <param name="kb">键盘消息</param>
        /// <param name="btnCountPerRow">每行按钮数量，自动检查如果超过则新建行</param>
        /// <param name="buttons">按钮</param>
        /// <returns></returns>
        public static KeyboardMessage AppendButtonsWithNewRow(this KeyboardMessage kb, int btnCountPerRow, params IEnumerable<Button> buttons)
        {
            if (btnCountPerRow < 1)
                btnCountPerRow = 1;

            kb.Content ??= new KeyboardContent
            {
                Rows = []
            };

            List<Row> rows = kb.Content.Rows;
            Row? currentRow = null;

            foreach (Button button in buttons)
            {
                if (currentRow is null || currentRow.Buttons.Count >= btnCountPerRow)
                {
                    currentRow = new Row { Buttons = [] };
                    rows.Add(currentRow);
                }

                currentRow.Buttons.Add(button);
            }

            return kb;
        }

        /// <summary>
        /// 添加通用分类键盘组件
        /// </summary>
        /// <param name="kb"></param>
        /// <param name="command"></param>
        /// <param name="currentPage"></param>
        /// <param name="totalPages"></param>
        /// <returns></returns>
        public static KeyboardMessage AddPaginationRow(this KeyboardMessage kb, string command, int currentPage, int totalPages)
        {
            List<Button> buttons = [];

            // 首页和上一页
            if (currentPage > 1)
            {
                buttons.Add(Button.CreateCmdButton("<<", $"{command}1", enter: true, style: 1));
                buttons.Add(Button.CreateCmdButton("<", $"{command}{currentPage - 1}", enter: true, style: 1));
            }

            // 页码指示和跳转
            buttons.Add(Button.CreateCmdButton($"{currentPage} / {totalPages}", $"{command} ", enter: false, style: 1));

            // 下一页和末页
            if (currentPage < totalPages)
            {
                buttons.Add(Button.CreateCmdButton(">", $"{command}{currentPage + 1}", enter: true, style: 1));
                buttons.Add(Button.CreateCmdButton(">>", $"{command}{totalPages}", enter: true, style: 1));
            }

            return kb.AppendButtonsWithNewRow(5, buttons);
        }

        /// <summary>
        /// 创建一个 qqbot-cmd-input 标签字符串。点击后 text 会填充到输入框，show 为展示文本。
        /// </summary>
        /// <param name="show">按钮或链接展示的文本</param>
        /// <param name="text">填充到输入框的指令文本</param>
        /// <param name="thisIsText">交换两个参数（指示 this 为 text）</param>
        /// <returns>形如 &lt;qqbot-cmd-input text="..." show="..."/&gt; 的字符串</returns>
        public static string CreateCmdInput(this string show, string text, bool thisIsText = false)
        {
            // 转义双引号（避免属性值被截断）
            string escapedText = text.Replace("\"", "&quot;");
            string escapedShow = show.Replace("\"", "&quot;");
            if (thisIsText)
            {
                return $"<qqbot-cmd-input text=\"{escapedShow}\" show=\"{escapedText}\"/>";
            }
            return $"<qqbot-cmd-input text=\"{escapedText}\" show=\"{escapedShow}\"/>";
        }

        /// <summary>
        /// 创建展示文本与填充内容相同的 qqbot-cmd-input 标签。
        /// </summary>
        public static string CreateCmdInput(this string text) => CreateCmdInput(text, text);

        /// <summary>
        /// 创建展示文本与填充内容相同的 qqbot-cmd-enter 标签。（仅 C2C）
        /// </summary>
        public static string CreateCmdEnter(this string text)
        {
            return $"<qqbot-cmd-enter text=\"{text}\"/>";
        }
    }
}
