; The name of the installer
Name "TradeLinkSuite"
XPStyle on
CRCCheck force
ShowInstDetails hide
SetOverWrite on
Icon "Install\tradelinkinstaller.ico"

!define SHORT_APP_NAME "TradeLink"
!define SUPPORT_EMAIL "tradelink-users@googlegroups.com"
!addplugindir Install
!include "Install\DotNET.nsh"
!include "Install\FileAssociation.nsh"

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
  !insertmacro CheckDotNET
  
  ; Set output path to the installation directory.
  SetOutPath $INSTDIR
  DetailPrint "Installing version ${PVERSION}..."
  ; remove existing shortcuts and recreate
  	
  
  CreateDirectory "$SMPROGRAMS\TradeLink"
  CreateShortCut "$SMPROGRAMS\TradeLink\Asp.lnk" "$INSTDIR\ASP.exe" "" "$INSTDIR\ASP.exe" 0
  CreateShortCut "$SMPROGRAMS\TradeLink\Quotopia.lnk" "$INSTDIR\Quotopia.exe" "" "$INSTDIR\Quotopia.exe" 0
  CreateShortCut "$SMPROGRAMS\TradeLink\Gauntlet.lnk" "$INSTDIR\Gauntlet.exe" "" "$INSTDIR\Gauntlet.exe" 0
  CreateShortCut "$SMPROGRAMS\TradeLink\Chartographer.lnk" "$INSTDIR\Chartographer.exe" "" "$INSTDIR\Chartographer.exe" 0
  CreateShortCut "$SMPROGRAMS\TradeLink\Replay.lnk" "$INSTDIR\Replay.exe" "" "$INSTDIR\Replay.exe" 0
  CreateShortCut "$SMPROGRAMS\TradeLink\Time and Sales.lnk" "$INSTDIR\TimeSales.exe" "" "$INSTDIR\TimeSales.exe" 0
  CreateShortCut "$SMPROGRAMS\TradeLink\Uninstall TradeLink.lnk" "$INSTDIR\uninstall.exe" "" "$INSTDIR\uninstall.exe" 0
  CreateShortCut "$SMPROGRAMS\TradeLink\Kadina.lnk" "$INSTDIR\Kadina.EXE" "" "$INSTDIR\Kadina.EXE" 0 
  CreateShortCut "$SMPROGRAMS\TradeLink\Record.lnk" "$INSTDIR\Record.EXE" "" "$INSTDIR\Record.EXE" 0  
  CreateShortCut "$SMPROGRAMS\TradeLink\SterlingPro.lnk" "$INSTDIR\SterServer.EXE" "" "$INSTDIR\SterServer.EXE" 0  
  CreateShortCut "$SMPROGRAMS\TradeLink\TDAmeritrade.lnk" "$INSTDIR\TDServer.EXE" "" "$INSTDIR\TDServer.EXE" 0  
  CreateShortCut "$SMPROGRAMS\TradeLink\TDAmeritradeX.lnk" "$INSTDIR\TDServerX.EXE" "" "$INSTDIR\TDServerX.EXE" 0  
  CreateShortCut "$SMPROGRAMS\TradeLink\MBTrading.lnk" "$INSTDIR\ServerMB.EXE" "" "$INSTDIR\ServerMB.EXE" 0  
  CreateShortCut "$SMPROGRAMS\TradeLink\Esignal.lnk" "$INSTDIR\ServerEsignal.exe" "" "$INSTDIR\ServerEsignal.exe" 0  
  CreateShortCut "$SMPROGRAMS\TradeLink\DBFX.lnk" "$INSTDIR\ServerDBFX.exe" "" "$INSTDIR\ServerDBFX.exe" 0  
  CreateShortCut "$SMPROGRAMS\TradeLink\TikConverter.lnk" "$INSTDIR\TikConverter.exe" "" "$INSTDIR\TikConverter.exe" 0
  CreateShortCut "$SMPROGRAMS\TradeLink\Blackwood.lnk" "$INSTDIR\ServerBlackwood.exe" "" "$INSTDIR\ServerBlackwood.exe" 0  
  CreateShortCut "$SMPROGRAMS\TradeLink\REDI.lnk" "$INSTDIR\ServerRedi.exe" "" "$INSTDIR\ServerRedi.exe" 0  
  CreateShortCut "$SMPROGRAMS\TradeLink\LogViewer.lnk" "$INSTDIR\LogViewer.exe" "" "$INSTDIR\LogViewer.exe" 0  
  CreateShortCut "$SMPROGRAMS\TradeLink\IQFeed.lnk" "$INSTDIR\IQFeedBroker.exe" "" "$INSTDIR\IQFeedBroker.exe" 0  
  
  
  ; install these files
  File "VERSION.txt"
  File "Kadina\bin\release\Kadina.exe"
  File "Kadina\bin\release\Kadina.exe.config"
  File "Replay\bin\release\Replay.exe"
  File "ServerSterling\bin\release\SterServer.exe"
  File "ServerSterling\Interop.SterlingLib.dll"
  File /nonfatal "ServerTD\bin\release\TDServer.exe.config"
  File /nonfatal "ServerTD\bin\release\TDServer.exe"
  File /nonfatal "ServerTD\bin\release\Interop.MSXML2.dll"
  File /nonfatal "ServerTD\bin\release\NZipLib.dll"
  File /nonfatal "ServerTDX\bin\release\TDServerX.exe"
  File /nonfatal "ServerTDX\bin\release\AxInterop.tdaactx.dll"
  File /nonfatal "ServerTDX\bin\release\Interop.tdaactx.dll"
  File /nonfatal "ServerTDX\bin\release\Interop.MSXML2.dll"
  File /nonfatal "ServerTDX\bin\release\Interop.MSXML2.dll"
  File /nonfatal "ServerTDX\ActiveXComp\ActiveX\TDAACTX.ocx"
  RegDLL "$INSTDIR\TDAACTX.ocx"
  
  
  File "Responses\bin\release\Responses.dll"
  File "Quotopia\bin\release\Quotopia.exe"
  File "Quotopia\bin\release\Quotopia.exe.config"
  File "Quotopia\bin\release\Multimedia.dll"
  File "TradeLinkCommon\bin\release\TradeLinkCommon.dll"
  File "TradeLinkAppKit\bin\release\TradeLinkAppKit.dll"
  File "TradeLinkApi\bin\release\TradeLinkApi.dll"
  File "Gauntlet\bin\release\Gauntlet.exe"
  File "Gauntlet\bin\release\Gauntlet.exe.config"
  File "Gauntlet\bin\release\EarlyClose.csv"
  File "Chartographer\bin\release\Chartographer.exe.config"
  File "Chartographer\bin\release\Chartographer.exe"
  File "TimeAndSales\bin\release\TimeSales.exe"
  File "ASP\bin\release\ASP.exe"
  File "TradeLinkResearch\bin\release\TradeLinkResearch.dll"
  File "Record\bin\release\Record.exe"
  File "ServerMB\bin\release\ServerMB.exe"
  File "ServerMB\bin\release\ServerMB.exe.config"
  File "ServerMB\bin\release\Interop.MBTCOMLib.dll"  
  File "ServerMB\bin\release\Interop.MBTHISTLib.dll"    
  File "ServerMB\bin\release\Interop.MBTORDERSLib.dll"    
  File "ServerMB\bin\release\Interop.MBTQUOTELib.dll"   
  File "Install\VCRedistInstall.exe"
  File "Install\TickDataInstall.exe"
  File "ServerEsignal\bin\release\Interop.IESignal.dll"
  File "ServerEsignal\bin\release\ServerEsignal.exe"
  File "ServerEsignal\bin\release\ServerEsignal.exe.config"
  File "ServerDBFX\bin\release\ServerDBFX.exe"
  File "ServerDBFX\bin\release\ServerDBFX.exe.config"
  File "ServerDBFX\bin\release\Interop.FXCore.dll"
  File "ServerBlackwood\bin\release\ServerBlackwood.exe.config"
  File "ServerBlackwood\bin\release\ServerBlackwood.exe"
  File "ServerBlackwood\bin\release\Blackwood.Framework.dll"
  File "ServerBlackwood\bin\release\BWCMessageLib.dll"
  File "ServerBlackwood\bin\release\zlib.net.dll"
  File "LogViewer\bin\release\LogViewer.exe"
  File "LogViewer\bin\release\LogViewer.exe.config"
  File "ServerRedi\bin\release\ServerRedi.exe"
  File "ServerRedi\bin\release\ServerRedi.exe.config"
  File "ServerRedi\bin\release\Interop.RediLib.dll"
  File "ServerRedi\bin\release\VBRediClasses.dll"
  File "ServerIQ-DTN\bin\Release\IQFeedBroker.exe"
  File "ServerIQ-DTN\bin\Release\IQFeedBroker.exe.config"

  File "TikConverter\bin\release\TikConverter.exe"

  ; removing brokerserver folder as people transition to single installer
  Delete "$PROGRAMFILES\TradeLink\BrokerServer\*.*"  
  RMDir /r "$PROGRAMFILES\TradeLink\BrokerServer"
  
  ; Write the installation path into the registry
  WriteRegStr HKLM SOFTWARE\TradeLinkSuite "Install_Dir" "$INSTDIR"
    
  ; make sure TickData folder is present
  CreateDirectory $PROGRAMFILES\TradeLink\TickData
  
  ; skip tickdata and vcredistuble installs if running in silent mode
  StrCmp $SILENT "YES" finishinstall
  
  DetailPrint "Checking for TickData..."
  ReadRegStr $0 HKLM SOFTWARE\TradeLinkSuite "InstalledTickData"
  StrCmp $0 "Yes" vcredistinstall
  ExecWait "TickDataInstall.exe"
  WriteRegStr HKLM SOFTWARE\TradeLinkSuite "InstalledTickData" "Yes"
  DetailPrint "TickData was installed."
  
 vcredistinstall:
  DetailPrint "Checking for VCRedistributable..."
  ReadRegStr $0 HKLM SOFTWARE\TradeLinkSuite "InstalledVcRedist"
  StrCmp $0 "Yes" finishinstall
  ExecWait "VCRedistInstall.exe"
  DetailPrint "VCRedistributable was installed."
  WriteRegStr HKLM SOFTWARE\TradeLinkSuite "InstalledVcRedist" "Yes"
  
