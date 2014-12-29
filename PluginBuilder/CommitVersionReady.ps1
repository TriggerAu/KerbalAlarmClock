$PluginName = (get-item $PSScriptRoot).Parent.Name
$UploadDir = "$($PSScriptRoot)\..\..\_Uploads\$($PluginName)"

#Get newest version
$Version =  (Get-ChildItem $UploadDir -Filter "v*.*.*.*"|sort -Descending)[0].name.replace("v","")

$Choices= [System.Management.Automation.Host.ChoiceDescription[]] @("&Yes","&No")
$ChoiceRtn = $host.ui.PromptForChoice("`r`nGit Commit - v$($Version)","Do you wish to Commit the version ready message?",$Choices,0)
if ($ChoiceRtn -eq 0 )
{
    git checkout develop
    git add -A * ..\*
    git commit -m "Version $($Version) ready"
	git push
}