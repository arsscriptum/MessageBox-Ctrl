#╔════════════════════════════════════════════════════════════════════════════════╗
#║                                                                                ║
#║   new-build.ps1                                                                ║
#║   Test functions for my WPF control                                            ║
#║                                                                                ║
#╟────────────────────────────────────────────────────────────────────────────────╢
#║   Guillaume Plante <codegp@icloud.com>                                         ║
#║   Code licensed under the GNU GPL v3.0. See the LICENSE file for details.      ║
#╚════════════════════════════════════════════════════════════════════════════════╝

[CmdletBinding(SupportsShouldProcess)]
param(
    [Parameter(Mandatory = $false)]
    [Alias("rel", "r")]
    [switch]$Release,
    [Parameter(Mandatory = $false)]
    [Alias("c")]
    [switch]$Clean,
    [Parameter(Mandatory = $false)]
    [Alias("i")]
    [switch]$Init
)

$ProjectPath = (Resolve-Path -Path "$PSScriptRoot").Path
$scriptsPath = Join-Path $ProjectPath "scripts"
$testPath = Join-Path $ProjectPath "test"
$CommonScriptPath = Join-Path $scriptsPath "Common.ps1"
$libsPath = Join-Path $ProjectPath "libs"
$BuildPath = Join-Path $ProjectPath "src"
$BinPath = Join-Path $BuildPath "bin"
$ObjPath = Join-Path $BuildPath "obj"
$ArtifactsPath = Join-Path $BuildPath "artifacts"
$TestArtifactsPath = Join-Path $testPath "artifacts"

$DevRootPath = (Resolve-Path -Path "$PSScriptRoot\..").Path
$PackageVaultProjectPath = Join-Path $DevRootPath "packages-vault"
$DeployFinalPath = "$PackageVaultProjectPath\libs\MessageBox-Ctrl"
$ProjectPathBinPath = Join-Path $ProjectPath "bin"
$AllDeployPaths = @($TestArtifactsPath,$DeployFinalPath,$libsPath)

if ($Clean) {
    Write-Host "=========================================================" -f DarkGray
    Write-Host "  CLEANING UP BUILD FILES ...`n" -f White
    Write-Host "  ✔️ $DeployFinalPath " -f DarkCyan
    Write-Host "  ✔️ $BinPath " -f DarkCyan
    Write-Host "  ✔️ $ObjPath " -f DarkCyan
    Write-Host "  ✔️ $ArtifactsPath " -f DarkCyan
    Remove-Item -Path "$DeployFinalPath" -Recurse -Force -ErrorAction Ignore | Out-Null
    Remove-Item -Path "$BinPath" -Recurse -Force -ErrorAction Ignore | Out-Null
    Remove-Item -Path "$ObjPath" -Recurse -Force -ErrorAction Ignore | Out-Null
    Remove-Item -Path "$ArtifactsPath" -Recurse -Force -ErrorAction Ignore | Out-Null
}

Write-Host "=========================================================" -f DarkGray
Write-Host "  CREATING DEPLOY PATHS  ...`n" -f Yellow
ForEach($dp in $AllDeployPaths){
   Write-Host "  ✔️ $dp " -f Blue
   New-Item -Path "$dp" -ItemType Directory -Force -EA Ignore | Out-Null    
}

Write-Host "=========================================================" -f DarkGray

if ($Release) {
    $Target = "Release"
} else {
    $Target = "Debug"
}

Write-Host "=========================================================" -f DarkGray
Write-Host " Setting up build..`n" -f DarkYellow
Write-Host "  ✔️ Configuration $Target`n`n" -f DarkYellow

$IncludeScript = Join-Path $scriptsPath "Include.ps1"
$BuildQueueScript = Join-Path $scriptsPath "BuildQueue.ps1"
$BuildRequestScript = Join-Path $scriptsPath "BuildRequest.ps1"
$RegisterDepScript = Join-Path $scriptsPath "Register-Dependencies.ps1"

