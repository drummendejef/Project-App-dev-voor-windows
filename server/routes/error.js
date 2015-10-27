var express = require('express');
var passport= require('passport');
var app     = express();
var router = express.Router();


router.get('/', function(req, res){
    res.render('template/error');
});

module.exports = router;
