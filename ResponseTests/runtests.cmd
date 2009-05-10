@echo off

for /F "delims=;" %%i in ('dir /b /x c:\progra~1\NUnit*') do set n_dir=%%i
set NUNIT=c:\progra~1\%n_dir%
"%NUNIT%\bin\nunit-console.exe" "%~f2" /config:%1
"%NUNIT%\bin\net-2.0\nunit-console.exe" "%~f2" /config:%1