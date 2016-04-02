/// <binding BeforeBuild='InstallBower' ProjectOpened='InstallBower' />
var gulp = require('gulp');
var gutil = require('gulp-util');
var install = require("gulp-install");

/* global gulp */
gulp.task('default',['NPM'], function () { });

gulp.task('NPM', function () {

    gulp.src(['./package.json', './Yaqaap/bower.json']).pipe(install());

});

