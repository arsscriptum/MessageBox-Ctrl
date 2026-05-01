# MessageBox.ps1
# Wrapper functions around TestWarningDll.exe
# Requires $ENV:TestWarning to point to the executable

function Invoke-WarningDialog {
    param(
        [Parameter(Mandatory = $true)]  [string] $Message,
        [Parameter(Mandatory = $false)] [string] $Title   = "Error",
        [Parameter(Mandatory = $false)] [string] $Alert   = $null,
        [Parameter(Mandatory = $false)] [string] $Style   = $null,
        [Parameter(Mandatory = $false)] [double] $Volume  = 0.3
    )

    if (-not $ENV:TestWarning) {
        Write-Error "ENV:TestWarning is not set."
        return
    }
    if (-not (Test-Path $ENV:TestWarning)) {
        Write-Error "Executable not found: $ENV:TestWarning"
        return
    }

    $argList = @("-m", $Message, "-t", $Title)

    if ($Alert) { $argList += @("-a", $Alert) }
    if ($Style) { $argList += @("-s", $Style) }

    & $ENV:TestWarning @argList
}

function Show-CriticalError {
    param(
        [Parameter(Mandatory = $true)]  [string] $Message,
        [Parameter(Mandatory = $false)] [string] $Title = "CRITICAL ERROR",
        [Parameter(Mandatory = $false)] [string] $Alert = "NukeAlert"
    )
    Invoke-WarningDialog -Message $Message -Title $Title -Alert $Alert -Style "Critical"
}

function Show-Warning {
    param(
        [Parameter(Mandatory = $true)]  [string] $Message,
        [Parameter(Mandatory = $false)] [string] $Title = "WARNING",
        [Parameter(Mandatory = $false)] [string] $Alert = "Alert1"
    )
    Invoke-WarningDialog -Message $Message -Title $Title -Alert $Alert -Style "Warning"
}

function Show-TempAlert {
    param(
        [Parameter(Mandatory = $true)]  [int] $Temperature,
        [Parameter(Mandatory = $false)] [string] $Title = "TEMPERATURE CRITICAL",
        [Parameter(Mandatory = $false)] [string] $Alert = "NukeAlert"
    )
    $Str = "The System Temperature has reached Critical Point of {0} degres!" -f $Temperature
    Invoke-WarningDialog -Message $Str -Title $Title -Alert $Alert -Style "TempAlert"
}

function Show-InfoDialog {
    param(
        [Parameter(Mandatory = $true)]  [string] $Message,
        [Parameter(Mandatory = $false)] [string] $Title = "NOTICE",
        [Parameter(Mandatory = $false)] [string] $Alert = $null
    )
    Invoke-WarningDialog -Message $Message -Title $Title -Alert $Alert -Style "Normal"
}

function Show-Win10Dialog {
    param(
        [Parameter(Mandatory = $true)]  [string] $Message,
        [Parameter(Mandatory = $false)] [string] $Title = "Notice",
        [Parameter(Mandatory = $false)] [string] $Alert = $null
    )
    Invoke-WarningDialog -Message $Message -Title $Title -Alert $Alert -Style "Windows10"
}

function Get-AlertSounds {
    param()
    if (-not $ENV:TestWarning) { Write-Error "ENV:TestWarning is not set."; return }
    & $ENV:TestWarning -l
}

function Get-DialogStyles {
    param()
    if (-not $ENV:TestWarning) { Write-Error "ENV:TestWarning is not set."; return }
    & $ENV:TestWarning -x
}