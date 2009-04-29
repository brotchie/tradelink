@echo off
echo Obtaining latest version information, please wait...
FOR /F "tokens=2 skip=4" %%G IN ('svn info --revision HEAD') DO ^
IF NOT DEFINED REVISION SET REVISION=%%G
echo Building installer for BrokerServer-%REVISION%
echo Removing last installer...
del BrokerServer*.exe > NUL
echo %REVISION% >> VERSION.txt
echo Getting latest releases...
echo.
set ANVILDIR=AnvilRelease_x32_2_7_7_2
echo ANVILSERVER %ANVILDIR%
xcopy /q /y ..\AnvilServer\Release\AnvilServer.dll %ANVILDIR% > NUL
echo.
echo IBSERVER
xcopy /q /y ..\Release\TWSServer.exe . > NUL
xcopy /q /y ..\Release\TWSServer.Config.txt . > NUL
xcopy /q /y ..\Release\TradeLibFast.dll > NUL
echo.

if not exist "c:\progra~1\nsis\makensis.exe" (
echo NSIS must be installed to use this script.
echo http://nsis.sourceforge.net/
echo.
echo Build failed.
echo.
pause
goto :eof
) else (
echo NSIS found.  Building installer...
echo.
c:\progra~1\nsis\makensis.exe /v1 BrokerServer.nsi > NUL
)

if exist BrokerServer.exe (
echo Installer build completed.
echo.
echo Removing working releases...
ren BrokerServer.exe BrokerServer.tmp > NUL
ren TwsSocketClient.dll SC.tmp > NUL
del *.exe > NUL
del *.dll > NUL
del VERSION.txt > NUL
del Tws*.txt > NUL
ren BrokerServer.tmp BrokerServer-%REVISION%.exe > NUL
ren SC.tmp TwsSocketClient.dll > NUL
) else (
echo Installer failed, see errors above!
echo.
echo quitting...
pause
goto :eof
)
