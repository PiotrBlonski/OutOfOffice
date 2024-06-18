const express = require("express")
var bodyParser = require('body-parser');
const app = express()
app.use(bodyParser.urlencoded({ extended: false }));
app.use(bodyParser.json());
app.set('view engine', 'ejs')

//register routes
const userRouter = require('./routes/user').router
const projectsRouter = require('./routes/projects').router
const requestsRouter = require('./routes/requests').router
const employeesRouter = require('./routes/employees').router

app.use('/user', userRouter)
app.use('/projects', projectsRouter)
app.use('/requests', requestsRouter)
app.use('/employees', employeesRouter)

app.get('/', (req, res) => {
    res.render('index')
})

app.listen(3000)

/* https should be used if certificate is obtained
const https = require("https");
const fs = require("fs");
https.createServer({
    key: fs.readFileSync("key.pem"),
    cert: fs.readFileSync("cert.pem"),
}, app)
    .listen(3000, () => {
        console.log('server is runing at port 3000')
    }); */