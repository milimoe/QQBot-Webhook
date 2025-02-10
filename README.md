# QQ 官方机器人 API 的 Webhook 实现

基于 ASP.NET Core Web API 9.0，高性能、轻量、跨平台。

> 本项目是个 C#.NET SDK，具体业务逻辑需要自行编写实现。

# 引用的包

- Rebex.Elliptic.Ed25519（用于签名验证）

- Scalar.AspNetCore（一个非常美观的 OpenApi UI）

## 使用

1、构建后，在生成目录下编辑 `appsettings.json` 文件，配置 Bot 的 `AppId` 和 `Secret`，这里使用了[官方文档](https://bot.q.qq.com/wiki/develop/api-v2/dev-prepare/interface-framework/event-emit.html#webhook%E6%96%B9%E5%BC%8F)的例子：

```json
"Bot": {
  "AppId": "11111111",
  "Secret": "DG5g3B4j9X2KOErG"
}
```

2、配置 `Kestrel` 服务器网址（SSL 证书 `Certificate` 以 pfx 为例）：

```json
"Kestrel": {
  "Endpoints": {
    "Http": {
      "Url": "http://example.com"
    },
    "Https": {
      "Url": "https://example.com",
      "Certificate": {
        "Path": "xxx.pfx",
        "Password": "xxx"
      }
    }
  }
},
```

`Http` 监听可以删除。

3、启动 `QQBot.exe`。

4、最后，将 Webhook 回调地址：`https://example.com/qqbot` 发布在 QQ 开放平台上。
