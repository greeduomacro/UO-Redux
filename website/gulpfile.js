var gulp       = require("gulp"),
    bower      = require("gulp-bower"),
    notify     = require("gulp-notify"),
    watch      = require("gulp-watch"),
    sass       = require("gulp-sass"),
    minifycss  = require("gulp-minify-css"),
    rename     = require("gulp-rename"),
    gzip       = require("gulp-gzip"),
    livereload = require("gulp-livereload")
		;

var config = {
	sassPath:    "./assets/sass",
	bowerDir:    "./src/components",
	releasePath: "./dist"
};

var gzipOpts = {
	threshold:   "1kb",
	gzipOptions: {
		level: 9
	}
};

gulp.task("bower", function() {
	return bower()
			.pipe(gulp.dest(config.bowerDir));
});

gulp.task("icons", function() {
	return gulp.src(config.bowerDir + "/fontawesome/fonts/**.*")
			.pipe(gulp.dest(config.releasePath + "/fonts"));
});

//http://www.revsys.com/blog/2014/oct/21/ultimate-front-end-development-setup/
/** compile SASS **/
gulp.task("sass", function() {
	return gulp.src("assets/*.scss")
			.pipe(sass())
			.pipe(gulp.dest("static/stylesheets"))
			.pipe(rename({suffix: ".min"}))
			.pipe(minifycss())
			.pipe(gulp.dest("static/stylesheets"))
			.pipe(gzip(gzipOpts))
			.pipe(gulp.dest("static/stylesheets"))
			.pipe(livereload());
});

/** watch for changes **/
gulp.task("watch", function() {
	livereload.listen();
	gulp.watch("assets/*.scss", ["sass"]);
});

gulp.task("default", ["bower", "icons", "sass", "watch"]);