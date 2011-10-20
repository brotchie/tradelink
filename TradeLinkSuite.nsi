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
!include "Install\Registry.nsh"
!include "Install\FileAssociation.nsh"

; The file to write
OutFile "TradeLinkSuite.exe"

; The default installation directory
InstallDir $PROGRAMFILES\TradeLink\

; Registry key to check for directory (so if you install again, it will 
; overwrite the old one automatically)
;InstallDirRegKey HKLM "Software\TradeLinkSuite" "Install_Dir"

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
  Delete "$SMPROGRAMS\TradeLink\*.*"
  Delete "$SMPROGRAMS\TradeLink Connectors\*.*"
  RMDir "$SMPROGRAMS\TradeLink"
  RMDir "$SMPROGRAMS\TradeLink Connectors"

  CreateDirectory "$SMPROGRAMS\TradeLink"
  CreateShortCut "$SMPROGRAMS\TradeLink\Asp.lnk" "$INSTDIR\ASP.exe" "" "$INSTDIR\ASP.exe" 0
  CreateShortCut "$SMPROGRAMS\TradeLink\Quotopia.lnk" "$INSTDIR\Quotopia.exe" "" "$INSTDIR\Quotopia.exe" 0
  CreateShortCut "$SMPROGRAMS\TradeLink\Gauntlet.lnk" "$INSTDIR\Gauntlet.exe" "" "$INSTDIR\Gauntlet.exe" 0
  CreateShortCut "$SMPROGRAMS\TradeLink\Chartographer.lnk" "$INSTDIR\Chartographer.exe" "" "$INSTDIR\Chartographer.exe" 0
  CreateShortCut "$SMPROGRAMS\TradeLink\Replay.lnk" "$INSTDIR\Replay.exe" "" "$INSTDIR\Replay.exe" 0
  CreateShortCut "$SMPROGRAMS\TradeLink\Time and Sales.lnk" "$INSTDIR\TimeSales.exe" "" "$INSTDIR\TimeSales.exe" 0
  CreateShortCut "$SMPROGRAMS\TradeLink\Uninstall TradeLink.lnk" "$INSTDIR\uninstall.exe" "" "$INSTDIR\uninstall.exe" 0
  CreateShortCut "$SMPROGRAMS\TradeLink\TikConverter.lnk" "$INSTDIR\TikConverter.exe" "" "$INSTDIR\TikConverter.exe" 0
  CreateShortCut "$SMPROGRAMS\TradeLink\TickData.lnk" "$LOCALAPPDATA\TradeLinkTicks\" "" "" 0
  CreateShortCut "$SMPROGRAMS\TradeLink\Logs.lnk" "$LOCALAPPDATA\" "" "$LOCALAPPDATA\" 0
  CreateShortCut "$SMPROGRAMS\TradeLink\Kadina.lnk" "$INSTDIR\Kadina.EXE" "" "$INSTDIR\Kadina.EXE" 0 
  CreateShortCut "$SMPROGRAMS\TradeLink\Record.lnk" "$INSTDIR\Record.EXE" "" "$INSTDIR\Record.EXE" 0  

  CreateDirectory "$SMPROGRAMS\TradeLink Connectors"
  CreateShortCut "$SMPROGRAMS\TradeLink Connectors\HoldBros-GrayBox.lnk" "$INSTDIR\GbTLServer.exe" "" "$INSTDIR\GbTLServer.exe" 0
  CreateShortCut "$SMPROGRAMS\TradeLink Connectors\SterlingPro.lnk" "$INSTDIR\SterServer.EXE" "" "$INSTDIR\SterServer.EXE" 0  
  CreateShortCut "$SMPROGRAMS\TradeLink Connectors\TDAmeritrade.lnk" "$INSTDIR\TDServer.EXE" "" "$INSTDIR\TDServer.EXE" 0  
  CreateShortCut "$SMPROGRAMS\TradeLink Connectors\TDAmeritradeX.lnk" "$INSTDIR\TDServerX.EXE" "" "$INSTDIR\TDServerX.EXE" 0  
  CreateShortCut "$SMPROGRAMS\TradeLink Connectors\MBTrading.lnk" "$INSTDIR\ServerMB.EXE" "" "$INSTDIR\ServerMB.EXE" 0  
  CreateShortCut "$SMPROGRAMS\TradeLink Connectors\Esignal.lnk" "$INSTDIR\ServerEsignal.exe" "" "$INSTDIR\ServerEsignal.exe" 0  
  CreateShortCut "$SMPROGRAMS\TradeLink Connectors\DBFX.lnk" "$INSTDIR\ServerDBFX.exe" "" "$INSTDIR\ServerDBFX.exe" 0  
  CreateShortCut "$SMPROGRAMS\TradeLink Connectors\Blackwood.lnk" "$INSTDIR\ServerBlackwood.exe" "" "$INSTDIR\ServerBlackwood.exe" 0  
  CreateShortCut "$SMPROGRAMS\TradeLink Connectors\REDI.lnk" "$INSTDIR\ServerRedi.exe" "" "$INSTDIR\ServerRedi.exe" 0  
  CreateShortCut "$SMPROGRAMS\TradeLink Connectors\IQFeed.lnk" "$INSTDIR\IQFeedBroker.exe" "" "$INSTDIR\IQFeedBroker.exe" 0  
  CreateShortCut "$SMPROGRAMS\TradeLink Connectors\NxCore32.lnk" "$INSTDIR\ServerNxCore32.exe" "" "$INSTDIR\ServerNxCore32.exe" 0  
  CreateShortCut "$SMPROGRAMS\TradeLink Connectors\NxCore64.lnk" "$INSTDIR\ServerNxCore64.exe" "" "$INSTDIR\ServerNxCore64.exe" 0  
  CreateShortCut "$SMPROGRAMS\TradeLink Connectors\RealTick.lnk" "$INSTDIR\RealTickConnector.exe" "" "$INSTDIR\RealTickConnector.exe" 0  
  CreateShortCut "$SMPROGRAMS\TradeLink Connectors\DAS.lnk" "$INSTDIR\ServerDAS.exe" "" "$INSTDIR\ServerDAS.exe" 0  
  CreateShortCut "$SMPROGRAMS\TradeLink Connectors\Rithmic.lnk" "$INSTDIR\ServerRithmic.exe" "" "$INSTDIR\ServerRithmic.exe" 0  
  
  
  ; install these files
  File "VERSION.txt"
  File "Kadina\bin\release\Kadina.exe"
  File "Replay\bin\release\Replay.exe"
  File "ServerSterling\bin\x86\release\SterServer.exe"
  File "ServerSterling\bin\x86\release\Interop.SterlingLib.dll"
  
  File "ServerRithmic\bin\release\ServerRithmic.exe"
  File "ServerRithmic\bin\release\ServerRithmic.exe.config"
  File "ServerRithmic\bin\release\rapi.dll"
  File "ServerRithmic\bin\release\RithmicCertificate.pk12"
  

  
  File "Responses\TA-Lib-Core.dll"
  File "Responses\bin\release\Responses.dll"
  File "Quotopia\bin\release\Quotopia.exe"
  File "Quotopia\bin\release\Multimedia.dll"
  File "TradeLinkCommon\bin\release\TradeLinkCommon.dll"
  File "TradeLinkCommon\bin\Release\TradeLinkCommon.XML"
  File "TradeLinkAppKit\bin\release\Ionic.zip.dll"
  File "TradeLinkAppKit\bin\release\TradeLinkAppKit.dll"
  File "TradeLinkAppKit\bin\Release\TradeLinkAppKit.XML"
  File "TradeLinkApi\bin\release\TradeLinkApi.dll"
  File "TradeLinkApi\bin\Release\TradeLinkAPI.XML"
  File "Gauntlet\bin\release\Gauntlet.exe"
  File "Gauntlet\bin\release\EarlyClose.csv"
  File "Chartographer\bin\release\Chartographer.exe"
  File "TimeAndSales\bin\release\TimeSales.exe"
  File "ASP\bin\release\ASP.exe"
  File "TradeLinkResearch\bin\release\TradeLinkResearch.dll"
  File "TradeLinkResearch\bin\Release\TradeLinkResearch.XML"
  File "Record\bin\release\Record.exe"
  File "ServerMB\bin\x86\release\ServerMB.exe"
  File "ServerMB\bin\x86\release\Interop.MBTCOMLib.dll"  
  File "ServerMB\bin\x86\release\Interop.MBTHISTLib.dll"    
  File "ServerMB\bin\x86\release\Interop.MBTORDERSLib.dll"    
  File "ServerMB\bin\x86\release\Interop.MBTQUOTELib.dll"   
  File "Install\VCRedistInstall.exe"
  File "Install\TickDataInstall.exe"
  File "Install\TickDataMigrate.exe"
  File /nonfatal "ServerEsignal\bin\release\Interop.IESignal.dll"
  
  File "ServerGrayBox\bin\release\GbTLServer.exe"
  File "ServerGrayBox\bin\release\GBAPILibrary.dll"
  File "ServerGrayBox\bin\release\Interop.GBXCTRLLib.dll"
  File "ServerGrayBox\bin\release\Interop.GBQUOTESLib.dll"
  
  File "ServerDAS\bin\release\ServerDAS.exe"
  File "ServerDAS\bin\release\daslibrary.dll"
  File "ServerDAS\bin\release\Imass.EventPool.dll"


  File "ServerDBFX\bin\release\ServerDBFX.exe"
  File "ServerDBFX\bin\release\Interop.FXCore.dll"

  File "ServerBlackwood\bin\release\ServerBlackwood.exe"
  File "ServerBlackwood\bin\release\Blackwood.Framework.dll"
  File "ServerBlackwood\bin\release\BWCMessageLib.dll"
  File "ServerBlackwood\bin\release\zlib.net.dll"
  File "ServerRedi\bin\release\ServerRedi.exe"
  File "ServerRedi\bin\release\Interop.RediLib.dll"
  File "ServerRedi\bin\release\VBRediClasses.dll"
  
  File "ServerRealTick\bin\release\RealTickConnector.exe"
  File "ServerRealTick\dll\*"
  
  
  File "ServerNxCore\dll\NxCoreAPI64.dll"
  File "ServerNxCore64\bin\x64\release\ServerNxCore64.exe"
  File "ServerNxCore\bin\x86\release\ServerNxCore32.exe"
  File "ServerNxCore\dll\NxCoreAPI.dll"
  

  File "TikConverter\bin\release\TikConverter.exe"
  
  File "ServerFIX\bin\release\ServerFIX.exe"

  File "ServerFIX\bin\release\SessionSettings.txt"
  File "ServerFIX\bin\release\quickfix_net.dll"
  File "ServerFIX\bin\release\quickfix_net_messages.dll"
  File "ServerFIX\bin\release\quickfix_net_test.dll"
  
    ; may fail if you don't have 3rd party vendor software installed
  File /nonfatal "ServerEsignal\bin\release\ServerEsignal.exe"
  File /nonfatal "ServerIQ-DTN\bin\Release\IQFeedBroker.exe"
    File /nonfatal "ServerTD\bin\release\TDServer.exe"
  File /nonfatal "ServerTD\bin\release\Interop.MSXML2.dll"
  File /nonfatal "Install\libcurl.dll"
  File /nonfatal "Install\LibCurlNet.dll"
  File /nonfatal "Install\LibCurlShim.dll"
  File /nonfatal "ServerTD\bin\release\NZipLib.dll"
  File /nonfatal "ServerTDX\bin\release\TDServerX.exe"
  File /nonfatal "ServerTDX\bin\release\AxInterop.tdaactx.dll"
  File /nonfatal "ServerTDX\bin\release\Interop.tdaactx.dll"
  File /nonfatal "ServerTDX\bin\release\Interop.MSXML2.dll"
  File /nonfatal "ServerTDX\bin\release\Interop.MSXML2.dll"
  File /nonfatal "ServerTDX\ActiveXComp\ActiveX\TDAACTX.ocx"
  RegDLL "$INSTDIR\TDAACTX.ocx"
  
    ; don't overwrite config files files
  SetOverWrite off
  File "ServerFIX\bin\release\ServerFIX.exe.config"
  File "ServerNxCore\bin\x86\release\ServerNxCore32.exe.config"
  File "ServerNxCore64\bin\x64\release\ServerNxCore64.exe.config"
  File "ServerRedi\bin\release\ServerRedi.exe.config"
	File "ServerDAS\bin\release\ServerDAS.exe.config"
  File "Kadina\bin\release\Kadina.exe.config"
  File "ServerSterling\bin\x86\release\SterServer.exe.config"
  File "Replay\bin\release\Replay.exe.config"
  File "Quotopia\bin\release\Quotopia.exe.config"
  File "Gauntlet\bin\release\Gauntlet.exe.config"
  File "Chartographer\bin\release\Chartographer.exe.config"
  File "TimeAndSales\bin\release\TimeSales.exe.config"
  File "ASP\bin\release\ASP.exe.config"
  File "Record\bin\release\Record.exe.config"
  File "ServerMB\bin\x86\release\ServerMB.exe.config"

  ; these may fail if you do not have 3rd party vendor software installed
  File /nonfatal "ServerIQ-DTN\bin\Release\IQFeedBroker.exe.config"
  File "ServerDBFX\bin\release\ServerDBFX.exe.config"
  File "ServerBlackwood\bin\release\ServerBlackwood.exe.config"
  File /nonfatal "ServerTD\bin\release\TDServer.exe.config"
  File /nonfatal "ServerTDX\bin\release\TDServerX.exe.config"
  File /nonfatal "ServerEsignal\bin\release\ServerEsignal.exe.config"

  
  ; done writing config files
  SetOverWrite on

  ; removing brokerserver folder as people transition to single installer
  Delete "$PROGRAMFILES\TradeLink\BrokerServer\*.*"  
  RMDir /r "$PROGRAMFILES\TradeLink\BrokerServer"
    Delete "$PROGRAMFILES\TradeLink\TradeLinkSuite\*.*"  
  RMDir /r "$PROGRAMFILES\TradeLink\TradeLinkSuite"
  
  ; Write the installation path into the registry
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\TradeLinkSuite" "Install_Dir" "$INSTDIR"
    
  ; make sure TickData folder is present
  CreateDirectory "$LOCALAPPDATA\TradeLinkTicks"
  
  ; check for .net35
  File "Install\DotNetInstaller.exe"
  DetailPrint "Checking for .net35..."
  ReadRegStr $0 HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\TradeLinkSuite" "InstalledDOTNET35"
  StrCmp $0 "Yes" dotnetinstalled
  Call IsNetfx35Installed
  
 dotnetinstalled:
  
  
  ; skip tickdata and vcredistuble installs if running in silent mode
  StrCmp $SILENT "YES" finishinstall


