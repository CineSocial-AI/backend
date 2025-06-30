#!/bin/bash

# CineSocial Development Environment Script
set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
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

log_debug() {
    echo -e "${BLUE}[DEBUG]${NC} $1"
}

# Function to show help
show_help() {
    echo "CineSocial Development Environment"
    echo ""
    echo "Usage: $0 [COMMAND]"
    echo ""
    echo "Commands:"
    echo "  start          Start development environment"
    echo "  stop           Stop development environment"
    echo "  restart        Restart development environment"
    echo "  logs           Show logs from all services"
    echo "  logs [service] Show logs from specific service"
    echo "  build          Build all Docker images"
    echo "  clean          Clean up containers and volumes"
    echo "  test           Run tests"
    echo "  db-migrate     Run database migrations"
    echo "  db-seed        Seed database with sample data"
    echo "  status         Show status of all services"
    echo "  shell [service] Open shell in service container"
    echo "  help           Show this help message"
    echo ""
    echo "Services: backend, frontend, database"
}

# Check if Docker is running
check_docker() {
    if ! docker info >/dev/null 2>&1; then
        log_error "Docker is not running. Please start Docker first."
        exit 1
    fi
}

# Start development environment
start_dev() {
    log_info "🚀 Starting CineSocial development environment..."
    
    check_docker
    
    # Create necessary directories
    mkdir -p data/dev
    
    # Start services
    docker-compose up -d
    
    log_info "✅ Development environment started!"
    log_info "📱 Frontend: http://localhost:3000"
    log_info "🚀 Backend API: http://localhost:5000"
    log_info "📊 Health Check: http://localhost:5000/health"
    
    # Show status
    show_status
}

# Stop development environment
stop_dev() {
    log_info "🛑 Stopping CineSocial development environment..."
    
    docker-compose down
    
    log_info "✅ Development environment stopped!"
}

# Restart development environment
restart_dev() {
    log_info "🔄 Restarting CineSocial development environment..."
    
    stop_dev
    sleep 2
    start_dev
}

# Show logs
show_logs() {
    local service=$1
    
    if [[ -z "$service" ]]; then
        log_info "📋 Showing logs from all services..."
        docker-compose logs -f
    else
        log_info "📋 Showing logs from $service..."
        docker-compose logs -f "$service"
    fi
}

# Build images
build_images() {
    log_info "🔨 Building Docker images..."
    
    check_docker
    
    docker-compose build --no-cache
    
    log_info "✅ Images built successfully!"
}

# Clean up
clean_up() {
    log_warn "🧹 Cleaning up development environment..."
    
    # Stop and remove containers
    docker-compose down -v --remove-orphans
    
    # Remove images
    docker-compose down --rmi local
    
    # Clean up volumes
    docker volume prune -f
    
    log_info "✅ Cleanup completed!"
}

# Run tests
run_tests() {
    log_info "🧪 Running tests..."
    
    # Backend tests
    log_info "Running backend tests..."
    docker-compose exec backend dotnet test --verbosity normal
    
    # Frontend tests
    log_info "Running frontend tests..."
    docker-compose exec frontend npm test -- --watchAll=false --verbose
    
    log_info "✅ All tests completed!"
}

# Database migration
db_migrate() {
    log_info "🗃️ Running database migrations..."
    
    docker-compose exec backend dotnet ef database update
    
    log_info "✅ Database migrations completed!"
}

# Seed database
db_seed() {
    log_info "🌱 Seeding database with sample data..."
    
    # This would typically run a seeding script
    # For now, we'll just show a message
    log_warn "Database seeding not implemented yet."
    log_info "You can manually add data through the API or create a seeding script."
}

# Show status
show_status() {
    log_info "📊 Service Status:"
    docker-compose ps
    
    echo ""
    log_info "🔗 Quick Links:"
    echo "  Frontend:    http://localhost:3000"
    echo "  Backend API: http://localhost:5000"
    echo "  Health:      http://localhost:5000/health"
    echo "  API Docs:    http://localhost:5000/swagger"
}

# Open shell in service
open_shell() {
    local service=$1
    
    if [[ -z "$service" ]]; then
        log_error "Please specify a service: backend, frontend"
        exit 1
    fi
    
    log_info "🐚 Opening shell in $service container..."
    
    case $service in
        backend)
            docker-compose exec backend /bin/bash
            ;;
        frontend)
            docker-compose exec frontend /bin/sh
            ;;
        *)
            log_error "Unknown service: $service"
            log_info "Available services: backend, frontend"
            exit 1
            ;;
    esac
}

# Parse command
case "${1:-help}" in
    start)
        start_dev
        ;;
    stop)
        stop_dev
        ;;
    restart)
        restart_dev
        ;;
    logs)
        show_logs "$2"
        ;;
    build)
        build_images
        ;;
    clean)
        clean_up
        ;;
    test)
        run_tests
        ;;
    db-migrate)
        db_migrate
        ;;
    db-seed)
        db_seed
        ;;
    status)
        show_status
        ;;
    shell)
        open_shell "$2"
        ;;
    help|--help|-h)
        show_help
        ;;
    *)
        log_error "Unknown command: $1"
        echo ""
        show_help
        exit 1
        ;;
esac