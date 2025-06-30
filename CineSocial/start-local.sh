#!/bin/bash

# CineSocial - Super Simple Local Startup Script
echo "🎬 Starting CineSocial locally..."

# Colors
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m'

# Check if Docker is running
if ! docker info >/dev/null 2>&1; then
    echo -e "${RED}❌ Docker is not running!${NC}"
    echo "Please start Docker Desktop and try again."
    exit 1
fi

# Check if Docker Compose is available
if ! command -v docker-compose &> /dev/null; then
    echo -e "${RED}❌ Docker Compose not found!${NC}"
    echo "Please install Docker Desktop (includes Docker Compose)."
    exit 1
fi

echo -e "${YELLOW}🔧 Starting services...${NC}"

# Start all services
docker-compose up -d

# Wait a moment for services to start
echo -e "${YELLOW}⏳ Waiting for services to start...${NC}"
sleep 10

# Check if services are running
if docker-compose ps | grep -q "Up"; then
    echo ""
    echo -e "${GREEN}🎉 CineSocial is running!${NC}"
    echo ""
    echo "📱 Frontend: http://localhost:3000"
    echo "🚀 Backend:  http://localhost:5000"
    echo "📊 Health:   http://localhost:5000/health"
    echo "📖 API Docs: http://localhost:5000/swagger"
    echo ""
    echo "Commands:"
    echo "  Stop:      docker-compose down"
    echo "  Logs:      docker-compose logs -f"
    echo "  Restart:   docker-compose restart"
    echo ""
    echo -e "${GREEN}Happy coding! 🚀${NC}"
else
    echo -e "${RED}❌ Failed to start services${NC}"
    echo "Check logs with: docker-compose logs"
fi