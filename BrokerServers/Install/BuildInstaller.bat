@echo off
echo Obtaining latest version information, please wait...
FOR /F "tokens=2 skip=4" %%G IN ('svn info --revision HEAD') DO ^
IF NOT DEFINED REVISION SET REVISION=%%G
echo Building installer for BrokerServer-%REVISION%
echo Removing last installer...
del BrokerServer*.exe
echo %REVISION% >> VERSION.txt
echo Getting latest releases...
echo.
set ANVILDIR=AnvilRelease_x32_2_7_0_5
echo ANVILSERVER %ANVILDIR%
xcopy /q /y ..\AnvilServer\Release\AnvilServer.dll %ANVILDIR%
echo.
echo IBSERVER
xcopy /q /y ..\Release\TWSServer.exe .
xcopy /q /y ..\Release\TWSServer.Config.txt .
xcopy /q /y ..\Release\TradeLinkServer.dll
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
c:\progra~1\nsis\makensis.exe /v1 BrokerServer.nsi
)

if exist BrokerServer.exe (
echo Installer build completed.
echo.
echo Removing working releases...
ren BrokerServer.exe BrokerServer.tmp
ren TwsSocketClient.dll SC.tmp
del *.exe
del *.dll
del VERSION.txt
del Tws*.txt
ren BrokerServer.tmp BrokerServer-%REVISION%.exe
ren SC.tmp TwsSocketClient.dll
) else (
echo Installer failed, see errors above!
echo.
echo quitting...
pause
goto :eof
)
