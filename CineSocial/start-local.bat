@echo off
echo 🎬 Starting CineSocial locally...

REM Check if Docker is running
docker info >nul 2>&1
if %errorlevel% neq 0 (
    echo ❌ Docker is not running!
    echo Please start Docker Desktop and try again.
    pause
    exit /b 1
)

REM Check if Docker Compose is available
docker-compose --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ❌ Docker Compose not found!
    echo Please install Docker Desktop (includes Docker Compose).
    pause
    exit /b 1
)

echo 🔧 Starting services...
docker-compose up -d

echo ⏳ Waiting for services to start...
timeout /t 10 /nobreak >nul

REM Check if services are running
docker-compose ps | findstr "Up" >nul
if %errorlevel% equ 0 (
    echo.
    echo 🎉 CineSocial is running!
    echo.
    echo 📱 Frontend: http://localhost:3000
    echo 🚀 Backend:  http://localhost:5000
    echo 📊 Health:   http://localhost:5000/health
    echo 📖 API Docs: http://localhost:5000/swagger
    echo.
    echo Commands:
    echo   Stop:      docker-compose down
    echo   Logs:      docker-compose logs -f
    echo   Restart:   docker-compose restart
    echo.
    echo Happy coding! 🚀
    echo.
    echo Opening frontend in browser...
    start http://localhost:3000
) else (
    echo ❌ Failed to start services
    echo Check logs with: docker-compose logs
)

pause