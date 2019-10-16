---
title: TTTT
home: true
heroText: "Springfield Dim Sum -- YAML mode"
---

```
//指定,使用 Home.vue 做為填充的基礎
home: true

//將 heroText 設定成 “Springfield Dim Sum”
heroText: Springfield Dim Sum
```

| Tables        | Are           | Cool  |
| ------------- |:-------------:| -----:|
| col 3 is      | right-aligned | $1600 |
| col 2 is      | centered      |   $12 |
| zebra stripes | are neat      |    $1 |

## Emoji
>:tada: :100:
>[Emoji 其他可選用的圖示](https://www.webfx.com/tools/emoji-cheat-sheet/)


## 目錄
[[toc]]
> 需要在 markdown 語法中, 使用 ##兩階層以上的,才會被 generate 成目錄

::: tip
This is a tip
:::

::: warning 注意
This is a warning
:::

> ::: danger :fire:STOP
> - This is a dangerous warning
> :::

## 代碼區塊的高亮
``` js{4}
export default {
  data () {
    return {
      msg: 'Highlighted!'
    }
  }
}
```

## 引入程式碼檔案
```
<<< @/docs/指南/Markdown拓展/code.js
```
<<< @/docs/指南/Markdown拓展/test.sh
> 標注 指定行號 3 or 3-5 <br>
> Ex: @/docs/指南/Markdown拓展/code.js{3-5}

<<< @/docs/指南/Markdown拓展/code.js{3-5}