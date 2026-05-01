


function Save-SoundFile {
    [CmdletBinding(SupportsShouldProcess)]
    param(
        [Parameter(Mandatory = $true, position = 0)]
        [string]$Filename,
        [Parameter(Mandatory = $false)]
        [Alias("o")]
        [switch]$Overwrite
    )
    try {
        
        $DestPath = "C:\Dev\MessageBox-Ctrl\src\res\sounds"
        $TmpFilename = ((new-guid).GUID -as [string]).Substring(0, 6) + '.bak'
        $Url = "http://cd.textfiles.com/10000gp2/500SNDS/{0}" -f $Filename
        $Referer = "http://cd.textfiles.com/10000gp2/500SNDS/{0}" -f $Filename
        $OutFilePath = Join-Path "$DestPath" "$Filename"
        $OutTmpFilePath = Join-Path "$DestPath" "$TmpFilename"
        $Headerz = @{
            "Accept" = "*/*"
            "Accept-Encoding" = "identity;q=1, *;q=0"
            "Accept-Language" = "en-US,en;q=0.7"
            "Referer" = "$Referer"
            "Sec-GPC" = "1"
        }

        if (Test-Path -Path "$OutFilePath") {
            if ($Overwrite) {
                
            } else {
                Write-Host "File $Filename already downloaded! Skipping..."
                return
            }

        } else {
            $Res = Invoke-WebRequest -UseBasicParsing -Uri $Url -Headers $Headerz -OutFile "$OutFilePath" -ErrorAction Ignore -Passthru
            $StatusCode = $Res.StatusCode
            if ($StatusCode -ne 200) {
                $MessageRes = "[{0}] " -f $Res.StatusCode
                Write-Host "$MessageRes" -f DarkRed -n
                $MsgAddition=''
                if($Overwrite){
                    if(Test-Path "$OutTmpFilePath"){
                        $MsgAddition = "Restoring `"{0}`"" -f "$OutFilePath"
                        Write-Host "$MessageRes" -f DarkYellow
                        Move-Item -Path "$OutTmpFilePath" -Destination "$OutFilePath" -EA Ignore | Out-Null    
                    }
                }
                $MessageRes = " Error Occured - {0}. {1}" -f $Res.StatusDescription, $MsgAddition
                Write-Host "$MessageRes" -f DarkYellow
                
                return $False
            } else {
                $MessageRes = "[{0}] " -f $Res.StatusCode
                if($Overwrite){
                    Remove-Item -Path "$TmpFilename" -Force -ErrorAction Ignore | Out-Null
                }
                Write-Host "$MessageRes" -f White -n
                $MessageRes = "{0} Downloaded {1} bytes to {2}" -f $Res.StatusDescription, $Res.RawContentLength, $Filename
                Write-Host "$MessageRes" -f DarkCyan
                return $True
            }
        }
    }
    catch {
        throw $_
    }
}


function Save-AllSoundFiles {
    [CmdletBinding(SupportsShouldProcess)]
    param(
        [Parameter(Mandatory = $true, position = 0)]
        [string]$ListFilename,
        [Parameter(Mandatory = $false)]
        [Alias("o")]
        [switch]$Overwrite,
        [Parameter(Mandatory = $false)]
        [Alias("p")]
        [switch]$ShowProgress
    )
    try {
        $ProgressPreference = 'SilentlyContinue'
        if($ShowProgress){
            $ProgressPreference = 'Continue'
        }
        [string[]]$WaveFiles = Get-Content -Path "$ListFilename" 
        $WaveFilesCount = $WaveFiles.Count
        $CurrentIndex = 220
        $DownloadedFiles = 0
        $FailedDownloads = 0
        $Percent = 0
        $StatusString = "Starting..."
        if($ShowProgress){
            Write-Progress -Activity "SAVEWAVEFILES" -CurrentOperation "Saving $Filename" -Id 2 -PercentComplete $Percent
        }
        Write-Host "$WaveFilesCount Files detected"

        foreach ($f in $WaveFiles) {
            $ok = Save-SoundFile "$f" -Overwrite:$Overwrite
            if (!$ok) {
                $FailedDownloads++
            } else {
                $DownloadedFiles++
            }
            $CurrentIndex++
            $Percent = [math]::Min([math]::Round((($CurrentIndex / $WaveFilesCount) * 100)), 100)
            $StatusString = "{0} / {1} files downloaded. {2} errors. {3} % completed" -f $DownloadedFiles, $WaveFilesCount, $FailedDownloads, $Percent
            if($ShowProgress){
                Write-Progress -Activity "SAVEWAVEFILES" -CurrentOperation "Saving $Filename" -Id 2 -PercentComplete $Percent
            }
        }
        if($ShowProgress){
            Write-Progress -Activity "SAVEWAVEFILES" -Id 2 -PercentComplete 100 -Completed
        }
    }
    catch {
        throw $_
    }
}

