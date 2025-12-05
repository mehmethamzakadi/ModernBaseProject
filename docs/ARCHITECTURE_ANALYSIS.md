# Modern Base Project: Teknik Analiz ve Mimari Tasarım Dokümanı

## 1. Proje Kimliği ve Teknoloji Yığını

Bu proje, kurumsal standartlarda, yüksek performanslı, modüler ve dağıtık sistem mimarisine uygun bir başlangıç şablonu (Boilerplate) olarak tasarlanmıştır.

| Alan                     | Teknoloji / Kütüphane             | Notlar                                       |
| ------------------------ | --------------------------------- | -------------------------------------------- |
| **Backend Framework**    | .NET 10 (Latest)                  | Minimal API, C# 13/14 özellikleri            |
| **Mimari Desen**         | Vertical Slice Architecture (VSA) | Feature-based folder structure               |
| **Frontend**             | React + TypeScript                | Vite build tool ile                          |
| **Database**             | PostgreSQL                        | JSONB desteği ve yüksek performans için      |
| **ORM**                  | EF Core (Code First)              | Doğrudan DbContext kullanımı                 |
| **API Pattern**          | REPR (Request-Endpoint-Response)  | FastEndpoints veya Minimal API + MediatR     |
| **Messaging**            | RabbitMQ + MassTransit            | Outbox Pattern entegreli                     |
| **Caching**              | Redis                             | Distributed Cache & Output Cache             |
| **Logging & Monitoring** | Serilog + Seq + OpenTelemetry     | Merkezi loglama ve trace takibi              |
| **Validation**           | FluentValidation                  | Pipeline Behavior içinde otomatik validasyon |
| **Storage**              | Abstract File Service             | Local, AWS S3, MinIO desteği                 |
| **Real-time**            | SignalR                           | Canlı bildirimler için                       |

---

## 2. Backend Mimarisi Detayları

Backend, klasik katmanlı mimari (Onion) yerine, özellik tabanlı **Vertical Slice** mimarisi ile kurgulanacaktır. Nuget paket yönetimi merkezi bir yapıda olacaktır (Directory.Packages.props).

### 2.1. Klasör ve Proje Yapısı

Geleneksel Services, Repositories klasörleri yerine, işlevselliği temel alan bir yapı kurulacaktır.

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
│   ├── Constants/
│   │   ├── ApiRoutes.cs
│   │   ├── JwtClaims.cs
│   │   ├── Roles.cs
│   │   ├── Permissions.cs
│   │   ├── SeederConstants.cs
│   │   ├── ConfigurationKeys.cs
│   │   ├── CorsPolicies.cs
│   │   ├── JwtDefaults.cs
│   │   ├── SignalRDefaults.cs
│   │   └── ExceptionMessages.cs
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

### 2.2. Veri Erişimi ve Performans

- **No Generic Repository**: Generic Repository pattern kullanılmayacak. Handler'lar içinde `AppDbContext` doğrudan inject edilecek.

- **LINQ Projections**: Veri çekerken tüm entity'i çekmek yerine `.Select(x => new Dto { ... })` veya `ProjectToType<Dto>()` (Mapster) kullanılarak sadece ihtiyaç duyulan kolonlar çekilecek.

- **Query Splitting**: Büyük join işlemlerinde `.AsSplitQuery()` varsayılan davranış olarak değerlendirilecek.

### 2.3. Constants ve Magic String Yönetimi

Projede **Magic String** (hardcoded string değerler) kullanımından kaçınılır. Tüm sabit değerler merkezi `Constants` klasöründe organize edilmiştir.

**Constants Yapısı** (`ModernBaseProject.Core/Constants/`):

- **ApiRoutes.cs**: API endpoint path'leri (`/api/auth/login`, `/api/users`, vb.)
- **JwtClaims.cs**: JWT token claim type isimleri (`permissions`, `sub`, `email`)
- **Roles.cs**: Sistem rol isimleri (`SuperAdmin`, vb.)
- **Permissions.cs**: Permission key'leri (`User.Create`, `User.Read`, vb.)
- **SeederConstants.cs**: Database seed değerleri (admin email, password, vb.)
- **ConfigurationKeys.cs**: Configuration section key'leri (`Jwt:Key`, `DefaultConnection`, vb.)
- **CorsPolicies.cs**: CORS policy isimleri
- **JwtDefaults.cs**: JWT varsayılan değerleri (token expiry süreleri)
- **SignalRDefaults.cs**: SignalR varsayılan değerleri
- **ExceptionMessages.cs**: Exception mesajları

