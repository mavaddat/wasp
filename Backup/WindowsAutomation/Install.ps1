# cd C:\Users\MavaddatJavid\Projects\PowerShell\Win32Window\bin\Release
Start-Process -FilePath "C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe" -ArgumentList "$(Join-Path (Split-Path $MyInvocation.InvocationName) WindowsAutomation.dll)"
# Get-PSSnapin -registered
Add-PSSnapin WindowsAutomation
# get-help *-Window
Get-Command -PSSnapin WindowsAutomation
