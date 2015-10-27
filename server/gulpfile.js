var gulp = require("gulp"),
    csslint = require("gulp-csslint"),
    cssminifier = require("gulp-minify-css"),
    sourcemaps = require("gulp-sourcemaps"),
    jshint = require("gulp-jshint"),
    hintstylish = require("jshint-stylish"),
    uglify = require("gulp-uglify"),
    concat = require("gulp-concat"),
    notify = require("gulp-notify"),
    sass = require('gulp-sass'),
    imagemin= require('gulp-imagemin'),
    pngquant = require('imagemin-pngquant'),
    apidoc = require('gulp-apidoc');

gulp.task("default", function() {
    var cssWatcher = gulp.watch("./public/src/css/**/*.css", ["css-build"]);
    cssWatcher.on("change", function(event) {
        console.log("File: " + event.path + " was " + event.typed);
    });
    var sassWatcher = gulp.watch('./public/src/sass/**/*.scss', ['sass']);
    sassWatcher.on("change", function(event) {
        console.log("File: " + event.path + " was " + event.typed);
    });

    var jsWatcher = gulp.watch("./public/src/js/**/*.js", ["js-build"]);
    jsWatcher.on("change", function(event) {
        console.log("File: " + event.path + " was " + event.typed);
    });

    var imageWatcher = gulp.watch("./public/src/images/*", ["image"]);
    imageWatcher.on("change", function(event) {
        console.log("File: " + event.path + " was " + event.typed);
    });

    var apiDocWatcher = gulp.watch("./routes/api/*", ["apidoc"]);
    apiDocWatcher.on("change", function(event) {
        console.log("File: " + event.path + " was " + event.typed);
    });

});
gulp.task('dev', function () {
    nodemon({ script: 'server.js'
        , ext: 'html js'
        , ignore: ['ignored.js']
        , tasks: ['lint'] })
        .on('restart', function () {
            console.log('restarted!')
        });
    var cssWatcher = gulp.watch("./dist/css/**/*.css", ["css-build"]);
    cssWatcher.on("change", function(event) {
        console.log("File: " + event.path + " was " + event.typed);
    });
    var sassWatcher = gulp.watch('./sass/**/*.scss', ['sass']);
    sassWatcher.on("change", function(event) {
        console.log("File: " + event.path + " was " + event.typed);
    });

    var jsWatcher = gulp.watch("./scripts/**/*.js", ["js-build"]);
    jsWatcher.on("change", function(event) {
        console.log("File: " + event.path + " was " + event.typed);
    });

    var imageWatcher = gulp.watch("./public/src/images/*", ["image"]);
    imageWatcher.on("change", function(event) {
        console.log("File: " + event.path + " was " + event.typed);
    });

});


//tasks
gulp.task("css-build", function() {
    gulp.src("./public/src/css/**/*.css")
        .pipe(csslint({
            'ids' : false
        }))
        //.pipe(csslint.reporter("junit-xml"))
        //.pipe(csslint.reporter("fail"))
        .pipe(sourcemaps.init())
        .pipe(cssminifier())
        .pipe(sourcemaps.write())
        .pipe(gulp.dest("./public/css"));
});

gulp.task("js-build",function(){
    gulp.start('js-build:app.min', 'js-build:all.min','js-build:controllers.min');
});

gulp.task("js-build:app.min", function() {
    gulp.src(["./public/src/js/app.js","./public/src/js/functions.js"])
        .pipe(jshint())
        .pipe(jshint.reporter(hintstylish))
        .pipe(sourcemaps.init())
        .pipe(concat("app.min.js"))
        .pipe(sourcemaps.write())
        .pipe(gulp.dest("./public/js"))
        .pipe(notify({ message: 'js built'}));
});

gulp.task("js-build:all.min", function() {
    gulp.src(["./public/src/js/*.js","!./public/src/js/app.js","!/public/src/js/functions.js"])
        .pipe(jshint())
        .pipe(jshint.reporter(hintstylish))
        .pipe(sourcemaps.init())
        .pipe(concat("all.min.js"))
        .pipe(sourcemaps.write())
        .pipe(gulp.dest("./public/js"))
        .pipe(notify({ message: 'js built'}));
});

gulp.task("js-build:controllers.min", function() {
    gulp.src(["./public/src/js/controllers/*.js"])
        .pipe(jshint())
        .pipe(jshint.reporter(hintstylish))
        .pipe(sourcemaps.init())
        .pipe(concat("controllers.min.js"))
        .pipe(sourcemaps.write())
        .pipe(gulp.dest("./public/js"))
        .pipe(notify({ message: 'js built'}));
});

//sass
gulp.task('sass', function () {
    gulp.src('./public/src/sass/**/*.scss')
        .pipe(sass().on('error', sass.logError))
        .pipe(gulp.dest('./public/css'))
        .pipe(cssminifier())
        .pipe(concat("style.min.css"))
        .pipe(gulp.dest('./public/css'));
});

gulp.task('image', function () {
    gulp.src('./public/src/images/*')
        .pipe(imagemin({
            progressive: true,
            svgoPlugins: [{removeViewBox: false}],
            use: [pngquant()]
        }))
        .pipe(gulp.dest('./public/images'));
});

gulp.task('apidoc', function(done){
    apidoc({
        src: "routes/api/",
        dest: "public/api/"
    },done);
});


