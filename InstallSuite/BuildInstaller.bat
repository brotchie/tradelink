@echo off
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
echo NSIS found, building installer... 
)
echo.
echo NOTE
echo.
echo This script will build an installer from your last
echo compiled release configurations of all TradeLink applets.
echo.
echo If you have not compiled the 'TradeLinkSuite' solution,
echo This script or dependent scripts may fail.
echo.
echo If you receive errors, please make sure you have compiled
echo all the applets in RELEASE mode (not debug mode).
echo.
echo.
echo Press Ctrl+Break to quit.
pause
cls
echo.
echo Removing last installer...
del TradeLinkSuite.exe
echo.
echo Copying latest releases...
echo.
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
del *.exe
del *.config
del *.xml
del EarlyClose.csv
del *.dll
del /s /f /q Properties
rmdir properties
ren TLS.tmp TradeLinkSuite.exe
ren signtool.tmp signtool.exe
echo.
echo Signing installer
signtool sign /f ..\tls.pfx /p tradelink /d TradeLink /du http://tradelink.googlecode.com TradeLinkSuite.exe
echo Run TradeLinkSuite.exe to install
) else (
echo Installer not found, please see error messages above...
)

echo.
