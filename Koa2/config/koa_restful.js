const koa = require('koa');
const router = require('koa-router')();
const bodyParser = require('koa-bodyparser');
const apiRouter = require('../router');

const app = new koa();

// 首页
const index = router.get('/', ctx => {
    ctx.response.body = 'hello world';
}).routes();

app.use(index);
app.use(bodyParser());
app.use(apiRouter.routes());

app.listen(3000);
 