finishinstall:  
  ; Write the uninstall keys for Windows
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\TradeLinkSuite" "DisplayName" "TradeLinkSuite"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\TradeLinkSuite" "Path" "$INSTDIR\"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\TradeLinkSuite" "Version" "${PVERSION}"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\TradeLinkSuite" "UninstallString" '"$INSTDIR\uninstall.exe"'
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\TradeLinkSuite" "NoModify" 1
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\TradeLinkSuite" "NoRepair" 1
  WriteUninstaller "uninstall.exe"
  
  ; register tik extension
  ${registerExtension} "$INSTDIR\TimeSales.exe" ".tik" "TradeLink TickData"
  
  ; don't open browser on silent
  StrCmp $SILENT "YES" final
  ExecShell "open" "http://franta.com/support"
  final:

SectionEnd

Section "ResponseExamples"
  File /r /x *.user /x *.xml /x *.dll /x *.pdf /x *.csv /x *.txt /x *.Cache /x .svn /x obj /x bin Responses
  CreateShortCut "$SMPROGRAMS\TradeLink\ResponseExamples.lnk" "$INSTDIR\Responses\Responses.csproj" "" "$INSTDIR\Responses\Responses.csproj" 0
SectionEnd
;--------------------------------

!if $%INCLUDEBS% == "BS"
!define VERSION "{PVERSION}"
!include "_servers.nsh"
!else

