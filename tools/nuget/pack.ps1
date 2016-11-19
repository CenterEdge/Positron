$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Definition
$rootPath = "$scriptPath\..\.."

Get-ChildItem $rootPath\packaging\*.nuspec | % {
	& $scriptPath\nuget.exe pack $_.FullName -OutputDirectory $rootPath\packaging
}
