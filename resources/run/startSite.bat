echo off
set IIS_CONFIG_PATH=%cd%
set IISEXPRESS_PATH=c:\Progra~2\IIS Express
pushd ..
set parentPath=%cd%
pushd ..
set parentPath=%cd%
popd
popd
rem echo IIS_CONFIG_PATH="%IIS_CONFIG_PATH%"
rem echo parentPath= "%parentPath%"
echo on
set CURRENT_BRANCH=%parentPath%
"%IISEXPRESS_PATH%\iisexpress.exe" /config:"%IIS_CONFIG_PATH%/applicationhost.config" /site:%1 /apppool:"Clr4IntegratedAppPool" /trace:error
