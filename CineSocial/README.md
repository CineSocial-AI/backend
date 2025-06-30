# 🎬 CineSocial - Movie Social Platform

A modern full-stack movie social platform built with .NET 8 and React TypeScript.

## 🚀 **Quick Start (30 seconds)**

### **Prerequisites**
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (Required)

### **Run Locally**

**Option 1: Super Simple (recommended)**
```bash
# Mac/Linux
./start-local.sh

# Windows
start-local.bat
```

**Option 2: Manual Commands**
```bash
docker-compose up -d
```

### **Access Application**
- **🌐 Frontend**: http://localhost:3000
- **🚀 Backend API**: http://localhost:5000
- **📖 API Documentation**: http://localhost:5000/swagger

---

## 🛠️ **Development Commands**

```bash
# Start everything
./scripts/dev.sh start

# View logs
./scripts/dev.sh logs

# Stop everything
./scripts/dev.sh stop

# Clean up
./scripts/dev.sh clean
```

---

## 🏗️ **Tech Stack**

- **Backend**: .NET 8, Entity Framework, SQLite
- **Frontend**: React 18, TypeScript, Tailwind CSS
- **Database**: SQLite (dev), PostgreSQL (prod)
- **Auth**: JWT tokens
- **Containerization**: Docker & Docker Compose

---

## 📚 **Features**

✅ User authentication and registration  
✅ Movie browsing and search  
✅ Movie reviews and ratings  
✅ Watchlist management  
✅ Genre filtering  
✅ Responsive design  
✅ RESTful API  
✅ Swagger documentation  

---

## 🚀 **Production Deployment**

See [DEPLOYMENT.md](DEPLOYMENT.md) for production deployment instructions.

---

## 📞 **Need Help?**

1. **Check logs**: `docker-compose logs -f`
2. **Restart**: `docker-compose restart`
3. **Clean slate**: `docker-compose down -v && docker-compose up -d`

---

**🎬 Happy coding! 🚀**