#╔════════════════════════════════════════════════════════════════════════════════╗
#║                                                                                ║
#║   WarnMessage.ps1                                                              ║
#║                                                                                ║
#╟────────────────────────────────────────────────────────────────────────────────╢
#║   Guillaume Plante <codegp@icloud.com>                                         ║
#║   Code licensed under the GNU GPL v3.0. See the LICENSE file for details.      ║
#╚════════════════════════════════════════════════════════════════════════════════╝

enum AlertId {
    TwentyEightFiftyFour = 0
    AirgapOff = 1
    Alert1 = 2
    Alert2 = 3
    AlertVoice = 4
    AnomalyDetected = 5
    #...
    VideoGameAlarm = 15
}

enum StyleId {
    Normal = 0
    Critical = 1
    Warning = 2
    TempAlert = 3
    Windows10 = 4
}


function Show-WarningMessage {
    [CmdletBinding()]
    param(
        [Parameter(Position = 0, Mandatory = $true, HelpMessage = "Message in the messagebox")]
        [Alias('m')]
        [string]$Message,

        [Parameter(Position = 1, Mandatory = $true, HelpMessage = "Title in the messagebox")]
        [Alias('t')]
        [string]$Title,

        [Parameter(Mandatory = $false)]
        [Alias('s')]
        [StyleId]$Style,

        [Parameter(Mandatory = $false)]
        [Alias('a')]
        [AlertId]$Alert
    )

    try {

        [System.Collections.ArrayList]$CmdArguments = [System.Collections.ArrayList]::new()
        $FilePath = "$ENV:TESTWARNING"
        if ([string]::IsNullOrEmpty($FilePath)) {
            Write-Warning "TESTWARNING Environment Variable NOT SET"
            [System.IO.FileSystemInfo]$fi=gci "$ENV:Programs" -File -Recurse -Filter "TestWarningDll.exe" -Depth 1 -ErrorAction Ignore | select -First 1
            if($fi -eq $Null){
                throw "TestWarning Environment Variable NOT SET and not found"
            }
            $FilePath = $fi.Fullname
        }
        [void]$CmdArguments.Add("-m")
        [void]$CmdArguments.Add("$Message")
        [void]$CmdArguments.Add("-t")
        [void]$CmdArguments.Add("$Title")
        
        if ($Style) {
            [void]$CmdArguments.Add("-s")
            [void]$CmdArguments.Add("$Style")
        }
        if ($Alert) {
            [void]$CmdArguments.Add("-a")
            [void]$CmdArguments.Add("$Alert")
        }
      
        $CmdArguments | % { $AllCmds += "$_ " }
     
        $ProcessArgs = @{
            FilePath = "$FilePath"
            ArgumentList = $CmdArguments
            WorkingDirectory = "$($PWD.Path)"
            NoNewWindow = $True
            PassThru = $False
            Wait = $False
        }
        Write-Verbose "$AllCmds"

        Start-Process @ProcessArgs


    } catch {
        Write-Error "$_"
    }
}

New-alias -Name warnmsg -Value Show-WarningMessage -Force -ErrorAction Ignore -Scope GLobal | Out-Null