tickdata:  
  DetailPrint "Checking for TickData..."
  ReadRegStr $0 HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\TradeLinkSuite" "InstalledTickData"
  StrCmp $0 "Yes" vcredistinstall
  ExecWait "TickDataInstall.exe"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\TradeLinkSuite" "InstalledTickData" "Yes"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\TradeLinkSuite" "TickDataPath" "$LOCALAPPDATA\TradeLinkTicks"
  DetailPrint "TickData was installed."
  
  
 vcredistinstall:
  DetailPrint "Checking for VCRedistributable..."
  ReadRegStr $0 HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\TradeLinkSuite" "InstalledVcRedist"
  StrCmp $0 "Yes" finishinstall
  ExecWait "VCRedistInstall.exe"
  DetailPrint "VCRedistributable was installed."
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\TradeLinkSuite" "InstalledVcRedist" "Yes"
  
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
  ExecShell "open" "http://www.pracplay.com/landings/afterinstallingtradelink"
  final:

SectionEnd

;--------------------------------

!if $%INCLUDEBS% == "BS"
!define VERSION "{PVERSION}"
!include "_servers.nsh"
!else

!endif

; Uninstaller

Section "ResponseExamples"
  File /r /x *.user /x *.xml /x *.dll /x *.pdf /x *.csv /x *.txt /x *.Cache /x .svn /x obj /x bin Responses
  File /r Responses\TA*.*
  CreateShortCut "$SMPROGRAMS\TradeLink\ResponseExamples.lnk" "$INSTDIR\Responses\Responses.csproj" "" "$INSTDIR\Responses\Responses.csproj" 0
  
