
## 重點摘要
#### Study by
> https://v0.vuepress.vuejs.org/zh/guide/

#### [快速上手](https://v0.vuepress.vuejs.org/zh/guide/getting-started.html#%E5%85%A8%E5%B1%80%E5%AE%89%E8%A3%85)
> 在 vuepress 己做全域安裝的前提下 ,在指定的目錄內,建立 新的 vuepress. 
> ```bash
> npm i - D vuepress
> ```

#### [基本配置](https://v0.vuepress.vuejs.org/zh/guide/basic-config.html#%E9%85%8D%E7%BD%AE%E6%96%87%E4%BB%B6)

#### [静態资源](https://v0.vuepress.vuejs.org/zh/guide/assets.html#%E7%9B%B8%E5%AF%B9%E8%B7%AF%E5%BE%84) 
> - configureWebpack
> - $withBase
> - :no_bell: [有問題還未排除的試例](基礎路徑/)


#### [Markdown 拓展](https://v0.vuepress.vuejs.org/zh/guide/markdown.html#header-anchors)
> - 資料夾內的 README.md 文件，会被自动编译为 index.html
> - 如果指向的路徑沒有指定文件, 雖然預設會讀取 index.html ,但如果路徑結尾沒有設定 / ,則會產生 404 的錯誤 
> - 路徑跳轉的配置方式,如同 本 README.md 中配置.
> - [yaml 和其他 試例](Markdown拓展/yaml.md)
> - [Json 試例](Markdown拓展/json.md)
> - :bell: [markdown - lineNumbers](Markdown拓展/lineNumbers-err.md)
> - :electric_plug: [markdown-it](https://github.com/markdown-it/)
> 
#### [在 Markdown 中 使用 Vue](https://v0.vuepress.vuejs.org/zh/guide/using-vue.html) 
> - :electric_plug: 瀏覽器的 API 訪問限制
> - [模板语法 / 使用组件 / Badge](Markdown_Vue/)


#### [多语言支持](https://v0.vuepress.vuejs.org/zh/guide/i18n.html)
> - [themeConfig 的配置](多语言支持/)


#### [自定義主題](https://v0.vuepress.vuejs.org/zh/guide/custom-themes.html))
> - [x](自定義主題/)
> ::: tip 心得
> 1. config.js 必須存置在 docs/.vuepress/ 之下,才會有真正作用,如果出現 themeConfig 設定了沒作用,
>   很可能就是 config.js 位置放錯造成的問題.
> ::: 


#### []()
