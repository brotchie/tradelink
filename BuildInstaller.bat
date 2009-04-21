@echo off
del /q VERSION.txt > NUL
cls
echo Getting latest version information, please wait...
FOR /F "tokens=2 skip=4" %%G IN ('svn info --revision HEAD') DO ^
IF NOT DEFINED REVISION SET REVISION=%%G
echo %REVISION% > VERSION.txt
cls
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
c:\progra~1\nsis\makensis.exe /v1 TradeLinkSuite.nsi  > NUL
if ERRORLEVEL 1 (
echo.
echo ERROR Building installer...  did you compile the solution?
echo. 
echo quitting...
echo.
pause
goto :eof
) else (
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
if exist InstallSuite\_zipresponses (
del /q *.zip > NUL
echo Zipping responses...
echo .\Responses\*.cs >> _pat.txt
echo .\Responses\*.resx >> _pat.txt
echo .\Responses\*.settings >> _pat.txt
echo .\Responses\*.sln >> _pat.txt
echo .\Responses\*.csproj >> _pat.txt
echo .\Responses\*.config >> _pat.txt
echo .\Responses\*.ico >> _pat.txt
echo .\Responses\*.cmd >> _pat.txt
zip -R Responses-%REVISION%.zip .\Responses -i@_pat.txt > NUL
del _pat.txt
)

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