Write-Host "=========================================================" -f DarkGray
Write-Host " Global Class Declaration (build queue, build requests, etc...)`n" -f DarkGray
Write-Host "  ✔️  Including Script $IncludeScript" -f DarkCyan
. "$IncludeScript"
Write-Host "  ✔️  Including Script $BuildQueueScript" -f DarkCyan
. "$BuildQueueScript"
Write-Host "  ✔️  Including Script $BuildRequestScript" -f DarkCyan
. "$BuildRequestScript"

. "$CommonScriptPath" -Reset


Write-Host "=========================================================" -f DarkGray
Write-Host " Dependencies...`n" -f DarkGray
Write-Host "  ✔️  Including Script $RegisterDepScript" -f Magenta
. "$RegisterDepScript"


[string]$BuildInfo = "Target {0}, {1}" -f $Target, $tmp

Write-Host "=========================================================" -f DarkGray
Write-Host " Initialization Completed!...`n" -f Blue
Write-Host "  ✔️  Creating a NEW BUILD REQUEST $BuildInfo" -f Blue

$request2 = New-BuildRequest -WorkingDirectory "$BuildPath" -ProjectFilePath "MessageBox.csproj" -Architecture "win-x64" -OutputPath "$BinPath\$Target" -DeployPaths $AllDeployPaths -ArtifactsPath "artifacts" -Configuration "$Target" -Framework "net472" -Version "1.0.1" -LogLevel Normal -Owner "gp"

$request2.AddProperty("LOGGING_ENABLED", "true")

$request3 = New-BuildRequest -WorkingDirectory "$testPath" -ProjectFilePath "TestWarningDll.csproj" -Architecture "win-x64" -OutputPath "$ENV:Programs\MessageBox-Ctrl" -ArtifactsPath "artifacts" -Configuration "$Target" -Framework "net472" -Version "1.0.1" -LogLevel Normal -Buil -Owner "gp" -Type Publish

while (BuildsRemaining) {
    $BuildRequest = Get-NextBuildRequest
    "STARTED BUILD $($BuildRequest.BuildId)" | Out-BuildTitle
    StartBuild $BuildRequest
}


<#
    $DestinationDeployPath = Join-Path "$ENV:Programs" "MessageBox-Ctrl"

    Write-Host "=========================================================" -f DarkGray
    Write-Host " DEPLOYING BINARIES TO MAIN SOLUTION $DestinationDeployPath`n" -f DarkYellow

    Remove-Item -Path "$DestinationDeployPath" -Recurse -Force -ErrorAction Ignore | Out-Null
    New-Item -Path "$DestinationDeployPath" -ItemType Directory -Force -ErrorAction Ignore | Out-Null
    if ($Release) {
        $sourcePath = Join-Path $libsPath "Release"
    } else {
        $sourcePath = Join-Path $libsPath "Debug"
    }

    Get-ChildItem -Path $sourcePath -File | % {
        $fn = $_.FullName
        $bn = $_.BaseName
        $srcFile = "$bn"
        Write-Host "  ✔️ $srcFile => " -f White -n
        Write-Host "$DestinationDeployPath" -f DarkMagenta
        Copy-Item "$fn" "$DestinationDeployPath" -Force
    }

#>


$ToClean = @()
$BuildPathsToClean = @("$(Join-Path $ProjectPath "src")","$(Join-Path $ProjectPath "test")")
$BuildPathsToClean | % {
 # $ToClean += Join-Path "$_" "bin"
  $ToClean += Join-Path "$_" "obj"
  $ToClean += Join-Path "$_" "artifacts"
}
Remove-Item -Path "$ToClean" -Recurse -Force -ErrorAction Ignore | Out-Null

Write-Host "=========================================================" -f DarkGray
Write-Host "  CLEANING UP BUILD FILES ...`n" -f White
$ToClean | % {
    Write-Host "  ✔️ $_ " -f DarkCyan
}