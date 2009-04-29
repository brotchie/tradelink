

; The name of the installer
Name "TradeLink BrokerServers"
Icon "favicon.ico"



XPStyle on
CRCCheck force
SetOverWrite on
ShowInstDetails show

; The file to write
OutFile "BrokerServer.exe"

; The default installation directory
InstallDir $PROGRAMFILES\TradeLink\BrokerServer\

; Registry key to check for directory (so if you install again, it will 
; overwrite the old one automatically)
InstallDirRegKey HKLM "Software\NSIS_TLBrokerServer" "Install_Dir"

;--------------------------------

; Pages
Page license
Page components
Page directory
Page instfiles

UninstPage uninstConfirm
UninstPage instfiles

LicenseText "TradeLink BrokerServer Agreement"
LicenseData "LICENSE.txt"
LicenseForceSelection radiobuttons "I have read and accept the above" "I decline"
CompletedText "Direct installation questions or problems to http://groups.google.com/group/tradelink-users"

;--------------------------------

Section "BrokerServer"

  SectionIn RO
    ; Set output path to the installation directory.
  SetOutPath $INSTDIR
  CreateDirectory "$SMPROGRAMS\TradeLink"
  
  ; files included in install
  File "VERSION.txt"
  File "README.txt"
  File "LICENSE.txt"
  File "TwsSocketClient.dll"
  File TradeLibFast.dll 
  ; write path for TradeLibFast.dll
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\App Paths\TradeLibFast.dll" "Path" "$INSTDIR\"
  
  ; Write the installation path into the registry
  WriteRegStr HKLM Software\NSIS_TLBrokerServer "Install_Dir" "$INSTDIR"


  ; shortcut to uninstaller
  CreateShortCut "$SMPROGRAMS\TradeLink\Uninstall BrokerServer.lnk" "$INSTDIR\uninstall.exe" "" "$INSTDIR\uninstall.exe" 0
  ; Write the uninstall keys for Windows
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\NSIS_TLBrokerServer" "DisplayName" "TradeLinkAnvilServer"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\NSIS_TLBrokerServer" "UninstallString" '"$INSTDIR\uninstall.exe"'
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\NSIS_TLBrokerServer" "NoModify" 1
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\NSIS_TLBrokerServer" "NoRepair" 1
  WriteUninstaller "uninstall.exe"  

SectionEnd

; The stuff to install
Section "AnvilServer"


  CreateShortCut "$SMPROGRAMS\TradeLink\Anvil+BrokerServer.lnk" "$INSTDIR\AnvilRelease_x32_2_7_7_2\Anvil.exe" "" "$INSTDIR\AnvilRelease_x32_2_7_7_2\Anvil.exe" 0
  ; Put file there
 
  
  File /r "AnvilRelease_x32_2_7_7_2"


SectionEnd

Section "InteractiveBrokers Server"
  File "TWSServer.exe"
  ;File "TWSSocketClient.dll"
  File "TwsServer.Config.txt"
  CreateShortCut "$SMPROGRAMS\TradeLink\TWS+BrokerServer.lnk" "$INSTDIR\TWSServer.exe" "" "$INSTDIR\TWSServer.exe" 0
SectionEnd



;--------------------------------

; Uninstaller

Section "Uninstall"
  
  ; Remove registry keys
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\NSIS_TLBrokerServer"
  DeleteRegKey HKLM SOFTWARE\NSIS_TLBrokerServer
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\App Paths\TradeLibFast.dll"

  ; Remove files and uninstaller
  Delete $INSTDIR\uninstall.exe
  Delete "$INSTDIR\AnvilRelease_x32_2_7_7_2\*.*"
  Delete "$INSTDIR\*.*"
  ; Remove shortcuts, if any
  Delete "$SMPROGRAMS\TradeLink\*BrokerServer.lnk"
  

  ; Remove directories used
  RMDir "$INSTDIR\AnvilRelease_x32_2_7_5_0"
  RMDir "$INSTDIR"
  



SectionEnd

Function .onInit

  ; plugins dir should be automatically removed after installer runs
  InitPluginsDir
  File /oname=$PLUGINSDIR\splash.bmp "brokerserversplash.bmp"
  splash::show 1000 $PLUGINSDIR\splash

  Pop $0 ; $0 has '1' if the user closed the splash screen early,
         ; '0' if everything closed normally, and '-1' if some error occurred.
FunctionEnd




