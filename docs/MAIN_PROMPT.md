# Modern Enterprise Base Project - Complete Implementation Prompt

## Project Overview

You will build a **modern, enterprise-grade boilerplate application** with a .NET 10 backend and React TypeScript frontend. This is a foundational template following **Vertical Slice Architecture** principles, designed for high performance, modularity, and distributed systems.

---

## 🎯 Core Requirements

### Technology Stack

**Backend:**

- .NET 10 (latest) with Minimal API
- C# 13/14 features
- PostgreSQL with JSONB support
- Entity Framework Core (Code First, no migrations initially)
- FastEndpoints or Minimal API + MediatR for REPR pattern
- FluentValidation for request validation
- JWT-based authentication with refresh tokens

**Frontend:**

- React 18+ with TypeScript
- Vite as build tool
- Shadcn/UI for components
- TanStack Query (React Query) for server state
- Zustand for client state
- Axios for HTTP requests

**Infrastructure:**

- RabbitMQ + MassTransit for messaging (with Outbox Pattern)
- Redis for distributed caching
- SignalR for real-time notifications
- Serilog + Seq + OpenTelemetry for logging/monitoring
- Docker Compose for container orchestration

---

## 📁 Backend Architecture

### Project Structure

Create a solution with the following structure:

```
src/
├── ModernBaseProject.Core/
│   ├── Domain/
│   │   ├── Common/
│   │   │   ├── BaseEntity.cs
│   │   │   ├── IAggregateRoot.cs
│   │   │   └── ValueObject.cs
│   │   └── Entities/
│   │       ├── User.cs
│   │       ├── Role.cs
│   │       ├── Permission.cs
│   │       ├── RolePermission.cs
│   │       └── FileAttachment.cs
│   ├── Exceptions/
│   │   ├── NotFoundException.cs
│   │   ├── ValidationException.cs
│   │   └── UnauthorizedException.cs
│   ├── Interfaces/
│   │   ├── IFileStorageService.cs
│   │   ├── INotificationService.cs
│   │   ├── ICurrentUserService.cs
│   │   └── IEmailService.cs
│   └── Behaviors/
│       ├── ValidationBehavior.cs
│       └── LoggingBehavior.cs
│
├── ModernBaseProject.Infrastructure/
│   ├── Persistence/
│   │   ├── AppDbContext.cs
│   │   ├── Configurations/ (Entity Configurations)
│   │   └── Seeding/
│   │       ├── PermissionSeeder.cs
│   │       └── AdminSeeder.cs
│   ├── FileStorage/
│   │   ├── LocalFileStorageService.cs
│   │   └── S3FileStorageService.cs
│   ├── Notifications/
│   │   ├── EmailService.cs
│   │   └── Hubs/
│   │       └── NotificationHub.cs
│   └── Authentication/
│       ├── JwtProvider.cs
│       ├── PasswordHasher.cs
│       └── PermissionAuthorizationHandler.cs
│
└── ModernBaseProject.API/
    ├── Features/  ⭐ VERTICAL SLICE ARCHITECTURE
    │   ├── Users/
    │   │   ├── CreateUser/
    │   │   │   ├── CreateUserEndpoint.cs
    │   │   │   ├── CreateUserCommand.cs
    │   │   │   ├── CreateUserHandler.cs
    │   │   │   ├── CreateUserValidator.cs
    │   │   │   └── CreateUserResponse.cs
    │   │   ├── GetUserById/
    │   │   ├── GetUsersList/
    │   │   └── Login/
    │   │       ├── LoginEndpoint.cs
    │   │       ├── LoginCommand.cs
    │   │       ├── LoginHandler.cs
    │   │       └── LoginResponse.cs (with AccessToken, RefreshToken)
    │   ├── Products/
    │   └── Orders/
    ├── Middleware/
    │   └── GlobalExceptionHandler.cs
    ├── Extensions/
    │   ├── ServiceExtensions.cs
    │   └── WebApplicationExtensions.cs
    └── Program.cs
```

