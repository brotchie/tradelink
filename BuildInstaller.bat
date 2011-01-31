@echo off
del /q VERSION.txt > NUL
cls
echo Getting latest version information, please wait...
FOR /F "tokens=2 skip=4" %%G IN ('svn info --revision HEAD') DO ^
IF NOT DEFINED REVISION SET REVISION=%%G
echo %REVISION% > VERSION.txt
cls
if exist "Install\_includebs.txt" (
SET INCLUDEBS=BS
echo Including BrokerServer...
) else (
SET INCLUDEBS=NOBS
echo Ignoring BrokerServer...
)
echo Checking for NSIS...
if not exist "c:\progra~1\nsis\makensis.exe" (
echo You must install NSIS to build an installer...
echo http://nsis.sourceforge.net
echo.
echo Build failed.
echo.
pause
goto :eof
) else ( 
echo NSIS found, building installer for TradeLinkSuite-%REVISION%
)
echo.
echo NOTE:
echo.
echo Please be sure you've compiled the solution in RELEASE mode.
echo.
echo.
echo Removing last installer...
del /q TradeLinkSuite*.exe > NUL
echo Building TradeLink executable...
c:\progra~1\nsis\makensis.exe /v1 /DPVERSION=%REVISION% /DINCLUDEBS=%INCLUDEBS% TradeLinkSuite.nsi  > _buildinstaller.txt
if ERRORLEVEL 1 (
cat _buildinstaller.txt
echo.
echo ERROR Building installer...  did you compile the solution?
echo.
echo Try right clicking on TradeLinkSuite.nsi and choosing compile, 
echo then see what errors come up...  perhaps a project did not compile.
echo. 
echo quitting...
echo.
pause
del /q _buildinstaller.txt
goto :eof
) else (
del /q _buildinstaller.txt
echo Build complete.
echo.
)

if not exist TradeLinkSuite.exe (
echo Installer not found, please see error messages above...
pause
goto :eof
)

ren TradeLinkSuite.exe TLS.tmp > NUL
echo Removing working files...
del VERSION.txt > NUL
ren TLS.tmp TradeLinkSuite-%REVISION%.exe > NUL


if exist InstallSuite\_gendocs (
echo Generating documentation from source code... (may take a moment)
if exist "c:\progra~1\doxygen\bin\doxygen.exe" (
del /q /s html > NUL
c:\progra~1\doxygen\bin\doxygen.exe InstallSuite\Doxyfile > NUL 2>&1
) else (
echo Doxygen not installed, skipping documentation...
)
)



echo.
