#!/usr/bin/env bash
set -euo pipefail

CLUSTER_NAME="ticketbuddy"
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
SUDO_USER="${SUDO_USER:-$(logname 2>/dev/null || whoami)}"

RED='\033[0;31m'; GREEN='\033[0;32m'; YELLOW='\033[1;33m'; NC='\033[0m'
info()    { echo -e "${GREEN}[INFO]${NC} $*"; }
warn()    { echo -e "${YELLOW}[WARN]${NC} $*"; }
error()   { echo -e "${RED}[ERROR]${NC} $*"; exit 1; }

install_docker_if_missing() {
  if command -v docker &>/dev/null; then
    return
  fi

  info "Docker not found — installing..."
  . /etc/os-release
  sudo apt-get update
  sudo apt-get install -y ca-certificates curl gnupg
  sudo install -m 0755 -d /etc/apt/keyrings
  curl -fsSL https://download.docker.com/linux/$ID/gpg | sudo gpg --dearmor -o /etc/apt/keyrings/docker.gpg
  sudo chmod a+r /etc/apt/keyrings/docker.gpg
  echo "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.gpg] https://download.docker.com/linux/$ID $VERSION_CODENAME stable" | sudo tee /etc/apt/sources.list.d/docker.list > /dev/null
  sudo apt-get update
  sudo apt-get install -y docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin
  sudo usermod -aG docker $USER
  info "Docker installed. You may need to log out and back in for group membership to take effect."
}

install_kubectl_if_missing() {
  if command -v kubectl &>/dev/null; then
    return
  fi

  local kubectl_path
  kubectl_path=$(sudo -u "$SUDO_USER" which kubectl 2>/dev/null || true)
  if [[ -n "$kubectl_path" ]]; then
    ln -sf "$kubectl_path" /usr/local/bin/kubectl
    return
  fi

  info "kubectl not found — installing..."
  sudo apt install -y snap
  sudo snap install kubectl --classic
}

ensure_docker_running() {
  local desktop_socket="/home/${SUDO_USER}/.docker/desktop/docker.sock"
  if [[ -S "$desktop_socket" ]]; then
    export DOCKER_HOST="unix://${desktop_socket}"
  fi
  docker info &>/dev/null || error "Docker Desktop is not running. Please start it and try again."
}

check_github_token() {
  if [[ -z "${TICKETBUDDY_GITHUB_TOKEN:-}" ]]; then
    error "TICKETBUDDY_GITHUB_TOKEN environment variable is not set. It is required to restore private NuGet packages.\n  Export it with: export TICKETBUDDY_GITHUB_TOKEN=<your_token>"
  fi
}

install_kind_if_missing() {
  if command -v kind &>/dev/null; then
    return
  fi

  local kind_path
  kind_path=$(sudo -u "$SUDO_USER" which kind 2>/dev/null || true)
  if [[ -n "$kind_path" ]]; then
    ln -sf "$kind_path" /usr/local/bin/kind
    return
  fi

  info "kind not found — installing..."
  local os arch
  os="$(uname -s | tr '[:upper:]' '[:lower:]')"
  arch="$(uname -m)"
  [[ "$arch" == "x86_64" ]]  && arch="amd64"
  [[ "$arch" == "aarch64" ]] && arch="arm64"
  curl -sSLo /usr/local/bin/kind "https://kind.sigs.k8s.io/dl/v0.27.0/kind-${os}-${arch}"
  chmod +x /usr/local/bin/kind
  info "kind installed."
}

install_helm_if_missing() {
  if command -v helm &>/dev/null; then
    return
  fi

  local helm_path
  helm_path=$(sudo -u "$SUDO_USER" which helm 2>/dev/null || true)
  if [[ -n "$helm_path" ]]; then
    ln -sf "$helm_path" /usr/local/bin/helm
    return
  fi

  info "Helm not found — installing..."
  curl -fsSL https://raw.githubusercontent.com/helm/helm/main/scripts/get-helm-3 | bash
  info "Helm installed."
}

