param([String]$VersionString)
#param([String]$VersionString = $(throw "-VersionString is required so we know what folder to build.`r`n`r`n"))

#Run by powershell BuildKACFiles.ps1 -VersionString "X.X.X.X"
#  or it will try and read the dll version

$PluginName = "KerbalAlarmClock"
$GitHubName = "KerbalAlarmClock"
$KSPRootPath = "$($PSScriptRoot)\..\.."

$SourcePath= "$($KSPRootPath)\$($GitHubName)"
$DestRootPath="$($KSPRootPath)\_Uploads\$($PluginName)"
$7ZipPath="c:\Program Files\7-Zip\7z.exe" 

if ($VersionString -eq "")
{
	$dll = get-item "$SourcePath\$($GitHubName)\bin\Release\$($PluginName).dll"
	$VersionString = $dll.VersionInfo.ProductVersion
}

if ($VersionString -eq "")
{
	throw "No version read from the dll and no -VersionString provided so we don't know what folder to build.`r`n`r`n"

}

"Getting the readme content`r`n"
$readme = (Get-Content -Raw "$($PSScriptRoot)\..\PluginFiles\ReadMe-$($PluginName).txt")
#If couldn't load it then bork out
if (!$?) {
    "Couldn't load the readme file. Quitting...`r`n"
    return
}
$reldescr = [regex]::match($readme,"Version\s$($VersionString).+?(?=[\r\n]*Version\s\d+|$)","singleline,ignorecase").Value
if ($reldescr -eq ""){
    "Couldn't find readme section for v$($VersionString)`r`n"
    return
}

#Now get the KSPVersion from the first line
$KSPVersion = [regex]::match($reldescr,"KSP\sVersion\:.+?(?=[\r\n]|$)","singleline,ignorecase").Value.Split(":")[1].Trim(" ")
if ($KSPVersion.Split(".").Length -lt 3){$KSPVersion += ".0"}
if ($KSPVersion.Split(".").Length -lt 3){$KSPVersion += ".0"}

$DestFullPath= "$($DestRootPath)\v$($VersionString)"


"`r`nThis will build v$($VersionString) of the $($PluginName)"
"`tKSPVersion:`t$($KSPVersion)"
"`tFrom:`t$($SourcePath)"
"`tTo:`t$($DestFullPath)"
$Choices= [System.Management.Automation.Host.ChoiceDescription[]] @("&Yes","&No")
$ChoiceRtn = $host.ui.PromptForChoice("Do you wish to Continue?","This will erase any existing $($VersionString) Folder",$Choices,1)

if($ChoiceRtn -eq 0)
{
    "Here Goes..."

    if(Test-Path -Path $DestFullPath)
    {
        "Deleting $DestFullPath"
        Remove-Item -Path $DestFullPath -Recurse

    }

    #Create the folders
    "Creating Folders..."
    New-Item $DestRootPath -name "v$($VersionString)" -ItemType Directory

    #Dont create this or it will copy the files into a subfolder
    New-Item $DestFullPath -name "$($PluginName)Source_$($VersionString)" -ItemType Directory

    #Copy the items 
    "Copying Plugin..."
    Copy-Item "$SourcePath\PluginFiles" "$($DestFullPath)\$($PluginName)_$($VersionString)" -Recurse
    Copy-Item "$SourcePath\$($GitHubName)\bin\Release\$($PluginName).dll" "$($DestFullPath)\$($PluginName)_$($VersionString)\GameData\TriggerTech\$($PluginName)" 
    #Update the Text files with the version String
    (Get-Content "$($DestFullPath)\$($PluginName)_$($VersionString)\info.txt") |
        ForEach-Object {$_ -replace "%VERSIONSTRING%",$VersionString} |
            Set-Content "$($DestFullPath)\$($PluginName)_$($VersionString)\info.txt"
    (Get-Content "$($DestFullPath)\$($PluginName)_$($VersionString)\ReadMe-$($PluginName).txt") |
        ForEach-Object {$_ -replace "%VERSIONSTRING%",$VersionString} |
            Set-Content "$($DestFullPath)\$($PluginName)_$($VersionString)\ReadMe-$($PluginName).txt"
	Move-Item "$($DestFullPath)\$($PluginName)_$($VersionString)\ReadMe-$($PluginName).txt" "$($DestFullPath)\$($PluginName)_$($VersionString)\GameData\TriggerTech\"



    #Build the Version File
    "Building Version file..."
    $VersionTemplate = Get-Content "$($PSScriptRoot)\$($PluginName).version"
    $VersionTemplate = $VersionTemplate.Replace("%PLUGIN_MAJOR%",$VersionString.Split(".")[0])
    $VersionTemplate = $VersionTemplate.Replace("%PLUGIN_MINOR%",$VersionString.Split(".")[1])
    $VersionTemplate = $VersionTemplate.Replace("%PLUGIN_PATCH%",$VersionString.Split(".")[2])
    $VersionTemplate = $VersionTemplate.Replace("%PLUGIN_BUILD%",$VersionString.Split(".")[3])
    
    $VersionTemplate = $VersionTemplate.Replace("%KSP_MAJOR%",$KSPVersion.Split(".")[0])
    $VersionTemplate = $VersionTemplate.Replace("%KSP_MINOR%",$KSPVersion.Split(".")[1])
    $VersionTemplate = $VersionTemplate.Replace("%KSP_PATCH%",$KSPVersion.Split(".")[2])

    $VersionTemplate | Set-Content "$($DestFullPath)\$($PluginName)_$($VersionString)\GameData\TriggerTech\$($PluginName)\$($PluginName).version"

    Copy-Item "$($DestFullPath)\$($PluginName)_$($VersionString)\GameData\TriggerTech\$($PluginName)\$($PluginName).version" "$($SourcePath)\$($GitHubName)\$($PluginName).version"

    #Copy the source files
    "Copying Source..."
    Copy-Item "$SourcePath\$($GitHubName)\*.cs"  "$($DestFullPath)\$($PluginName)Source_$($VersionString)"
    Copy-Item "$SourcePath\$($GitHubName)\*.csproj"  "$($DestFullPath)\$($PluginName)Source_$($VersionString)"
    New-Item "$DestFullPath\$($PluginName)Source_$($VersionString)\" -name "Properties" -ItemType Directory
    Copy-Item "$SourcePath\$($GitHubName)\Properties\*.cs" "$($DestFullPath)\$($PluginName)Source_$($VersionString)\Properties\"
    

    # Now Zip it up

    & "$($7ZipPath)" a "$($DestFullPath)\$($PluginName)_$($VersionString).zip" "$($DestFullPath)\$($PluginName)_$($VersionString)\GameData" -xr!"info.txt"
	& "$($7ZipPath)" a "$($DestFullPath)\$($PluginName)_$($VersionString).zip" "$($DestFullPath)\$($PluginName)_$($VersionString)\info.txt"
	#& "$($7ZipPath)" a "$($DestFullPath)\$($PluginName)_$($VersionString).zip" "$($DestFullPath)\$($PluginName)_$($VersionString)\GameData\TriggerTech\ReadMe-$($PluginName).txt"
    & "$($7ZipPath)" a "$($DestFullPath)\$($PluginName)Source_$($VersionString).zip" "$($DestFullPath)\$($PluginName)Source_$($VersionString)" 
}

else
{
    "Skipping..."
}

