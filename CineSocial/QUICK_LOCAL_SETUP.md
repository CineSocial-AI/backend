# 🚀 CineSocial - Quick Local Setup

## **Fastest Way to Run Locally (5 minutes)**

### **Prerequisites** (Install these first)
- **Docker Desktop** - [Download here](https://www.docker.com/products/docker-desktop/)
- **Git** - [Download here](https://git-scm.com/downloads)

### **One-Command Setup**

1. **Clone and start everything:**
```bash
git clone <your-repo-url>
cd CineSocial
./scripts/dev.sh start
```

That's it! 🎉

### **Access Your Application**
- **🌐 Frontend**: http://localhost:3000
- **🚀 Backend API**: http://localhost:5000
- **📊 Health Check**: http://localhost:5000/health

---

## **Alternative: Manual Docker Commands**

If you prefer manual control:

```bash
# Start all services
docker-compose up -d

# View logs
docker-compose logs -f

# Stop everything
docker-compose down
```

---

## **No Docker? Direct Development**

### **Backend (.NET)**
```bash
cd CineSocial.Adapters.WebAPI
dotnet run
```
**Runs on**: http://localhost:5000

### **Frontend (React)**
```bash
cd frontend
npm install
npm start
```
**Runs on**: http://localhost:3000

---

## **Troubleshooting**

### **Common Issues**

#### 1. **Port Already in Use**
```bash
# Stop any existing services
docker-compose down
sudo lsof -i :3000 -i :5000
# Kill any processes using these ports
```

#### 2. **Docker Not Running**
- Start Docker Desktop
- Wait for it to fully load
- Try again

#### 3. **Database Issues**
```bash
# Reset database
docker-compose down -v
docker-compose up -d
```

#### 4. **Frontend Won't Load**
```bash
# Rebuild frontend
docker-compose build frontend
docker-compose up -d frontend
```

---

## **Development Commands**

```bash
# Start development environment
./scripts/dev.sh start

# View all logs
./scripts/dev.sh logs

# View specific service logs
./scripts/dev.sh logs backend
./scripts/dev.sh logs frontend

# Stop everything
./scripts/dev.sh stop

# Restart everything
./scripts/dev.sh restart

# Run tests
./scripts/dev.sh test

# Clean up everything
./scripts/dev.sh clean

# Open backend shell
./scripts/dev.sh shell backend

# Show help
./scripts/dev.sh help
```

---

## **What's Included**

✅ **Backend**: .NET 8 API with Entity Framework  
✅ **Frontend**: React 18 with TypeScript  
✅ **Database**: SQLite (automatically created)  
✅ **Authentication**: JWT-based user system  
✅ **API Documentation**: Swagger UI  
✅ **Hot Reload**: Automatic code refresh  

---

## **Default Test Data**

The application starts with:
- **Movies**: Sample movie data
- **Genres**: Action, Comedy, Drama, etc.
- **Admin User**: Create one through the registration

---

## **API Endpoints**

- **Swagger UI**: http://localhost:5000/swagger
- **Health**: http://localhost:5000/health
- **Movies**: http://localhost:5000/api/movies
- **Genres**: http://localhost:5000/api/genres
- **Auth**: http://localhost:5000/api/auth/register

---

## **Environment Variables** (Optional)

Create `.env` file in root directory:
```env
# Database (SQLite by default)
ConnectionStrings__DefaultConnection=Data Source=./Data/cinesocial.db

# JWT Settings
JwtSettings__SecretKey=your-development-secret-key-here
JwtSettings__Issuer=CineSocial
JwtSettings__Audience=CineSocial-Users

# CORS (for frontend)
CorsSettings__AllowedOrigins=http://localhost:3000
```

---

## **Need Help?**

1. **Check logs**: `./scripts/dev.sh logs`
2. **Restart services**: `./scripts/dev.sh restart`
3. **Clean everything**: `./scripts/dev.sh clean`
4. **Verify setup**: `./scripts/verify-deployment.sh`

---

## **Performance Tips**

- **Use Docker**: Faster and more consistent
- **Close other apps**: Free up memory
- **Use SSD**: Better performance
- **Allocate more RAM to Docker**: 4GB recommended

---

**🎬 You're ready to develop! Happy coding! 🚀**