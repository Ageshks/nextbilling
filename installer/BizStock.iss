; BizStock installer configuration for GitHub Actions / Windows x64

#define MyAppName "BizStock"
#define MyAppVersion GetEnv("AppVersion")
#define MyAppPublisher "NextraDev"
#define MyAppURL "https://example.com"

[Setup]
AppId={{D9F5D8D1-6C46-4B06-8ED3-7C7F2F7FD4A5}}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={autopf}\BizStock
DefaultGroupName=BizStock
Compression=lzma
OutputDir={#OutputDir}
OutputBaseFilename=BizStock-Setup-{#MyAppVersion}
PrivilegesRequired=admin
ArchitecturesInstallIn64BitMode=x64
ArchitecturesAllowed=x64
UsePreviousAppDir=yes
UsePreviousTasks=yes
UsePreviousGroup=yes
CreateAppDir=yes
ChangesEnvironment=no
UninstallDisplayIcon={app}\BizStock.exe
CloseApplications=PromptAndClose
AppendDefaultDirName=yes
LicenseFile=
InfoAfterFile=
DisableProgramGroupPage=no
CreateUninstallRegKey=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "Create a &desktop shortcut"; GroupDescription: "Additional icons:"; Flags: unchecked
Name: "startmenuicon"; Description: "Create a &Start Menu shortcut"; GroupDescription: "Additional icons:"; Flags: checked

[Files]
Source: "{#SourceDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\BizStock"; Filename: "{app}\BizStock.exe"; Tasks: startmenuicon
Name: "{commondesktop}\BizStock"; Filename: "{app}\BizStock.exe"; Tasks: desktopicon

[Run]
Filename: "{app}\BizStock.exe"; Description: "Launch BizStock"; Flags: nowait postinstall skipifdoesntexist

[UninstallDelete]
; Keep user-owned data, backups, and customer/business documents intact.
; BizStock stores mutable data under %LOCALAPPDATA%\BizStock and is intentionally not removed here.

[InstallDelete]
; Keep previously installed business data and user-generated files intact during upgrades.

[Registry]
Root: HKLM; Subkey: "Software\Microsoft\Windows\CurrentVersion\Uninstall\BizStock"; ValueType: string; ValueName: "DisplayName"; ValueData: "BizStock"; Flags: createvalueifdoesntexist
Root: HKLM; Subkey: "Software\Microsoft\Windows\CurrentVersion\Uninstall\BizStock"; ValueType: string; ValueName: "UninstallString"; ValueData: """{uninstallexe}"""; Flags: createvalueifdoesntexist
Root: HKLM; Subkey: "Software\Microsoft\Windows\CurrentVersion\Uninstall\BizStock"; ValueType: string; ValueName: "InstallLocation"; ValueData: "{app}"; Flags: createvalueifdoesntexist
Root: HKLM; Subkey: "Software\Microsoft\Windows\CurrentVersion\Uninstall\BizStock"; ValueType: string; ValueName: "Publisher"; ValueData: "{#MyAppPublisher}"; Flags: createvalueifdoesntexist
Root: HKLM; Subkey: "Software\Microsoft\Windows\CurrentVersion\Uninstall\BizStock"; ValueType: string; ValueName: "DisplayVersion"; ValueData: "{#MyAppVersion}"; Flags: createvalueifdoesntexist
Root: HKLM; Subkey: "Software\Microsoft\Windows\CurrentVersion\Uninstall\BizStock"; ValueType: dword; ValueName: "NoModify"; ValueData: 1; Flags: createvalueifdoesntexist
Root: HKLM; Subkey: "Software\Microsoft\Windows\CurrentVersion\Uninstall\BizStock"; ValueType: dword; ValueName: "NoRepair"; ValueData: 1; Flags: createvalueifdoesntexist
