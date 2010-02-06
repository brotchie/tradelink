# DotNET version checking macro.
# Written by AnarkiNet(AnarkiNet@gmail.com)
# Downloads and runs the Microsoft .NET Framework version 2.0 Redistributable and runs it if the user does not have the correct version.

!include WordFunc.nsh
!include LogicLib.nsh
!insertmacro VersionCompare

!macro CheckDotNET

!define DOTNET_URL "http://www.microsoft.com/downloads/info.aspx?na=90&p=&SrcDisplayLang=en&SrcCategoryId=&SrcFamilyId=0856EACB-4362-4B0D-8EDD-AAB15C5E04F5&u=http%3a%2f%2fdownload.microsoft.com%2fdownload%2f5%2f6%2f7%2f567758a3-759e-473e-bf8f-52154438565a%2fdotnetfx.exe"
!define DOTNET_VERSION "2.0" ; minimum .net runtime version

DetailPrint "Checking your .NET Framework..."
StrCpy $8 ""
CheckBegin:
	Push $0
	Push $1
	 
	System::Call "mscoree::GetCORVersion(w .r0, i ${NSIS_MAX_STRLEN}, *i) i .r1"
	StrCmp $1 0 +2
		StrCpy $0 "none"
	 
	Pop $1
	Exch $0
	Pop $0
	${If} $0 == "none"
		DetailPrint ".NET Framework not found"
	  ${If} $8 == ""
			Goto InvalidDotNET
		${Else}
			Goto InvalidDotNetAfterInstall
		${EndIf}
	${EndIf}
	 
	StrCpy $0 $0 "" 1 # skip "v"
	${VersionCompare} $0 ${DOTNET_VERSION} $1
	${If} $1 == 2
		DetailPrint ".NET Framework Version found: $0, but is not up to the required version: ${DOTNET_VERSION}"
	  ${If} $8 == ""
			Goto InvalidDotNET
		${Else}
			Goto InvalidDotNetAfterInstall
		${EndIf}
	${EndIf}
	Goto ValidDotNET

InvalidDotNET:
	MessageBox MB_YESNO|MB_ICONQUESTION \
	"${SHORT_APP_NAME} requires a set of files, called the .NET Framework 2.0, that your computer does not have.  It is Microsoft's latest and greatest technology for desktop programs.$\n$\nWe can automatically download and install it for you.  Doing so will also allow you to run other .NET desktop programs.  The download and installation takes six minutes on a modern computer with a high speed Internet connection.$\n$\nIs this okay?" \
	IDYES Download IDNO AbortMe
	
AbortMe:
	DetailPrint "Install aborted due to missing .NET Framework"
	MessageBox MB_OK "${SHORT_APP_NAME} cannot run without the .NET Framework.  We apologize for the inconvenience.  If you feel like you are getting this message in error, please email ${SUPPORT_EMAIL}.$\n$\nWe hope that you will consider trying ${SHORT_APP_NAME} in the future."
  Quit
 
Download:
	DetailPrint "Beginning download of latest .NET Framework version."
	NSISDL::download ${DOTNET_URL} "$TEMP\dotnetfx.exe"
	DetailPrint "Completed download."
	Pop $R0
	StrCmp $R0 "success" DownloadSuccess
	StrCmp $R0 "cancel" DownloadCancelled
	; some transport error -- try again!
	Sleep 200;
	NSISDL::download ${DOTNET_URL} "$TEMP\dotnetfx.exe"
	DetailPrint "Completed download."
	Pop $R0
	StrCmp $R0 "success" DownloadSuccess
	StrCmp $R0 "cancel" DownloadCancelled
	Goto DownloadFail
		
DownloadFail:
	Delete "$TEMP\dotnetfx.exe"
	MessageBox MB_OK "The download failed with the message below.  We apologize for the inconvenience.$\n$\n$R0$\n$\nCheck your Internet connection and make sure that you are connected.  If you need help, email ${SUPPORT_EMAIL}."
	Quit
	
DownloadCancelled:
	Delete "$TEMP\dotnetfx.exe"
	DetailPrint "Install aborted due to missing .NET Framework"
	MessageBox MB_OK "You cancelled the installation of the .NET Framework.  ${SHORT_APP_NAME} cannot run without the .NET Framework.  We apologize for the inconvenience.$\n$\nWe hope that you will consider trying ${SHORT_APP_NAME} in the future."
	Quit
		
DownloadSuccess:
	DetailPrint "Pausing installation while downloaded .NET Framework installer runs."
	NsisDotNetInstaller::LaunchInstaller "$TEMP\dotnetfx.exe"
	DetailPrint "Completed .NET Framework install/update. Cleaning temporary files..."
	Delete "$TEMP\dotnetfx.exe"
	DetailPrint "Completed cleaning temporary files.  Verifying install..."
	StrCpy $8 "AfterInstall"
	Goto CheckBegin
	
InvalidDotNetAfterInstall:
	
ValidDotNET:
	DetailPrint ".NET Framework found and is up to date..."
!macroend