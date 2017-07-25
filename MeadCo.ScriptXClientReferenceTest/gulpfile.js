/// <binding BeforeBuild='default' ProjectOpened='default' />
/*
This file in the main entry point for defining Gulp tasks and using Gulp plugins.
Click here to learn more. http://go.microsoft.com/fwlink/?LinkId=518007
*/

var gulp = require('gulp');
var bower = require('gulp-bower');
var concat = require('gulp-concat');

gulp.task('default', ['copyLibs']);

// install all bower components inside bower_components folder:
gulp.task('bower', function () {
    return bower('./bower_components');
});

// Now that we have the front-end libraries, let’s move them to a different folder. 
// For this, we need to set a destination for each of them. Following task does it:

// This means we need to know what is there, despite it being in the bower.json file ...

// Note: Dont copy the jQuery that has been pulled in as a dependency of scriptxprint-html
gulp.task('copyLibs', ['bower'], function () {
    gulp.src(['bower_components/scriptxprint-html/src/*.*'])
        .pipe(gulp.dest("Scripts/MeadCo.ScriptX"));
});
