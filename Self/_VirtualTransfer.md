# _VirtualTransfer_.cshtml 分析報告

## Source Metadata
- **檔案路徑**: `n:\CUB_Dev\Genesis_MVC\Areas\Example\Views\Self\_VirtualTransfer_.cshtml`
- **類型**: ASP.NET MVC Partial View (包含 Vue 元件定義)

## 1. 元件架構 (Architecture)

此檔案定義了一個名為 `virtual-transfer` 的 Vue 自訂元件，旨在實現高效能的穿梭框 (Transfer) 介面。它主要解決了大量資料在傳統穿梭框中渲染緩慢的問題。

### 核心組成
- **依賴套件**: `vue-virtual-scroll-list` (註冊為 `virtual-list` 元件)。
- **內建子元件 `item`**: 用於 `virtual-list` 的行渲染元件，包含單一 `el-checkbox` 及標籤顯示。
- **模板設計**:
    - **左側面板**: 顯示「未授權」的資料。
    - **右側面板**: 顯示「已授權」的資料。
    - **中間按鈕**: 提供資料移入/移出的動作按鈕 (`>` 與 `<`)。

## 2. 屬性與事件 (Interface)

| 屬性 (Props) | 類型 | 預設值 | 說明 |
| :--- | :--- | :--- | :--- |
| `value` | `Array` | - | 已選取的 ID 列表 (對應右側面板)，支援 `v-model`。 |
| `data` | `Array` | - | 原始資料所有項目列表。 |
| `props` | `Object` | `{ key: 'id', label: 'name' }` | 配置欄位映射，定義哪一個 Property 作為 ID 以及哪一個作為 Display Name。 |

### 事件 (Events)
- **`input`**: 當資料發生遷移時觸發，回傳更新後的 `value` (已選取 ID 陣列)。

## 3. 邏輯處理 (Logic)

### 資料分組 (Computed)
- **`leftList` / `rightList`**: 根據 `value` (ID 陣列) 是否包含該項目進行分組。基於效能考量使用了 `Set` 物件進行 `has` 判斷。
- **`filteredLeft` / `filteredRight`**: 根據各自的搜尋關鍵字 (`leftKeyword`/`rightKeyword`) 對清單進行模糊搜尋過濾。

### 操作行為 (Methods)
- **`toggle(item)`**: 在 `selectedSet` 中追蹤當前單選/多選的狀態，但不直接更動 `value`。只有在點擊中間按鈕時才生效。
- **`toRight()`**: 將 `selectedSet` 中的 ID 加入 `value` 並透過 `$emit('input')` 通知父元件。
- **`toLeft()`**: 從 `value` 中移除 `selectedSet` 中的 ID 並透過 `$emit('input')` 通知父元件。

## 4. UI/UX 與 樣式 (Styling)

- **高效能渲染**: 使用虛擬列表控制 DOM 數量 (固定顯示 20 筆)。
- **樣式定義**:
    - 使用 `flex` 佈局實現三欄式結構。
    - 面板有框線，列表部分有 `overflow: auto` 並固定高度為 `400px`。
    - 關鍵字搜尋欄位嵌入在面板頂部。

## 5. 關聯參考
- 使用元件: `virtual-list` (第三方)
- 使用元件: `el-checkbox`, `el-button` (Element UI)
- eBundle ,必須有相應的配置
