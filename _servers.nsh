
!define ANVILDIRNAME "AnvilRelease_x32_2_8_4_3"

!include "Install\nsDialogs.nsh"

Page custom nsDialogsPage
Var Dialog
Var Label
Var Text
Var ANVILDIRNAME
Var Text_State
Var Checkbox
Var Checkbox_State
Var DOANVIL

Function nsDialogsPage

	${If} $DOANVIL != "INSTALLANVIL"
		Abort
	${EndIf}
	nsDialogs::Create 1018
	Pop $Dialog

	${If} $Dialog == error
		Abort
	${EndIf}
	
	${NSD_CreateLabel} 0 0 100% 12u "Provide path to anvil"
	Pop $Label
	
		

	
	nsDialogs::SelectFolderDialog "Select Anvil Release Folder" "$PROGRAMFILES\Anvil"

	Pop $ANVILDIRNAME 
	${If} $Text == error
		Abort
	${EndIf}
	
	nsDialogs::Show

	
	  CreateShortCut "$SMPROGRAMS\TradeLink Connectors\Assent.lnk" "$INSTDIR\${ANVILDIRNAME}\Anvil.exe" "" "$INSTDIR\${ANVILDIRNAME}\Anvil.exe" 0
  ; Put file there
  ;File /nonfatal /r "BrokerServers\Install\${ANVILDIRNAME}" 
  File "/oname=$INSTDIR\${ANVILDIRNAME}\AnvilServer.dll" "BrokerServers\AnvilServer\Release\AnvilServer.dll"
  File "/oname=$INSTDIR\${ANVILDIRNAME}\AnvilServer.Config.txt" "BrokerServers\AnvilServer\Release\AnvilServer.Config.txt"
  WriteINIStr "$INSTDIR\${ANVILDIRNAME}\Anvil.ini" Extension Path "$INSTDIR\${ANVILDIRNAME}\"
	DetailPrint "Installed AnvilServer in $ANVILDIRNAME"

FunctionEnd

Section "TradeLibFast"

    ; Set output path to the installation directory.
	DetailPrint "Installing version ${VERSION}..."
  SectionIn RO
  SetOutPath $INSTDIR
  CreateDirectory "$SMPROGRAMS\TradeLink Connectors"
  
  ; files included in install
  File "BrokerServers\release\TradeLibFast.dll"

  
finishinstall:  
  
  ; write path for TradeLibFast.dll
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\App Paths\TradeLibFast.dll" "Path" "$INSTDIR\"
  


SectionEnd





; The stuff to install
Section "AnvilServer"

	StrCpy $DOANVIL "INSTALLANVIL"


SectionEnd

Section "InteractiveBrokers Server"
  File "BrokerServers\release\TWSServer.exe"
  File "BrokerServers\TWSServer\TwsSocketClient.dll"
  File "BrokerServers\release\TwsServer.Config.txt"
  CreateShortCut "$SMPROGRAMS\TradeLink Connectors\InteractiveBrokers.lnk" "$INSTDIR\TWSServer.exe" "" "$INSTDIR\TWSServer.exe" 0
SectionEnd

Section "Genesis Server"
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