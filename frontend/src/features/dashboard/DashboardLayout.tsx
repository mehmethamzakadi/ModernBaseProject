import { useEffect } from 'react';
import { Link, Outlet, useNavigate } from 'react-router-dom';
import { useAuthStore } from '../../stores/authStore';
import { useNotificationStore } from '../../stores/notificationStore';
import { connection, startConnection } from '../../lib/signalr';
import { NotificationToast } from '../../components/NotificationToast';
import { APP_ROUTES } from '../../constants';

export const DashboardLayout = () => {
  const { user, logout } = useAuthStore();
  const navigate = useNavigate();
  const addNotification = useNotificationStore((state) => state.addNotification);

  useEffect(() => {
    startConnection();

    connection.on('ReceiveNotification', (message: string) => {
      addNotification(message, 'info');
    });

    return () => {
      connection.off('ReceiveNotification');
    };
  }, [addNotification]);

  const handleLogout = () => {
    logout();
    navigate(APP_ROUTES.LOGIN);
  };

  return (
    <div className="flex min-h-screen bg-gray-100">
      <aside className="w-64 bg-gray-800 text-white">
        <div className="p-6">
          <h2 className="text-2xl font-bold">Modern Base</h2>
        </div>
        <nav className="mt-6">
          <Link
            to={APP_ROUTES.DASHBOARD}
            className="block px-6 py-3 hover:bg-gray-700 transition"
          >
            Dashboard
          </Link>
          <Link
            to={`${APP_ROUTES.DASHBOARD}/users`}
            className="block px-6 py-3 hover:bg-gray-700 transition"
          >
            Users
          </Link>
          <Link
            to={`${APP_ROUTES.DASHBOARD}/files`}
            className="block px-6 py-3 hover:bg-gray-700 transition"
          >
            Files
          </Link>
        </nav>
      </aside>

      <div className="flex-1 flex flex-col">
        <header className="bg-white shadow-sm">
          <div className="flex justify-between items-center px-6 py-4">
            <h3 className="text-xl font-semibold text-gray-800">
              Welcome, {user?.username || 'User'}
            </h3>
            <button
              onClick={handleLogout}
              className="bg-red-500 text-white px-4 py-2 rounded-lg hover:bg-red-600"
            >
              Logout
            </button>
          </div>
        </header>

        <main className="flex-1 p-6">
          <Outlet />
        </main>
      </div>
      <NotificationToast />
    </div>
  );
};
