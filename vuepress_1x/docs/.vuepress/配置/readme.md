
## 重點摘要
#### Study by
> https://v0.vuepress.vuejs.org/zh/config/

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


#### [自定義主題](https://v0.vuepress.vuejs.org/zh/guide/custom-themes.html)
> - [readme](自定義主題/)
> ::: tip 心得
> 1. config.js 必須存置在 docs/.vuepress/ 之下,才會有真正作用,如果出現 themeConfig 設定了沒作用,
>   很可能就是 config.js 位置放錯造成的問題.
> ::: 


#### [部署](https://v0.vuepress.vuejs.org/zh/guide/deploy.html)
> 各個部署的工具和方式,都有稍微的研究一下並筆記如下
> #### GitHub Pages
> #### GitLab Pages and GitLab CI
>> 沒有相關的環境
> #### Netlify
>> [netlify 超佛心的靜態網站hosting服務] (https://blog.alantsai.net/posts/2018/07/migrate-blog-to-ssg-demo-devops-8-netlify-free-static-site-hosting-service)
> #### Heroku 
>> [摘錄：雖然 Heroku 有提供免費使用額度，但因為免費的機器有「30 分鐘沒有人連線的話就會進入休眠」以及「每天至少要休眠 8 小時」的限制.....](https://ithelp.ithome.com.tw/articles/10189021)
> #### Google Firebase
> #### Surge
> #### Ref
>> [靜態網頁部署 Github pages、Surge.sh、Netlify](https://medium.com/cypressyi-technote/%E9%9D%9C%E6%85%8B%E7%B6%B2%E9%A0%81%E9%83%A8%E7%BD%B2-github-pages-surge-sh-netlify-8bc8ac7123b0)