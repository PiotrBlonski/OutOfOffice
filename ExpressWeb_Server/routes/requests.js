require('dotenv').config()
const express = require('express')
const router = express.Router()
const auth = require('../routes/user')
const connection = require('../dbconnection/connection')

//get list of leave requests
router.get('/leave', auth.authenticateToken, (req, res) => {
    allowed = ['Employee', 'Administrator', 'HR Manager', 'Project Manager']

    //do not allow users outside of list
    if (allowed.indexOf(req.userdata.Position) == -1)
        return res.sendStatus(401)

    //Default constrain
    let constrain = `WHERE Employee.login = '${req.user.name}'`

    //change constrain to none if user is administrator
    if (req.userdata.Position == allowed[1])
        constrain = ''
    //project manager is limited only to records of employees involved in their project
    else if (req.userdata.Position == allowed[3])
        constrain = `JOIN projectmember ON employee.id = projectmember.employee_id JOIN project ON projectmember.project_id = project.id WHERE project.manager_id = '${req.userdata.Id}'`
    //hr manager is limited to their assigned employees
    else if (req.userdata.Position == allowed[2])
        constrain = `JOIN peoplepartner ON employee.id = peoplepartner.employee_id WHERE peoplepartner.Partner_Id = '${req.userdata.Id}'`

    //final query including constrain
    connection.query(`SELECT leaverequest.*, employee.Name AS Owner FROM leaverequest JOIN Employee on employee_Id = Employee.Id ${constrain};`, (error, results) => {
        if (error) return res.sendStatus(500)
        res.json(results)
    })
})

//submit leave request
router.post('/leave/submit', auth.authenticateToken, (req, res) => {
    //List of positions allowed to submit leave request
    allowed = ['Employee']

    //check for roles with submit permissions
    if (allowed.indexOf(req.userdata.Position) == -1)
        return res.sendStatus(401)

    //final query submitting request
    connection.query(`INSERT INTO leaverequest (Employee_Id, Reason_Id, StartDate, EndDate, Comment, Status) VALUES ('${req.userdata.Id}', '${req.body.reason}', '${req.body.startdate}', '${req.body.enddate}', '${req.body.comment}', 1);`, (error) => {
        if (error) return res.sendStatus(500)
        res.sendStatus(200)
    })
})


//edit leave request
router.post('/leave/edit', auth.authenticateToken, (req, res) => {
    //query checks if criteria (owning entry and being owner) for updating are met
    connection.query(`SELECT * FROM leaverequest WHERE Id = '${req.body.Id}' AND Employee_Id = '${req.userdata.Id}';`, (error, results) => {
        if (error) return res.sendStatus(500)
        if (results.length == 0) return res.sendStatus(401) //if they are not disallow submitting

        //Update record if criteria are met
        connection.query(`UPDATE leaverequest SET Reason_Id = '${req.body.reason}', StartDate = '${req.body.startdate}', EndDate = '${req.body.enddate}', Comment = '${req.body.comment}' WHERE Id = '${req.body.Id}';`, (error) => {
            if (error) return res.sendStatus(500)
            res.sendStatus(200)
        })
    })
})

//canceling leave requests
router.post('/leave/cancel', auth.authenticateToken, (req, res) => {
    //query checks if criteria (owning entry and being owner) for updating are met
    connection.query(`SELECT * FROM leaverequest WHERE Id = '${req.body.Id}' AND Employee_Id = '${req.userdata.Id}';`, (error, results) => {
        if (error) return res.sendStatus(500)
        if (results.length == 0) return res.sendStatus(401) //if criteria are not met disallow submitting

        //Update record if criteria are met
        connection.query(`UPDATE leaverequest SET Status = '3' WHERE Id = '${req.body.Id}';`, (error) => {
            if (error) return res.sendStatus(500)
            res.sendStatus(200)
        })
    })
})

