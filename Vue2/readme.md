### Note

    (官網)[https://koajs.com/]



### 起手式

安装 `koa`
```
npm i koa -D
```

### 基本用例 
    (base_server.js)[./config/base_server.js]

### Cascading
    (base_server.js)[./config/ex_Cascading.js]

### app.listen（...）

### 錯誤處理
```
app.on('error', err => {
  log.error('server error', err)
});
```