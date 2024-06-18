require('dotenv').config()
const express = require('express')
const router = express.Router()
const auth = require('../routes/user')
const connection = require('../dbconnection/connection')

//get list of projects
router.get('/', auth.authenticateToken, (req, res) => {
    allowed = ['Administrator', 'HR Manager', 'Project Manager', 'Employee']

    //do not allow users outside of list
    if (allowed.indexOf(req.userdata.Position) == -1)
        return res.sendStatus(401)

    let constrain = ''
    let distinct = req.userdata.Position == allowed[1] ? 'DISTINCT' : ''
    //apply constrains
    if (req.userdata.Position == allowed[3])
        constrain = `JOIN projectmember ON projectmember.Project_Id = project.Id WHERE projectmember.Employee_Id = ${req.userdata.Id}`
    else if (req.userdata.Position == allowed[2])
        constrain = `WHERE project.Manager_Id = ${req.userdata.Id}`
    else if (req.userdata.Position == allowed[1])
        constrain = `JOIN projectmember ON projectmember.Project_Id = project.Id JOIN employee ON employee.Id = projectmember.Employee_Id JOIN peoplepartner ON peoplepartner.Employee_Id = employee.Id WHERE peoplepartner.Partner_Id = ${req.userdata.Id}`

    connection.query(`SELECT ${distinct} project.*, project.ProjectType_Id - 1 AS ProjectTypeId FROM project ${constrain};`, (error, results) => {
        if (error) return res.sendStatus(500)
        res.json(results)
    })
})

//get list of employees assigned to project
router.get('/assigned', auth.authenticateToken, (req, res) => {
    allowed = ['Administrator', 'Project Manager']

    //do not allow users outside of list
    if (allowed.indexOf(req.userdata.Position) == -1)
        return res.sendStatus(401)

    //do not allow access to project that manager is not assigned to
    connection.query(`SELECT * FROM project WHERE id = ${req.body.projectid} AND Manager_Id = ${req.userdata.Id};`, (error, results) => {
        if (error) return res.sendStatus(500)
        if (req.userdata.Position != allowed[0] && results.length == 0) return res.sendStatus(401)

        connection.query(`SELECT employee.* FROM employee JOIN projectmember on projectmember.Employee_Id = employee.Id WHERE projectmember.Project_Id = ${req.body.projectid};`, (error, results) => {
            if (error) return res.sendStatus(500)
            res.json(results)
        })
    })
})

//gets project types
router.get('/types', auth.authenticateToken, (req, res) => {
    allowed = ['Administrator', 'Project Manager']

    //do not allow users outside of list
    if (allowed.indexOf(req.userdata.Position) == -1)
        return res.sendStatus(401)

    connection.query(`SELECT Name FROM projecttype;`, (error, results) => {
        if (error) return res.sendStatus(500)
        res.json(results)
    })
})

//submit new project
router.post('/submit', auth.authenticateToken, (req, res) => {
    allowed = ['Administrator', 'Project Manager']

    //do not allow users outside of list
    if (allowed.indexOf(req.userdata.Position) == -1)
        return res.sendStatus(401)

    //administrator can make anyone head of project
    manager = req.userdata.Position == allowed[0] ? req.body.manager : req.userdata.Id

    //add new project wit query
    connection.query(`INSERT INTO project (Name, ProjectType_Id, StartDate, EndDate, Manager_Id, Comment, Status) VALUES ('${req.body.name}', '${req.body.projecttype}', '${req.body.startdate}', '${req.body.enddate}', '${manager}', '${req.body.comment}', 1);`, (error) => {
        if (error) return res.sendStatus(500)
        connection.query(`SELECT Id FROM project WHERE Name = '${req.body.name}' AND ProjectType_Id = '${req.body.projecttype}' AND StartDate = '${req.body.startdate}' AND EndDate = '${req.body.enddate}' AND Manager_Id = '${manager}' AND Comment = '${req.body.comment}' AND Status = 1;`, (error, results) => {
            if (error) return res.sendStatus(500)
            res.status(200).send("Project created with id:" + results[0].Id)
        })
    })
})

