

; The name of the installer
Name "TradeLink BrokerServers"
Icon "Install\favicon.ico"



XPStyle on
CRCCheck force
SetOverWrite on
ShowInstDetails hide

; The file to write
OutFile "BrokerServer.exe"

; The default installation directory
InstallDir $PROGRAMFILES\TradeLink\BrokerServer\

; anvil release
!define ANVILDIRNAME "AnvilRelease_x32_2_8_2_0"


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
LicenseData "Install\LICENSE.txt"
LicenseForceSelection radiobuttons "I have read and accept the above" "I decline"
CompletedText "Direct installation questions or problems to http://groups.google.com/group/tradelink-users"


!define VERSION "{PVERSION}"
!include "..\_servers.nsi"

Function .onInit

  ; plugins dir should be automatically removed after installer runs
  InitPluginsDir
  File /oname=$PLUGINSDIR\splash.bmp "Install\brokerserversplash.bmp"
  splash::show 1000 $PLUGINSDIR\splash

  Pop $0 ; $0 has '1' if the user closed the splash screen early,
         ; '0' if everything closed normally, and '-1' if some error occurred.
FunctionEnd




