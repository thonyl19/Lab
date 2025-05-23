# Swagger 設定與使用說明

本專案使用 Swagger/OpenAPI 來產生 API 文件，並提供互動式 API 測試介面。

---

## 1. 安裝的套件與執行步驟

### 必要 NuGet 套件
- `Swashbuckle.AspNetCore` (版本 6.5.0)

### 安裝指令
在 API 專案目錄下執行：
```bash
dotnet add package Swashbuckle.AspNetCore --version 6.5.0
```

### 執行步驟
1. 還原套件：
   ```bash
   dotnet restore
   ```
2. 執行 API 專案：
   ```bash
   dotnet run
   ```
3. 開啟瀏覽器訪問 Swagger UI：
   ```
   https://localhost:5001/swagger
   ```
   或
   ```
   http://localhost:5000/swagger
   ```

---

## 2. 需要設置的程序 / 程式碼

### (1) 註冊 Swagger 服務
**檔案：** `DTC.API/Program.cs`

```csharp
// 註冊 Swagger 服務
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Todo API",
        Version = "v1",
        Description = "A simple Todo API built with .NET 6",
        Contact = new OpenApiContact
        {
            Name = "Your Name",
            Email = "your.email@example.com"
        }
    });
});
```

### (2) 啟用 Swagger 中間件
**檔案：** `DTC.API/Program.cs`

```csharp
// 啟用 Swagger 中間件
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo API V1");
        c.RoutePrefix = "swagger";
    });
}
```

### (3) 典型完整範例
**檔案：** `DTC.API/Program.cs`

```csharp
// ... 其他 using ...
using Microsoft.OpenApi.Models;

// ... 其他服務註冊 ...
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Todo API",
        Version = "v1",
        Description = "A simple Todo API built with .NET 6",
        Contact = new OpenApiContact
        {
            Name = "Your Name",
            Email = "your.email@example.com"
        }
    });
});

// ... 其他中間件 ...
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo API V1");
        c.RoutePrefix = "swagger";
    });
}
```

---

## 3. 注意事項
- Swagger UI 預設只在開發環境啟用，生產環境請關閉。
- 若遇到 CORS 問題，請確認已正確設定 CORS 政策。
- API 文件會自動根據 Controller 和 Model 的註解產生。
- 建議在 Controller 和 Model 中加入 XML 文件註解，以產生更完整的 API 文件。

---

## 4. 參考文件
- [Swashbuckle.AspNetCore 官方文件](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)
- [OpenAPI 規格](https://swagger.io/specification/) 