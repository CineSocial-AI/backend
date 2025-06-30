#!/bin/bash

# CineSocial Production Deployment Script
set -e

echo "🚀 Starting CineSocial production deployment..."

# Configuration
DOMAIN=${DOMAIN:-"yourdomain.com"}
API_DOMAIN=${API_DOMAIN:-"api.yourdomain.com"}
EMAIL=${EMAIL:-"your-email@domain.com"}
ENV_FILE=${ENV_FILE:-".env.docker"}

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Helper functions
log_info() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

log_warn() {
    echo -e "${YELLOW}[WARN]${NC} $1"
}

log_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Check prerequisites
check_prerequisites() {
    log_info "Checking prerequisites..."
    
    if ! command -v docker &> /dev/null; then
        log_error "Docker is not installed. Please install Docker first."
        exit 1
    fi
    
    if ! command -v docker-compose &> /dev/null; then
        log_error "Docker Compose is not installed. Please install Docker Compose first."
        exit 1
    fi
    
    if [[ ! -f "$ENV_FILE" ]]; then
        log_error "Environment file $ENV_FILE not found. Please create it first."
        exit 1
    fi
    
    log_info "Prerequisites check passed ✅"
}

# Create necessary directories
create_directories() {
    log_info "Creating necessary directories..."
    
    mkdir -p secrets
    mkdir -p nginx/ssl
    mkdir -p scripts
    mkdir -p data/postgres
    mkdir -p data/certbot
    
    log_info "Directories created ✅"
}

# Generate secrets if they don't exist
generate_secrets() {
    log_info "Generating secrets..."
    
    if [[ ! -f "secrets/db_password.txt" ]]; then
        openssl rand -base64 32 > secrets/db_password.txt
        log_info "Database password generated"
    fi
    
    if [[ ! -f "secrets/jwt_secret.txt" ]]; then
        openssl rand -base64 64 > secrets/jwt_secret.txt
        log_info "JWT secret generated"
    fi
    
    # Set proper permissions
    chmod 600 secrets/*.txt
    log_info "Secrets generated ✅"
}

# Build and start services
deploy_services() {
    log_info "Building and deploying services..."
    
    # Build images
    log_info "Building Docker images..."
    docker-compose -f docker-compose.prod.yml build --no-cache
    
    # Start database first
    log_info "Starting database..."
    docker-compose -f docker-compose.prod.yml up -d database
    
    # Wait for database to be ready
    log_info "Waiting for database to be ready..."
    sleep 30
    
    # Start backend
    log_info "Starting backend..."
    docker-compose -f docker-compose.prod.yml up -d backend
    
    # Wait for backend to be ready
    log_info "Waiting for backend to be ready..."
    sleep 30
    
    # Start frontend
    log_info "Starting frontend..."
    docker-compose -f docker-compose.prod.yml up -d frontend
    
    # Start reverse proxy
    log_info "Starting reverse proxy..."
    docker-compose -f docker-compose.prod.yml up -d reverse-proxy
    
    log_info "Services deployed ✅"
}

# Setup SSL certificates
setup_ssl() {
    log_info "Setting up SSL certificates..."
    
    # First, get certificates
    docker-compose -f docker-compose.prod.yml --profile ssl run --rm certbot
    
    # Set up certificate renewal
    echo "0 12 * * * docker-compose -f $(pwd)/docker-compose.prod.yml --profile ssl run --rm certbot renew" | crontab -
    
    log_info "SSL certificates configured ✅"
}

# Health check
health_check() {
    log_info "Performing health checks..."
    
    # Check if services are running
    if docker-compose -f docker-compose.prod.yml ps | grep -q "Up"; then
        log_info "Services are running ✅"
    else
        log_error "Some services are not running ❌"
        docker-compose -f docker-compose.prod.yml ps
        exit 1
    fi
    
    # Check backend health
    sleep 10
    if curl -f http://localhost/api/health &> /dev/null; then
        log_info "Backend health check passed ✅"
    else
        log_warn "Backend health check failed. This might be expected on first deployment."
    fi
    
    # Check frontend health
    if curl -f http://localhost/health &> /dev/null; then
        log_info "Frontend health check passed ✅"
    else
        log_warn "Frontend health check failed. This might be expected on first deployment."
    fi
}

# Show deployment info
show_deployment_info() {
    log_info "🎉 Deployment completed!"
    echo ""
    echo "📋 Deployment Information:"
    echo "  📱 Frontend: https://$DOMAIN"
    echo "  🚀 API: https://$API_DOMAIN"
    echo "  📊 Health: https://$DOMAIN/health"
    echo ""
    echo "🔧 Management Commands:"
    echo "  View logs: docker-compose -f docker-compose.prod.yml logs -f"
    echo "  Stop services: docker-compose -f docker-compose.prod.yml down"
    echo "  Update deployment: ./scripts/deploy.sh"
    echo ""
    echo "📁 Important files:"
    echo "  Environment: $ENV_FILE"
    echo "  Database password: secrets/db_password.txt"
    echo "  JWT secret: secrets/jwt_secret.txt"
    echo ""
}

# Cleanup function
cleanup() {
    if [[ $? -ne 0 ]]; then
        log_error "Deployment failed. Cleaning up..."
        docker-compose -f docker-compose.prod.yml down
    fi
}

# Set trap for cleanup
trap cleanup EXIT

# Main deployment process
main() {
    log_info "🎬 CineSocial Production Deployment"
    echo "Domain: $DOMAIN"
    echo "API Domain: $API_DOMAIN"
    echo "Email: $EMAIL"
    echo ""
    
    check_prerequisites
    create_directories
    generate_secrets
    deploy_services
    
    # SSL setup (optional, comment out if not needed)
    if [[ "${SETUP_SSL:-true}" == "true" ]]; then
        setup_ssl
    fi
    
    health_check
    show_deployment_info
}

# Parse command line arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        --domain)
            DOMAIN="$2"
            shift 2
            ;;
        --api-domain)
            API_DOMAIN="$2"
            shift 2
            ;;
        --email)
            EMAIL="$2"
            shift 2
            ;;
        --no-ssl)
            SETUP_SSL="false"
            shift
            ;;
        --help)
            echo "Usage: $0 [OPTIONS]"
            echo "Options:"
            echo "  --domain DOMAIN          Main domain (default: yourdomain.com)"
            echo "  --api-domain DOMAIN      API domain (default: api.yourdomain.com)"
            echo "  --email EMAIL            Email for SSL certificates"
            echo "  --no-ssl                 Skip SSL setup"
            echo "  --help                   Show this help"
            exit 0
            ;;
        *)
            log_error "Unknown option: $1"
            exit 1
            ;;
    esac
done

# Run main function
main