//edit project
router.post('/edit', auth.authenticateToken, (req, res) => {
    allowed = ['Administrator', 'Project Manager']

    //do not allow users outside of list
    if (allowed.indexOf(req.userdata.Position) == -1)
        return res.sendStatus(401)

    //administrator can make anyone head of project
    manager = req.userdata.Position == allowed[0] ? req.body.manager : req.userdata.Id

    //check if project manager is owner of project
    connection.query(`SELECT * FROM project WHERE project.Manager_Id = ${req.userdata.Id} AND project.Id = ${req.body.projectid};`, (error, results) => {
        if (error) return res.sendStatus(500)
        if (req.userdata.Position != allowed[0] && results.length == 0) return res.sendStatus(400)

        connection.query(`UPDATE project SET ProjectType_Id = '${req.body.projecttype}', StartDate = '${req.body.startdate}', EndDate = '${req.body.enddate}', Manager_Id = '${manager}', Comment = '${req.body.comment}', Status = '${req.body.status}' WHERE Id = '${req.body.projectid}';`, (error) => {
            if (error) return res.sendStatus(500)
            res.sendStatus(200)
        })
    })
})

router.post('/remove', auth.authenticateToken, (req, res) => {
    allowed = ['Administrator']

    //do not allow users outside of list
    if (allowed.indexOf(req.userdata.Position) == -1)
        return res.sendStatus(401)

    //remove members then project
    connection.query(`DELETE FROM projectmember WHERE Project_Id = ${req.body.id};`, (error) => {
        if (error) return res.sendStatus(500)
        connection.query(`DELETE FROM project WHERE Id = ${req.body.id};`, (error) => {
            if (error) return res.sendStatus(500)
            res.sendStatus(200)
        })
    })
})

//get list of projects
router.post('/assign', auth.authenticateToken, (req, res) => {
    allowed = ['Administrator', 'Project Manager']

    //do not allow users outside of list
    if (allowed.indexOf(req.userdata.Position) == -1)
        return res.sendStatus(401)

    //check if employee is member of project already
    connection.query(`SELECT * FROM projectmember WHERE employee_Id = ${req.body.employeeid} AND project_Id = ${req.body.projectid};`, (error, results) => {
        if (error) return res.sendStatus(500)
        if (results.length > 0) return res.sendStatus(400)

        connection.query(`INSERT INTO projectmember(employee_Id, project_Id) VALUES (${req.body.employeeid}, ${req.body.projectid});`, (error) => {
            if (error) return res.sendStatus(500)
            res.sendStatus(200)
        })
    })
})

//get list of projects
router.post('/unassign', auth.authenticateToken, (req, res) => {
    allowed = ['Administrator', 'Project Manager']

    //do not allow users outside of list
    if (allowed.indexOf(req.userdata.Position) == -1)
        return res.sendStatus(401)

    //check if employee is member of project
    connection.query(`SELECT * FROM projectmember WHERE employee_Id = ${req.body.employeeid} AND project_Id = ${req.body.projectid};`, (error, results) => {
        if (error) return res.sendStatus(500)
        if (results.length == 0) return res.sendStatus(400)
        //check if project manager is owner of project
        connection.query(`SELECT * FROM projectmember JOIN project ON project.Id = projectmember.Project_Id WHERE employee_Id = ${req.body.employeeid} AND project_Id = ${req.body.projectid} AND project.Manager_Id = ${req.userdata.Id};`, (error, results) => {
            if (error) return res.sendStatus(500)
            if (req.userdata.Position != allowed[0] && results.length == 0) return res.sendStatus(400)

            connection.query(`DELETE FROM projectmember WHERE employee_Id = ${req.body.employeeid} AND project_Id = ${req.body.projectid};`, (error) => {
                if (error) return res.sendStatus(500)
                res.sendStatus(200)
            })
        })
    })
})

module.exports = {
    router
}