
!define ANVILDIRNAME "AnvilRelease_x32_2_8_2_0"

Section "TradeLibFast"

    ; Set output path to the installation directory.
	DetailPrint "Installing version ${VERSION}..."
  SectionIn RO
  SetOutPath $INSTDIR
  CreateDirectory "$SMPROGRAMS\TradeLink"
  
  ; files included in install
  File "BrokerServers\release\TradeLibFast.dll"

  
finishinstall:  
  
  ; write path for TradeLibFast.dll
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\App Paths\TradeLibFast.dll" "Path" "$INSTDIR\"
  


SectionEnd

; The stuff to install
Section "AnvilServer"


  CreateShortCut "$SMPROGRAMS\TradeLink\Assent.lnk" "$INSTDIR\${ANVILDIRNAME}\Anvil.exe" "" "$INSTDIR\${ANVILDIRNAME}\Anvil.exe" 0
  ; Put file there
  File /nonfatal /r "BrokerServers\Install\${ANVILDIRNAME}" 
  File "/oname=$INSTDIR\${ANVILDIRNAME}\AnvilServer.dll" "BrokerServers\AnvilServer\Release\AnvilServer.dll"
  WriteINIStr "$INSTDIR\${ANVILDIRNAME}\Anvil.ini" Extension Path "$INSTDIR\${ANVILDIRNAME}\"


SectionEnd

Section "InteractiveBrokers Server"
  File "BrokerServers\release\TWSServer.exe"
  File "BrokerServers\TWSServer\TwsSocketClient.dll"
  File "BrokerServers\release\TwsServer.Config.txt"
  CreateShortCut "$SMPROGRAMS\TradeLink\InteractiveBrokers.lnk" "$INSTDIR\TWSServer.exe" "" "$INSTDIR\TWSServer.exe" 0
SectionEnd

Section "Genesis Server"
  File "BrokerServers\release\ServerGenesis.exe"  
  File "BrokerServers\release\GTAPI.dll"  
  File "BrokerServers\ServerGenesis\GenesisServer.Config.txt"
  File "BrokerServers\ServerGenesis\GenesisServer.Login.txt"
  CreateShortCut "$SMPROGRAMS\TradeLink\Genesis.lnk" "$INSTDIR\ServerGenesis.EXE" "" "$INSTDIR\ServerGenesis.EXE" 0  
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
  Delete "$INSTDIR\${ANVILDIRNAME}\*.*"
  Delete "$INSTDIR\*.*"
  ; Remove shortcuts, if any
  Delete "$SMPROGRAMS\TradeLink\InteractiveBrokers.lnk"
  Delete "$SMPROGRAMS\TradeLink\Genesis.lnk"
  Delete "$SMPROGRAMS\TradeLink\Assent.lnk"
  

  ; Remove directories used
  RMDir "$INSTDIR\${ANVILDIRNAME}"
  RMDir "$INSTDIR"
  



SectionEnd