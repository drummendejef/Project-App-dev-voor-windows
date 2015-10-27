var express         = require('express');
var session         = require('express-session');
var app             = express();
var compression     = require('compression');
var path            = require('path');
var logger          = require('morgan');
var cookieParser    = require('cookie-parser');
var bodyParser      = require('body-parser');
var multer          = require('multer');
var cors            = require("./libs/enable_cors")(app);

//auth
var passport    = require('passport');


var user        = require('./routes/api/user');
var countries   = require('./routes/api/countries');
var cities      = require('./routes/api/cities');
var bobs        = require('./routes/api/bobs');
var destinations= require('./routes/api/destinations');
var parties     = require('./routes/api/parties');
var trips       = require('./routes/api/trips');
var chatrooms       = require('./routes/api/chatrooms');
var error       = require('./routes/error');

// view engine setup
app.set('views', path.join(__dirname, 'views'));
app.set('partials',path.join(__dirname,'views/partials'));
app.set('view engine', 'jade');


//custom
app.enable('trust proxy');//set proxy on true, ip ophalen

app.use(compression());
// GZIP all assets

app.use(logger('dev')); //log alles van request naar de console
app.use(cookieParser());

//deze 2 nodig om auth en formdata te gebruiken
app.use(bodyParser.urlencoded({ extended: true  }));
app.use(multer({ dest: './public/uploads/'}));

app.use(express.static(__dirname + '/public'));
app.use('/bower_components',  express.static(__dirname + '/bower_components'));


//auth
app.use(session({
  secret: 'keyboard cat',
  resave: false,
  expires: true,
  saveUninitialized: false,
  path:"/*" //NEEDED
}));
app.use(passport.initialize());
app.use(passport.session());

require('./libs/auth');


//api routes
app.use('/api/user', user);
app.use('/api/countries', countries);
app.use('/api/cities', cities);
app.use('/api/bobs', bobs);
app.use('/api/destinations', destinations);
app.use('/api/parties', parties);
app.use('/api/trips', trips);
app.use('/api/chatrooms', chatrooms);
app.use('/404',error);




// catch 404 and forward to error handler
app.use(function(req, res, next) {
  var err = new Error('Not Found');
  err.status = 404;
  next(err);
});

// error handlers

// development error handler
// will print stacktrace
if (app.get('env') === 'development') {
  app.use(function(err, req, res, next) {
    res.status(err.status || 500);
    res.render('template/error', {
      message: err.message,
      error: err
    });
  });
}

// production error handler
// no stacktraces leaked to user
app.use(function(err, req, res, next) {
  res.status(err.status || 500);
  res.render('template/error', {
    message: err.message,
    error: {}
  });
});


module.exports = app;
