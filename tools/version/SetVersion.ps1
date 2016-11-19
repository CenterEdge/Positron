Param(
	[Version]
	$Version = $null,
	
	# Increment the revision number
	[switch]
	$Increment,
	
	# Force the version number change, even if it's less than the current version
	[switch]
	$Force
)

$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition
$rootPath = "$scriptPath\..\.."

# Validate parameters

if (($Version -eq $null) -and (-not $Increment)) {
	Write-Host "You must either provide -Version parameter or use the -Increment flag"
	exit
}

if (($Version -ne $null) -and ($Version.Build -eq -1)) {
	Write-Host "Build number is required in Version parameter"
	exit
}

# Load the current version number
$curVersion = New-Object Version (Get-Content "$scriptPath\..\..\src\Version.txt")

# If no specific version was provided, use the current version (it will be incremented later)
if ($Version -eq $null) {
	$Version = $curVersion
}

# Increment the version number if requested
if ($Increment) {
	$Version = New-Object System.Version $Version.Major, $Version.Minor,($Version.Build + 1)
}

if (-not $Force) {
	# Validate that we are actually incrementing the version number

	if ($curVersion -eq $Version) {
		Write-Host "Version number is already $curVersion"
		exit
	}

	if ($curVersion -gt $Version) {
		Write-Host "Version number is $curVersion, which is already greater than $Version"
		exit
	}
}

$versionStr = $Version.ToString()

# Update C# assembly info files

$regex = New-Object System.Text.RegularExpressions.RegEx "^(\s*\[assembly:\s*Assembly(?:File)?Version\(`")[\d\.]*?(`"\)\]\s*?)$", ([System.Text.RegularExpressions.RegexOptions]::Multiline -bor [System.Text.RegularExpressions.RegexOptions]::IgnoreCase)

foreach ($file in @(Get-ChildItem $rootPath\src -Filter AssemblyInfo.cs -Recurse)) {
    $content = [System.String]::Join("`r`n", (Get-Content $file.FullName))    
    $regex.Replace($content, "`${1}$version`${2}") | Out-File $file.FullName -Encoding UTF8 -Force -Width 10000
}

# Update nuspec files

Get-ChildItem $rootPath\packaging\*.nuspec | % {
	$regex = New-Object System.Text.RegularExpressions.RegEx "(<version>)[\.\d]*(</version>)", IgnoreCase
	$filename = $_.FullName
	$regex.Replace([System.String]::Join("`r`n", (Get-Content $filename)), "`${1}$Version`${2}") | Out-File $filename -Force -Encoding UTF8 -Width 5000
}

# Write new version to Version.txt

$versionStr | Out-File $rootPath\src\Version.txt -Force -Encoding UTF8

# Write to display

Write-Host "New version: $Version"