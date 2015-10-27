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
 * @api {get} /api/user/ GET user
 * @apiVersion 0.0.1
 * @apiName /
 * @apiGroup User
 * @apiDescription Get small info about current user
 *
 * @apiSuccess {Integer} ID Table: Users
 * @apiSuccess {String} Firstname Table: Users
 * @apiSuccess {String} Lastname Table: Users
 * @apiSuccess {String} Email Table: Users
 * @apiSuccess {String} Cellphone Table: Users
 * @apiSuccess {Boolean} IsBob Table: Users
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
        var id= req.user[0].ID;
        pool.getConnection(function(error, connection) {
            connection.query({
                    sql: 'SELECT ID, Firstname, Lastname, Email, Cellphone, (Bobs_ID IS NOT NULL) AS IsBob FROM Users ' +
                        'WHERE ID=?',
                    timeout: 40000 // 40s
                },
                [id],
                function (error, results, fields) {
                    //if (error) throw error;
                    if (error){
                        res.json({success:false});
                    } else{
                        res.json(results[0]);
                    }
                }
            );
        });
    }else{
        res.json({success:false, error:'Not authenticated'});
    }

});

/**
 * @api {get} /api/user/profile GET profile
 * @apiVersion 0.0.1
 * @apiName profile
 * @apiGroup User
 * @apiDescription Get profile of the current user.
 *
 * @apiSuccess {Integer} Users_ID Table: Users/Bobs
 * @apiSuccess {String} Firstname Table: Users
 * @apiSuccess {String} Lastname Table: Users
 * @apiSuccess {String} Email Table: Users
 * @apiSuccess {String} Cellphone Table: Users
 * @apiSuccess {Boolean} IsBob
 * @apiSuccess {Integer} Bobs_ID Table: Users/Bob
 * @apiSuccess {Integer} BobsType_ID Table: Bob
 * @apiSuccess {String} BobsType_Name Table: BobsType
 * @apiSuccess {Integer} Autotype_ID Table: Autotype/Bob
 * @apiSuccess {String} Autotype_Name Table: AutoType
 * @apiSuccess {String} LicensePlate
 *
 * @apiErrorExample Error-Response:
 *     HTTP/1.1 404 Not Found
 *     {
 *       success: false
 *     }
 */
