# Frontend Development Guide

## 🏗️ Architecture

Frontend, backend ile aynı **Feature-based** yapıyı takip eder.

### Folder Structure

```
src/
├── features/           # Feature modules (backend ile eşleşir)
│   ├── auth/          # Login, authService
│   ├── users/         # User list, userService
│   └── dashboard/     # Layout, home
├── stores/            # Zustand stores
│   └── authStore.ts   # Authentication state
├── lib/               # Shared utilities
│   └── axios.ts       # Axios instance with interceptors
├── hooks/             # Custom hooks
│   └── useAuth.ts     # Auth hook
├── components/        # Shared components
│   └── ProtectedRoute.tsx
└── types/             # TypeScript interfaces
    └── index.ts       # API models
```

## 🔑 Key Concepts

### 1. State Management

**Zustand** - Client state için (auth, UI state)
```typescript
const { user, isAuthenticated, logout } = useAuthStore();
```

**TanStack Query** - Server state için (API data)
```typescript
const { data: users, isLoading } = useQuery({
  queryKey: ['users'],
  queryFn: userService.getAll,
});
```

### 2. Authentication Flow

1. User logs in → `authService.login()`
2. Store tokens → `localStorage` + `authStore`
3. Axios interceptor adds token to requests
4. On 401 error → Auto refresh token
5. If refresh fails → Redirect to login

### 3. Protected Routes

```typescript
<Route
  path="/dashboard"
  element={
    <ProtectedRoute>
      <DashboardLayout />
    </ProtectedRoute>
  }
/>
```

### 4. API Services

Her feature kendi service dosyasına sahip:

```typescript
// features/users/userService.ts
export const userService = {
  getAll: async () => { ... },
  getById: async (id) => { ... },
  create: async (data) => { ... },
  update: async (id, data) => { ... },
  delete: async (id) => { ... },
};
```

## 🚀 Adding New Features

### Example: Products Feature

1. **Create folder structure:**
```
src/features/products/
├── ProductListPage.tsx
├── ProductForm.tsx
└── productService.ts
```

2. **Create service:**
```typescript
// productService.ts
import { api } from '../../lib/axios';

export const productService = {
  getAll: async () => {
    const { data } = await api.get('/products');
    return data;
  },
};
```

3. **Create page:**
```typescript
// ProductListPage.tsx
import { useQuery } from '@tanstack/react-query';
import { productService } from './productService';

export const ProductListPage = () => {
  const { data: products } = useQuery({
    queryKey: ['products'],
    queryFn: productService.getAll,
  });

  return <div>{/* Render products */}</div>;
};
```

4. **Add route:**
```typescript
// App.tsx
<Route path="products" element={<ProductListPage />} />
```

## 🎨 Styling

Şu anda inline styles kullanılıyor (minimal). İlerleyen aşamalarda:

- **Tailwind CSS** eklenebilir
- **Shadcn/UI** component library eklenebilir
- **CSS Modules** kullanılabilir

## 🔐 Permission Guard (Future)

```typescript
// components/PermissionGuard.tsx
export const PermissionGuard = ({ permission, children }) => {
  const { hasPermission } = useAuth();
  
  if (!hasPermission(permission)) return null;
  
  return <>{children}</>;
};

// Usage
<PermissionGuard permission="User.Create">
  <button>Create User</button>
</PermissionGuard>
```

## 📡 SignalR Integration (Future)

```typescript
// lib/signalr.ts
import * as signalR from '@microsoft/signalr';

export const connection = new signalR.HubConnectionBuilder()
  .withUrl('http://localhost:5000/hubs/notifications')
  .build();

// Usage in component
useEffect(() => {
  connection.start();
  connection.on('ReceiveNotification', (message) => {
    console.log('Notification:', message);
  });
}, []);
```

## 🧪 Testing (Future)

```bash
npm install -D vitest @testing-library/react
```

```typescript
// UserListPage.test.tsx
import { render, screen } from '@testing-library/react';
import { UserListPage } from './UserListPage';

test('renders user list', () => {
  render(<UserListPage />);
  expect(screen.getByText('Users')).toBeInTheDocument();
});
```

## 📦 Environment Variables

```env
VITE_API_URL=http://localhost:5000/api
VITE_SIGNALR_URL=http://localhost:5000/hubs
```

## 🔧 Development Tips

1. **Hot Reload**: Vite provides instant HMR
2. **Type Safety**: Always define TypeScript interfaces
3. **Query Keys**: Use consistent naming for React Query keys
4. **Error Handling**: Axios interceptor handles 401, add more as needed
5. **Code Splitting**: Use React.lazy() for large components

## 📚 Resources

- [React Query Docs](https://tanstack.com/query/latest)
- [Zustand Docs](https://docs.pmnd.rs/zustand)
- [React Router Docs](https://reactrouter.com)
- [Vite Docs](https://vitejs.dev)
