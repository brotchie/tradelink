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
del TradeLinkSuite.exe
echo.
echo Copying latest releases...
echo.
echo ASP
xcopy /q /y ..\ASP\bin\Release\ASP.exe* .
echo BOX-EXAMPLES
xcopy /q /y ..\Box-Examples\bin\Release\box.dll .
echo TRADELIB
xcopy /q /y ..\Tradelib\bin\Release\TradeLib.dll .
xcopy /q /y ..\TradeLib\bin\Release\EarlyClose.csv .
echo REPLAY
xcopy /q /y ..\Replay\bin\Release\Replay.exe* .
echo QUOTOPIA
xcopy /q /y ..\Quotopia\bin\Release\Quotopia.exe* .
xcopy /q /i /y ..\Quotopia\bin\Release\Properties .\Properties
echo GAUNTLET
xcopy /q /y ..\Gauntlet\bin\Release\Gauntlet.exe* .
echo TIME and SALES
xcopy /q /y ..\TimeAndSales\bin\Release\TimeSales.exe* .
echo SPLITEPF
xcopy /q /y ..\SplitEPF\bin\Release\SplitEPF.exe .
echo EPF2IDX
xcopy /q /y ..\EPF2IDX\bin\Release\EPF2IDX.exe .
echo CHARTOGRAPHER
xcopy /q /y ..\Chartographer\bin\Release\Chartographer.exe* .
echo TATTLE
xcopy /q /y ..\Tattle\bin\Release\Tattle.exe* .
echo KADINA
xcopy /q /y ..\Kadina\bin\Release\Kadina.exe* .
echo TRIPT
xcopy /q /y ..\Tript\bin\Release\Tript.exe* .
echo RESEARCHLIB
xcopy /q /y ..\ResearchLib\bin\Release\ResearchLib.dll .
echo RECORD
xcopy /q /y ..\Record\bin\Release\Record.exe* .
echo.




echo Building TradeLink executable...
c:\progra~1\nsis\makensis.exe /v1 TradeLinkSuite.nsi
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
ren TradeLinkSuite.exe TLS.tmp
ren signtool.exe signtool.tmp
echo Removing working releases...
del VERSION.txt
del *.config
del *.xml
del EarlyClose.csv
del *.dll
del /s /f /q Properties
del *.exe
rmdir properties
ren TLS.tmp TradeLinkSuite-%REVISION%.exe
ren signtool.tmp signtool.exe
) else (
echo Installer not found, please see error messages above...
)

echo.