SectionEnd

Section "TA-Lib Excel Example and Plugins"
  File /r "Install\talib_excel"
SectionEnd

Section "CSharpIde (Write Strategies)"

 File "Install\CsharpIdeInstall.exe"
   
 vcredistinstall:
  DetailPrint "Checking for CSharpIDE..."
  ReadRegStr $0 HKLM SOFTWARE\TradeLinkSuite "InstalledCSharpIDE"
  StrCmp $0 "Yes" finishinstall
  ExecWait "CsharpIdeInstall.exe"
  DetailPrint "CSharpIDE was installed."
  WriteRegStr HKLM SOFTWARE\TradeLinkSuite "InstalledCSharpIDE" "Yes"
  
finishinstall:  

SectionEnd

Section "Anonymous Suite Usage Tracking"

  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\TradeLinkSuite" "TrackUsage" "Yes"
  
finishinstall:  

SectionEnd


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
  Delete "$SMPROGRAMS\TradeLink\*.*"
  Delete "$SMPROGRAMS\TradeLink Connectors\*.*"

  ; Remove directories used
  RMDir "$SMPROGRAMS\TradeLink"
  RMDir "$SMPROGRAMS\TradeLink Connectors"
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

Function IsNetfx35Installed
;Check is Net 3.5 is install
;Push 0 for true, Pop -1 for false
${registry::Read} 'HKEY_LOCAL_MACHINE\Software\Microsoft\NET Framework Setup\NDP\v3.5' “SP” $R0 $R1
${If} $R1 == ”
;Push -1
ExecWait "DotNetInstaller.exe"
WriteRegStr HKLM SOFTWARE\TradeLinkSuite "InstalledDOTNET35" "Yes"
${Else}
;Push 0
DetailPrint ".net35 found"
${EndIf}
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