**Kullanım Örnekleri**:

```csharp
// ❌ Magic String Kullanımı (Yanlış)
app.MapPost("/api/auth/login", ...);
options.AddPolicy("User.Create", ...);
var email = "admin@domain.com";

// ✅ Constants Kullanımı (Doğru)
app.MapPost(ApiRoutes.Login, ...);
options.AddPolicy(Permissions.UserCreate, ...);
var email = SeederConstants.AdminEmail;
```

**Frontend Constants**: Frontend tarafında da `frontend/src/constants/index.ts` dosyasında tüm sabit değerler (API routes, localStorage keys, app routes, permissions) merkezi olarak yönetilir.

**Faydalar**:

- ✅ Tip güvenliği ve IntelliSense desteği
- ✅ Yazım hatalarının compile-time'da yakalanması
- ✅ Refactoring kolaylığı
- ✅ Kod tekrarının azalması
- ✅ Bakım kolaylığı

Detaylı bilgi için: [MAGIC_STRINGS_SOLUTION.md](MAGIC_STRINGS_SOLUTION.md)

### 2.4. Dosya Yönetim Sistemi (File Storage)

Sistem soyut bir yapı üzerine kurulacak, böylece ortam (Local/Cloud) değişse de kod değişmeyecek.

**Interface**: `IFileStorageService` → `UploadAsync`, `DeleteAsync`, `GetUrl`

**Implementation**:

- **LocalFileStorageService**: Geliştirme ortamında `wwwroot` veya volume mount edilmiş bir klasöre yazar.
- **CloudFileStorageService**: Prod ortamında AWS S3, Azure Blob veya MinIO'ya yazar.

**Veritabanı Takibi**: Yüklenen dosyaların metadata'sı (Dosya adı, uzantısı, boyutu, yolu, yükleyen user) `FileAttachments` tablosunda tutulacak.

### 2.5. Bildirim Sistemi (Notification System)

Bildirimler ikiye ayrılacak: **Anlık (Real-time)** ve **Asenkron (Email/SMS)**.

#### Real-Time (SignalR)

- **NotificationHub**: Kullanıcı sisteme girdiğinde `UserId` ile bir gruba dahil edilir.
- Backend'den `IHubContext` kullanılarak `await _hub.Clients.User(userId).SendAsync("ReceiveNotification", message)` şeklinde anlık bildirim atılır.

#### Email/SMS (Asenkron - MassTransit)

- Kullanıcıyı bekletmemek için email gönderimi kuyruğa atılır.
- `SendEmailCommand` → RabbitMQ → `EmailConsumer`
- Consumer içinde SMTP veya 3. parti (SendGrid/Mailgun) servisleri çağrılır.

### 2.6. Kimlik Yönetimi ve RBAC (Auth)

Keycloak yerine, tam kontrollü yerel bir yapı kurulacak.

**Tablolar**:

- **Users**: `(Id, Username, PasswordHash, Email, RefreshToken...)`
- **Roles**: `(Id, Name - Örn: Admin, StandartUser)`
- **Permissions**: `(Id, Key, Description - Örn: "User.Create", "Order.Approve")`
- **RolePermissions**: Role ve Permission ara tablosu

**Security**:

- **Access Token**: 15-30 dk ömürlü JWT. Payload içinde `Sub`, `Email` ve `Permissions` (veya `Roles`) claimleri olacak.
- **Refresh Token**: Uzun ömürlü, DB'de hashlenerek saklanan, IP ve Device bilgisi ile eşleşen token.

**Seed Data**: Uygulama başladığında `SystemAdmin` rolü ve tüm `Permissions` (Reflection ile koddan taranarak) veritabanına yoksa eklenir.

---

## 3. Frontend Mimarisi Detayları

React projesi de Backend'in Feature yapısına benzer modülerlikte olacak.

