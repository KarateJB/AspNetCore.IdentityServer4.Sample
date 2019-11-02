const gulp = require('gulp');
const exec = require('gulp-exec');

const options = {
  continueOnError: false, // default = false, true means don't emit error event
  pipeStdout: false // default = false, true means stdout is written to file.contents
};
var reportOptions = {
  err: true, // default = true, false means don't write err
  stderr: true, // default = true, false means don't write stderr
  stdout: true // default = true, false means don't write stdout
};

gulp.task('auth', function() {
  return gulp
    .src('./**')
    .pipe(
      exec(
        'dotnet run  --no-build --project ./src/AspNetCore.IdentityServer4.Auth/AspNetCore.IdentityServer4.Auth.csproj'
      )
    )
    .pipe(exec.reporter(reportOptions));
});

gulp.task('webapi', function() {
  return gulp
    .src('./**')
    .pipe(
      exec(
        'dotnet run  --no-build --project ./src/AspNetCore.IdentityServer4.WebApi/AspNetCore.IdentityServer4.WebApi.csproj'
      )
    )
    .pipe(exec.reporter(reportOptions));
});

gulp.task('run', gulp.parallel('auth', 'webapi', function(){
  console.log('Excuted...');
}));
