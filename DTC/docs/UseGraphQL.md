# GraphQL 實作說明

本專案使用 HotChocolate 框架實作 GraphQL API，與現有的 REST API 並存。

---

## 1. 安裝的套件與執行步驟

### 必要 NuGet 套件
- `HotChocolate.AspNetCore` (版本 13.5.1)
- `HotChocolate.Data.EntityFramework` (版本 13.5.1)

### 安裝指令
在 API 專案目錄下執行：
```bash
dotnet add package HotChocolate.AspNetCore --version 13.5.1
dotnet add package HotChocolate.Data.EntityFramework --version 13.5.1
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
3. 開啟瀏覽器訪問 GraphQL IDE (Banana)：
   ```
   https://localhost:5001/graphql
   ```
   或
   ```
   http://localhost:5000/graphql
   ```

---

## 2. 專案結構

```
DTC.API/
├── GraphQL/
│   ├── Types/           # GraphQL 類型定義
│   ├── Queries/         # 查詢定義
│   ├── Mutations/       # 變更定義
│   └── Subscriptions/   # 訂閱定義（如果需要）
├── Models/              # 資料模型（與 REST API 共用）
└── Data/               # 資料存取層（與 REST API 共用）
```

---

## 3. 實作規則

### (1) 類型定義
- 每個 GraphQL 類型都應該放在 `Types` 目錄下
- 類型名稱應該與對應的 C# 類別名稱一致
- 使用 `[GraphQLName]` 特性來定義 GraphQL 中的名稱
- 使用 `[GraphQLDescription]` 特性來添加描述

範例：
```csharp
[GraphQLName("Todo")]
[GraphQLDescription("A todo item")]
public class TodoType
{
    public int Id { get; set; }
    public string Title { get; set; }
    public bool IsCompleted { get; set; }
}
```

### (2) 查詢定義
- 所有查詢都應該放在 `Queries` 目錄下
- 查詢方法應該返回 `IQueryable<T>` 或具體的類型
- 使用 `[UseDbContext]` 特性來注入 DbContext
- 使用 `[UsePaging]` 特性來支援分頁
- 使用 `[UseFiltering]` 特性來支援過濾
- 使用 `[UseSorting]` 特性來支援排序

範例：
```csharp
public class Query
{
    [UseDbContext(typeof(ApplicationDbContext))]
    [UsePaging]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Todo> GetTodos([Service] ApplicationDbContext context)
    {
        return context.Todos;
    }
}
```

### (3) 變更定義
- 所有變更都應該放在 `Mutations` 目錄下
- 變更方法應該返回受影響的實體
- 使用 `[UseDbContext]` 特性來注入 DbContext
- 使用 `[UseTransaction]` 特性來支援事務

範例：
```csharp
public class Mutation
{
    [UseDbContext(typeof(ApplicationDbContext))]
    public async Task<Todo> CreateTodo(
        [Service] ApplicationDbContext context,
        CreateTodoInput input)
    {
        var todo = new Todo
        {
            Title = input.Title,
            IsCompleted = false
        };
        context.Todos.Add(todo);
        await context.SaveChangesAsync();
        return todo;
    }
}
```

### (4) 輸入類型
- 所有輸入類型都應該放在 `Types` 目錄下
- 使用 `[GraphQLName]` 特性來定義 GraphQL 中的名稱
- 使用 `[GraphQLDescription]` 特性來添加描述

範例：
```csharp
[GraphQLName("CreateTodoInput")]
[GraphQLDescription("Input for creating a new todo")]
public class CreateTodoInput
{
    [GraphQLDescription("The title of the todo")]
    public string Title { get; set; }
}
```

---

## 4. 註冊 GraphQL 服務

在 `Program.cs` 中註冊 GraphQL 服務：

```csharp
builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddType<TodoType>()
    .AddFiltering()
    .AddSorting()
    .AddProjections();
```

---

## 5. 注意事項
- GraphQL 端點與 REST API 端點並存，互不影響
- 使用 GraphQL IDE (Banana) 進行測試和開發
- 注意效能問題，特別是在處理大量資料時
- 適當使用 DataLoader 來避免 N+1 查詢問題
- 實作適當的錯誤處理機制

---

## 6. 參考文件
- [HotChocolate 官方文件](https://chillicream.com/docs/hotchocolate)
- [GraphQL 規格](https://graphql.org/learn/)
- [Entity Framework Core 文件](https://docs.microsoft.com/en-us/ef/core/) 