router.get('/profile/', function(req, res, next) {
    //var id=req.params.id;
    var id=1;
    var sql="";

    if(id!=null){
        sql='SELECT Users.ID as Users_ID, Firstname, Lastname, Email, Cellphone, (Bobs_ID IS NOT NULL) AS IsBob, '+
        'Bobs.ID as Bobs_ID, Bobs.BobsType_ID As BobsType_ID, BobsType.Name As BobsType_Name, Bobs.Autotype_ID as Autotype_ID, Autotype.Name as Autotype_Name, Bobs.LicensePlate FROM Users '+
        'INNER JOIN Bobs ON Users.Bobs_ID=Bobs.ID '+
        'INNER JOIN BobsType ON BobsType.ID=Bobs.BobsType_ID '+
        'INNER JOIN Autotype ON Autotype.ID=Bobs.Autotype_ID ' +
        'WHERE Users.ID=?';
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
/**
 * @api {get} /api/user/points GET points[]
 * @apiVersion 0.0.1
 * @apiName points
 * @apiGroup User
 * @apiDescription Get all points of the current user.
 *
 * @apiSuccess {Integer} PointsDescription_ID Table: Users_PointsDescription/PointsDescription
 * @apiSuccess {String} PointsDescription_Name Table: PointsDescription
 * @apiSuccess {DoublePrecision} Points Table: PointsDescription
 * @apiSuccess {Timestamp} Added Table: PointsDescription
 *
 * @apiErrorExample Error-Response:
 *     HTTP/1.1 404 Not Found
 *     {
 *       success: false
 *     }
 */
router.get('/points/', function(req, res, next) {
    var sql='SELECT PointsDescription_ID,PointsDescription.Description as PointsDescription_Name,PointsDescription.Points as Points, Added FROM Bob.Users_PointsDescription '+
        'INNER JOIN PointsDescription ON PointsDescription.ID=Users_PointsDescription.PointsDescription_ID';


    pool.getConnection(function(error, connection) {
        connection.query({
                sql: sql,
                timeout: 40000 // 40s
            },
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
});

/**
 * @api {get} /api/user/points/amount GET Total points
 * @apiVersion 0.0.1
 * @apiName pointsAmount
 * @apiGroup User
 * @apiDescription Get the total points of the current user.
 *
 * @apiSuccess {DoublePrecision} Points Table: PointsDescription
 *
 * @apiErrorExample Error-Response:
 *     HTTP/1.1 404 Not Found
 *     {
 *       success: false
 *     }
 */
router.get('/points/amount', function(req, res, next) {
    var sql='SELECT SUM(PointsDescription.Points) as Points FROM Bob.Users_PointsDescription '+
            'INNER JOIN PointsDescription ON PointsDescription.ID=Users_PointsDescription.PointsDescription_ID';

    pool.getConnection(function(error, connection) {
        connection.query({
                sql: sql,
                timeout: 40000 // 40s
            },
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

/**
 * @api {post} /api/user/login POST Login
 * @apiVersion 0.0.1
 * @apiName login
 * @apiGroup User
 * @apiDescription Login the user
 *
 * @apiParam {String} Email
 * @apiParam {String} Password Gehashed
 *
 *
 * @apiSuccessExample {json} Success-Response:
 *     HTTP/1.1 200 OK
 *     {
 *       success: true
 *     }
 *
 * @apiErrorExample Error-Response:
 *     HTTP/1.1 404 Not Found
 *     {
 *       success: false
 *     }
 */
router.post('/login',jsonParser, function(req, res, next) {
    passport.authenticate('local', function(err, user, info) {
        //console.log(err, info, user);
        //login db
        if (err)   { return res.json({ error: err.message, success: false }); }
        if (!user) { return res.json({error : "Invalid Login", success: false}); }
        req.login(user, {}, function(err) {
            if (err) { return res.json({error:err, success: false}); }
            return res.json(
                { user: {
                    ID: req.user[0].ID,
                    Email: req.user[0].Email,
                    FirstName: req.user[0].FirstName,
                    LastName: req.user[0].LastName,
                    Cellphone:req.user[0].Cellphone,
                    IsBob:req.user[0].IsBob
                },
                    success: true
                });
        });
    })(req, res, next);
});

/**
 * @api {post} /api/user/logoff POST Logoff
 * @apiVersion 0.0.1
 * @apiName logoff
 * @apiGroup User
 * @apiDescription Logoff current user
 *
 *
 *
 * @apiSuccessExample {json} Success-Response:
 *     HTTP/1.1 200 OK
 *     {
 *       success: true
 *     }
 *
 * @apiErrorExample Error-Response:
 *     HTTP/1.1 404 Not Found
 *     {
 *       success: false
 *     }
 */
router.post('/logoff', function(req, res){
    req.logout();
    console.log(req.isAuthenticated());
    res.json({success: true});

});

/**
 * @api {post} /api/user/login POST Register
 * @apiVersion 0.0.1
 * @apiName register
 * @apiGroup User
 * @apiDescription Register a new user
 *
 * @apiParam {String} Firstname
 * @apiParam {String} Lastname
 * @apiParam {String} Email
 * @apiParam {String} [Cellphone]
 * @apiParam {String} Password
 * @apiParam {String} [FacebookID]
 * @apiParam {Boolean} IsBob
 *
 * @apiParam {DoublePrecision} [PricePerKm] Table: Bobs
 * @apiParam {Integer} [BobsType_ID] Table: Bobs
 * @apiParam {String} [LicensePlate] Table: Bobs
 * @apiParam {Integer} [AutoType_ID] Table: Bobs
 *
 * @apiSuccessExample {json} Success-Response:
 *     HTTP/1.1 201 OK
 *     {
 *       success: true
 *     }
 *
 * @apiErrorExample Error-Response:
 *     HTTP/1.1 404 Not Found
 *     {
 *       success: false
 *     }
 */
router.post('/register', function(req, res){
    res.json({success: true});
});

/**
 * @api {put} /api/user/login PUT Register
 * @apiVersion 0.0.1
 * @apiName edit
 * @apiGroup User
 * @apiDescription Edit a currentuser
 *
 * @apiParam {String} Firstname
 * @apiParam {String} Lastname
 * @apiParam {String} Email
 * @apiParam {String} [Cellphone]
 * @apiParam {String} Password
 * @apiParam {String} [FacebookID]
 * @apiParam {Boolean} IsBob
 *
 * @apiParam {DoublePrecision} [PricePerKm] Table: Bobs
 * @apiParam {Integer} [BobsType_ID] Table: Bobs
 * @apiParam {String} [LicensePlate] Table: Bobs
 * @apiParam {Integer} [AutoType_ID] Table: Bobs
 *
 * @apiSuccessExample {json} Success-Response:
 *     HTTP/1.1 200 OK
 *     {
 *       success: true
 *     }
 *
 * @apiErrorExample Error-Response:
 *     HTTP/1.1 404 Not Found
 *     {
 *       success: false
 *     }
 */
router.put('/register', function(req, res){
    res.json({success: true});
});
module.exports = router;
