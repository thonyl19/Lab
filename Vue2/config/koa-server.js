const Koa = require('koa')

const appConfig = require('./../app.config')

const app = new Koa()
const uri = 'http://' + appConfig.appIp + ':' + appConfig.appPort

const server = app.listen(appConfig.appPort,appConfig.appIp,() => {
    console.log('Example app listening on ' + uri + '\n');
})

process.on('SIGTERM', () => {
    console.log('Stopping dev server')
    server.close(() => {
        process.exit(0)
    })
})