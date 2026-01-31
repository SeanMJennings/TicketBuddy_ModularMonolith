#!/bin/bash
set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

if [ "$EUID" -ne 0 ]; then
    echo "Please run as root (sudo ./setup.sh)"
    exit 1
fi

if [ ! -f /etc/os-release ] || ! grep -qE "^ID=(ubuntu|debian)$" /etc/os-release; then
    echo "This script only supports Ubuntu and Debian"
    exit 1
fi

. /etc/os-release
SUDO_USER_HOME=$(eval echo ~$SUDO_USER)

echo "Installing prerequisites..."
apt-get update
apt-get install -y curl ca-certificates

has_dotnet10=false
if command -v dotnet >/dev/null 2>&1; then
    if dotnet --list-sdks 2>/dev/null | grep -q "^10\."; then
        has_dotnet10=true
    fi
fi

if [ "$has_dotnet10" = false ]; then
    echo "Installing .NET 10 SDK..."
    curl -fsSL https://packages.microsoft.com/config/$ID/$VERSION_ID/packages-microsoft-prod.deb -o packages-microsoft-prod.deb
    dpkg -i packages-microsoft-prod.deb
    rm packages-microsoft-prod.deb
    apt-get update
    apt-get install -y dotnet-sdk-10.0
fi

if ! command -v docker >/dev/null 2>&1; then
    echo "Installing Docker..."
    apt-get update
    apt-get install -y ca-certificates curl gnupg
    install -m 0755 -d /etc/apt/keyrings
    curl -fsSL https://download.docker.com/linux/$ID/gpg | gpg --dearmor -o /etc/apt/keyrings/docker.gpg
    chmod a+r /etc/apt/keyrings/docker.gpg
    echo "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.gpg] https://download.docker.com/linux/$ID $VERSION_CODENAME stable" | tee /etc/apt/sources.list.d/docker.list > /dev/null
    apt-get update
    apt-get install -y docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin
    usermod -aG docker $SUDO_USER
fi

if ! command -v node >/dev/null 2>&1; then
    echo "Installing Node.js LTS..."
    curl -fsSL https://deb.nodesource.com/setup_lts.x | bash -
    apt-get install -y nodejs
fi

hash -r

if ! command -v aspire >/dev/null 2>&1; then
    echo "Installing .NET Aspire workload..."
    dotnet workload install aspire
fi

echo "Configuring GitHub NuGet feed..."
echo "Create a GitHub PAT with 'read:packages' scope at: https://github.com/settings/tokens"
read -p "Enter GitHub Personal Access Token: " token

if [ -n "$token" ]; then
    dotnet nuget remove source TicketBuddyGitHub 2>/dev/null || true
    dotnet nuget add source "https://nuget.pkg.github.com/SeanMJennings/index.json" \
        --name "TicketBuddyGitHub" \
        --username "SeanMJennings" \
        --password "$token" \
        --store-password-in-clear-text

    echo "export GITHUB_TOKEN=$token" >> "$SUDO_USER_HOME/.bashrc"
    export GITHUB_TOKEN="$token"
fi

echo "Setting up HTTPS development certificates..."
dotnet dev-certs https --clean 2>/dev/null || true
dotnet dev-certs https

echo "Installing UI dependencies..."
cd "$SCRIPT_DIR/UI"
sudo -u $SUDO_USER npm install
echo "Installing Playwright browsers..."
sudo -u $SUDO_USER npx playwright install
cd "$SCRIPT_DIR"

echo "Restoring .NET packages..."
dotnet restore "$SCRIPT_DIR/ModularMonolith/TicketBuddy.sln"

echo ""
echo "Setup complete!"
echo "Note: You may need to log out and back in for Docker group membership to take effect."
echo "Run with: cd ModularMonolith/LocalHosting/Host.Aspire && dotnet run"