### Key Implementation Details

#### 1. Database & Entities

**BaseEntity.cs:**

```csharp
public abstract class BaseEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
}
```

**User Entity:** Include Username, Email, PasswordHash, RefreshToken, RefreshTokenExpiryTime

**Role-Permission System:**

- Many-to-Many relationship between Roles and Permissions
- RolePermissions join table
- Permission keys like: "User.Create", "User.Read", "User.Update", "User.Delete"

#### 2. Data Access Pattern

**❌ NO Generic Repository Pattern**

**✅ Direct DbContext Injection in Handlers:**

```csharp
public class GetUsersHandler
{
    private readonly AppDbContext _context;

    public async Task<List<UserDto>> Handle(GetUsersQuery request)
    {
        return await _context.Users
            .Where(u => u.IsActive)
            .Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email
            })
            .ToListAsync();
    }
}
```

**✅ Use LINQ Projections** - Always select only needed columns
**✅ Use `.AsSplitQuery()`** for complex joins

#### 3. Authentication System

**JWT Token Structure:**

- **Access Token**: 15-30 minutes lifetime
  - Claims: Sub (UserId), Email, Permissions (comma-separated)
- **Refresh Token**: Long-lived (7-30 days)
  - Store hashed in database
  - Include IP address and device info validation

**Login Flow:**

1. Validate credentials
2. Generate Access Token + Refresh Token
3. Save Refresh Token to database (hashed)
4. Return both tokens

**Refresh Token Flow:**

1. Validate refresh token exists and not expired
2. Validate IP/Device matches (optional)
3. Generate new Access Token
4. Optionally rotate Refresh Token

#### 4. File Storage System

**Interface:**

```csharp
public interface IFileStorageService
{
    Task<string> UploadAsync(Stream fileStream, string fileName, string contentType);
    Task DeleteAsync(string filePath);
    Task<string> GetUrlAsync(string filePath);
}
```

**Implementations:**

- LocalFileStorageService: Saves to `wwwroot/uploads`
- S3FileStorageService: Uses AWS SDK (implement interface only, no actual AWS integration needed)

**FileAttachment Entity:**

- FileName, Extension, Size, Path, UploadedBy (UserId), UploadedAt

#### 5. Notification System

**SignalR Hub:**

```csharp
public class NotificationHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst("sub")?.Value;
        if (userId != null)
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
    }
}
```

**Send Notification:**

```csharp
await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", new
{
    Message = "Your order has been approved",
    Type = "Success"
});
```

**Email Queue (MassTransit):**

- Create `SendEmailCommand` message
- Publish to RabbitMQ
- Create `EmailConsumer` that processes the queue
- Use a mock SMTP service (log to console in dev)

#### 6. Seeding Logic (Program.cs startup)

```csharp
// 1. Ensure database created
await dbContext.Database.EnsureCreatedAsync();

// 2. Seed Permissions (scan from code attributes)
var permissions = ScanPermissionsFromCode();
await SeedPermissionsAsync(permissions);

// 3. Create SuperAdmin role if not exists
// 4. Create admin@domain.com user if not exists
// 5. Assign all permissions to SuperAdmin role
// 6. Assign SuperAdmin role to admin user
```

---

## 🎨 Frontend Architecture

### Project Structure

