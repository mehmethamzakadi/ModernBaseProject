import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { LoginPage } from './features/auth/LoginPage';
import { DashboardLayout } from './features/dashboard/DashboardLayout';
import { DashboardHome } from './features/dashboard/DashboardHome';
import { UserListPage } from './features/users/UserListPage';
import { FileUpload } from './features/files/FileUpload';
import { ProtectedRoute } from './components/ProtectedRoute';
import { NotificationToast } from './components/NotificationToast';
import { APP_ROUTES } from './constants';

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      refetchOnWindowFocus: false,
      retry: 1,
    },
  },
});

function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <BrowserRouter>
        <NotificationToast />
        <Routes>
          <Route path={APP_ROUTES.LOGIN} element={<LoginPage />} />
          <Route
            path={APP_ROUTES.DASHBOARD}
            element={
              <ProtectedRoute>
                <DashboardLayout />
              </ProtectedRoute>
            }
          >
            <Route index element={<DashboardHome />} />
            <Route path="users" element={<UserListPage />} />
            <Route path="files" element={<FileUpload />} />
          </Route>
          <Route path="/" element={<Navigate to={APP_ROUTES.DASHBOARD} replace />} />
        </Routes>
      </BrowserRouter>
    </QueryClientProvider>
  );
}

export default App;