install_kube_prometheus_stack() {
  info "Installing kube-prometheus-stack (Prometheus + Grafana)..."

  helm repo add prometheus-community https://prometheus-community.github.io/helm-charts 2>/dev/null || true
  helm repo update

  kubectl create namespace monitoring --dry-run=client -o yaml | kubectl apply -f -

  helm upgrade --install kube-prometheus-stack prometheus-community/kube-prometheus-stack \
    --namespace monitoring \
    --set alertmanager.enabled=false \
    --set grafana.adminPassword=admin \
    --set grafana.service.type=NodePort \
    --set grafana.service.nodePort=30030 \
    --set prometheus.service.type=NodePort \
    --set prometheus.service.nodePort=30090 \
    --set prometheus.prometheusSpec.serviceMonitorSelectorNilUsesHelmValues=false \
    --wait --timeout=5m

  info "kube-prometheus-stack installed."
}

install_metrics_server() {
  kubectl apply -f https://github.com/kubernetes-sigs/metrics-server/releases/latest/download/components.yaml
  kubectl patch deployment metrics-server -n kube-system \
    --type='json' \
    -p='[{"op":"add","path":"/spec/template/spec/containers/0/args/-","value":"--kubelet-insecure-tls"}]'
}

create_cluster() {
  if kind get clusters 2>/dev/null | grep -q "^${CLUSTER_NAME}$"; then
    warn "Cluster '${CLUSTER_NAME}' already exists — skipping creation."
    return
  fi

  info "Creating kind cluster '${CLUSTER_NAME}'..."
  kind create cluster \
    --name "$CLUSTER_NAME" \
    --config "$SCRIPT_DIR/kind-config.yaml"
  info "Cluster created."
}

build_images() {
  info "Building Docker images (this may take a few minutes on first run)..."
  cd "$REPO_ROOT"

  info "  Building keycloak image..."
  docker build \
    -t ticketbuddy-keycloak:local \
    -f ModularMonolith/Modules/Keycloak.Users/Infrastructure.Keycloak/Dockerfile \
    ModularMonolith/Modules/Keycloak.Users/Infrastructure.Keycloak/

  info "  Building migrations image..."
  docker build \
    -t ticketbuddy-migrations:local \
    --build-arg TICKETBUDDY_GITHUB_TOKEN="$TICKETBUDDY_GITHUB_TOKEN" \
    -f ModularMonolith/Database/Host.Migrations/Dockerfile \
    ModularMonolith/

  info "  Building api image..."
  docker build \
    -t ticketbuddy-api:local \
    --build-arg TICKETBUDDY_GITHUB_TOKEN="$TICKETBUDDY_GITHUB_TOKEN" \
    -f ModularMonolith/Host/Dockerfile \
    ModularMonolith/

  info "  Building dataseeder image..."
  docker build \
    -t ticketbuddy-dataseeder:local \
    --build-arg TICKETBUDDY_GITHUB_TOKEN="$TICKETBUDDY_GITHUB_TOKEN" \
    -f ModularMonolith/LocalHosting/LocalHost.Dataseeder/Dockerfile \
    ModularMonolith/

  info "  Building ui image..."
  docker build \
    -t ticketbuddy-ui:local \
    --build-arg USE_COMPOSE_ENV=true \
    -f UI/Dockerfile \
    UI/

  info "All images built."
}

load_images() {
  info "Loading images into kind cluster..."

  for image in \
    ticketbuddy-keycloak:local \
    ticketbuddy-migrations:local \
    ticketbuddy-api:local \
    ticketbuddy-dataseeder:local \
    ticketbuddy-ui:local; do
    info "  Loading $image..."
    kind load docker-image "$image" --name "$CLUSTER_NAME"
  done

  for image in \
    postgres:latest \
    redis:7.0-alpine \
    masstransit/rabbitmq \
    busybox \
    mcr.microsoft.com/dotnet/aspire-dashboard:latest \
    oliver006/redis_exporter:latest \
    quay.io/prometheuscommunity/postgres-exporter:latest; do
    info "  Loading $image..."
    docker pull --platform linux/amd64 "$image"
    printf 'FROM %s\n' "$image" | docker build --platform linux/amd64 --load -t "$image" -
    docker save "$image" -o /tmp/kind-image.tar
    kind load image-archive /tmp/kind-image.tar --name "$CLUSTER_NAME"
    rm -f /tmp/kind-image.tar
  done

  info "Images loaded."
}

