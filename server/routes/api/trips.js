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
/**
 * @api {get} /api/trips GET trips[]
 * @apiVersion 0.0.1
 * @apiName /
 * @apiGroup Trips
 * @apiDescription Get all trips of current user
 *
 * @apiSuccess {Integer} Trips_ID Table: Trips
 * @apiSuccess {String} Trips_CurrentLocation Table: Trips, {latitude:"",longitude:""}
 * @apiSuccess {Integer} Trips_Bobs_ID Table: Trips
 * @apiSuccess {Integer} Users_ID Table: Users
 * @apiSuccess {Integer} Destinations_ID Table: Users_Destinations/Trips
 * @apiSuccess {String} Destinations_Location Table: Users_Destinations, {latitude:"",longitude:""}
 * @apiSuccess {Integer} Cities_ID Table: Cities/Destinations
 * @apiSuccess {String} Cities_Name Table: Cities
 * @apiSuccess {Boolean} Default Table: Users_Destinations
 * @apiSuccess {Timestamp} Added Table: Users_Destinations
 * @apiSuccess {String} Name Table: Users_Destinations
 *
 *
 * @apiErrorExample Error-Response:
 *     HTTP/1.1 404 Not Found
 *     {
 *       success: false
 *     }
 */
router.get('/', function(req, res, next) {
    if(req.isAuthenticated()){
        var id=1;
        var sql="";

        if(id!=null){
            sql='SELECT Trips.ID as Trips_ID, Trips.CurrenLocation as Trips_CurrentLocation, Trips.Bobs_ID as Trips_Bobs_ID, a.Users_ID, a.Destinations_ID,Destinations.Location as Destinations_Location, Destinations.Cities_ID, Cities.Name as Cities_Name, a.Default, a.Added, a.Name FROM Trips '+
                'INNER JOIN Users_Destinations as a ON Trips.Destinations_ID= a.Destinations_ID  '+
                'INNER JOIN Users ON Users.ID= a.Users_ID '+
                'INNER JOIN Destinations ON Destinations.ID=a.Destinations_ID '+
                'INNER JOIN Cities On Cities.ID=Destinations.Cities_ID '+
                'WHERE a.Users_ID=?';
        }


        pool.getConnection(function(error, connection) {
            connection.query({
                    sql: sql,
                    timeout: 40000 // 40s
                },
                [id],
                function (error, results, fields) {
                    if (error) throw error;
                    if (error){
                        res.json({success:false});
                    } else{
                        res.json(results);
                    }
                }
            );
        });
    }else{
        res.json({success:false, error:'Not authenticated'});
    }


});

/**
 * @api {get} /api/trips/current GET CurrentTrip
 * @apiVersion 0.0.1
 * @apiName current
 * @apiGroup Trips
 * @apiDescription Get trip of current user
 *
 * @apiSuccess {Integer} Trips_ID Table: Trips
 * @apiSuccess {String} Trips_CurrentLocation Table: Trips, {latitude:"",longitude:""}
 * @apiSuccess {Integer} Trips_Bobs_ID Table: Trips
 * @apiSuccess {Integer} Users_ID Table: Users
 * @apiSuccess {Integer} Destinations_ID Table: Users_Destinations/Trips
 * @apiSuccess {String} Destinations_Location Table: Users_Destinations, {latitude:"",longitude:""}
 * @apiSuccess {Integer} Cities_ID Table: Cities/Destinations
 * @apiSuccess {String} Cities_Name Table: Cities
 * @apiSuccess {Boolean} Default Table: Users_Destinations
 * @apiSuccess {Timestamp} Added Table: Users_Destinations
 * @apiSuccess {String} Name Table: Users_Destinations
 *
 *
 * @apiErrorExample Error-Response:
 *     HTTP/1.1 404 Not Found
 *     {
 *       success: false
 *     }
 */
router.get('/current', function(req, res, next) {
    var id=1;
    var sql="";

    if(id!=null){
        sql='SELECT Trips.ID as Trips_ID, Trips.CurrenLocation as Trips_CurrentLocation, Trips.Bobs_ID as Trips_Bobs_ID, a.Users_ID, a.Destinations_ID,Destinations.Location as Destinations_Location, Destinations.Cities_ID, Cities.Name as Cities_Name, a.Default, a.Added, a.Name FROM Trips '+
            'INNER JOIN Users_Destinations as a ON Trips.Destinations_ID= a.Destinations_ID '+
            'INNER JOIN Users ON Users.ID= a.Users_ID '+
            'INNER JOIN Destinations ON Destinations.ID=a.Destinations_ID '+
            'INNER JOIN Cities On Cities.ID=Destinations.Cities_ID '+
            'WHERE a.Users_ID=? '+
            'ORDER BY Trips.Added DESC '+
            'LIMIT 1';
    }


    pool.getConnection(function(error, connection) {
        connection.query({
                sql: sql,
                timeout: 40000 // 40s
            },
            [id],
            function (error, results, fields) {
                if (error) throw error;
                if (error){
                    res.json({success:false});
                } else{
                    res.json(results[0]);
                }
            }
        );
    });


});

module.exports = router;
