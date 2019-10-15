module.exports = {
  locales:{
    '/':{
      lang:'zh-cn',
      title: "Hello VuePress",
      description: "Just playing around",
    }
  },
  configureWebpack: {
    resolve: {
      alias: {
        img: "../docs/static/img/"
      }
    }
  },

  markdown: {
    lineNumbers: true
  }
};
