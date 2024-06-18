const mysql = require('mysql2');

var connection = mysql.createPool({
  host: "localhost",
  port: 3306,
  user: "root",
  password: "",
  database: "outofofficedb",
  timezone : "+00:00",
  multipleStatements: true
});

module.exports = connection