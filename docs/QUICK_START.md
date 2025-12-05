# Quick Start Guide

## Prerequisites Check
```bash
# Check .NET version (should be 10.0 or higher)
dotnet --version

# Check Docker
docker --version
docker-compose --version
```

## Step 1: Start Infrastructure Services

```bash
# Navigate to project root
cd d:\Calismalarim\ModernBaseProject

# Start all infrastructure services
docker-compose up -d

# Verify services are running
docker-compose ps
```

Expected services:
- ✅ modernbase-postgres (port 5435)
- ✅ modernbase-redis (port 6379)
- ✅ modernbase-rabbitmq (ports 5672, 15672)
- ✅ modernbase-seq (port 5341)
- ✅ modernbase-maildev (ports 1080, 1025)

## Step 2: Run the API

```bash
# Navigate to API project
cd src\ModernBaseProject.API

# Run the application
dotnet run
```

The API will:
1. Start on http://localhost:5000
2. Create database if not exists
3. Run migrations
4. Seed initial data (admin user, roles, permissions)

## Step 3: Test the API

### Option A: Using Swagger UI
1. Open browser: http://localhost:5000/swagger
2. Try the `/api/auth/login` endpoint
3. Use credentials:
   - Email: `admin@domain.com`
   - Password: `Admin123!`
4. Copy the `accessToken` from response
5. Click "Authorize" button in Swagger
6. Enter: `Bearer {your-access-token}`
7. Try the `/api/users` endpoint

### Option B: Using curl

```bash
# Login
curl -X POST http://localhost:5000/api/auth/login ^
  -H "Content-Type: application/json" ^
  -d "{\"email\":\"admin@domain.com\",\"password\":\"Admin123!\"}"

# Copy the accessToken from response, then:

# Get Users (replace YOUR_TOKEN with actual token)
curl -X GET http://localhost:5000/api/users ^
  -H "Authorization: Bearer YOUR_TOKEN"
```

### Option C: Using PowerShell

```powershell
# Login
$loginResponse = Invoke-RestMethod -Uri "http://localhost:5000/api/auth/login" `
  -Method Post `
  -ContentType "application/json" `
  -Body '{"email":"admin@domain.com","password":"Admin123!"}'

$token = $loginResponse.accessToken
Write-Host "Token: $token"

# Get Users
$headers = @{
    "Authorization" = "Bearer $token"
}

$users = Invoke-RestMethod -Uri "http://localhost:5000/api/users" `
  -Method Get `
  -Headers $headers

$users | ConvertTo-Json
```

## Step 4: Monitor Logs

Open Seq dashboard: http://localhost:5341

You should see:
- Application startup logs
- Database seeding logs
- Authentication logs
- Request/response logs

## Step 5: Check RabbitMQ (Optional)

Open RabbitMQ Management: http://localhost:15672
- Username: `guest`
- Password: `guest`

## Troubleshooting

### Database Connection Issues
```bash
# Check if PostgreSQL is running
docker ps | findstr postgres

# Check PostgreSQL logs
docker logs modernbase-postgres

# Restart PostgreSQL
docker-compose restart postgres
```

### Port Already in Use
```bash
# Check what's using port 5000
netstat -ano | findstr :5000

# Kill the process (replace PID with actual process ID)
taskkill /PID <PID> /F

# Or change the port in launchSettings.json
```

### Build Errors
```bash
# Clean and rebuild
cd src
dotnet clean
dotnet build
```

### Database Reset
```bash
# Stop API first, then:
docker-compose down -v
docker-compose up -d

# Run API again - it will recreate and seed the database
cd src\ModernBaseProject.API
dotnet run
```

## Expected Results

### Successful Login Response:
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "base64-encoded-string",
  "email": "admin@domain.com",
  "username": "admin"
}
```

### Successful Get Users Response:
```json
[
  {
    "id": "guid-here",
    "username": "admin",
    "email": "admin@domain.com",
    "isActive": true
  }
]
```

## Next Steps

1. ✅ API is running
2. ✅ Authentication works
3. ✅ Database is seeded
4. 🔄 Ready to implement frontend
5. 🔄 Ready to add more features

## Useful Commands

```bash
# Stop all services
docker-compose down

# View API logs
cd src\ModernBaseProject.API
dotnet run --verbosity detailed

# Check database
docker exec -it modernbase-postgres psql -U postgres -d modernbase
# Then: \dt (list tables), \q (quit)

# View Seq logs in real-time
# Just keep http://localhost:5341 open in browser
```

## Support

If you encounter issues:
1. Check logs in Seq (http://localhost:5341)
2. Check docker-compose logs: `docker-compose logs`
3. Verify all services are running: `docker-compose ps`
4. Check the PROJECT_STATUS.md for implementation details