!endif

; Uninstaller

Section "Uninstall"
  
  ; unregister com components
  UnRegDLL "$INSTDIR\TDAACTX.ocx"
  ; Remove registry keys
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\TradeLinkSuite"
  DeleteRegKey HKLM SOFTWARE\TradeLinkSuite
  ; unregister tickdata
  ${unregisterExtension} ".tik" "TradeLink TickData"

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
  Delete "$SMPROGRAMS\TradeLink\Record.lnk"
  Delete "$SMPROGRAMS\TradeLink\Sterling.lnk"
  Delete "$SMPROGRAMS\TradeLink\Gauntlet.lnk"
  Delete "$SMPROGRAMS\TradeLink\TDServer.lnk"
  Delete "$SMPROGRAMS\TradeLink\Uninstall TradeLinkSuite"
  Delete "$SMPROGRAMS\TradeLink\MB.lnk"
  Delete "$SMPROGRAMS\TradeLink\Esignal.lnk"
  Delete "$SMPROGRAMS\TradeLink\DBFX.lnk"
  Delete "$SMPROGRAMS\TradeLink\Blackwood.lnk"
  Delete "$SMPROGRAMS\TradeLink\LogViewer.lnk"

  ; Remove directories used
  RMDir "$SMPROGRAMS\TradeLink"
  RMDir "$INSTDIR"

  SectionEnd
  
  
Function .onInit

  Call IsSilent
  ; plugins dir should be automatically removed after installer runs
  InitPluginsDir
  File /oname=$PLUGINSDIR\splash.bmp "Install\tradelinklogo.bmp"
  splash::show 1000 $PLUGINSDIR\splash

  		 
  Pop $0 ; $0 has '1' if the user closed the splash screen early,
         ; '0' if everything closed normally, and '-1' if some error occurred.
FunctionEnd

Function IsSilent
  Var /GLOBAL SILENT
  StrCpy $SILENT "NO"
  Push $0
  Push $CMDLINE
  Push "/S"
  Call StrStr
  Pop $0
  StrCpy $0 $0 3
  StrCmp $0 "/S" silent
  StrCmp $0 "/S " silent
    StrCpy $0 0
    Goto notsilent
  silent: StrCpy $SILENT "YES"
  notsilent: StrCpy $SILENT "NO"
FunctionEnd

Function StrStr
  Exch $R1 ; st=haystack,old$R1, $R1=needle
  Exch    ; st=old$R1,haystack
  Exch $R2 ; st=old$R1,old$R2, $R2=haystack
  Push $R3
  Push $R4
  Push $R5
  StrLen $R3 $R1
  StrCpy $R4 0
  ; $R1=needle
  ; $R2=haystack
  ; $R3=len(needle)
  ; $R4=cnt
  ; $R5=tmp
  loop:
    StrCpy $R5 $R2 $R3 $R4
    StrCmp $R5 $R1 done
    StrCmp $R5 "" done
    IntOp $R4 $R4 + 1
    Goto loop
  done:
  StrCpy $R1 $R2 "" $R4
  Pop $R5
  Pop $R4
  Pop $R3
  Pop $R2
  Exch $R1
FunctionEnd