```
src/
├── app/
│   ├── App.tsx
│   ├── router.tsx
│   └── providers.tsx
├── components/
│   ├── ui/ (Shadcn components)
│   ├── layouts/
│   │   ├── DashboardLayout.tsx
│   │   ├── Sidebar.tsx
│   │   └── Header.tsx
│   └── guards/
│       └── PermissionGuard.tsx
├── features/
│   ├── auth/
│   │   ├── components/
│   │   │   ├── LoginForm.tsx
│   │   │   └── RegisterForm.tsx
│   │   ├── hooks/
│   │   │   └── useAuth.ts
│   │   ├── services/
│   │   │   └── auth-service.ts
│   │   └── types/
│   │       └── auth.types.ts
│   ├── users/
│   │   ├── components/
│   │   │   ├── UserList.tsx
│   │   │   ├── UserForm.tsx
│   │   │   └── UserCard.tsx
│   │   ├── hooks/
│   │   │   └── useUsers.ts
│   │   └── services/
│   │       └── user-service.ts
│   └── dashboard/
│       ├── components/
│       │   ├── StatCard.tsx
│       │   └── RecentActivity.tsx
│       └── pages/
│           └── DashboardPage.tsx
├── hooks/
│   ├── useAuth.ts
│   └── useTheme.ts
├── lib/
│   ├── axios.ts (Axios instance with interceptors)
│   ├── utils.ts
│   └── signalr.ts (SignalR connection)
├── stores/
│   ├── auth-store.ts (Zustand)
│   └── ui-store.ts (Zustand)
└── types/
    └── api.types.ts
```

### Key Implementation Details

#### 1. State Management Strategy

**TanStack Query** for server state:

```typescript
// useUsers.ts
export const useUsers = () => {
  return useQuery({
    queryKey: ["users"],
    queryFn: () => userService.getAll(),
    staleTime: 1000 * 60 * 5, // 5 minutes
  });
};
```

**Zustand** for client state:

```typescript
// auth-store.ts
interface AuthState {
  user: User | null;
  accessToken: string | null;
  setAuth: (user: User, accessToken: string) => void;
  logout: () => void;
}

export const useAuthStore = create<AuthState>((set) => ({
  user: null,
  accessToken: null,
  setAuth: (user, accessToken) => set({ user, accessToken }),
  logout: () => set({ user: null, accessToken: null }),
}));
```

#### 2. Axios Configuration

```typescript
// lib/axios.ts
const axiosInstance = axios.create({
  baseURL: import.meta.env.VITE_API_URL,
});

// Add auth token to requests
axiosInstance.interceptors.request.use((config) => {
  const token = useAuthStore.getState().accessToken;
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Handle token refresh on 401
axiosInstance.interceptors.response.use(
  (response) => response,
  async (error) => {
    if (error.response?.status === 401) {
      // Attempt token refresh
      // If fails, logout user
    }
    return Promise.reject(error);
  }
);
```

#### 3. Permission Guard Component

```typescript
interface PermissionGuardProps {
  permission: string;
  children: React.ReactNode;
}

export const PermissionGuard: React.FC<PermissionGuardProps> = ({
  permission,
  children,
}) => {
  const { user } = useAuthStore();

  const hasPermission = user?.permissions?.includes(permission);

  if (!hasPermission) return null;

  return <>{children}</>;
};
```

#### 4. SignalR Integration

```typescript
// lib/signalr.ts
const connection = new HubConnectionBuilder()
  .withUrl(`${API_URL}/notifications`, {
    accessTokenFactory: () => useAuthStore.getState().accessToken!,
  })
  .withAutomaticReconnect()
  .build();

connection.on("ReceiveNotification", (notification) => {
  toast.success(notification.message);
});

await connection.start();
```

#### 5. Admin Dashboard Layout

Create a responsive dashboard with:

- **Sidebar**: Navigation menu with icons
- **Header**: User profile dropdown, notifications bell icon
- **Content Area**: Route-based content rendering
- **Theme Toggle**: Dark/Light mode support

---

## 🐳 Docker Infrastructure

### docker-compose.yml

