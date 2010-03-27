
Name "SampleTikData-20070926"

OutFile "Install-SampleTikData-20070926.exe"
; Icon for project (comment out if not used)
Icon "..\Install\tradelinkinstaller.ico"

InstallDir "$LOCALAPPDATA\TradeLinkTicks"

Section "SampleTikData-20070926"

  SectionIn RO
    ; Set output path to the installation directory.
  SetOutPath $INSTDIR

  
  ; standard tradelink stuff (comment out if not needed)
  File "*.TIK"
 
 WriteUninstaller "uninstall.exe"  

SectionEnd


XPStyle on
CRCCheck force
ShowInstDetails show

; The file to write
;OutFile "Install-${PROJECT}.exe"

; Registry key to check for directory (so if you install again, it will 
; overwrite the old one automatically)
InstallDirRegKey HKLM "Software\SampleTikData-20070926" "Install_Dir"

;--------------------------------

; Pages
Page components
Page directory
Page instfiles

UninstPage uninstConfirm
UninstPage instfiles

;--------------------------------


;--------------------------------

; Uninstaller

Section "Uninstall"
  
  ; Remove registry keys
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\SampleTikData-20070926"
  DeleteRegKey HKLM SOFTWARE\SampleTikData-20070926

  ; Remove files and uninstaller
  Delete "$INSTDIR\*.*"
  RMDir "$INSTDIR"
  



SectionEnd





