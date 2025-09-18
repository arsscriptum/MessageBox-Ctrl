


function Show-WarningDllDialog {
    [CmdletBinding(SupportsShouldProcess)]
    param()
    Add-Type -AssemblyName PresentationFramework
    Add-Type -AssemblyName PresentationCore

    
    $ctrl = New-Object MessageBox.WarningDialog
    $ctrl.ShowDialog()

}