```
src/
├── app/ (Routing, Provider setup)
├── components/ (Shared UI components - Shadcn/UI)
├── constants/ (API routes, storage keys, app routes, permissions)
├── features/  <-- BACKEND İLE EŞLENİK YAPILAR
│   ├── auth/ (Login form, register, hooks)
│   ├── users/ (UserList, UserForm, user-service.ts)
│   └── dashboard/
├── hooks/ (useAuth, useTheme)
├── lib/ (Axios instance, utils)
└── stores/ (Zustand stores - authStore, uiStore)
```

### State Stratejisi

- **Veri çekme işlemleri** (Users list, Product details) için **TanStack Query (React Query)** kullanılacak. "Stale-while-revalidate" stratejisi ile kullanıcıya anında veri gösterilecek.
- **Client state** (Modal açık mı, Sidebar kapalı mı) için **Zustand** kullanılacak.

### Frontend Constants

Backend'deki Constants yaklaşımına benzer şekilde, frontend'de de tüm magic string'ler merkezi `constants/index.ts` dosyasında yönetilir:

- **API_ROUTES**: API endpoint path'leri
- **STORAGE_KEYS**: localStorage key'leri (`accessToken`, `refreshToken`)
- **APP_ROUTES**: Uygulama route'ları (`/login`, `/dashboard`)
- **PERMISSIONS**: Permission key'leri (backend ile senkronize)
- **HTTP_HEADERS**: HTTP header isimleri ve değerleri
- **DEFAULT_URLS**: Varsayılan API ve SignalR URL'leri

**Kullanım Örnekleri**:

```typescript
// ❌ Magic String (Yanlış)
await api.post("/auth/login", credentials);
localStorage.getItem("accessToken");
<Route path="/login" element={<LoginPage />} />;

// ✅ Constants (Doğru)
await api.post(API_ROUTES.AUTH.LOGIN, credentials);
localStorage.getItem(STORAGE_KEYS.ACCESS_TOKEN);
<Route path={APP_ROUTES.LOGIN} element={<LoginPage />} />;
```

### Admin Dashboard

- **Layout**: Sidebar, Header (User profile, Notifications bell), Content Area
- **Yetki Yönetimi**: Frontend tarafında `<PermissionGuard permission={PERMISSIONS.USER_CREATE}> <Button>Ekle</Button> </PermissionGuard>` bileşeni ile yetkisi olmayana butonlar gizlenecek. Permission string'leri `PERMISSIONS` constant'ından kullanılır.

---

## 4. Altyapı ve DevOps (CI/CD)

Proje `docker-compose.yml` ile tek komutla ayağa kalkacak şekilde tasarlanmıştır.

### 4.1. Konteyner Servisleri

- **API**: .NET 10 API
- **UI**: React (Nginx veya Vite preview üzerinde)
- **PostgreSQL**: Ana veritabanı
- **Redis**: Cache ve Pub/Sub store
- **RabbitMQ**: Mesaj kuyruğu
- **Seq**: Logların aktığı arayüz
- **MailDev/Papercut**: Localde atılan mailleri yakalamak ve görüntülemek için SMTP mock sunucusu

### 4.2. CI/CD Pipeline Adımları

1. **Lint & Test**: Kod pushlandığında Backend (xUnit) ve Frontend testleri koşar.
2. **Build**: Docker image'ları build alınır (Multi-stage build ile optimize edilir).
3. **Push**: Image'lar Container Registry'e (Docker Hub/GHCR) itilir.
4. **Deploy**: Hedef sunucuya SSH ile bağlanıp `docker compose pull && docker compose up -d` komutu çalıştırılır.

---

## 5. Başlangıç Senaryosu (Seed Work)

Proje ilk kez çalıştığında (`Program.cs` start):

1. **Migration Check**: DB var mı kontrol eder, yoksa oluşturur ve bekleyen migrationları basar.
2. **Permission Seed**: Kod içerisindeki (Controller veya Endpointlerdeki attribute'lardan okunan) tüm izin tanımlarını `Permissions` tablosuna yazar/günceller.
3. **Admin Seed**: `SeederConstants.AdminEmail` (`admin@domain.com`) kullanıcısı yoksa oluşturur.
4. **Role Assign**: Admin kullanıcısına `Roles.SuperAdmin` rolünü ve tüm permissionları (`Permissions` sınıfından) atar.

**Not**: Tüm seed değerleri `SeederConstants`, rol isimleri `Roles`, permission key'leri ise `Permissions` constant'larından kullanılır. Magic string kullanımından kaçınılır.
