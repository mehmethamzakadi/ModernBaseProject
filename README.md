# Modern Base Project

Enterprise-grade boilerplate application with .NET 10 backend and React TypeScript frontend, following Vertical Slice Architecture principles.

## 🚀 Quick Start

### Prerequisites
- .NET 10 SDK
- Docker & Docker Compose
- Node.js 18+ (for frontend)
- PostgreSQL (or use Docker)

### Running with Docker

1. Start infrastructure services:
```bash
docker-compose up -d
```

This will start:
- PostgreSQL (port 5435)
- Redis (port 6379)
- RabbitMQ (port 5672, Management UI: 15672)
- Seq (port 5341)
- MailDev (port 1080)

2. Run the API:
```bash
cd src/ModernBaseProject.API
dotnet run
```

The API will be available at `http://localhost:5000`

### Default Admin Credentials
- Email: `admin@domain.com`
- Password: `Admin123!`

## 📁 Project Structure

```
src/
├── ModernBaseProject.Core/          # Domain entities, interfaces, exceptions
├── ModernBaseProject.Infrastructure/ # EF Core, Authentication, File Storage
└── ModernBaseProject.API/           # Minimal API with Vertical Slices
    └── Features/
        └── Users/
            ├── Login/
            ├── CreateUser/
            └── GetUsersList/
```

## 🔑 Key Features

### Backend
- ✅ Vertical Slice Architecture
- ✅ JWT Authentication with Refresh Tokens
- ✅ Role-Permission Based Authorization
- ✅ Direct DbContext (No Generic Repository)
- ✅ LINQ Projections for Performance
- ✅ FluentValidation Pipeline
- ✅ Global Exception Handling
- ✅ File Upload (Local Storage)
- ✅ SignalR for Real-time Notifications
- ✅ Serilog + Seq Logging

### API Endpoints

#### Authentication
- `POST /api/auth/login` - Login with email/password

#### Authentication
- `POST /api/auth/refresh` - Refresh access token

#### Users (All require authentication + permissions)
- `GET /api/users` - Get all users (requires User.Read)
- `GET /api/users/{id}` - Get user by ID (requires User.Read)
- `POST /api/users` - Create new user (requires User.Create)
- `PUT /api/users/{id}` - Update user (requires User.Update)
- `DELETE /api/users/{id}` - Delete user (requires User.Delete)

#### Files (Require authentication)
- `POST /api/files/upload` - Upload file

## 🛠️ Technology Stack

### Backend
- .NET 10
- Entity Framework Core 10
- PostgreSQL
- FluentValidation
- MediatR
- JWT Bearer Authentication
- SignalR
- Serilog

### Infrastructure
- Docker & Docker Compose
- PostgreSQL 16
- Redis 7
- RabbitMQ 3
- Seq (Logging)
- MailDev (Email Testing)

## 📝 Database Seeding

On first run, the application automatically seeds:
- Default permissions (User.Create, User.Read, User.Update, User.Delete)
- SuperAdmin role with all permissions
- Admin user (admin@domain.com / Admin123!)

## 🔐 Security

- Passwords hashed with BCrypt
- JWT tokens with 30-minute expiry
- Refresh tokens with 7-day expiry
- Role-based permission system

## 📊 Monitoring

- **Seq Dashboard**: http://localhost:5341
- **RabbitMQ Management**: http://localhost:15672 (guest/guest)
- **MailDev**: http://localhost:1080

## 🧪 Testing

```bash
# Run tests
dotnet test
```

## 📖 API Documentation

Swagger UI is available at: `http://localhost:5000/swagger`

## 🎯 Next Steps

1. ✅ User CRUD operations completed
2. ✅ Refresh token endpoint completed
3. ✅ Permission-based authorization completed
4. ✅ File upload endpoint completed
5. Add frontend React application
6. Configure RabbitMQ + MassTransit
7. Add Redis caching

## 📚 Documentation

- [Quick Start Guide](QUICK_START.md)
- [API Testing Guide](API_TESTING.md)
- [Project Status](PROJECT_STATUS.md)
- [Architecture Analysis](docs/ARCHITECTURE_ANALYSIS.md)

## 📄 License

MIT License
