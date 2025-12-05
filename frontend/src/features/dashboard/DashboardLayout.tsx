import { useEffect } from 'react';
import { Outlet } from 'react-router-dom';
import { useNotificationStore } from '../../stores/notificationStore';
import { connection, startConnection } from '../../lib/signalr';
import { NotificationToast } from '../../components/NotificationToast';
import { Sidebar } from '../../components/layout/Sidebar';
import { Header } from '../../components/layout/Header';

export const DashboardLayout = () => {
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

  return (
    <div className="flex min-h-screen bg-slate-50">
      <Sidebar />
      <div className="flex flex-1 flex-col pl-64">
        <Header />
        <main className="flex-1 p-8">
          <Outlet />
        </main>
      </div>
      <NotificationToast />
    </div>
  );
};
