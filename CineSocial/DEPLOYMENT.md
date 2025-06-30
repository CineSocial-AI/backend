# CineSocial Deployment Guide

## 🚀 Overview

This guide provides comprehensive instructions for deploying CineSocial, a full-stack movie social platform, using Docker containers with production-ready best practices.

## 📋 Prerequisites

### System Requirements
- **Docker** 20.10+ and **Docker Compose** 2.0+
- **Linux/Ubuntu** 18.04+ (recommended) or **macOS/Windows** with Docker Desktop
- **Minimum**: 2 GB RAM, 10 GB disk space
- **Recommended**: 4 GB RAM, 20 GB disk space

### Domain Requirements (Production)
- Domain name (e.g., `yourdomain.com`)
- API subdomain (e.g., `api.yourdomain.com`)
- Valid email for SSL certificates

## 🏗️ Architecture

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   Nginx Proxy   │    │   React Frontend │    │   .NET Backend  │
│   (Port 80/443) │────│   (Port 3000)    │────│   (Port 5000)   │
└─────────────────┘    └─────────────────┘    └─────────────────┘
         │                                               │
         └─────────────────────┬─────────────────────────┘
                               │
                    ┌─────────────────┐
                    │  PostgreSQL DB  │
                    │   (Port 5432)   │
                    └─────────────────┘
```

## 🛠️ Quick Start (Development)

### 1. Clone and Setup
```bash
git clone <repository-url>
cd CineSocial

