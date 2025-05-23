# DTC GraphQL API 文件

## 概述

DTC 提供 GraphQL API 來操作待辦事項。本文檔詳細說明 GraphQL 查詢和變更的使用方法。

## 端點

GraphQL 端點位於：`https://localhost:5001/graphql`

## 類型定義

### Todo 類型

```graphql
type Todo {
  id: Int!
  title: String!
  description: String
  isCompleted: Boolean!
  createdAt: DateTime!
  updatedAt: DateTime
}
```

### 輸入類型

```graphql
input TodoInput {
  title: String!
  description: String
  isCompleted: Boolean!
}
```

## 查詢

### 取得所有待辦事項

```graphql
query {
  todos {
    id
    title
    description
    isCompleted
    createdAt
    updatedAt
  }
}
```

#### 回應

```json
{
  "data": {
    "todos": [
      {
        "id": 1,
        "title": "完成專案文件",
        "description": "更新 API 文件和使用說明",
        "isCompleted": false,
        "createdAt": "2024-03-20T10:00:00Z",
        "updatedAt": "2024-03-20T10:00:00Z"
      }
    ]
  }
}
```

### 取得特定待辦事項

```graphql
query {
  todo(id: 1) {
    id
    title
    description
    isCompleted
    createdAt
    updatedAt
  }
}
```

#### 參數

- `id`: 待辦事項 ID (必填)

#### 回應

```json
{
  "data": {
    "todo": {
      "id": 1,
      "title": "完成專案文件",
      "description": "更新 API 文件和使用說明",
      "isCompleted": false,
      "createdAt": "2024-03-20T10:00:00Z",
      "updatedAt": "2024-03-20T10:00:00Z"
    }
  }
}
```

## 變更

### 建立待辦事項

```graphql
mutation {
  createTodo(input: {
    title: "新待辦事項"
    description: "待辦事項描述"
    isCompleted: false
  }) {
    id
    title
    description
    isCompleted
    createdAt
    updatedAt
  }
}
```

#### 參數

- `input`: TodoInput 類型 (必填)
  - `title`: 標題 (必填)
  - `description`: 描述 (選填)
  - `isCompleted`: 是否完成 (必填)

#### 回應

```json
{
  "data": {
    "createTodo": {
      "id": 2,
      "title": "新待辦事項",
      "description": "待辦事項描述",
      "isCompleted": false,
      "createdAt": "2024-03-20T10:00:00Z",
      "updatedAt": "2024-03-20T10:00:00Z"
    }
  }
}
```

### 更新待辦事項

```graphql
mutation {
  updateTodo(id: 1, input: {
    title: "更新後的標題"
    description: "更新後的描述"
    isCompleted: true
  }) {
    id
    title
    description
    isCompleted
    updatedAt
  }
}
```

#### 參數

- `id`: 待辦事項 ID (必填)
- `input`: TodoInput 類型 (必填)

#### 回應

```json
{
  "data": {
    "updateTodo": {
      "id": 1,
      "title": "更新後的標題",
      "description": "更新後的描述",
      "isCompleted": true,
      "updatedAt": "2024-03-20T10:00:00Z"
    }
  }
}
```

### 刪除待辦事項

```graphql
mutation {
  deleteTodo(id: 1)
}
```

#### 參數

- `id`: 待辦事項 ID (必填)

#### 回應

```json
{
  "data": {
    "deleteTodo": true
  }
}
```

## 錯誤處理

GraphQL API 使用標準的 GraphQL 錯誤格式：

```json
{
  "errors": [
    {
      "message": "錯誤訊息",
      "locations": [
        {
          "line": 2,
          "column": 3
        }
      ],
      "path": ["createTodo"],
      "extensions": {
        "code": "ERROR_CODE"
      }
    }
  ]
}
```

常見錯誤代碼：
- `NOT_FOUND`: 資源不存在
- `VALIDATION_ERROR`: 輸入驗證錯誤
- `INTERNAL_ERROR`: 伺服器內部錯誤

## 使用建議

1. 使用 GraphQL Playground 進行測試和開發
2. 只請求需要的欄位，避免過度獲取
3. 使用變數來參數化查詢
4. 處理錯誤情況

## 範例

### 使用變數

```graphql
mutation CreateTodo($input: TodoInput!) {
  createTodo(input: $input) {
    id
    title
    description
    isCompleted
    createdAt
    updatedAt
  }
}

# 變數
{
  "input": {
    "title": "新待辦事項",
    "description": "待辦事項描述",
    "isCompleted": false
  }
}
```

### 批量操作

```graphql
mutation {
  createTodo1: createTodo(input: {
    title: "待辦事項 1"
    isCompleted: false
  }) {
    id
    title
  }
  createTodo2: createTodo(input: {
    title: "待辦事項 2"
    isCompleted: false
  }) {
    id
    title
  }
}
```

## 支援

如有問題，請提交 Issue 或聯繫開發團隊。 