# 可以正常顯示的路徑
![An image](../../static/img/test.png)

# configureWebpack  @alias 無法作用的路徑
```html
![An image](~@alias/test.png)
```
# $withBase 無法作用的路徑
```html
<img :src="$withBase('/assets/img/test.png')" alt="foo">
```