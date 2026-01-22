#Requires -RunAsAdministrator

$ErrorActionPreference = "Stop"

if (-not (Get-Command choco -ErrorAction SilentlyContinue)) {
    Write-Host "Installing Chocolatey..."
    Set-ExecutionPolicy Bypass -Scope Process -Force
    [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072
    Invoke-Expression ((New-Object System.Net.WebClient).DownloadString('https://community.chocolatey.org/install.ps1'))
    $env:Path = [System.Environment]::GetEnvironmentVariable("Path", "Machine") + ";" + [System.Environment]::GetEnvironmentVariable("Path", "User")
}

$hasDotnet10 = (Get-Command dotnet -ErrorAction SilentlyContinue) -and ((dotnet --list-sdks 2>$null) -match "^10\.")
$hasDocker = Get-Command docker -ErrorAction SilentlyContinue
$hasNode = Get-Command node -ErrorAction SilentlyContinue

if (-not $hasDotnet10) {
    Write-Host "Installing .NET 10 SDK..."
    choco install dotnet-10.0-sdk -y --no-progress
}
if (-not $hasDocker) {
    Write-Host "Installing Docker Desktop..."
    choco install docker-desktop -y --no-progress
}
if (-not $hasNode) {
    Write-Host "Installing Node.js..."
    choco install nodejs-lts -y --no-progress
}

$env:Path = [System.Environment]::GetEnvironmentVariable("Path", "Machine") + ";" + [System.Environment]::GetEnvironmentVariable("Path", "User")

if (-not (Get-Command aspire -ErrorAction SilentlyContinue)) {
    Write-Host "Installing .NET Aspire workload..."
    dotnet workload install aspire
}

Write-Host "Configuring GitHub NuGet feed..."
Write-Host "Create a GitHub PAT with 'read:packages' scope at: https://github.com/settings/tokens"
$token = Read-Host "Enter GitHub Personal Access Token"

if ($token) {
    dotnet nuget remove source TicketBuddyGitHub 2>$null
    dotnet nuget add source "https://nuget.pkg.github.com/SeanMJennings/index.json" --name "TicketBuddyGitHub" --username "SeanMJennings" --password $token --store-password-in-clear-text
    [System.Environment]::SetEnvironmentVariable("GITHUB_TOKEN", $token, "User")
    $env:GITHUB_TOKEN = $token
}

Write-Host "Setting up HTTPS development certificates..."
dotnet dev-certs https --clean
dotnet dev-certs https --trust

Write-Host "Installing UI dependencies..."
Push-Location "$PSScriptRoot\UI"
npm install
Pop-Location

Write-Host "Restoring .NET packages..."
dotnet restore "$PSScriptRoot\ModularMonolith\TicketBuddy.sln"

Write-Host "`nSetup complete. Run with: cd ModularMonolith\LocalHosting\Host.Aspire && dotnet run"
