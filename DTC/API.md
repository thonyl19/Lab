# DTC API 文件

## 概述

DTC API 提供 RESTful API 和 GraphQL 兩種方式來操作待辦事項。本文檔詳細說明各個端點的使用方法和參數。

## REST API

### 取得所有待辦事項

```http
GET /api/Todo
```

#### 回應

```json
[
  {
    "id": 1,
    "title": "完成專案文件",
    "description": "更新 API 文件和使用說明",
    "isCompleted": false,
    "createdAt": "2024-03-20T10:00:00Z",
    "updatedAt": "2024-03-20T10:00:00Z"
  }
]
```

### 取得特定待辦事項

```http
GET /api/Todo/{id}
```

#### 參數

- `id` (路徑參數): 待辦事項 ID

#### 回應

```json
{
  "id": 1,
  "title": "完成專案文件",
  "description": "更新 API 文件和使用說明",
  "isCompleted": false,
  "createdAt": "2024-03-20T10:00:00Z",
  "updatedAt": "2024-03-20T10:00:00Z"
}
```

### 建立待辦事項

```http
POST /api/Todo
```

#### 請求內容

```json
{
  "title": "新待辦事項",
  "description": "待辦事項描述",
  "isCompleted": false
}
```

#### 回應

```json
{
  "id": 2,
  "title": "新待辦事項",
  "description": "待辦事項描述",
  "isCompleted": false,
  "createdAt": "2024-03-20T10:00:00Z",
  "updatedAt": "2024-03-20T10:00:00Z"
}
```

### 更新待辦事項

```http
PUT /api/Todo/{id}
```

#### 參數

- `id` (路徑參數): 待辦事項 ID

#### 請求內容

```json
{
  "id": 1,
  "title": "更新後的標題",
  "description": "更新後的描述",
  "isCompleted": true
}
```

#### 回應

- 204 No Content

### 刪除待辦事項

```http
DELETE /api/Todo/{id}
```

#### 參數

- `id` (路徑參數): 待辦事項 ID

#### 回應

- 204 No Content

## GraphQL API

### 查詢

#### 取得所有待辦事項

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

#### 取得特定待辦事項

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

### 變更

#### 建立待辦事項

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

#### 更新待辦事項

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

#### 刪除待辦事項

```graphql
mutation {
  deleteTodo(id: 1)
}
```

## 錯誤處理

所有 API 端點在發生錯誤時都會返回適當的 HTTP 狀態碼和錯誤訊息：

- 400 Bad Request: 請求格式錯誤
- 404 Not Found: 資源不存在
- 500 Internal Server Error: 伺服器內部錯誤

錯誤回應格式：

```json
{
  "error": {
    "code": "ERROR_CODE",
    "message": "錯誤描述"
  }
}
```

## 認證與授權

目前 API 不需要認證即可使用。未來版本可能會加入 JWT 認證機制。

## 速率限制

API 目前沒有實作速率限制。建議合理使用，避免過度請求。

## 版本控制

API 版本通過 URL 路徑控制：

- 當前版本: `/api/Todo`
- 未來版本: `/api/v2/Todo`

## 支援

如有問題，請提交 Issue 或聯繫開發團隊。 