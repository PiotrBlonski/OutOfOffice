require('dotenv').config()
const express = require('express')
const router = express.Router()
const auth = require('../routes/user')
const connection = require('../dbconnection/connection')
const path = require("path")
const fs = require('fs')
const multer = require('multer')
var imagesPath = path.resolve("images")

router.use(express.json())

//get list of positions in company
router.get('/positions', auth.authenticateToken, (req, res) => {
    connection.query(`SELECT Name FROM employeeposition;`, (error, results) => {
        if (error) return res.sendStatus(500)
        res.json(results)
    })
})

//get list of subdivisions in company
router.get('/subdivisions', auth.authenticateToken, (req, res) => {
    connection.query(`SELECT Name FROM subdivision;`, (error, results) => {
        if (error) return res.sendStatus(500)
        res.json(results)
    })
})

//get data of currently logged user
router.get('/me', auth.authenticateToken, (req, res) => {
    connection.query(`SELECT employee.Id as Id, employee.Name AS Name, Subdivision_Id, Position_Id, Status, Balance, peoplepartner.Partner_Id AS Partner_Id FROM employee LEFT JOIN peoplepartner ON employee.id = peoplepartner.employee_id WHERE employee.Id = ${req.userdata.Id};`, (error, results) => {
        if (error) return res.sendStatus(500)
        res.json(results[0])
    })
})

//get list of employees
router.get('/', auth.authenticateToken, (req, res) => {
    allowed = ['Administrator', 'HR Manager', 'Project Manager']

    //do not allow users outside of list
    if (allowed.indexOf(req.userdata.Position) == -1)
        return res.sendStatus(401)

    let constrain = ''
    //hr can only access assigned employees, project maanger can access lower rank employees
    if (req.userdata.Position == allowed[1])
        constrain = `WHERE peoplepartner.Partner_Id = ${req.userdata.Id}`
    else if (req.userdata.Position == allowed[2])
        constrain = `WHERE employee.Position_Id = 1`

    connection.query(`SELECT employee.Id as Id, Name, Subdivision_Id, Position_Id, Status, Balance, peoplepartner.Partner_Id AS Partner_Id FROM employee LEFT JOIN peoplepartner ON employee.id = peoplepartner.employee_id  ${constrain};`, (error, results) => {
        if (error) return res.sendStatus(500)
        if (results.length == 0) res.sendStatus(401)
        res.json(results)
    })
})

//get employee by id
router.get('/:id', auth.authenticateToken, (req, res) => {
    allowed = ['Administrator', 'HR Manager', 'Project Manager']

    //do not allow users outside of list
    if (allowed.indexOf(req.userdata.Position) == -1)
        return res.sendStatus(401)

    constrain = `WHERE employee.Id = ${req.params.id}`
    //hr can only access assigned employee, project maanger can access lower rank employee, employee can access their own data
    if (req.userdata.Position == allowed[1])
        constrain = `WHERE peoplepartner.Partner_Id = ${req.userdata.Id} AND employee.Id = ${req.params.id}`
    else if (req.userdata.Position == allowed[2])
        constrain = `WHERE employee.Position_Id = 1 AND employee.Id = ${req.params.id}`

    connection.query(`SELECT employee.Id AS Id, Name, Subdivision_Id, Position_Id, Status, Balance, peoplepartner.Partner_Id AS Partner_Id FROM employee LEFT JOIN peoplepartner ON employee.id = peoplepartner.employee_id ${constrain};`, (error, results) => {
        if (error) return res.sendStatus(500)
        if (results.length == 0) res.sendStatus(401)
        res.json(results[0])
    })
})

//edit employee
router.post('/edit', auth.authenticateToken, (req, res) => {
    allowed = ['Administrator', 'HR Manager']

    //do not allow users outside of list
    if (allowed.indexOf(req.userdata.Position) == -1)
        return res.sendStatus(401)

    constrain = `WHERE employee.Id = ${req.body.id}`
    JOINedtable = ''

    //set constrain for HR Manager
    if (req.userdata.Position == allowed[1]) {
        constrain = `WHERE peoplepartner.Partner_Id = ${req.userdata.Id} AND employee.Id = ${req.body.id}`
        JOINedtable = `JOIN peoplepartner ON peoplepartner.Employee_Id = employee.Id`
    }

    //only admin can choose position
    let position = req.userdata.Position == allowed[1] ? 1 : req.body.position
    connection.query(`UPDATE employee ${JOINedtable} SET Name = '${req.body.name}', Subdivision_Id = '${req.body.subdivision}', Position_Id = '${position}', Status = '${req.body.status}', Balance = '${req.body.balance}' ${constrain};`, (error, results) => {
        if (error) return res.sendStatus(500)
        if (results.length == 0) res.sendStatus(401)

        connection.query(`DELETE FROM peoplepartner WHERE employee_Id = ${req.body.id};`)

        let partner = req.userdata.Position == allowed[1] ? req.userdata.Id : req.body.partner
        connection.query(`INSERT INTO peoplepartner (Employee_Id, Partner_Id) VALUES (${req.body.id}, ${partner});`, (error) => {
            if (error) return res.sendStatus(500)
            res.sendStatus(200)
        })
    })
})

router.get('/avatar/:id', (req, res) => {
    connection.query(`SELECT COUNT(*) FROM employee;`, async (error, result) => {
        if (error || result[0]['COUNT(*)'] < req.params.id) return res.sendStatus(404)

        var path = `${imagesPath}\\users\\${req.params.id}`
        var defaultpath = `${imagesPath}\\users\\0`

        if (!fs.existsSync(path))
            fs.mkdirSync(path)

        ReturnFile(path, defaultpath, res)
    })
})

router.post('/avatar/:id', auth.authenticateToken, multer({ dest: path.resolve("images\\users\\") }).single('file'), (req, res) => {
    allowed = ['Administrator', 'HR Manager']

    //do not allow users outside of list
    if (allowed.indexOf(req.userdata.Position) == -1)
        return res.sendStatus(401)

    var failed
    connection.query(`SELECT Id FROM employee WHERE Id = ${req.params.id};`, (error, results) => {
        if (error || results.length == 0) return res.sendStatus(500)
        var userfolder = req.file.destination + "\\" + req.params.id
        fs.rename(req.file.destination + "\\" + req.file.filename, userfolder + "\\avatar" + req.file.originalname.substring(req.file.originalname.lastIndexOf('.')), (error) => {
            if (error) fs.rm(req.file.path, () => { failed = true })
            failed ? res.sendStatus(500) : res.sendStatus(200)
        })
    })
})

function ReturnFile(path, defaultpath, res) {
    fs.open(path, 'r', (err, fd) => {
        if (err || !fs.existsSync(path))
            return res.sendStatus(404)

        var files = fs.readdirSync(path)

        if (files.length == 0) {
            files = fs.readdirSync(defaultpath)
            return res.sendFile(defaultpath + "\\" + files[0])
        }

        return res.sendFile(path + "\\" + files[0])
    })
}

module.exports = {
    router
}