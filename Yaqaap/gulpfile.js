/// <binding BeforeBuild='InstallBower' ProjectOpened='InstallBower' />
var gulp = require('gulp');
var gutil = require('gulp-util');
var install = require("gulp-install");

/* global gulp */
gulp.task('default',['Bower'], function () { });

gulp.task('Bower', function () {

    gulp.src(['./bower.json']).pipe(install());

});

