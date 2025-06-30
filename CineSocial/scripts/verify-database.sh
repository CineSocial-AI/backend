#!/bin/bash

# CineSocial Database Verification Script
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

# Function to test database connection
test_db_connection() {
    log_debug "Attempting to connect to database..."
    
    if docker-compose exec -T database pg_isready -U cinesocial_user -d cinesocial > /dev/null 2>&1; then
        log_debug "Database connection successful"
        return 0
    else
        log_error "Cannot connect to database"
        return 1
    fi
}

# Function to test database schema
test_db_schema() {
    log_debug "Checking database schema..."
    
    local tables=$(docker-compose exec -T database psql -U cinesocial_user -d cinesocial -t -c "
        SELECT table_name 
        FROM information_schema.tables 
        WHERE table_schema = 'public'
        ORDER BY table_name;
    " 2>/dev/null | tr -d ' ' | grep -v '^$')
    
    local expected_tables="AspNetRoles
AspNetRoleClaims
AspNetUsers
AspNetUserClaims
AspNetUserLogins
AspNetUserRoles
AspNetUserTokens
Genres
Movies
MovieGenres
Reviews
WatchLists
__EFMigrationsHistory"
    
    log_debug "Found tables: $tables"
    
    # Check if main tables exist
    if echo "$tables" | grep -q "AspNetUsers" && 
       echo "$tables" | grep -q "Movies" && 
       echo "$tables" | grep -q "Genres"; then
        return 0
    else
        log_error "Required tables not found"
        return 1
    fi
}

# Function to test database operations
test_db_operations() {
    log_debug "Testing database CRUD operations..."
    
    # Test INSERT
    local test_result=$(docker-compose exec -T database psql -U cinesocial_user -d cinesocial -t -c "
        INSERT INTO Genres (Id, Name, CreatedAt, UpdatedAt) 
        VALUES (gen_random_uuid(), 'Test Genre', NOW(), NOW()) 
        RETURNING Id;
    " 2>/dev/null | tr -d ' ' | head -1)
    
    if [[ -z "$test_result" ]]; then
        log_error "INSERT operation failed"
        return 1
    fi
    
    local genre_id="$test_result"
    log_debug "Created test genre with ID: $genre_id"
    
    # Test SELECT
    local select_result=$(docker-compose exec -T database psql -U cinesocial_user -d cinesocial -t -c "
        SELECT Name FROM Genres WHERE Id = '$genre_id';
    " 2>/dev/null | tr -d ' ')
    
    if [[ "$select_result" != "TestGenre" ]]; then
        log_error "SELECT operation failed"
        return 1
    fi
    
    # Test UPDATE
    docker-compose exec -T database psql -U cinesocial_user -d cinesocial -c "
        UPDATE Genres SET Name = 'Updated Test Genre' WHERE Id = '$genre_id';
    " > /dev/null 2>&1
    
    local update_result=$(docker-compose exec -T database psql -U cinesocial_user -d cinesocial -t -c "
        SELECT Name FROM Genres WHERE Id = '$genre_id';
    " 2>/dev/null | tr -d ' ')
    
    if [[ "$update_result" != "UpdatedTestGenre" ]]; then
        log_error "UPDATE operation failed"
        return 1
    fi
    
    # Test DELETE
    docker-compose exec -T database psql -U cinesocial_user -d cinesocial -c "
        DELETE FROM Genres WHERE Id = '$genre_id';
    " > /dev/null 2>&1
    
    local delete_result=$(docker-compose exec -T database psql -U cinesocial_user -d cinesocial -t -c "
        SELECT COUNT(*) FROM Genres WHERE Id = '$genre_id';
    " 2>/dev/null | tr -d ' ')
    
    if [[ "$delete_result" != "0" ]]; then
        log_error "DELETE operation failed"
        return 1
    fi
    
    log_debug "All CRUD operations successful"
    return 0
}

# Function to test database performance
test_db_performance() {
    log_debug "Testing database performance..."
    
    # Test query execution time
    local start_time=$(date +%s%N)
    
    docker-compose exec -T database psql -U cinesocial_user -d cinesocial -c "
        SELECT COUNT(*) FROM AspNetUsers;
        SELECT COUNT(*) FROM Movies;
        SELECT COUNT(*) FROM Genres;
    " > /dev/null 2>&1
    
    local end_time=$(date +%s%N)
    local duration=$(( (end_time - start_time) / 1000000 )) # Convert to milliseconds
    
    log_debug "Query execution time: ${duration}ms"
    
    # Performance should be under 1000ms for basic queries
    if [[ $duration -lt 1000 ]]; then
        return 0
    else
        log_warn "Database queries slower than expected (${duration}ms)"
        return 1
    fi
}

# Function to test database backup capability
test_db_backup() {
    log_debug "Testing database backup functionality..."
    
    local backup_file="/tmp/cinesocial_test_backup_$(date +%Y%m%d_%H%M%S).sql"
    
    # Create backup
    if docker-compose exec -T database pg_dump -U cinesocial_user cinesocial > "$backup_file" 2>/dev/null; then
        log_debug "Backup created successfully: $backup_file"
        
        # Check if backup file is not empty and contains expected content
        if [[ -s "$backup_file" ]] && grep -q "PostgreSQL database dump" "$backup_file"; then
            # Clean up test backup
            rm -f "$backup_file"
            return 0
        else
            log_error "Backup file is empty or invalid"
            rm -f "$backup_file"
            return 1
        fi
    else
        log_error "Failed to create database backup"
        return 1
    fi
}

# Function to test database security
test_db_security() {
    log_debug "Testing database security configuration..."
    
    # Test that database doesn't accept connections with wrong credentials
    if docker-compose exec -T database psql -U wronguser -d cinesocial -c "SELECT 1;" > /dev/null 2>&1; then
        log_error "Database accepts connections with wrong credentials"
        return 1
    else
        log_debug "Database properly rejects wrong credentials"
    fi
    
    # Test that database user has appropriate permissions
    local can_create_table=$(docker-compose exec -T database psql -U cinesocial_user -d cinesocial -t -c "
        CREATE TEMP TABLE test_permissions (id SERIAL PRIMARY KEY);
        DROP TABLE test_permissions;
        SELECT 'success';
    " 2>/dev/null | tr -d ' ' | tail -1)
    
    if [[ "$can_create_table" == "success" ]]; then
        log_debug "Database user has appropriate permissions"
        return 0
    else
        log_error "Database user lacks necessary permissions"
        return 1
    fi
}

# Function to test database migrations
test_db_migrations() {
    log_debug "Testing database migrations..."
    
    # Check if migrations history table exists and has entries
    local migration_count=$(docker-compose exec -T database psql -U cinesocial_user -d cinesocial -t -c "
        SELECT COUNT(*) FROM __EFMigrationsHistory;
    " 2>/dev/null | tr -d ' ')
    
    if [[ "$migration_count" -gt 0 ]]; then
        log_debug "Database migrations applied successfully ($migration_count migrations)"
        return 0
    else
        log_error "No database migrations found"
        return 1
    fi
}

# Function to show database statistics
show_db_stats() {
    log_info "📊 Database Statistics:"
    
    echo ""
    log_debug "Database size:"
    docker-compose exec -T database psql -U cinesocial_user -d cinesocial -c "
        SELECT pg_size_pretty(pg_database_size('cinesocial')) AS database_size;
    " 2>/dev/null
    
    echo ""
    log_debug "Table row counts:"
    docker-compose exec -T database psql -U cinesocial_user -d cinesocial -c "
        SELECT 
            schemaname,
            tablename,
            n_tup_ins AS inserts,
            n_tup_upd AS updates,
            n_tup_del AS deletes,
            n_live_tup AS live_rows
        FROM pg_stat_user_tables
        WHERE schemaname = 'public'
        ORDER BY tablename;
    " 2>/dev/null
    
    echo ""
    log_debug "Connection information:"
    docker-compose exec -T database psql -U cinesocial_user -d cinesocial -c "
        SELECT 
            current_database() AS database,
            current_user AS user,
            version() AS postgresql_version;
    " 2>/dev/null
}

# Main verification function
main() {
    log_info "🗃️  CineSocial Database Verification"
    echo ""
    
    # Check if Docker Compose is available
    if ! command -v docker-compose &> /dev/null; then
        log_error "Docker Compose not found. Please install Docker Compose first."
        exit 1
    fi
    
    # Check if database container is running
    if ! docker-compose ps database | grep -q "Up"; then
        log_error "Database container is not running. Please start it first:"
        log_info "  docker-compose up -d database"
        exit 1
    fi
    
    # Wait for database to be ready
    log_info "Waiting for database to be ready..."
    sleep 5
    
    # Run all tests
    run_test "Database Connection" "test_db_connection"
    run_test "Database Schema" "test_db_schema"
    run_test "Database CRUD Operations" "test_db_operations"
    run_test "Database Performance" "test_db_performance"
    run_test "Database Backup" "test_db_backup"
    run_test "Database Security" "test_db_security"
    run_test "Database Migrations" "test_db_migrations"
    
    # Show database statistics
    echo ""
    show_db_stats
    
    # Show test results
    echo ""
    log_info "📋 Test Results Summary:"
    echo "  Total tests run: $TOTAL_TESTS"
    echo "  Tests passed: $TESTS_PASSED"
    echo "  Tests failed: $TESTS_FAILED"
    
    if [[ $TESTS_FAILED -eq 0 ]]; then
        echo ""
        log_info "🎉 ALL DATABASE TESTS PASSED!"
        log_info "✅ Database is working correctly and ready for production."
        exit 0
    else
        echo ""
        log_error "❌ SOME DATABASE TESTS FAILED!"
        log_error "Please fix the issues before deploying to production."
        exit 1
    fi
}

# Handle command line arguments
case "${1:-verify}" in
    verify)
        main
        ;;
    connection)
        run_test "Database Connection Only" "test_db_connection"
        ;;
    schema)
        run_test "Database Schema Only" "test_db_schema"
        ;;
    operations)
        run_test "Database Operations Only" "test_db_operations"
        ;;
    performance)
        run_test "Database Performance Only" "test_db_performance"
        ;;
    backup)
        run_test "Database Backup Only" "test_db_backup"
        ;;
    security)
        run_test "Database Security Only" "test_db_security"
        ;;
    migrations)
        run_test "Database Migrations Only" "test_db_migrations"
        ;;
    stats)
        show_db_stats
        ;;
    help|--help|-h)
        echo "CineSocial Database Verification Script"
        echo ""
        echo "Usage: $0 [COMMAND]"
        echo ""
        echo "Commands:"
        echo "  verify      Run all database tests (default)"
        echo "  connection  Test database connection only"
        echo "  schema      Test database schema only"
        echo "  operations  Test CRUD operations only"
        echo "  performance Test database performance only"
        echo "  backup      Test backup functionality only"
        echo "  security    Test security configuration only"
        echo "  migrations  Test database migrations only"
        echo "  stats       Show database statistics only"
        echo "  help        Show this help message"
        ;;
    *)
        log_error "Unknown command: $1"
        echo ""
        echo "Use '$0 help' for usage information."
        exit 1
        ;;
esac