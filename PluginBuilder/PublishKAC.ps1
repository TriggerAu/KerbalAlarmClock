$GitHubName="KerbalAlarmClock"
$PluginName="KerbalAlarmClock"
$CurseID="220289"
$CurseName="220289-kerbal-alarm-clock"
$KerbalStuffModID = 231
$SpaceDockID="699"
$UploadDir = "$($PSScriptRoot)\..\..\_Uploads\$($PluginName)"
$KerbalStuffWrapper = "D:\Programming\KSP\_Scripts\KerbalStuffWrapper\KerbalStuffWrapper.exe"


function MergeDevToMaster() {
    $Choices= [System.Management.Automation.Host.ChoiceDescription[]] @("&Yes","&No")
    $ChoiceRtn = $host.ui.PromptForChoice("`r`nGit Merge Dev To Master Confirmation","Do you wish to merge Dev and Master?",$Choices,0)
    if ($ChoiceRtn -eq 0 )
    {
        write-host -ForegroundColor Yellow "`r`nMERGING DEVELOP TO MASTER"

	    git checkout master
	    git merge --no-ff develop -m "Merge $($Version) to master"
	    git tag -a "v$($Version)" -m "Released version $($Version)"

	    write-host -ForegroundColor Yellow "`r`nPUSHING MASTER AND TAGS TO GITHUB"
	    git push
	    git push --tags
	
	    write-host -ForegroundColor Yellow "Back to Develop Branch"
        git checkout develop

	    write-host -ForegroundColor Yellow "----------------------------"
	    write-host -ForegroundColor Yellow "Finished Version $($Version)"
	    write-host -ForegroundColor Yellow "----------------------------"
	}
}

function CreateGitHubRelease() {
	write-host -ForegroundColor Yellow "`r`n Creating Release"

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

	write-host -ForegroundColor Yellow "-----------------------------------"
	write-host -ForegroundColor Yellow "Finished GitHub Release $($Version)"
	write-host -ForegroundColor Yellow "-----------------------------------"
}

function CreateKerbalStuffRelease() {
    $Choices= [System.Management.Automation.Host.ChoiceDescription[]] @("&Yes","&No")
    $ChoiceRtn = $host.ui.PromptForChoice("`r`nKerbalStuff Confirmation","Do you wish to upload v$($Version) to KerbalStuff?",$Choices,0)
    if ($ChoiceRtn -eq 0 )
    {
		"Updating Mod at KerbalStuff"
		$File = get-item "$($UploadDir)\v$($Version)\$($pluginname)_$($Version).zip"
		& $KerbalStuffWrapper updatemod /m:$KerbalStuffModID /u:"$KerbalStuffLogin" /p:"$KerbalStuffPW" /k:"$KerbalStuffKSPVersion" /v:"$Version" /f:"$($File.FullName)" /l:"$relKStuff" /n:true
	}
}

$boundary = [System.Guid]::NewGuid().ToString()
$boundaryLine = "--$($boundary)`n"
function FormSection($name, $content){
    $tmpbody += $boundaryLine
    $tmpbody += "Content-Disposition: form-data; name=`"$($name)`"`n"
    $tmpbody += "`n"
    $tmpbody += $content + "`n"
    $tmpbody
}

