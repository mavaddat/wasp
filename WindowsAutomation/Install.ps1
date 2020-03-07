# cd "$env:USERPROFILE\Projects\PowerShell\Win32Window\bin\Release"
$dotNetFrameworkDir = Get-ChildItem -Path C:\Windows\Microsoft.NET\Framework\ -Directory | Sort-Object | select -Last 1
if ( Test-Path -Path "$($dotNetFrameworkDir.FullName)\InstallUtil.exe" -PathType Leaf) {
	Start-Process -FilePath "$($dotNetFrameworkDir.FullName)\InstallUtil.exe" -ArgumentList "$(Join-Path (Split-Path $MyInvocation.InvocationName) WindowsAutomation.dll)"
}
# Get-PSSnapin -registered
Add-PSSnapin WindowsAutomation
# get-help *-Window
Get-Command -PSSnapin WindowsAutomation
