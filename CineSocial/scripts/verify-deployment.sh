#!/bin/bash

# CineSocial Deployment Verification Script
set -e

# Configuration
DOMAIN=${DOMAIN:-"localhost"}
API_DOMAIN=${API_DOMAIN:-"localhost:5000"}
PROTOCOL=${PROTOCOL:-"http"}
FRONTEND_URL="${PROTOCOL}://${DOMAIN}"
API_URL="${PROTOCOL}://${API_DOMAIN}"

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

# Test counter
TESTS_PASSED=0
TESTS_FAILED=0
TOTAL_TESTS=0

# Function to run a test
run_test() {
    local test_name="$1"
    local test_command="$2"
    
    TOTAL_TESTS=$((TOTAL_TESTS + 1))
    echo ""
    log_info "Testing: $test_name"
    
    if eval "$test_command"; then
        log_info "✅ PASSED: $test_name"
        TESTS_PASSED=$((TESTS_PASSED + 1))
        return 0
    else
        log_error "❌ FAILED: $test_name"
        TESTS_FAILED=$((TESTS_FAILED + 1))
        return 1
    fi
}

# Function to test if service is responding
test_service_health() {
    local service_name="$1"
    local url="$2"
    local expected_status="$3"
    
    log_debug "Testing $service_name at $url"
    
    local response=$(curl -s -o /dev/null -w "%{http_code}" "$url" --connect-timeout 10 --max-time 30 2>/dev/null || echo "000")
    
    if [[ "$response" == "$expected_status" ]]; then
        log_debug "$service_name responding correctly (HTTP $response)"
        return 0
    else
        log_error "$service_name not responding correctly (HTTP $response, expected $expected_status)"
        return 1
    fi
}

# Function to test frontend
test_frontend() {
    local frontend_health_url="$FRONTEND_URL/health"
    
    # If we're testing production, try the main URL instead of health endpoint
    if [[ "$PROTOCOL" == "https" ]]; then
        test_service_health "Frontend" "$FRONTEND_URL" "200"
    else
        # For development, try health endpoint first, fallback to main page
        if ! test_service_health "Frontend Health" "$frontend_health_url" "200"; then
            test_service_health "Frontend Main Page" "$FRONTEND_URL" "200"
        else
            return 0
        fi
    fi
}

# Function to test backend API
test_backend_api() {
    test_service_health "Backend API Health" "$API_URL/api/health" "200"
}

# Function to test backend health endpoint
test_backend_health() {
    test_service_health "Backend Health" "$API_URL/health" "200"
}

# Function to test API endpoints
test_api_endpoints() {
    log_debug "Testing API endpoints..."
    
    # Test movies endpoint
    local movies_response=$(curl -s -o /dev/null -w "%{http_code}" "$API_URL/api/movies" --connect-timeout 10 --max-time 30 2>/dev/null || echo "000")
    
    if [[ "$movies_response" == "200" ]]; then
        log_debug "Movies API endpoint responding correctly"
    else
        log_error "Movies API endpoint not responding correctly (HTTP $movies_response)"
        return 1
    fi
    
    # Test genres endpoint
    local genres_response=$(curl -s -o /dev/null -w "%{http_code}" "$API_URL/api/genres" --connect-timeout 10 --max-time 30 2>/dev/null || echo "000")
    
    if [[ "$genres_response" == "200" ]]; then
        log_debug "Genres API endpoint responding correctly"
        return 0
    else
        log_error "Genres API endpoint not responding correctly (HTTP $genres_response)"
        return 1
    fi
}

# Function to test SSL certificate (for production)
test_ssl_certificate() {
    if [[ "$PROTOCOL" != "https" ]]; then
        log_debug "Skipping SSL test (not HTTPS)"
        return 0
    fi
    
    log_debug "Testing SSL certificate..."
    
    # Test SSL certificate validity
    local ssl_check=$(echo | openssl s_client -connect "${DOMAIN}:443" -servername "$DOMAIN" 2>/dev/null | openssl x509 -noout -dates 2>/dev/null)
    
    if [[ -n "$ssl_check" ]]; then
        log_debug "SSL certificate is valid"
        
        # Check certificate expiration
        local not_after=$(echo "$ssl_check" | grep "notAfter" | cut -d= -f2)
        local expiry_date=$(date -d "$not_after" +%s 2>/dev/null || echo "0")
        local current_date=$(date +%s)
        local days_until_expiry=$(( (expiry_date - current_date) / 86400 ))
        
        if [[ $days_until_expiry -gt 7 ]]; then
            log_debug "SSL certificate expires in $days_until_expiry days"
            return 0
        else
            log_warn "SSL certificate expires soon ($days_until_expiry days)"
            return 1
        fi
    else
        log_error "SSL certificate check failed"
        return 1
    fi
}

