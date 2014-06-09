$GitHubName="KerbalAlarmClock"
$PluginName="KerbalAlarmClock"
$UploadDir = "$($PSScriptRoot)\..\..\_Uploads\$($PluginName)"

$Version = Read-Host -Prompt "Enter the Version Number to Publish" 


if ($Version -eq "")
{
    "No version string supplied... Quitting"
    return
}
else
{
    $Path = "$UploadDir\v$($Version)\$($PluginName)_$($Version)\GameData\TriggerTech\$($PluginName).dll"
    "DLL Path:`t$($Path)"
    if (Test-Path $Path)
    {
	    $dll = get-item $Path
	    $VersionString = $dll.VersionInfo.ProductVersion

        if ($Version -ne $VersionString) {
            "Versions dont match`r`nEntered:`t$Version`r`nFrom File:`t$VersionString"
            return
        } else {
            $OAuthToken = Read-Host -Prompt "OAuth Token"
        }
    } else {
        "Cant find the dll - have you built the dll first?"
        return
    }
}


    

"`r`nThis will Merge the devbranch with master and push the release of v$($Version) of the $($PluginName)"
"`tFrom:`t$UploadDir\v$($Version)"
"`tOAauth:`t$OAuthToken"
$Choices= [System.Management.Automation.Host.ChoiceDescription[]] @("&Yes","&No")
$ChoiceRtn = $host.ui.PromptForChoice("Do you wish to Continue?","Be sure develop is ready before hitting yes",$Choices,1)

if($ChoiceRtn -eq 0)
{
	#git add -A *
	#git commit -m "Version history $($Version)"
	
	#write-host -ForegroundColor Yellow "`r`nPUSHING DEVELOP TO GITHUB"
	#git push

    write-host -ForegroundColor Yellow "`r`nMERGING DEVELOP TO MASTER"

	git checkout master
	git merge --no-ff develop -m "Merge $($Version) to master"
	git tag -a "v$($Version)" -m "Released version $($Version)"

	write-host -ForegroundColor Yellow "`r`nPUSHING MASTER AND TAGS TO GITHUB"
	git push
	git push --tags
	
	write-host -ForegroundColor Yellow "----------------------------"
	write-host -ForegroundColor Yellow "Finished Version $($Version)"
	write-host -ForegroundColor Yellow "----------------------------"
	
	write-host -ForegroundColor Yellow "`r`n Creating Release"
	$readme = (Get-Content -Raw "$($PSScriptRoot)\..\PluginFiles\ReadMe-$($PluginName).txt")

    if ($?) {
        "Couldn't load the readme file. Quitting..."
        return
    }
	$reldescr = [regex]::match($readme,"Version\s$($Version).+?(?=[\r\n]*Version\s\d+|$)","singleline,ignorecase").Value

	#Now get the KSPVersion from the first line
	$KSPVersion = [regex]::match($reldescr,"KSP\sVersion\:.+?(?=[\r\n]|$)","singleline,ignorecase").Value
	
	#Now drop the first line
	$reldescr = [regex]::replace($reldescr,"^.+?\r\n","","singleline,ignorecase")
	
	$reldescr = $reldescr.Trim("`r`n")
	$reldescr = $reldescr.Replace("- ","* ")
	$reldescr = $reldescr.Replace("`r`n","\r\n")
	$reldescr = $reldescr.Replace("`"","\`"")
	
	$reldescr = "$($reldescr)\r\n\r\n``````$($KSPVersion)``````"

	$CreateBody = "{`"tag_name`":`"v$($Version)`",`"name`":`"v$($Version) Release`",`"body`":`"$($relDescr)`"}"
	
	$RestResult = Invoke-RestMethod -Method Post `
		-Uri "https://api.github.com/repos/TriggerAu/$($GitHubName)/releases" `
		-Headers @{"Accept"="application/vnd.github.v3+json";"Authorization"="token $($OAuthToken)"} `
		-Body $CreateBody
	if ($?)
	{
		write-host -ForegroundColor Yellow "Uploading File"
		$File = get-item "$($UploadDir)\v$($Version)\$($pluginname)_$($Version).zip"
		$RestResult = Invoke-RestMethod -Method Post `
			-Uri "https://uploads.github.com/repos/TriggerAu/$($GitHubName)/releases/$($RestResult.id)/assets?name=$($File.Name)" `
			-Headers @{"Accept"="application/vnd.github.v3+json";"Authorization"="token $($OAuthToken)";"Content-Type"="application/zip"} `
			-InFile $File.fullname
		
		"Result = $($RestResult.state)"
	}

	write-host -ForegroundColor Yellow "----------------------------"
	write-host -ForegroundColor Yellow "Finished Release $($Version)"
	write-host -ForegroundColor Yellow "----------------------------"

	write-host -ForegroundColor Yellow "Back to Develop Branch"
    git checkout develop
}
else
{
    "Skipping..."
}
