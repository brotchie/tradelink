; The name of the installer
Name "TradeLinkSuite"
XPStyle on
CRCCheck force
ShowInstDetails show
Icon "tradelinkinstaller.ico"

; The file to write
OutFile "TradeLinkSuite.exe"

; The default installation directory
InstallDir $PROGRAMFILES\TradeLink\TradeLink

; Registry key to check for directory (so if you install again, it will 
; overwrite the old one automatically)
InstallDirRegKey HKLM "Software\NSIS_TradeLink" "Install_Dir"

; Pages
Page components
Page directory
Page instfiles

UninstPage uninstConfirm
UninstPage instfiles

CompletedText "For additional documentation, see http://code.google.com/p/tradelink"

; The stuff to install
Section "TradeLinkSuite"

  SectionIn RO
  
  ; Set output path to the installation directory.
  SetOutPath $INSTDIR
  
  CreateDirectory "$SMPROGRAMS\TradeLink"
  CreateShortCut "$SMPROGRAMS\TradeLink\Quotopia.lnk" "$INSTDIR\Quotopia.exe" "" "$INSTDIR\Quotopia.exe" 0
  CreateShortCut "$SMPROGRAMS\TradeLink\Gauntlet.lnk" "$INSTDIR\Gauntlet.exe" "" "$INSTDIR\Gauntlet.exe" 0
  CreateShortCut "$SMPROGRAMS\TradeLink\Chartographer.lnk" "$INSTDIR\Chartographer.exe" "" "$INSTDIR\Chartographer.exe" 0
  CreateShortCut "$SMPROGRAMS\TradeLink\Replay.lnk" "$INSTDIR\TLReplay.exe" "" "$INSTDIR\TLReplay.exe" 0
  CreateShortCut "$SMPROGRAMS\TradeLink\Time and Sales.lnk" "$INSTDIR\TimeSales.exe" "" "$INSTDIR\TimeSales.exe" 0
  CreateShortCut "$SMPROGRAMS\TradeLink\Uninstall TradeLink.lnk" "$INSTDIR\uninstall.exe" "" "$INSTDIR\uninstall.exe" 0
  CreateShortCut "$SMPROGRAMS\TradeLink\Epf-To-Idx.lnk" "$INSTDIR\EPF2IDX.EXE" "" "$INSTDIR\EPF2IDX.EXE" 0  
  CreateShortCut "$SMPROGRAMS\TradeLink\SplitEPF.lnk" "$INSTDIR\SplitEPF.EXE" "" "$INSTDIR\SplitEPF.EXE" 0  
  
  
  ; Put file there
  File "TLReplay.exe"
  File "TLReplay.exe.config"
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
  File "calcpl.xla"  
  File /r /x ".svn" "Properties"
  ; Write the installation path into the registry
  WriteRegStr HKLM SOFTWARE\NSIS_TradeLink "Install_Dir" "$INSTDIR"
  	
  
  
  ; Write the uninstall keys for Windows
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\TradeLink" "DisplayName" "TradeLink"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\TradeLink" "UninstallString" '"$INSTDIR\uninstall.exe"'
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\TradeLink" "NoModify" 1
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\TradeLink" "NoRepair" 1
  
    ; where you would normally put WriteUninstaller ${INSTDIR}\uninstaller.exe put instead:
 
!ifndef INNER
  SetOutPath ${INSTDIR}
 
  ; this packages the signed uninstaller
 
  File $%TEMP%\uninstaller.exe
!endif
 

SectionEnd
;--------------------------------

; Uninstaller

Section "Uninstall"




SectionEnd




!ifdef INNER
  !echo "Inner invocation"                  ; just to see what's going on
  OutFile "$%TEMP%\tempinstaller.exe"       ; not really important where this is
!else
  !echo "Outer invocation"
 
  ; Call makensis again, defining INNER.  This writes an installer for us which, when
  ; it is invoked, will just write the uninstaller to some location, and then exit.
  ; Be sure to substitute the name of this script here.
 
  !system "$\"${NSISDIR}\makensis$\" /DINNER TradeLink.nsi" = 0
 
  ; So now run that installer we just created as %TEMP%\tempinstaller.exe.  Since it
  ; calls quit the return value isn't zero.
 
  !system "$%TEMP%\tempinstaller.exe" = 2
 
  ; That will have written an uninstaller binary for us.  Now we sign it with your
  ; favourite code signing tool.
 
  !system "SIGNTOOL sign /f ..\tls.pfx /p tradelink $%TEMP%\uninstaller.exe" = 0
 
  ; Good.  Now we can carry on writing the real installer.
 
  OutFile "TradeLinkSuite.exe"
!endif
 
 
Function .onInit
!ifdef INNER
 
  ; If INNER is defined, then we aren't supposed to do anything except write out
  ; the installer.  This is better than processing a command line option as it means
  ; this entire code path is not present in the final (real) installer.
 
  WriteUninstaller "$%TEMP%\uninstaller.exe"
  Quit  ; just bail out quickly when running the "inner" installer
!endif
 
  ; plugins dir should be automatically removed after installer runs
  InitPluginsDir
  File /oname=$PLUGINSDIR\splash.bmp "tradelinklogo.bmp"
  splash::show 5000 $PLUGINSDIR\splash

  Pop $0 ; $0 has '1' if the user closed the splash screen early,
         ; '0' if everything closed normally, and '-1' if some error occurred.
FunctionEnd
 
 
!ifdef INNER
Section "Uninstall"
 
  ; your normal uninstaller section or sections (they're not needed in the "outer"
  ; installer and will just cause warnings because there is no WriteInstaller command)
  
    
  ; Remove registry keys
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\TradeLink"
  DeleteRegKey HKLM SOFTWARE\NSIS_TradeLink

  ; Remove files and uninstaller
  Delete $INSTDIR\TradeLinkSuite.nsi
  Delete $INSTDIR\uninstall.exe
  Delete "$INSTDIR\*.*"
  Delete "$INSTDIR\Properties\*.*"
  RMDir "$INSTDIR\Properties"
  ; Remove shortcuts, if any
  Delete "$SMPROGRAMS\TradeLink\*.*"

  ; Remove directories used
  RMDir "$SMPROGRAMS\TradeLink"
  RMDir "$INSTDIR"
  
 
SectionEnd
!endif




