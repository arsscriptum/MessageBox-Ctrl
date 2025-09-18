







function Invoke-StartWarningTask {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $false)]
        [ValidateRange(5, 120)]
        [int]$Temperature = 100
        
    )
    try {
         [int]$Delay = 30
        $UseVbs = $True

        $ScriptWarning = @"

&"C:\Dev\MessageBox-Ctrl\test\bin\Debug\net472\TestWarningDll.exe" "{0}"

"@
        $LogFile = "$ENV:Temp\task_warning.log"
        [string]$ScriptString = $ScriptWarning -f $Temperature

        [string]$ScriptBase64 = [Convert]::ToBase64String([System.Text.Encoding]::Unicode.GetBytes($ScriptString))
        $now = [datetime]::Now.AddSeconds(10)
        # Example Usage
        $selectedUser = "DESKTOP-6K3G95V\gp"

        [string]$TaskName = "WarningDelayedRemote"

        try {
            Write-Host "Unregister task $TaskName" -NoNewline -f DarkYellow
            Unregister-ScheduledTask -TaskName $TaskName -Confirm:$false -ErrorAction Stop
            Remove-SchedTasks -TaskName $TaskName
            Write-Host "Success" -f DarkGreen
        } catch {
            Write-Host "Failed" -f DarkRed
        }
        [string]$folder = Invoke-EnsureSharedScriptFolder
        [string]$VBSFile = Join-Path "$folder" "hidden_powershell.vbs"
        [string]$VBSContent = @"
Set objShell = CreateObject("WScript.Shell")
objShell.Run "powershell.exe -ExecutionPolicy Bypass -EncodedCommand $ScriptBase64", 0, False
"@

        [string]$ArgumentString = "-WindowStyle Hidden -ExecutionPolicy Bypass -EncodedCommand {0}" -f $ScriptBase64
        Write-host "Create Scheduled Task with Base64 Encoded Command"

        if ($UseVbs) {
            New-Item -Path "$VBSFile" -ItemType File -Value "$VBSContent" -Force | Out-Null

            Write-Host "Create a Scheduled Task to Run the VBS Script"
            $WScriptCmd = Get-Command -Name "wscript.exe" -CommandType Application -ErrorAction Stop
            $WScriptBin = $WScriptCmd.Source
            $Action = New-ScheduledTaskAction -Execute "$WScriptBin" -Argument "$VBSFile"

        } else {
            $Action = New-ScheduledTaskAction -Execute "powershell.exe" -Argument "-WindowStyle Hidden -ExecutionPolicy Bypass -EncodedCommand $ScriptBase64"
        }


        $Trigger = New-ScheduledTaskTrigger -At $now -Once:$false
        $Principal = New-ScheduledTaskPrincipal -UserId "$selectedUser" -LogonType Interactive -RunLevel Highest
        $Task = New-ScheduledTask -Action $Action -Trigger $Trigger -Principal $Principal

        write-host "Register and Run Task"
        Register-ScheduledTask -TaskName $TaskName -InputObject $Task | Out-Null
        Add-SchedTasks -TaskName $TaskName
        Start-ScheduledTask -TaskName $TaskName

        Write-Host "In 10 seconds... $LogFile"

    } catch {
        write-error "$_"
    }

}

function Invoke-StopWarningTask {
    [CmdletBinding()]
    param( )
    try {
        [string]$TaskName = "WarningDelayedRemote"
        [int]$NumPowershell = (tasklist | Select-String "powershell" -Raw | measure).Count
        try {
            Stop-ScheduledTask -TaskName $TaskName -ErrorAction Stop
            Write-Host "Unregister task $TaskName" -NoNewline -f DarkYellow
            Unregister-ScheduledTask -TaskName $TaskName -Confirm:$false -ErrorAction Stop
            Remove-SchedTasks -TaskName $TaskName
            Write-Host "Success" -f DarkGreen
        } catch {
            Write-Host "Failed" -f DarkRed
        }
        [string[]]$Res = & "C:\Windows\system32\taskkill.exe" "/IM" "powershell.exe" "/F" 2> "$ENV:Temp\killres.txt"
        $Killed = $Res.Count
        Write-Host "NumPowershell $NumPowershell Killed $Killed"

    } catch {
        write-error "$_"
    }

}