# Function to test security headers
test_security_headers() {
    log_debug "Testing security headers..."
    
    local headers=$(curl -s -I "$FRONTEND_URL" --connect-timeout 10 --max-time 30 2>/dev/null || echo "")
    
    # Check for important security headers
    local required_headers=(
        "X-Content-Type-Options"
        "X-Frame-Options"
    )
    
    for header in "${required_headers[@]}"; do
        if echo "$headers" | grep -qi "$header"; then
            log_debug "Security header present: $header"
        else
            log_warn "Security header missing: $header"
        fi
    done
    
    # For HTTPS, check HSTS
    if [[ "$PROTOCOL" == "https" ]]; then
        if echo "$headers" | grep -qi "Strict-Transport-Security"; then
            log_debug "HSTS header present"
        else
            log_warn "HSTS header missing"
        fi
    fi
    
    return 0
}

# Function to test database connectivity through API
test_database_connectivity() {
    log_debug "Testing database connectivity through API..."
    
    # Test by trying to fetch data that requires database access
    local db_test_response=$(curl -s "$API_URL/api/movies" --connect-timeout 10 --max-time 30 2>/dev/null || echo "error")
    
    if [[ "$db_test_response" != "error" ]] && [[ -n "$db_test_response" ]]; then
        log_debug "Database connectivity confirmed through API"
        return 0
    else
        log_error "Database connectivity test failed"
        return 1
    fi
}

# Function to test Docker containers
test_docker_containers() {
    log_debug "Testing Docker containers status..."
    
    # Check if Docker Compose is available
    if ! command -v docker-compose &> /dev/null; then
        log_warn "Docker Compose not available for container status check"
        return 0
    fi
    
    # Check container status
    local containers_status=$(docker-compose ps 2>/dev/null || echo "error")
    
    if [[ "$containers_status" == "error" ]]; then
        log_warn "Cannot check Docker container status"
        return 0
    fi
    
    # Check if all required containers are running
    local required_services=("backend" "frontend")
    
    for service in "${required_services[@]}"; do
        if echo "$containers_status" | grep -q "$service.*Up"; then
            log_debug "Container running: $service"
        else
            log_error "Container not running: $service"
            return 1
        fi
    done
    
    return 0
}

# Function to test performance
test_performance() {
    log_debug "Testing application performance..."
    
    # Test frontend load time
    local start_time=$(date +%s%N)
    local frontend_response=$(curl -s -o /dev/null -w "%{http_code}" "$FRONTEND_URL" --connect-timeout 10 --max-time 30 2>/dev/null || echo "000")
    local end_time=$(date +%s%N)
    local frontend_duration=$(( (end_time - start_time) / 1000000 )) # Convert to milliseconds
    
    log_debug "Frontend response time: ${frontend_duration}ms"
    
    # Test API response time
    start_time=$(date +%s%N)
    local api_response=$(curl -s -o /dev/null -w "%{http_code}" "$API_URL/api/health" --connect-timeout 10 --max-time 30 2>/dev/null || echo "000")
    end_time=$(date +%s%N)
    local api_duration=$(( (end_time - start_time) / 1000000 )) # Convert to milliseconds
    
    log_debug "API response time: ${api_duration}ms"
    
    # Performance should be reasonable
    if [[ $frontend_duration -lt 5000 ]] && [[ $api_duration -lt 1000 ]]; then
        log_debug "Performance is acceptable"
        return 0
    else
        log_warn "Performance may be slower than expected"
        return 1
    fi
}

# Function to show deployment summary
show_deployment_summary() {
    log_info "📋 Deployment Summary:"
    echo ""
    log_debug "Configuration:"
    echo "  Frontend URL: $FRONTEND_URL"
    echo "  API URL: $API_URL"
    echo "  Protocol: $PROTOCOL"
    
    echo ""
    log_debug "Service Endpoints:"
    echo "  Main Application: $FRONTEND_URL"
    echo "  API Health: $API_URL/health"
    echo "  API Movies: $API_URL/api/movies"
    echo "  API Genres: $API_URL/api/genres"
    
    if [[ "$PROTOCOL" == "https" ]]; then
        echo ""
        log_debug "SSL Information:"
        local ssl_info=$(echo | openssl s_client -connect "${DOMAIN}:443" -servername "$DOMAIN" 2>/dev/null | openssl x509 -noout -subject -issuer -dates 2>/dev/null || echo "SSL info not available")
        echo "$ssl_info"
    fi
}

