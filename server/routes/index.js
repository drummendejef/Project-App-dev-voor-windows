var express = require('express');
var app     = express();
var router = express.Router();

/* GET home page. */
router.get('/', function(req, res, next) {
  if(req.query.debug=="true"){
    res.render('main/main', {title: 'Stijn Van Hulle'});
  }else{
    res.render('main/main', {title: 'Stijn Van Hulle'});
    //res.render('default', {title: 'Stijn Van Hulle'});
  }

});

module.exports = router;
