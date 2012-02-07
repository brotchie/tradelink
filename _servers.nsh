




Section "TradeLink c++"

    ; Set output path to the installation directory.
	DetailPrint "Installing version ${VERSION}..."
  SectionIn RO
  SetOutPath $INSTDIR
  CreateDirectory "$SMPROGRAMS\TradeLink Connectors"
  
  ; files included in install
  File "BrokerServers\release\TradeLibFast.dll"
  File "Install\vcredist_x86.20110514.exe"
  
    DetailPrint "Checking for VCRedistributable..."
  ReadRegStr $0 HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\TradeLinkSuite" "InstalledVcRedist"
  StrCmp $0 "Yes" finishvcredist
  DetailPrint "Installing vcredistributable..."
  ExecWait '"vcredist_x86.20110514.exe" /q:a /c:\"VCREDI~3.EXE /q:a /c:\"\"msiexec /i vcredist.msi /qn\"\" \"' $0
  DetailPrint "VCRedistributable installed.  Result:$0"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\TradeLinkSuite" "InstalledVcRedist" "Yes"

  
finishvcredist:  
	DetailPrint "VCRedistributable was installed."
  
  ; write path for TradeLibFast.dll
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\App Paths\TradeLibFast.dll" "Path" "$INSTDIR\"
  


SectionEnd


Section "Lightspeed Connector"
    CreateDirectory "$PROGRAMFILES\Lightspeed"
    File "/oname=$PROGRAMFILES\Lightspeed\LightspeedServer.Config.txt" "BrokerServers\Lightspeedserver\LightspeedServer.Config.txt"
	File "/oname=$PROGRAMFILES\Lightspeed\LightspeedServer.dll" "BrokerServers\release\LightspeedServer.dll"
	File "/oname=$PROGRAMFILES\Lightspeed\TradeLibFast.dll" "BrokerServers\release\TradeLibFast.dll"
SectionEnd



Section "InteractiveBrokers Connector"
  File "BrokerServers\release\TWSServer.exe"
  File "BrokerServers\TWSServer\TwsSocketClient.dll"
  File "BrokerServers\release\TwsServer.Config.txt"
  CreateShortCut "$SMPROGRAMS\TradeLink Connectors\InteractiveBrokers.lnk" "$INSTDIR\TWSServer.exe" "" "$INSTDIR\TWSServer.exe" 0
SectionEnd

Section "Genesis Connector"
  File "BrokerServers\release\ServerGenesis.exe"  
  File "BrokerServers\release\GTAPI.dll"  
  File "BrokerServers\ServerGenesis\GenesisServer.Config.txt"
  File "BrokerServers\ServerGenesis\GenesisServer.Login.txt"
  CreateShortCut "$SMPROGRAMS\TradeLink Connectors\Genesis.lnk" "$INSTDIR\ServerGenesis.EXE" "" "$INSTDIR\ServerGenesis.EXE" 0  
SectionEnd



;--------------------------------

; Uninstaller

Section "Uninstall"
  
  ; Remove registry keys
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\NSIS_TLBrokerServer"
  DeleteRegKey HKLM SOFTWARE\NSIS_TLBrokerServer
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\App Paths\TradeLibFast.dll"

  

  ; Remove directories used
  RMDir "$INSTDIR\${ANVILDIRNAME}"
  RMDir "$INSTDIR"
  



SectionEnd