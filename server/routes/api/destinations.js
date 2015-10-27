var express         = require('express');
var mysql           = require('mysql');
var path            = require("path");
var passport        = require('passport');
var app             = express();
var router          = express.Router();
var fs              = require('fs');
var passport        = require('passport');
var bodyParser      = require('body-parser');
var jsonParser      = bodyParser.json({ type: 'application/json' } );
var pool            = require('../../libs/mysql');

//users
router.get('/', function(req, res, next) {
    pool.getConnection(function(error, connection) {
        connection.query({
                sql: 'SELECT * FROM Destinations',
                timeout: 40000 // 40s
            },
            function (error, results, fields) {
                //if (error) throw error;
                if (error){
                    res.json({success:false});
                } else{
                    res.json(results);
                }
            }
        );
    });
});

module.exports = router;