apply_manifests() {
  local ns="ticketbuddy"

  info "Applying namespace..."
  kubectl apply -f "$SCRIPT_DIR/manifests/00-namespace.yaml"

  info "Deploying infrastructure (postgres, redis, rabbitmq, keycloak, aspire-dashboard)..."
  kubectl apply -f "$SCRIPT_DIR/manifests/01-postgres.yaml"
  kubectl apply -f "$SCRIPT_DIR/manifests/02-redis.yaml"
  kubectl apply -f "$SCRIPT_DIR/manifests/03-rabbitmq.yaml"
  kubectl apply -f "$SCRIPT_DIR/manifests/04-keycloak.yaml"
  kubectl apply -f "$SCRIPT_DIR/manifests/05-aspire-dashboard.yaml"

  info "Waiting for infrastructure to be ready..."
  kubectl wait --for=condition=ready pod -l app=postgresql    -n "$ns" --timeout=300s
  kubectl wait --for=condition=ready pod -l app=redis         -n "$ns" --timeout=120s
  kubectl wait --for=condition=ready pod -l app=rabbitmq      -n "$ns" --timeout=120s
  kubectl wait --for=condition=ready pod -l app=keycloak      -n "$ns" --timeout=180s

  info "Running database migrations..."
  kubectl apply -f "$SCRIPT_DIR/manifests/06-migrations-job.yaml"
  kubectl wait --for=condition=complete job/migrations -n "$ns" --timeout=120s
  info "Migrations complete."

  info "Deploying API..."
  kubectl apply -f "$SCRIPT_DIR/manifests/07-api.yaml"
  kubectl wait --for=condition=ready pod -l app=api -n "$ns" --timeout=120s
  info "API ready."

  info "Running dataseeder..."
  kubectl apply -f "$SCRIPT_DIR/manifests/08-dataseeder-job.yaml"
  kubectl wait --for=condition=complete job/dataseeder -n "$ns" --timeout=180s
  info "Dataseeder complete."

  info "Deploying UI..."
  kubectl apply -f "$SCRIPT_DIR/manifests/09-ui.yaml"
  kubectl wait --for=condition=ready pod -l app=ui -n "$ns" --timeout=60s
  info "UI ready."

  info "Applying ServiceMonitors..."
  kubectl apply -f "$SCRIPT_DIR/manifests/10-service-monitors.yaml"
}

provision_grafana_dashboards() {
  info "Provisioning Grafana dashboards..."

  local -A dashboards=(
    [rabbitmq]=10991
    [redis]=763
    [aspnetcore]=19924
  )

  local tmpfile
  tmpfile=$(mktemp /tmp/grafana-dashboard-XXXXXX.json)
  trap "rm -f $tmpfile" RETURN

  for name in "${!dashboards[@]}"; do
    local id="${dashboards[$name]}"
    info "  Fetching dashboard: ${name} (ID ${id})..."

    curl -sf "https://grafana.com/api/dashboards/${id}/revisions/latest/download" \
      | sed 's/\${DS_PROMETHEUS}/prometheus/g; s/\${DS_PROM}/prometheus/g' \
      > "$tmpfile"

    kubectl create configmap "grafana-dashboard-${name}" \
      --namespace monitoring \
      --from-file="${name}.json=${tmpfile}" \
      --dry-run=client -o yaml \
      | kubectl apply --server-side -f -

    kubectl label configmap "grafana-dashboard-${name}" \
      --namespace monitoring \
      --overwrite \
      grafana_dashboard=1
  done

  info "Grafana dashboards provisioned."
}

print_urls() {
  echo ""
  echo -e "${GREEN}════════════════════════════════════════${NC}"
  echo -e "${GREEN}  TicketBuddy is running!${NC}"
  echo -e "${GREEN}════════════════════════════════════════${NC}"
  echo ""
  echo "  UI               → http://localhost:5173"
  echo "  API              → http://localhost:5000"
  echo "  Keycloak         → http://localhost:8180  (admin / admin)"
  echo "  RabbitMQ Mgmt    → http://localhost:15672 (guest / guest)"
  echo "  Aspire Dashboard → http://localhost:18888"
  echo "  Prometheus       → http://localhost:9090"
  echo "  Grafana          → http://localhost:3000  (admin / admin)"
  echo ""
}

main() {
  install_docker_if_missing
  install_kubectl_if_missing
  install_kind_if_missing
  install_helm_if_missing
  ensure_docker_running
  check_github_token
  create_cluster
  install_metrics_server
  install_kube_prometheus_stack
  build_images
  load_images
  apply_manifests
  provision_grafana_dashboards
  print_urls
}

main "$@"
