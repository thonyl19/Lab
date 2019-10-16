# 可以正常顯示的路徑
![An image](../../static/img/test.png)

# configureWebpack  @img 無法作用的路徑
```html
![An image](~@img/test.png)
```
![An image](~@img/test.png)
::: tip 
    經確認,問題的原因是因為 config.js 的存置位置有誤([請參閱 自定義主題 的 心得說明 1.](/指南/#自定義主題))
:::

{{$withBase("/test.png")}}
# $withBase 無法作用的路徑
```html
<img :src="$withBase('/assets/img/test.png')" alt="foo">
```
<img :src="$withBase('/assets/img/test.png')" alt="foo">