; The name of the installer
Name "TradeLinkSuite"
XPStyle on
CRCCheck force
ShowInstDetails show
Icon "tradelinkinstaller.ico"

; The file to write
OutFile "TradeLinkSuite.exe"

; The default installation directory
InstallDir $PROGRAMFILES\TradeLink\TradeLinkSuite

; Registry key to check for directory (so if you install again, it will 
; overwrite the old one automatically)
InstallDirRegKey HKLM "Software\TradeLinkSuite" "Install_Dir"

; Pages
Page components
Page directory
Page instfiles

UninstPage uninstConfirm
UninstPage instfiles

CompletedText "For additional documentation, see http://tradelink.googlecode.com"

; The stuff to install
Section "TradeLinkSuite"

  SectionIn RO
  
  ; Set output path to the installation directory.
  SetOutPath $INSTDIR
  
  CreateDirectory "$SMPROGRAMS\TradeLink"
  CreateShortCut "$SMPROGRAMS\TradeLink\Asp.lnk" "$INSTDIR\ASP.exe" "" "$INSTDIR\ASP.exe" 0
  CreateShortCut "$SMPROGRAMS\TradeLink\Quotopia.lnk" "$INSTDIR\Quotopia.exe" "" "$INSTDIR\Quotopia.exe" 0
  CreateShortCut "$SMPROGRAMS\TradeLink\Gauntlet.lnk" "$INSTDIR\Gauntlet.exe" "" "$INSTDIR\Gauntlet.exe" 0
  CreateShortCut "$SMPROGRAMS\TradeLink\Chartographer.lnk" "$INSTDIR\Chartographer.exe" "" "$INSTDIR\Chartographer.exe" 0
  CreateShortCut "$SMPROGRAMS\TradeLink\Replay.lnk" "$INSTDIR\Replay.exe" "" "$INSTDIR\Replay.exe" 0
  CreateShortCut "$SMPROGRAMS\TradeLink\Time and Sales.lnk" "$INSTDIR\TimeSales.exe" "" "$INSTDIR\TimeSales.exe" 0
  CreateShortCut "$SMPROGRAMS\TradeLink\Tattle.lnk" "$INSTDIR\Tattle.exe" "" "$INSTDIR\Tattle.exe" 0
  CreateShortCut "$SMPROGRAMS\TradeLink\Uninstall TradeLinkSuite.lnk" "$INSTDIR\uninstall.exe" "" "$INSTDIR\uninstall.exe" 0
  CreateShortCut "$SMPROGRAMS\TradeLink\Epf-To-Idx.lnk" "$INSTDIR\EPF2IDX.EXE" "" "$INSTDIR\EPF2IDX.EXE" 0  
  CreateShortCut "$SMPROGRAMS\TradeLink\SplitEPF.lnk" "$INSTDIR\SplitEPF.EXE" "" "$INSTDIR\SplitEPF.EXE" 0  
  CreateShortCut "$SMPROGRAMS\TradeLink\Kadina.lnk" "$INSTDIR\Kadina.EXE" "" "$INSTDIR\Kadina.EXE" 0 
  CreateShortCut "$SMPROGRAMS\TradeLink\Tript.lnk" "$INSTDIR\Tript.EXE" "" "$INSTDIR\Tript.EXE" 0  
  CreateShortCut "$SMPROGRAMS\TradeLink\Record.lnk" "$INSTDIR\Record.EXE" "" "$INSTDIR\Record.EXE" 0  
  
  
  ; Put file there
  File "VERSION.txt"
  File "Kadina.exe"
  File "Kadina.exe.config"
  File "Replay.exe"
  File "box.dll"
  File "Quotopia.exe"
  File "Quotopia.exe.config"
  File "TradeLib.dll"
  File "Gauntlet.exe"
  File "Gauntlet.exe.config"
  File "EarlyClose.csv"
  File "Chartographer.exe.config"
  File "Chartographer.exe"
  File "TimeSales.exe"
  File "EPF2IDX.exe"
  File "SplitEPF.exe"
  File "Tattle.exe"
  File "Tript.exe"
  File "ASP.exe"
  File "Tript.exe.settings.xml"
  File "ResearchLib.dll"
  File "Record.exe"
  File /r /x ".svn" "Properties"
  ; Write the installation path into the registry
  WriteRegStr HKLM SOFTWARE\TradeLinkSuite "Install_Dir" "$INSTDIR"
  	
  
  
  ; Write the uninstall keys for Windows
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\TradeLinkSuite" "DisplayName" "TradeLinkSuite"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\TradeLinkSuite" "UninstallString" '"$INSTDIR\uninstall.exe"'
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\TradeLinkSuite" "NoModify" 1
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\TradeLinkSuite" "NoRepair" 1
  WriteUninstaller "uninstall.exe"

SectionEnd
;--------------------------------

; Uninstaller

Section "Uninstall"
  
  ; Remove registry keys
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\TradeLinkSuite"
  DeleteRegKey HKLM SOFTWARE\TradeLinkSuite

  ; Remove files and uninstaller
  Delete $INSTDIR\TradeLinkSuite.nsi
  Delete $INSTDIR\uninstall.exe
  Delete "$INSTDIR\*.*"
  Delete "$INSTDIR\Properties\*.*"
  RMDir "$INSTDIR\Properties"
  ; Remove shortcuts, if any
  Delete "$SMPROGRAMS\TradeLink\Asp.lnk"  
  Delete "$SMPROGRAMS\TradeLink\Kadina.lnk"
  Delete "$SMPROGRAMS\TradeLink\Quotopia.lnk"
  Delete "$SMPROGRAMS\TradeLink\Time and Sales.lnk"
  Delete "$SMPROGRAMS\TradeLink\Replay.lnk"
  Delete "$SMPROGRAMS\TradeLink\Gauntlet.lnk"
  Delete "$SMPROGRAMS\TradeLink\Epf-To-Idx.lnk"
  Delete "$SMPROGRAMS\TradeLink\SplitEPF.lnk"
  Delete "$SMPROGRAMS\TradeLink\Tattle.lnk"
  Delete "$SMPROGRAMS\TradeLink\Tript.lnk"
  Delete "$SMPROGRAMS\TradeLink\Uninstall TradeLinkSuite"

  ; Remove directories used
  RMDir "$SMPROGRAMS\TradeLink"
  RMDir "$INSTDIR"
  



SectionEnd

Function .onInit

  ; plugins dir should be automatically removed after installer runs
  InitPluginsDir
  File /oname=$PLUGINSDIR\splash.bmp "tradelinklogo.bmp"
  splash::show 2000 $PLUGINSDIR\splash

  Pop $0 ; $0 has '1' if the user closed the splash screen early,
         ; '0' if everything closed normally, and '-1' if some error occurred.
FunctionEnd




