# Modern Base Project - Frontend

React + TypeScript + Vite frontend application.

## 🚀 Quick Start

### Install Dependencies
```bash
npm install
```

### Run Development Server
```bash
npm run dev
```

The app will be available at `http://localhost:3000`

### Build for Production
```bash
npm run build
```

## 📁 Project Structure

```
src/
├── components/       # Shared components (ProtectedRoute)
├── features/         # Feature-based modules
│   ├── auth/        # Login, auth service
│   ├── users/       # User list, user service
│   └── dashboard/   # Dashboard layout and home
├── hooks/           # Custom hooks (useAuth)
├── lib/             # Axios instance with interceptors
├── stores/          # Zustand stores (authStore)
└── types/           # TypeScript interfaces
```

## 🔑 Features

- ✅ JWT Authentication with auto-refresh
- ✅ Protected routes
- ✅ TanStack Query for data fetching
- ✅ Zustand for state management
- ✅ React Router for navigation
- ✅ TypeScript for type safety
- ✅ Axios interceptors for auth

## 🛠️ Technology Stack

- React 18
- TypeScript
- Vite
- TanStack Query (React Query)
- Zustand
- React Router DOM
- Axios
- SignalR Client (ready for real-time)

## 🔐 Default Login

- Email: `admin@domain.com`
- Password: `Admin123!`

## 📝 Environment Variables

Create `.env` file:
```
VITE_API_URL=http://localhost:5000/api
```

## 🎯 Available Pages

- `/login` - Login page
- `/dashboard` - Dashboard home
- `/dashboard/users` - User management

## 📦 Scripts

- `npm run dev` - Start development server
- `npm run build` - Build for production
- `npm run preview` - Preview production build
- `npm run lint` - Run ESLint
