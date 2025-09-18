

function Set-WarningDialogVolume {
    [CmdletBinding(SupportsShouldProcess)]
    param(
        [Parameter(Mandatory = $true, Position = 0, HelpMessage = "Volume (0-100)")]
        [ValidateRange(0,100)]
        [int]$Volume
    )
    $regPath = "HKCU:\SOFTWARE\arsscriptum\PowerShell.Module.ZBookHardware"
    if (-not (Test-Path $regPath)) {
        New-Item -Path $regPath -Force | Out-Null
    }
    Set-ItemProperty -Path $regPath -Name "warning_volume" -Value $Volume -Type DWord
}