function CreateSpaceDockRelease() {
    $Choices= [System.Management.Automation.Host.ChoiceDescription[]] @("&Yes","&No")
    $ChoiceRtn = $host.ui.PromptForChoice("`r`nSpaceDock Confirmation","Do you wish to upload v$($Version) to SpaceDock?",$Choices,0)
    if ($ChoiceRtn -eq 0 )
    {
		"Updating Mod at SpaceDock"

        $File = get-item "$($UploadDir)\v$($Version)\$($pluginname)_$($Version).zip"
        
        #Login
        $Loginparams = @{username="$($SpaceDockLogin)";password="$($SpaceDockPW)"}
        $response = Invoke-WebRequest -UseBasicParsing -Method POST -Uri https://spacedock.info/api/login -Body $Loginparams -SessionVariable spacedocksession


        $uploadbody = FormSection "version" $version
        $uploadbody += FormSection "game-version" $KerbalStuffKSPVersion
        $uploadbody += FormSection "notify-followers" "yes"
        $uploadbody += FormSection "changelog" $relKStuff


        $fileBinary = [IO.File]::ReadAllBytes($File.FullName)
        $encoding = [System.Text.Encoding]::GetEncoding("iso-8859-1")
        $fileEncoded = $encoding.GetString($fileBinary)
        $uploadbody += FormSection "zipball" $fileEncoded
        
        $uploadbody += $boundaryLine + "--"

        #"=============================================================="
        #$uploadbody
        #"=============================================================="

        $updateResponse = Invoke-WebRequest -UseBasicParsing `
                                            -Method Post `
                                            -Uri "https://spacedock.info/api/mod/$($SpaceDockID)/update" `
                                            -WebSession $spacedocksession `
                                            -ContentType "multipart/form-data; boundary=$($boundary)" `
                                            -Body $uploadbody 


		#"Updating Mod at KerbalStuff"
		#$File = get-item "$($UploadDir)\v$($Version)\$($pluginname)_$($Version).zip"
		#& $KerbalStuffWrapper updatemod /m:$KerbalStuffModID /u:"$KerbalStuffLogin" /p:"$KerbalStuffPW" /k:"$KerbalStuffKSPVersion" /v:"$Version" /f:"$($File.FullName)" /l:"$relKStuff" /n:true
	}
}

function CreateCurseRelease() {
    $CurseVersions = Invoke-RestMethod -method Get -uri "https://kerbal.curseforge.com/api/game/versions?token=$($CurseForgeToken)"
    $Choices= [System.Management.Automation.Host.ChoiceDescription[]] @("&Yes","&No")
    $ChoiceRtn = $host.ui.PromptForChoice("`r`nLatest Curse Version is $(($Curseversions | Sort-Object id -Descending)[0].Name)","Do you wish to upload v$($Version) to Curseforge for this KSP Version?",$Choices,0)
    if ($ChoiceRtn -eq 0 )
    {


        $metadata =  "{`"changelog`":`"$($relDescr)`", `"displayName`":`"v$($Version) Release`", `"gameVersions`": [$(($Curseversions | Sort-Object id -Descending)[0].id)], `"releaseType`": `"release`"}"
        
        $uploadbody = FormSection "metadata" $metadata

        $File = get-item "$($UploadDir)\v$($Version)\$($pluginname)_$($Version).zip"
        $fileBinary = [IO.File]::ReadAllBytes($File.FullName)
        $encoding = [System.Text.Encoding]::GetEncoding("iso-8859-1")
        $fileEncoded = $encoding.GetString($fileBinary)

        $uploadbody += FormSection "file" $fileEncoded
        
        $uploadbody += $boundaryLine + "--"

        $RestResult = Invoke-RestMethod -UseBasicParsing `
                                        -Method Post `
			                            -Uri "https://kerbal.curseforge.com/api/projects/$($CurseID)/upload-file??token=$($CurseForgeToken)" `
                                        -Headers @{"X-Api-Token"=$($CurseForgeToken);} `
                                        -ContentType "multipart/form-data; boundary=$($boundary)" `
			                            -Body $uploadbody 

		
		"Result = $($RestResult.state)"
        
    }
}

function UpdateVersionCheckGHPagesAndPublish() {
    write-host -ForegroundColor Yellow "`r`nMERGING GHPages Dev to Master"

    $GHPagesPath = "$($PSScriptRoot)\..\..\$($GitHubName)_gh-pages"

	git --git-dir="$($GHPagesPath)\.git" --work-tree="$($GHPagesPath)" checkout gh-pages_develop
    #update the version file

    "|LATESTVERSION|$($Version)|LATESTVERSION|" | Out-File "$($GHPagesPath)\versioncheck.txt" -Encoding ascii

    #Commit these changes
    git --git-dir="$($GHPagesPath)\.git" --work-tree="$($GHPagesPath)" add "$($GHPagesPath)\versioncheck.txt"
    git --git-dir="$($GHPagesPath)\.git" --work-tree="$($GHPagesPath)" commit -m "Updating versioncheck.text for v$($Version)"
	write-host -ForegroundColor Yellow "`r`nPUSHING gh-pages_develop TO GITHUB"
    git --git-dir="$($GHPagesPath)\.git" --work-tree="$($GHPagesPath)" push

    #merge to main branch
	git --git-dir="$($GHPagesPath)\.git" --work-tree="$($GHPagesPath)" checkout gh-pages
	git --git-dir="$($GHPagesPath)\.git" --work-tree="$($GHPagesPath)" merge --no-ff gh-pages_develop -m "Merge versioncheck $($Version) to ghpages"

	write-host -ForegroundColor Yellow "`r`nPUSHING gh-pages TO GITHUB"
	git --git-dir="$($GHPagesPath)\.git" --work-tree="$($GHPagesPath)" push
	
	write-host -ForegroundColor Yellow "Back to Develop Branch"
    git --git-dir="$($GHPagesPath)\.git" --work-tree="$($GHPagesPath)" checkout gh-pages_develop

	write-host -ForegroundColor Yellow "----------------------------"
	write-host -ForegroundColor Yellow "Finished Website versioncheck update $($Version)"
	write-host -ForegroundColor Yellow "----------------------------"
	
}

#Get newest version
$Version =""
$VersionRead =  (Get-ChildItem $UploadDir -Filter "v*.*.*.*"|sort LastWriteTime -Descending)[0].name.replace("v","")
if ($VersionRead -ne $null) {
	$Choices= [System.Management.Automation.Host.ChoiceDescription[]] @("&Yes","&No")
	$ChoiceRtn = $host.ui.PromptForChoice("Version v$($VersionRead) detected","Is this the version you wish to build?",$Choices,0)
	
	if($ChoiceRtn -eq 0){$Version = $VersionRead}
} 
if ($Version -eq "") {
	$Version = Read-Host -Prompt "Enter the Version Number to Publish" 
}


if ($Version -eq "")
{
    "No version string supplied... Quitting"
    return
}
else
{
    $Path = "$UploadDir\v$($Version)\$($PluginName)_$($Version)\GameData\TriggerTech\$($PluginName)\$($PluginName).dll"
    "DLL Path:`t$($Path)"
    if (Test-Path $Path)
    {
	    $dll = get-item $Path
	    $VersionString = $dll.VersionInfo.ProductVersion

        if ($Version -ne $VersionString) {
            "Versions dont match`r`nEntered:`t$Version`r`nFrom File:`t$VersionString"
            return
        } else {
            if ($GitHubToken -ne $null){
                $OAuthToken = $GitHubToken
            } else {
                $OAuthToken = Read-Host -Prompt "GitHub OAuth Token"
                $global:GitHubToken = $OAuthToken
            }

            #if ($CurseForgeToken -eq $null -or $CurseForgeToken -eq "") {
            #    $CurseForgeToken = Read-Host -Prompt "CurseForge OAuth Token"
            #}

            if ($KerbalStuffPW -eq $null) {
                $global:KerbalStuffLogin = Read-Host -Prompt "KerbalStuff Login"
                $global:KerbalStuffPW = Read-Host -Prompt "KerbalStuff Password"
            }

        }
    } else {
        "Cant find the dll - have you built the dll first?"
        return
    }
}



