;Wnmp Installer Script
;Inno Setup http://www.jrsoftware.org/isdl.php#stable

#define Name "Wnmp"
#define Version "2.0.14"
#define Publisher "Kurt Cancemi"
#define URL "http://wnmp.x64architecture.com"
#define ExeName "Wnmp.exe"
#define Year "2014"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{44CF85C5-C9D2-435F-941B-75597AA9A6FB}
AppName={#Name}
AppVersion={#Version}
AppVerName={#Name} {#Version}
AppPublisher={#Publisher}
AppPublisherURL={#URL}
AppSupportURL={#URL}
AppUpdatesURL={#URL}
AppContact=kurt@x64Architecture.com
DefaultDirName={sd}\{#Name}
SourceDir=..\
VersionInfoDescription=Wnmp (version {#VERSION})
VersionInfoCopyright=Copyright 2012-{#Year} Kurt Cancemi
VersionInfoCompany=Kurt Cancemi
DefaultGroupName={#Name}
LicenseFile=license.txt
InfoAfterFile=contrib\postinstall.txt
OutputBaseFilename=Wnmp-{#Version} 
OutputDir=../Wnmp Output
SetupIconFile=contrib\logo.ico
Compression=lzma2
InternalCompressLevel=max
SolidCompression=yes
PrivilegesRequired=none
RestartIfNeededByRun=no

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 0,6.1

[Files]
Source: "Wnmp.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "*"; Excludes: "mariadb\mysql-test\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "../vc_2008_sp1_redist_x86.exe"; DestDir: {tmp}; Flags: deleteafterinstall
Source: "../vc_2012_redist_x86.exe"; DestDir: {tmp}; Flags: deleteafterinstall
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{group}\{#Name}"; Filename: "{app}\{#ExeName}"
Name: "{commondesktop}\{#Name}"; Filename: "{app}\{#ExeName}"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#Name}"; Filename: "{app}\{#ExeName}"; Tasks: quicklaunchicon

[Run]
Filename: "{app}\{#ExeName}"; Description: "{cm:LaunchProgram,{#StringChange(Name, '&', '&&')}}"; Flags: nowait postinstall skipifsilent
Filename: "{app}\contrib\ReadMe.html"; Description: "View the ReadMe.html"; Flags: postinstall shellexec skipifsilent unchecked
Filename: "{tmp}\vc_2008_sp1_redist_x86.exe";
Filename: "{tmp}\vc_2012_redist_x86.exe";
Filename: "http://www.getwnmp.org/"; Flags: shellexec runasoriginaluser postinstall unchecked; Description: "View Wnmp Website";
Filename: "http://www.getwnmp.org/contributing/"; Flags: shellexec runasoriginaluser postinstall; Description: "Make a contribution to Wnmp";
