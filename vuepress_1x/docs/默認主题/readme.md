---
sidebar: auto
sidebarDepth: 3
---

# Config Reference
## 重點摘要
### Study by
> https://v0.vuepress.vuejs.org/zh/config/

### 首頁
> - home: true

### 導航欄
> - 導航欄的配置可以在 config.js  或在各別的頁面上,使用 YAML front matter 方式指定是否啟用.
>> [在各別頁面關閉導航欄的範例](在各別頁面關閉導航欄的範例.html?_blank)

### 側邊欄
> - 配置與 導航欄一樣,可以在 config 或頁面 各別設定.


### 搜索框
> - 預設的搜索只有 頁面的標题、h2 和 h3 ,全文搜索需要另外使用 Algolia 搜索,
>      預設的搜索是開箱即用,但 Algolia 就需要再另外配置

### 上/下一篇鏈接


### Git 倉庫和编辑链接
> - 只限於 config.js 中 連接的文件位置.

### 特定页面的自定義布局
> - 使用 yaml ,必須注意其格式限制
>> 1. 行與行之間,必須是 換行做分隔,不能摻雜逗號
>> 2. 參數名稱之後直接接 : 符號 ,且必須與後面的變數值空一格做分開.
>> [自定義布局](自定義布局)