rem @echo off
if NOT DEFINED NUNIT OR NOT EXIST NUNIT (
for /F "delims=;" %%i in ('dir /b c:\progra~1\NUnit*') do set n_dir=%%i
set NUNIT=c:\progra~1\%n_dir%
)
"%NUNIT%\bin\nunit-console.exe" %1 /config:%2