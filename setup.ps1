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
$hasGitHub = Get-Command gh -ErrorAction SilentlyContinue

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

if (-not $hasGitHub) {
    Write-Host "Installing GitHub..."
    choco install gh -y --no-progress
}

$env:Path = [System.Environment]::GetEnvironmentVariable("Path", "Machine") + ";" + [System.Environment]::GetEnvironmentVariable("Path", "User")

if (-not (Get-Command aspire -ErrorAction SilentlyContinue)) {
    Write-Host "Installing .NET Aspire workload..."
    dotnet workload install aspire
}

Write-Host "Configuring GitHub NuGet feed..."
$gitHubUsername = Read-Host "Enter GitHub username"
gh auth login --scopes read:packages --git-protocol ssh --hostname github.com --skip-ssh-key
$token = gh auth token

if ($token) {
    dotnet nuget remove source TicketBuddyGitHub 2>$null
    dotnet nuget add source "https://nuget.pkg.github.com/SeanMJennings/index.json" --name "TicketBuddyGitHub" --username $gitHubUsername --password $token --store-password-in-clear-text
}

Write-Host "Setting up HTTPS development certificates..."
dotnet dev-certs https --clean
dotnet dev-certs https --trust

echo "You may still need to navigate to localhost:5001 in browser when running and allow"

Write-Host "Installing UI dependencies..."
Push-Location "$PSScriptRoot\UI"
npm install
Pop-Location

Push-Location "$PSScriptRoot\E2E"
Write-Host "Installing Playwright browsers..."
npx playwright install
Pop-Location

Write-Host "Restoring .NET packages..."
dotnet restore "$PSScriptRoot\ModularMonolith\TicketBuddy.slnx"

Write-Host "`nSetup complete. Run with: cd ModularMonolith\LocalHosting\Host.Aspire && dotnet run"