# Make scripts executable
chmod +x scripts/*.sh
```

### 2. Start Development Environment
```bash
# Start all services
./scripts/dev.sh start

# View logs
./scripts/dev.sh logs

# Stop services
./scripts/dev.sh stop
```

### 3. Access Application
- **Frontend**: http://localhost:3000
- **Backend API**: http://localhost:5000
- **Health Check**: http://localhost:5000/health

## 🌐 Production Deployment

### 1. Server Preparation

```bash
# Update system
sudo apt update && sudo apt upgrade -y

# Install Docker
curl -fsSL https://get.docker.com -o get-docker.sh
sudo sh get-docker.sh

# Install Docker Compose
sudo curl -L "https://github.com/docker/compose/releases/download/v2.21.0/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
sudo chmod +x /usr/local/bin/docker-compose

# Create deployment user
sudo useradd -m -s /bin/bash deploy
sudo usermod -aG docker deploy
```

### 2. Environment Configuration

```bash
# Copy environment template
cp .env.docker .env.production

# Edit configuration
nano .env.production
```

**Required Environment Variables:**
```env
# Database
POSTGRES_DB=cinesocial
POSTGRES_USER=cinesocial_user
POSTGRES_PASSWORD=your-secure-database-password

# Backend
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection=Host=database;Port=5432;Database=cinesocial;Username=cinesocial_user;Password=your-secure-database-password;

# JWT (Generate with: openssl rand -base64 64)
JwtSettings__SecretKey=your-super-secret-jwt-key-that-is-at-least-32-characters-long-and-very-secure

# CORS
CorsSettings__AllowedOrigins=https://yourdomain.com,https://www.yourdomain.com

# Frontend
REACT_APP_API_URL=https://api.yourdomain.com/api

# SSL
SSL_CERTIFICATE_EMAIL=your-email@domain.com
SSL_DOMAIN=yourdomain.com
SSL_API_DOMAIN=api.yourdomain.com
```

### 3. DNS Configuration

Configure your DNS provider:
```
A    yourdomain.com        → YOUR_SERVER_IP
A    api.yourdomain.com    → YOUR_SERVER_IP
AAAA yourdomain.com        → YOUR_SERVER_IPV6 (if available)
AAAA api.yourdomain.com    → YOUR_SERVER_IPV6 (if available)
```

### 4. Deploy

```bash
# Deploy with SSL
./scripts/deploy.sh --domain yourdomain.com --api-domain api.yourdomain.com --email your-email@domain.com

# Deploy without SSL (for testing)
./scripts/deploy.sh --no-ssl
```

## 🔧 Configuration Details

### Docker Compose Files

#### Development (`docker-compose.yml`)
- SQLite database for simplicity
- Direct port exposure (3000, 5000)
- Development environment settings
- Hot reload support

#### Production (`docker-compose.prod.yml`)
- PostgreSQL database with persistent volumes
- Nginx reverse proxy with SSL termination
- Health checks and restart policies
- Resource limits and scaling
- Secrets management

### Security Features

#### Network Security
- **Rate Limiting**: API endpoints protected against abuse
- **CORS**: Configured for specific domains only
- **Security Headers**: HSTS, CSP, X-Frame-Options, etc.
- **SSL/TLS**: Automatic Let's Encrypt certificates

#### Application Security
- **JWT Authentication**: Secure token-based auth
- **Password Hashing**: BCrypt with salt
- **Input Validation**: FluentValidation throughout
- **SQL Injection Protection**: Entity Framework with parameterized queries

#### Container Security
- **Non-root Users**: All containers run as non-root
- **Multi-stage Builds**: Minimal production images
- **Secrets Management**: Docker secrets for sensitive data
- **Health Checks**: Automated container health monitoring

## 📊 Monitoring & Maintenance

### Health Checks

```bash
# Check all services
curl -f http://yourdomain.com/health
curl -f https://api.yourdomain.com/health

# Check individual services
docker-compose -f docker-compose.prod.yml ps
```

### Logs

```bash
# View all logs
docker-compose -f docker-compose.prod.yml logs -f

# View specific service logs
docker-compose -f docker-compose.prod.yml logs -f backend
docker-compose -f docker-compose.prod.yml logs -f frontend
```

### Database Backup

```bash
# Create backup
docker-compose -f docker-compose.prod.yml exec database pg_dump -U cinesocial_user cinesocial > backup_$(date +%Y%m%d_%H%M%S).sql

# Restore backup
docker-compose -f docker-compose.prod.yml exec -T database psql -U cinesocial_user cinesocial < backup_20231201_120000.sql
```

### SSL Certificate Renewal

Certificates auto-renew via cron job:
```bash
# Check cron job
crontab -l

# Manual renewal
docker-compose -f docker-compose.prod.yml --profile ssl run --rm certbot renew
```

## 🔄 Updates & Scaling

### Application Updates

```bash
# Pull latest code
git pull origin main

# Rebuild and redeploy
docker-compose -f docker-compose.prod.yml build --no-cache
docker-compose -f docker-compose.prod.yml up -d

# Run migrations if needed
docker-compose -f docker-compose.prod.yml exec backend dotnet ef database update
```

### Horizontal Scaling

```bash
# Scale backend (2 instances)
docker-compose -f docker-compose.prod.yml up -d --scale backend=2

# Scale frontend (2 instances)
docker-compose -f docker-compose.prod.yml up -d --scale frontend=2
```

## 🚨 Troubleshooting

### Common Issues

#### 1. Database Connection Issues
```bash
# Check database status
docker-compose -f docker-compose.prod.yml exec database pg_isready -U cinesocial_user

# Reset database (⚠️ DESTRUCTIVE)
docker-compose -f docker-compose.prod.yml down -v
docker-compose -f docker-compose.prod.yml up -d database
```

#### 2. SSL Certificate Issues
```bash
# Check certificate status
docker-compose -f docker-compose.prod.yml logs certbot

# Force certificate renewal
docker-compose -f docker-compose.prod.yml --profile ssl run --rm certbot certonly --force-renewal
```

#### 3. Port Conflicts
```bash
# Check port usage
sudo netstat -tulpn | grep :80
sudo netstat -tulpn | grep :443

# Stop conflicting services
sudo systemctl stop apache2  # if Apache is running
sudo systemctl stop nginx    # if Nginx is running
```

### Performance Optimization

#### 1. Resource Limits
Edit `docker-compose.prod.yml`:
```yaml
deploy:
  resources:
    limits:
      cpus: '1.0'
      memory: 1G
    reservations:
      cpus: '0.5'
      memory: 512M
```

#### 2. Database Optimization
```sql
-- Connect to database
docker-compose -f docker-compose.prod.yml exec database psql -U cinesocial_user cinesocial

-- Check database size
SELECT pg_size_pretty(pg_database_size('cinesocial'));

-- Vacuum and analyze
VACUUM ANALYZE;
```

## 📁 File Structure

```
CineSocial/
├── Dockerfile                     # Backend Docker configuration
├── docker-compose.yml            # Development environment
├── docker-compose.prod.yml       # Production environment
├── .dockerignore                 # Docker ignore patterns
├── .env.docker                   # Environment template
├── nginx/
│   └── nginx.conf                # Production Nginx config
├── scripts/
│   ├── deploy.sh                 # Production deployment script
│   └── dev.sh                    # Development management script
├── secrets/                      # Docker secrets (git-ignored)
│   ├── db_password.txt
│   └── jwt_secret.txt
├── frontend/
│   ├── Dockerfile                # Frontend Docker configuration
│   ├── nginx.conf                # Frontend Nginx config
│   └── .dockerignore            # Frontend Docker ignore
└── [application source code]
```

## 🔗 Useful Commands

### Development
```bash
./scripts/dev.sh start           # Start development environment
./scripts/dev.sh stop            # Stop development environment
./scripts/dev.sh logs            # View all logs
./scripts/dev.sh logs backend    # View backend logs
./scripts/dev.sh test            # Run tests
./scripts/dev.sh build           # Build images
./scripts/dev.sh clean           # Clean up
./scripts/dev.sh shell backend   # Open backend shell
```

### Production
```bash
./scripts/deploy.sh              # Deploy production
docker-compose -f docker-compose.prod.yml ps                     # Check status
docker-compose -f docker-compose.prod.yml logs -f               # View logs
docker-compose -f docker-compose.prod.yml restart backend       # Restart service
docker-compose -f docker-compose.prod.yml down                  # Stop all
```

## 🆘 Support

For issues and questions:
1. Check the troubleshooting section above
2. Review logs: `docker-compose -f docker-compose.prod.yml logs -f`
3. Check health endpoints: `/health`
4. Verify environment configuration
5. Ensure DNS is properly configured

## 📝 Security Checklist

- [ ] Strong database passwords generated
- [ ] JWT secret keys are secure (64+ characters)
- [ ] CORS configured for production domains only
- [ ] SSL certificates installed and auto-renewing
- [ ] Security headers configured
- [ ] Rate limiting enabled
- [ ] Containers running as non-root users
- [ ] Secrets stored securely (not in environment files)
- [ ] Regular backups scheduled
- [ ] Monitoring and logging enabled