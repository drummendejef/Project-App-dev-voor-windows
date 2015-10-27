var passport                = require('passport');
var mysql                   = require('mysql');
var LocalStrategy           = require('passport-local').Strategy;
var BearerStrategy          = require('passport-http-bearer').Strategy;
var LdapStrategy            = require('passport-ldapauth').Strategy;
var pool                    = require('./mysql');

function findByEmail(email,password, fn) {
    pool.getConnection(function(error, connection) {
        if (error) throw error;
        var query;

            query='SELECT ID, FirstName, LastName, Email, Cellphone, (Bobs_ID IS NOT NULL) AS IsBob FROM Users WHERE Email=? AND Password=?';

            connection.query({//AND Password= ?
                    sql: query,
                    timeout: 40000 // 40s
                },
                [email, password],
                function (error, rows, fields) {
                    if (error) throw error;
                    var user= rows;
                    //console.log(user);
                    connection.release();
                    if(user!=null && user.length!=0){
                        return fn(null, user);
                    }else{
                        return fn(null, null);
                    }

                }
            );


    });


}

passport.use('local',new LocalStrategy({
        passReqToCallback : true,
        usernameField: 'email',
        passwordField: 'password'
    },
    function(body, email, password, done) {
        // asynchronous verification, for effect...
        process.nextTick(function () {

            findByEmail(email,password, function(err, user) {
                if (err) { return done(err); }
                if (!user) { return done(null, false); }
                //console.log(user);
                return done(null, user);
            })
        });
    }
));

passport.serializeUser(function(user, done) {
    done(null, user);
});

passport.deserializeUser(function(user, done) {
    done(null, user);
});