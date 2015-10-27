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
 * @api {get} /api/chatrooms GET rooms[]
 * @apiVersion 0.0.1
 * @apiName /
 * @apiGroup Chatroom
 * @apiDescription Get all chatrooms of current user
 *
 * @apiSuccess {Integer} ID Table:ChatRooms
 * @apiSuccess {Integer} Bobs_ID Table:ChatRooms
 * @apiSuccess {TimeStamp} Added Table:ChatRooms
 *
 * @apiErrorExample Error-Response:
 *     HTTP/1.1 404 Not Found
 *     {
 *       success: false
 *     }
 */

router.get('/', function(req, res, next) {
    var id=1;
    var sql="";

    if(id!=null){
        sql='SELECT ID,Bobs_ID, Added FROM Bob.ChatRooms ' +
            'WHERE Users_ID=? AND Active=1';
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

});


/**
 * @api {post} /api/chatrooms POST room
 * @apiVersion 0.0.1
 * @apiName newRoom
 * @apiGroup Chatroom
 * @apiDescription Add a new room
 *
 * @apiParam {String} Users_ID
 * @apiParam {String} Bobs_ID
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

router.post('/', function(req, res, next) {
    var id=1;
    var bob_id=null;
    var sql="";

    if(id!=null){
        sql='INSERT INTO ChatRooms(Users_ID,Bobs_ID) ' +
            'VALUES(?,?)';
    }


    pool.getConnection(function(error, connection) {
        connection.query({
                sql: sql,
                timeout: 40000 // 40s
            },
            [id,bob_id],
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
 * @api {get} /api/chatrooms/:id GET room comments
 * @apiVersion 0.0.1
 * @apiName room_comments
 * @apiGroup Chatroom
 * @apiDescription Get chatcomments from room
 *
 * @apiSuccess {Integer} ID Table:ChatComments
 * @apiSuccess {Integer} ChatRooms_ID Table:ChatComments
 * @apiSuccess {String} Comment Table:ChatComments
 * @apiSuccess {Integer} Users_ID Table:ChatComments/Users
 * @apiSuccess {String} Users_Firstname Table:Users
 * @apiSuccess {String} Users_Lastname Table:Users
 * @apiSuccess {String} Users_Email Table:Users
 * @apiSuccess {String} Users_Cellphone Table:Users
 *
 *
 * @apiErrorExample Error-Response:
 *     HTTP/1.1 404 Not Found
 *     {
 *       success: false
 *     }
 */
router.get('/:id', function(req, res, next) {
    var id=req.params.id;
    var sql="";

    if(id!=null){
        sql='SELECT ChatComments.ID,ChatComments.ChatRooms_ID,ChatComments.Comment,ChatComments.Added, ChatComments.Users_ID, Users.Firstname as Users_Firstname, Users.Lastname as Users_Lastname, Users.Email as Users_Email, Users.Cellphone as Users_Cellphone FROM ChatComments '+
            'INNER JOIN Users ON Users.ID= ChatComments.Users_ID ' +
            'WHERE ChatComments.ChatRooms_ID=? ' +
            'ORDER BY ChatComments.Added DESC';
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
