@echo off
echo Getting latest version information, please wait...
FOR /F "tokens=2 skip=4" %%G IN ('svn info --revision HEAD') DO ^
IF NOT DEFINED REVISION SET REVISION=%%G
echo %REVISION% >> VERSION.txt
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
del TradeLinkSuite.exe > NUL
echo.
echo Copying latest releases...
echo.
echo ASP
xcopy /q /y ..\ASP\bin\Release\ASP.exe* . > NUL
echo RESPONSES
xcopy /q /y ..\Responses\bin\Release\Responses.dll . > NUL
echo TRADELIB
xcopy /q /y ..\Tradelib\bin\Release\TradeLib.dll . > NUL
xcopy /q /y ..\TradeLib\bin\Release\EarlyClose.csv . > NUL
echo REPLAY
xcopy /q /y ..\Replay\bin\Release\Replay.exe* . > NUL
echo STERSERVER
xcopy /q /y ..\SterServer\bin\Release\SterServer.exe* . > NUL
xcopy /q /y ..\SterServer\bin\Release\Interop.SterlingLib.dll . > NUL
echo QUOTOPIA
xcopy /q /y ..\Quotopia\bin\Release\Quotopia.exe* . > NUL
xcopy /q /y ..\Quotopia\bin\Release\Multimedia.dll . > NUL
xcopy /q /i /y ..\Quotopia\bin\Release\Properties .\Properties > NUL
echo GAUNTLET
xcopy /q /y ..\Gauntlet\bin\Release\Gauntlet.exe* . > NUL
echo TIME and SALES
xcopy /q /y ..\TimeAndSales\bin\Release\TimeSales.exe* . > NUL
echo SPLITEPF
xcopy /q /y ..\SplitEPF\bin\Release\SplitEPF.exe . > NUL
echo CHARTOGRAPHER
xcopy /q /y ..\Chartographer\bin\Release\Chartographer.exe* . > NUL
echo TATTLE
xcopy /q /y ..\Tattle\bin\Release\Tattle.exe* . > NUL
echo KADINA
xcopy /q /y ..\Kadina\bin\Release\Kadina.exe* . > NUL
echo TRIPT
xcopy /q /y ..\Tript\bin\Release\Tript.exe* . > NUL
echo RESEARCHLIB
xcopy /q /y ..\ResearchLib\bin\Release\ResearchLib.dll . > NUL
echo RECORD
xcopy /q /y ..\Record\bin\Release\Record.exe* . > NUL
echo UPDATE
xcopy /q /y ..\Update\bin\Release\Update.exe* .  > NUL
echo.




echo Building TradeLink executable...
c:\progra~1\nsis\makensis.exe /v1 TradeLinkSuite.nsi  > NUL
if ERRORLEVEL 1 (
echo.
echo ERROR Building installer...
echo. 
echo quitting...
echo.
pause
goto :eof
) else (
echo Build complete.
echo.
)

if exist TradeLinkSuite.exe (
ren TradeLinkSuite.exe TLS.tmp > NUL
echo Removing working releases...
del VERSION.txt > NUL
del *.config > NUL
del *.xml > NUL
del EarlyClose.csv > NUL
del *.dll > NUL
del /s /f /q Properties > NUL
del *.exe > NUL
rmdir properties > NUL
ren TLS.tmp TradeLinkSuite-%REVISION%.exe > NUL
) else (
echo Installer not found, please see error messages above...
)

if exist _zipresponses (
del *.zip > NUL
echo Zipping responses...
cd ..\Responses
echo *.cs >> _pat.txt
echo *.resx >> _pat.txt
echo *.settings >> _pat.txt
echo *.sln >> _pat.txt
echo *.csproj >> _pat.txt
echo *.config >> _pat.txt
echo *.ico >> _pat.txt
zip -R ..\InstallSuite\Responses-%REVISION%.zip . -i@_pat.txt > NUL
del _pat.txt
cd ..\InstallSuite
)
echo.
