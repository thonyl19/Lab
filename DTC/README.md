# DTC (DotNet Todo Collection)

一個基於 .NET 6 的待辦事項管理系統，提供 RESTful API 和 GraphQL 支援。

## 專案架構

```
DTC/
├── src/
│   ├── DTC.API/                 # Web API 專案
│   │   ├── Controllers/         # REST API 控制器
│   │   ├── GraphQL/            # GraphQL 相關檔案
│   │   │   ├── Queries/        # GraphQL 查詢
│   │   │   ├── Mutations/      # GraphQL 變更
│   │   │   └── Types/          # GraphQL 類型定義
│   │   ├── Models/             # 資料模型
│   │   │   └── DTOs/           # 資料傳輸物件
│   │   ├── Data/               # 資料存取層
│   │   ├── Interfaces/         # 介面定義
│   │   └── Services/           # 服務層
│   └── DTC.API.Tests/          # 單元測試專案
└── tests/                      # 整合測試
    └── DTC.API.Tests/          # API 整合測試
```

## 技術堆疊

- .NET 6
- Entity Framework Core
- SQLite
- Swagger/OpenAPI
- GraphQL
- xUnit (測試框架)

## 功能特點

- RESTful API 支援
- GraphQL API 支援
- SQLite 資料庫
- Swagger UI 文件
- 單元測試
- 整合測試

## 快速開始

### 前置需求

- .NET 6 SDK
- Visual Studio 2022 或 VS Code

### 安裝步驟

1. 複製專案
```bash
git clone [repository-url]
cd DTC
```

2. 還原套件
```bash
dotnet restore
```

3. 建立資料庫
```bash
cd src/DTC.API
dotnet ef database update
```

4. 運行專案
```bash
dotnet run
```

### API 文件

- Swagger UI: https://localhost:5001/swagger
- GraphQL Playground: https://localhost:5001/graphql

## API 端點

### REST API

- GET /api/Todo - 取得所有待辦事項
- GET /api/Todo/{id} - 取得特定待辦事項
- POST /api/Todo - 建立新的待辦事項
- PUT /api/Todo/{id} - 更新待辦事項
- DELETE /api/Todo/{id} - 刪除待辦事項

### GraphQL

- Query
  - todos - 取得所有待辦事項
  - todo(id: Int!) - 取得特定待辦事項
- Mutation
  - createTodo - 建立新的待辦事項
  - updateTodo - 更新待辦事項
  - deleteTodo - 刪除待辦事項

## 測試

### 執行單元測試
```bash
dotnet test src/DTC.API.Tests
```

### 執行整合測試
```bash
dotnet test tests/DTC.API.Tests
```

## 開發指南

### 新增功能

1. 在 Models 中定義資料模型
2. 在 Data 中設定資料庫映射
3. 在 Controllers 中實作 REST API
4. 在 GraphQL 中實作查詢和變更
5. 編寫單元測試和整合測試

### 資料庫遷移

```bash
# 建立遷移
dotnet ef migrations add [MigrationName]

# 更新資料庫
dotnet ef database update
```

## 授權

MIT License 