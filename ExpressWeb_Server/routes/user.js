require('dotenv').config()
const express = require('express')
const router = express.Router()
const jwt = require('jsonwebtoken');
const bcrypt = require('bcrypt')
const connection = require('../dbconnection/connection')

router.use(express.json());

//create new user
router.post('/create', authenticateToken, checkDataStrength, (req, res) => {
    allowed = ['Administrator', "HR Manager"]

    //do not allow users outside of list
    if (allowed.indexOf(req.userdata.Position) == -1)
        return res.sendStatus(401)

    connection.query(`SELECT * FROM employee WHERE login = '${req.body.login}';`, async (error, results) => {
        if (error) return res.sendStatus(500)
        if (results.length > 0) return res.status(400).send("Login exists")

        //hash password to store in database
        const hashedPassword = await bcrypt.hash(req.body.password, 10)

        //only admin can choose position
        let position = req.userdata.Position == allowed[1] ? 1 : req.body.position
        //create new record in database
        connection.query(`INSERT INTO employee (Name, Subdivision_Id, Position_Id, Status, Balance, login, password) VALUES ('${req.body.name}', '${req.body.subdivision}', '${position}', '${req.body.status}', '${req.body.balance}', '${req.body.login}', '${hashedPassword}');`, async (error) => {
            if (error) return res.sendStatus(500)
            if (req.body.partner != null || req.userdata.Position == allowed[1]) {
                let partner = req.userdata.Position == allowed[1] ? req.userdata.Id : req.body.partner
                connection.query(`SELECT * FROM employee WHERE login = '${req.body.login}';`, (error, results) => {
                    if (error) return res.sendStatus(500)
                    connection.query(`INSERT INTO peoplepartner (Employee_Id, Partner_Id) VALUES ('${results[0].Id}', '${partner}');`, (error) => {
                        console.log(error)
                        if (error) return res.sendStatus(500)
                        res.status(200).send("User Created")
                    })
                })
            }
            else res.status(200).send("User Created")
        })
    })
})

//login using login and password
router.post('/login', (req, res) => {
    connection.query(`SELECT * FROM employee WHERE login = '${req.body.login}';`, async (error, results) => {
        if (error) return res.sendStatus(500)
        //check if user exists and password is valid
        if (results.length == 0 || req.body.password == null || !await bcrypt.compare(req.body.password, results[0].Password))
            return res.status(401).send("Incorrect login or password")

        //if account status is set to inactive do not allow logging in
        if (results[0].Status == 2)
            return res.status(401).send("Account is Inactive")

        //generate new token for user
        const user = { name: req.body.login }
        const accessToken = generateAccessToken(user)
        const refreshToken = jwt.sign(user, process.env.REFRESH_TOKEN_SECRET);

        //update token in database
        connection.query(`UPDATE employee SET RefreshToken = '${refreshToken}' WHERE login = '${req.body.login}';`, (error) => {
            if (error) return res.sendStatus(500)
            res.json({ accesstoken: accessToken, refreshtoken: refreshToken })
        })
    })
})

//logout user
router.post('/logout', authenticateToken, (req, res) => {
    //if tokenm got authenticated remove token from database to prevent access to any data
    connection.query(`UPDATE employee SET RefreshToken = NULL WHERE login = '${req.user.name}';`, (error) => {
        if (error) return res.sendStatus(500)
        res.status(200).send("User logged out")
    })
})

//change password when authorized
router.post('/changepassword', authenticateToken, async (req, res) => {
    if (req.body.newPassword.length < 6)
        return res.send("New password is too weak").status(400)

    //hash new password
    const hashedPassword = await bcrypt.hash(req.body.newPassword, 10)

    //change password through query
    connection.query(`UPDATE employee SET Password = '${hashedPassword}' WHERE login = '${req.user.name}';`, (error) => {
        if (error) return res.sendStatus(500)
        res.sendStatus(200)
    })
})

//refresh temporary token using assigned token stored in app
router.post('/refreshtoken', (req, res) => {
    const refreshToken = req.body.token
    if (refreshToken == null) return res.sendStatus(401)

    //check if assigned token is valid
    connection.query(`SELECT * FROM employee WHERE RefreshToken = '${refreshToken}';`, (error, results) => {
        if (error) return res.sendStatus(500)
        if (results.length == 0) return res.sendStatus(401)
        jwt.verify(refreshToken, process.env.REFRESH_TOKEN_SECRET, (error, user) => {
            if (error) return res.sendStatus(401)

            //generate new token
            const accessToken = generateAccessToken({ name: user.name })
            res.send(accessToken).status(200)
        })
    })
})


//function used to create temporary token
function generateAccessToken(user) {
    return accessToken = jwt.sign(user, process.env.ACCESS_TOKEN_SECRET, { expiresIn: '15m' });
}

//function used to test password strength when registering
function checkDataStrength(req, res, next) {
    if (req.body.login == null || req.body.login.length < 4) return res.status(400).send("Login must be larger than 3 characters")
    if (req.body.password == null || req.body.password.length < 6) return res.status(400).send("Password must be larger than 5 characters")
    next()
}

//function used to authenticate user if token matches it allows to receive data, also helps with identifying user
function authenticateToken(req, res, next) {
    //get token from header of website
    const authHeader = req.headers['authorization']
    const token = authHeader && authHeader.split(' ')[1]
    if (token == null) return res.sendStatus(401)

    //verify token with jwt
    jwt.verify(token, process.env.ACCESS_TOKEN_SECRET, (error, user) => {
        if (error) return res.sendStatus(401)
        //get data of token holder
        connection.query(`SELECT employee.*, employeeposition.Name as Position FROM employee JOIN employeeposition on employeeposition.Id = employee.Position_Id WHERE Login = '${user.name}';`, (error, results) => {
            if (error) return res.sendStatus(500)
            if (results[0].RefreshToken == null) return res.sendStatus(401)
            //store user data for identifying
            req.user = user
            req.userdata = results[0]
            next()
        })
    })
}

module.exports = {
    authenticateToken,
    router
}