//changing status of leave requests approving or denying by higher ups
router.post('/leave/changestatus', auth.authenticateToken, (req, res) => {
    //positions with any access
    allowed = ['Administrator', 'HR Manager', 'Project Manager']

    //do not allow users outside of list
    if (allowed.indexOf(req.userdata.Position) == -1)
        return res.sendStatus(401)

    let constrain = `WHERE leaverequest.Id = ${req.body.leaveid}`
    //project manager can only change status of employees assigned to their projects
    if (req.userdata.Position == allowed[2])
        constrain = `JOIN projectmember ON employee.id = projectmember.employee_id JOIN project ON projectmember.project_id = project.id WHERE project.manager_id = '${req.userdata.Id}' AND leaverequest.Id = '${req.body.leaveid}'`
    //hr manager can only changed their employees status
    else if (req.userdata.Position == allowed[1])
        constrain = `JOIN peoplepartner ON employee.id = peoplepartner.employee_id WHERE peoplepartner.Partner_Id = '${req.userdata.Id}' AND leaverequest.Id = ${req.body.leaveid}`

    //check if user can edit the record in case they shouldn not be able to access this record
    connection.query(`SELECT * FROM leaverequest join reason on leaverequest.Reason_Id = reason.Id join Employee on employee_Id = Employee.Id ${constrain};`, (error, results) => {
        if (error) return res.sendStatus(500)
        if (results.length < 1) return res.sendStatus(401) //if criteria are not met do not go further

        var startdate = new Date(results[0].StartDate)
        var enddate = new Date(results[0].EndDate)
        //finally change status of leave request either by approving or denying
        connection.query(`INSERT INTO approvalrequest (Approver_Id, LeaveRequest_Id, Status, Comment) VALUES ('${req.userdata.Id}', '${req.body.leaveid}', '${req.body.status}', '${req.body.comment}');`, (error) => {
            if (error) return res.sendStatus(500)
            
            let adjustment = req.body.status == 1 ? (enddate.getTime() - startdate.getTime()) / (1000 * 3600 * 24) : 0;
            //update status of leave request to reviewed and update employee balance
            connection.query(`UPDATE leaverequest, employee SET leaverequest.Status = 2, employee.Balance = employee.Balance - ${adjustment} WHERE leaverequest.Id = '${req.body.leaveid}' AND employee.Id = leaverequest.Employee_Id;`, (error) => {
                if (error) return res.sendStatus(500)
                res.sendStatus(200)
            })
        })
    })
})

//changing status of leave requests approving or denying by higher ups
router.post('/leave/remove', auth.authenticateToken, (req, res) => {
    //positions with any access
    allowed = ['Administrator']

    //do not allow users outside of list
    if (allowed.indexOf(req.userdata.Position) == -1)
        return res.sendStatus(401)

    //remove approval then leave request
    connection.query(`DELETE FROM approvalrequest WHERE LeaveRequest_Id = ${req.body.Id};`, (error) => {
        if (error) return res.sendStatus(500)
        connection.query(`DELETE FROM leaverequest WHERE Id = ${req.body.Id};`, (error) => {
            if (error) return res.sendStatus(500)
            res.sendStatus(200)
        })
    })
})

//get approval requests of leave requests
router.get('/approval', auth.authenticateToken, (req, res) => {
    allowed = ['Administrator', 'HR Manager', 'Project Manager']

    //do not allow users outside of list
    if (allowed.indexOf(req.userdata.Position) == -1)
        return res.sendStatus(401)

    let constrain = ''
    //project manager is limited only to records of employees involved in their project
    if (req.userdata.Position == allowed[2])
        constrain = `JOIN projectmember ON employee.id = approvalrequest.Approver_Id JOIN project ON projectmember.project_id = project.id WHERE project.manager_id = '${req.userdata.Id}'`
    //hr manager is limited to their assigned employees
    else if (req.userdata.Position == allowed[1])
        constrain = `JOIN peoplepartner ON employee.id = approvalrequest.Approver_Id WHERE peoplepartner.Partner_Id = '${req.userdata.Id}'`

    //get data with criteria
    connection.query(`SELECT approvalrequest.*, employee.Name AS Approver FROM approvalrequest JOIN leaverequest ON LeaveRequest_Id = leaverequest.id JOIN Employee ON approvalrequest.Approver_Id = Employee.Id ${constrain};`, (error, results) => {
        if (error) return res.sendStatus(500)
        res.json(results)
    })
})

//get list of possible reason of leave request to choose from
router.get('/reasons', (req, res) => {
    connection.query(`SELECT Name FROM reason;`, (error, results) => {
        if (error) return res.sendStatus(500)
        res.json(results)
    })
})

module.exports = {
    router
}