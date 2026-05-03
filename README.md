# QQ 官方机器人 API 的 Webhook 实现

基于 ASP.NET Core Web API 10.0，高性能、轻量、跨平台。

> 该项目为接收 QQ 官方机器人消息推送的 Webhook 监听模板，需配合 [官方文档](https://bot.q.qq.com/wiki/develop/api-v2/) 协助开发。

> 4 月 24 日起官方已全面放开 Markdown 的发送权限！（重大利好）

## 配置

1. 在 `appsettings.json` 中配置机器人的 `AppId` 和 `Secret`。

例如：
```json
"Bot": {
  "AppId": "11111111",
  "Secret": "DG5g3B4j9X2KOErG"
}
```

2. 配置监听地址和证书（要求必须 HTTPS）。或者使用反向代理（如 IIS、Nginx）。

例如：
```json
"Kestrel": {
  "Endpoints": {
    "Https": {
      "Url": "https://example.com",
      "Certificate": {
        "Path": "C:\\milimoe\\ssl.pfx",
        "Password": "password"
      }
    }
  }
}
```

## 使用

在 QQ 开放平台部署服务器域名以及 Webhook 的回调地址：`https://example.com/qqbot`（将 `example.com` 替换为你的服务器域名）。

## 引用的包
```
- Rebex.Elliptic.Ed25519（用于签名验证）
- Scalar.AspNetCore（一个非常美观的 OpenApi UI）
```
