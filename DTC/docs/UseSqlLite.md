# SQLite 設定與使用說明

本專案使用 SQLite 作為資料庫，搭配 Entity Framework Core 進行資料存取。

---

## 1. 安裝的套件與執行步驟

### 必要 NuGet 套件
- `Microsoft.EntityFrameworkCore.Sqlite`

### 安裝指令
在 API 專案目錄下執行：
```bash
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
```

### 執行步驟
1. 還原套件：
   ```bash
   dotnet restore
   ```
2. 建立資料庫（執行遷移）：
   ```bash
   dotnet ef database update
   ```
3. 執行 API 專案：
   ```bash
   dotnet run
   ```

---

## 2. 需要設置的程序 / 程式碼

### (1) 設定連線字串
**檔案：** `DTC.API/appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=DB/todo.db"
  },
  // ... 其他設定 ...
}
```

> 建議使用相對路徑（如 `DB/todo.db`），專案會自動在根目錄下建立 DB 目錄與資料庫檔案。

### (2) 確保 DB 目錄存在並轉為絕對路徑
**檔案：** `DTC.API/Program.cs`

```csharp
// 確保 DB 目錄存在，並將連線字串轉為絕對路徑
var dbFolder = Path.Combine(AppContext.BaseDirectory, "DB");
if (!Directory.Exists(dbFolder))
{
    Directory.CreateDirectory(dbFolder);
}
var dbPath = Path.Combine(dbFolder, "todo.db");
builder.Configuration["ConnectionStrings:DefaultConnection"] = $"Data Source={dbPath}";
```

### (3) 註冊 DbContext 並指定 SQLite
**檔案：** `DTC.API/Program.cs`

```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
```

### (4) 典型完整範例
**檔案：** `DTC.API/Program.cs`

```csharp
// ... 其他 using ...
using Microsoft.EntityFrameworkCore;

// ...
var dbFolder = Path.Combine(AppContext.BaseDirectory, "DB");
if (!Directory.Exists(dbFolder))
{
    Directory.CreateDirectory(dbFolder);
}
var dbPath = Path.Combine(dbFolder, "todo.db");
builder.Configuration["ConnectionStrings:DefaultConnection"] = $"Data Source={dbPath}";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
// ... 其他服務註冊 ...
```

---

## 3. 注意事項
- SQLite 適合開發、測試、小型專案，不建議用於高併發大型系統。
- 若遇到資料庫路徑或權限問題，請確認 `DB` 目錄存在且有寫入權限。
- 每次模型變更後，請執行 `dotnet ef migrations add [遷移名稱]` 並 `dotnet ef database update`。
- 若要重建資料庫，請刪除 `DB/todo.db` 後重新執行遷移。

---

## 4. 參考文件
- [EF Core 官方 SQLite 文件](https://learn.microsoft.com/ef/core/providers/sqlite/)
- [EF Core CLI 文件](https://learn.microsoft.com/ef/core/cli/dotnet) 