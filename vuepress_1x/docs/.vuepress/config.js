//themeConfig_i18n
let locales = {
  
    '/': {
      selectText: '選擇語言',
      label: '中文',
      editLinkText: '在 GitHub 上编辑此页',
      serviceWorker: {
        updatePopup: {
          message: "發现新内容可用.",
          buttonText: "刷新"
        }
      },
      algolia: {},
      nav: [
        { text: '主頁', link: '/' }
      ],
      sidebar: {
        '/': [/* ... */],
        
      }
    },
    '/en/': {
      // 多语言下拉菜单的标题
      selectText: 'Languages',
      // 该语言在下拉菜单中的标签
      label: 'English',
      // 编辑链接文字
      editLinkText: 'Edit this page on GitHub',
      // Service Worker 的配置
      serviceWorker: {
        updatePopup: {
          message: "New content is available.",
          buttonText: "Refresh"
        }
      },
      // 当前 locale 的 algolia docsearch 选项
      algolia: {},
      nav: [
        { text: 'Home', link: '/' }
      ],
      sidebar: {
        '/zh/': [/* ... */],
        '/zh/nested/': [/* ... */]
      }
    }
};
module.exports = {
  locales:{
    '/':{
      lang:'zh-cn',
      title: "這是 Vuepress 1.x 練習",
      description: "Just playing around",
    },
    '/en/':{
      lang:'en-US',
      title: "This is training for Vuepress 1.x",
      description: "Just playing around",
    }
  },
  configureWebpack: {
    resolve: {
      alias: {
        '@img': "../../static/img/"
      }
    }
  },
  themeConfig: {
    locales
    , lastUpdated: 'Last Updated'
  },
  markdown: {
    lineNumbers: true
  }
};