# Main verification function
main() {
    log_info "🚀 CineSocial Deployment Verification"
    echo ""
    
    # Show configuration
    log_info "Configuration:"
    echo "  Frontend: $FRONTEND_URL"
    echo "  API: $API_URL"
    echo "  Protocol: $PROTOCOL"
    echo ""
    
    # Run all tests
    run_test "Frontend Service" "test_frontend"
    run_test "Backend Health" "test_backend_health"
    run_test "Backend API" "test_backend_api"
    run_test "API Endpoints" "test_api_endpoints"
    run_test "Database Connectivity" "test_database_connectivity"
    run_test "Docker Containers" "test_docker_containers"
    run_test "Security Headers" "test_security_headers"
    run_test "SSL Certificate" "test_ssl_certificate"
    run_test "Performance" "test_performance"
    
    # Show deployment summary
    echo ""
    show_deployment_summary
    
    # Show test results
    echo ""
    log_info "📊 Verification Results:"
    echo "  Total tests run: $TOTAL_TESTS"
    echo "  Tests passed: $TESTS_PASSED"
    echo "  Tests failed: $TESTS_FAILED"
    
    if [[ $TESTS_FAILED -eq 0 ]]; then
        echo ""
        log_info "🎉 ALL DEPLOYMENT TESTS PASSED!"
        log_info "✅ CineSocial is successfully deployed and working correctly."
        
        echo ""
        log_info "🔗 Access your application:"
        echo "  🌐 Website: $FRONTEND_URL"
        echo "  🚀 API: $API_URL"
        
        exit 0
    else
        echo ""
        log_error "❌ SOME DEPLOYMENT TESTS FAILED!"
        log_error "Please fix the issues before considering the deployment complete."
        
        echo ""
        log_info "🔧 Troubleshooting tips:"
        echo "  1. Check container logs: docker-compose logs -f"
        echo "  2. Verify environment variables: cat .env.docker"
        echo "  3. Check DNS configuration: nslookup $DOMAIN"
        echo "  4. Test manual API calls: curl $API_URL/api/health"
        
        exit 1
    fi
}

# Function to show help
show_help() {
    echo "CineSocial Deployment Verification Script"
    echo ""
    echo "Usage: $0 [OPTIONS]"
    echo ""
    echo "Options:"
    echo "  --domain DOMAIN        Frontend domain (default: localhost)"
    echo "  --api-domain DOMAIN    API domain (default: localhost:5000)"
    echo "  --https               Use HTTPS protocol"
    echo "  --http                Use HTTP protocol (default)"
    echo "  --help                Show this help message"
    echo ""
    echo "Examples:"
    echo "  # Test local development"
    echo "  $0"
    echo ""
    echo "  # Test production deployment"
    echo "  $0 --domain yourdomain.com --api-domain api.yourdomain.com --https"
    echo ""
    echo "  # Test staging environment"
    echo "  $0 --domain staging.yourdomain.com --api-domain api-staging.yourdomain.com --https"
}

# Parse command line arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        --domain)
            DOMAIN="$2"
            FRONTEND_URL="${PROTOCOL}://${DOMAIN}"
            shift 2
            ;;
        --api-domain)
            API_DOMAIN="$2"
            API_URL="${PROTOCOL}://${API_DOMAIN}"
            shift 2
            ;;
        --https)
            PROTOCOL="https"
            FRONTEND_URL="${PROTOCOL}://${DOMAIN}"
            API_URL="${PROTOCOL}://${API_DOMAIN}"
            shift
            ;;
        --http)
            PROTOCOL="http"
            FRONTEND_URL="${PROTOCOL}://${DOMAIN}"
            API_URL="${PROTOCOL}://${API_DOMAIN}"
            shift
            ;;
        --help)
            show_help
            exit 0
            ;;
        *)
            log_error "Unknown option: $1"
            echo ""
            show_help
            exit 1
            ;;
    esac
done

# Update URLs after parsing all arguments
FRONTEND_URL="${PROTOCOL}://${DOMAIN}"
API_URL="${PROTOCOL}://${API_DOMAIN}"

# Run main function
main