"`r`nThis will Merge the devbranch with master and push the release of v$($Version) of the $($PluginName)"
"`tFrom:`t$UploadDir\v$($Version)"
"`tGitHub Oauth:`t$OAuthToken"
"`tCurse  Oauth:`t$CurseForgeToken"
"`tSpaceDock:`t$SpaceDockLogin : $SpaceDockPW"

$Choices= [System.Management.Automation.Host.ChoiceDescription[]] @("&Yes","&No")
$ChoiceRtn = $host.ui.PromptForChoice("Do you wish to Continue?","Be sure develop is ready before hitting yes",$Choices,1)

if($ChoiceRtn -eq 0)
{
    "Generating the readme content`r`n"
    $readme = (Get-Content -Raw "$($PSScriptRoot)\..\PluginFiles\ReadMe-$($PluginName).txt")

    #If couldn't load it then bork out
    if (!$?) {
        "Couldn't load the readme file. Quitting..."
        return
    }
	$reldescr = [regex]::match($readme,"Version\s$($Version).+?(?=[\r\n]*Version\s\d+|$)","singleline,ignorecase").Value

	#Now get the KSPVersion from the first line
	$KSPVersion = [regex]::match($reldescr,"KSP\sVersion\:.+?(?=[\r\n]|$)","singleline,ignorecase").Value
	
	#Now drop the first line
	$reldescr = [regex]::replace($reldescr,"^.+?\r\n","","singleline,ignorecase")
	
	$reldescr = $reldescr.Trim("`r`n")
	$reldescr = [regex]::replace($reldescr,"^- ","* ","multiline,ignorecase")
	$reldescr = $reldescr.Replace("`r`n","\r\n")
	$reldescr = $reldescr.Replace("`"","\`"")
	
    $ForumHeader = "[B][SIZE=4][COLOR=`"#FF0000`"]v$($Version) Now Available [/COLOR][/SIZE][/B]- [SIZE=3][B][URL=`"https://github.com/TriggerAu/$($GitHubName)/releases/tag/v$($Version)`"]Download from GitHub[/URL][/B] [/SIZE] or [SIZE=3][B][URL=`"https://kerbal.curseforge.com/ksp-mods/$($CurseName)/files`"]Download from Curse*[/URL][/B][/SIZE] or [SIZE=3][B][URL=`"https://kerbalstuff.com/mod/$($KerbalStuffModID)`"]Download from Kerbal Stuff[/URL][/B] [/SIZE]  [COLOR=`"#A9A9A9`"]* Once it's approved[/COLOR]"

    $ForumList = "[LIST]`r`n" + $reldescr + "`r`n[/LIST]"
    $ForumList = $ForumList.Replace("\r\n","`r`n").Replace("`r`n* ","`r`n[*]")

	$reldescr = "$($reldescr)\r\n\r\n``````$($KSPVersion)``````"

    $relKStuff = $reldescr.Replace("\r\n","`r`n")
    
    $KerbalStuffKSPVersion = $KSPVersion.Split(":")[1].Trim(" ")
    if ($KerbalStuffKSPVersion.split(".").length -gt 2 -and  $KerbalStuffKSPVersion.EndsWith(".0")){
        $KerbalStuffKSPVersion = $KerbalStuffKSPVersion.Substring(0,$KerbalStuffKSPVersion.Length-2)
    }

    "GitHub Description:"
    "-------------------"
    "$($reldescr)`r`n"

    "KStuff Description:"
    "-------------------"
    "$($relKStuff)`r`n"

    "KStuff KSPVersion:"
    "-------------------"
    "$($KerbalStuffKSPVersion)`r`n"

    "Forum Info:"
    "-------------------"
    "$($ForumHeader)`r`n"
    "$($ForumList)`r`n"

    $Choices= [System.Management.Automation.Host.ChoiceDescription[]] @("&Yes","&No")
    $ChoiceRtn = $host.ui.PromptForChoice("Do you wish to Continue?","Confirm Readme Notes",$Choices,0)

    if($ChoiceRtn -eq 0) {

    MergeDevToMaster

	"Pausing 3 Secs to let Github catchup"
	Start-Sleep -s 3
    CreateGitHubRelease

    #CreateSpaceDockRelease

    #CreateCurseRelease 

    #CreateKerbalStuffRelease

    $Choices= [System.Management.Automation.Host.ChoiceDescription[]] @("&Yes","&No")
    $ChoiceRtn = $host.ui.PromptForChoice("Update versioncheck.txt file","Do you wish to update the versioncheck file in ghpages?",$Choices,0)

    if($ChoiceRtn -eq 0)
    {
        UpdateVersionCheckGHPagesAndPublish
    }

    "GitHub Description:"
    "-------------------"
    "$($reldescr)`r`n"

    "KStuff Description:"
    "-------------------"
    "$($relKStuff)`r`n"

    "Forum Info:"
    "-------------------"
    "$($ForumHeader)`r`n"
    "$($ForumList)`r`n"

    }

}
else
{
    "Skipping..."
}