```yaml
version: "3.8"

services:
  api:
    build: ./src/API
    ports:
      - "5000:8080"
    environment:
      - ConnectionStrings__DefaultConnection=Host=postgres;Database=modernbase;Username=postgres;Password=postgres
      - Redis__Connection=redis:6379
      - RabbitMQ__Host=rabbitmq
    depends_on:
      - postgres
      - redis
      - rabbitmq

  ui:
    build: ./src/ui
    ports:
      - "3000:80"
    depends_on:
      - api

  postgres:
    image: postgres:16
    environment:
      POSTGRES_PASSWORD: postgres
      POSTGRES_DB: modernbase
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data

  redis:
    image: redis:7-alpine
    ports:
      - "6379:6379"

  rabbitmq:
    image: rabbitmq:3-management
    ports:
      - "5672:5672"
      - "15672:15672"

  seq:
    image: datalust/seq
    environment:
      ACCEPT_EULA: Y
    ports:
      - "5341:80"

  maildev:
    image: maildev/maildev
    ports:
      - "1080:1080"
      - "1025:1025"

volumes:
  postgres_data:
```

---

## ✅ Acceptance Criteria

Your implementation must include:

### Backend (Must Have):

- [ ] Vertical Slice Architecture with at least Users feature fully implemented
- [ ] JWT authentication with access + refresh tokens
- [ ] Role-Permission based authorization
- [ ] Direct DbContext usage (no generic repository)
- [ ] LINQ projections in all queries
- [ ] FluentValidation pipeline behavior
- [ ] Global exception handling middleware
- [ ] File upload endpoint with local storage
- [ ] SignalR hub for notifications
- [ ] Database seeding (Admin user + permissions)
- [ ] Serilog + Seq logging configuration

### Frontend (Must Have):

- [ ] Login/Register pages
- [ ] Dashboard layout (Sidebar + Header + Content)
- [ ] Users list page with TanStack Query
- [ ] User create/edit form
- [ ] PermissionGuard component
- [ ] Axios interceptors for auth
- [ ] SignalR connection for real-time notifications
- [ ] Zustand store for auth state
- [ ] Dark/Light theme toggle

### Infrastructure (Must Have):

- [ ] docker-compose.yml with all services
- [ ] PostgreSQL database service
- [ ] Redis cache service
- [ ] RabbitMQ message queue
- [ ] Seq logging UI
- [ ] MailDev for email testing

---

## 🚀 Implementation Order

**Phase 1: Core Backend**

1. Create solution structure
2. Define entities (User, Role, Permission)
3. Configure EF Core and DbContext
4. Implement JWT authentication
5. Create Login endpoint (Vertical Slice)

**Phase 2: Features & Infrastructure** 6. Add Users CRUD endpoints 7. Implement file storage service 8. Add SignalR hub 9. Setup RabbitMQ + MassTransit 10. Add seeding logic

**Phase 3: Frontend** 11. Setup React + Vite project 12. Create auth store and services 13. Build login/register pages 14. Create dashboard layout 15. Implement users list and forms 16. Add SignalR client

**Phase 4: DevOps** 17. Create Dockerfiles 18. Write docker-compose.yml 19. Add environment configuration 20. Test end-to-end

---

## 📝 Important Notes

1. **No Overengineering**: Keep it simple and pragmatic
2. **Performance First**: Use projections, split queries, caching
3. **Type Safety**: Full TypeScript on frontend, strong typing on backend
4. **Security**: Hash passwords (BCrypt), validate tokens, check permissions
5. **Error Handling**: Global exception handler, proper error responses
6. **Logging**: Log everything important (auth, errors, key operations)

---

## 🎯 Success Metrics

The project is complete when:

- ✅ User can register and login
- ✅ Access token expires and refreshes automatically
- ✅ Admin can see user list (with permission check)
- ✅ Files can be uploaded and retrieved
- ✅ Real-time notification appears when triggered
- ✅ All services start with `docker-compose up`
- ✅ Logs visible in Seq dashboard

---

Start with Phase 1 and build incrementally. Focus on working code over perfect code. This is a boilerplate - it should be extensible but not over-abstracted.

**Good luck! 🚀**
