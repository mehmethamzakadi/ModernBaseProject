import { Link, useLocation } from 'react-router-dom';
import { LayoutDashboard, Users, FileText, Settings } from 'lucide-react';
import { APP_ROUTES } from '../../constants';
import { cn } from '../../lib/utils';

const navigation = [
  { name: 'Dashboard', href: APP_ROUTES.DASHBOARD, icon: LayoutDashboard },
  { name: 'Users', href: `${APP_ROUTES.DASHBOARD}/users`, icon: Users },
  { name: 'Files', href: `${APP_ROUTES.DASHBOARD}/files`, icon: FileText },
  { name: 'Settings', href: `${APP_ROUTES.DASHBOARD}/settings`, icon: Settings },
];

export const Sidebar = () => {
  const location = useLocation();

  return (
    <aside className="fixed left-0 top-0 z-40 h-screen w-64 border-r border-slate-200 bg-white">
      <div className="flex h-16 items-center border-b border-slate-200 px-6">
        <h2 className="text-xl font-semibold text-slate-900">Modern Base</h2>
      </div>
      <nav className="space-y-1 p-4">
        {navigation.map((item) => {
          const isActive = location.pathname === item.href;
          return (
            <Link
              key={item.name}
              to={item.href}
              className={cn(
                'flex items-center gap-3 rounded-lg px-3 py-2.5 text-sm font-medium transition-colors',
                isActive
                  ? 'bg-indigo-50 text-indigo-700'
                  : 'text-slate-700 hover:bg-slate-100 hover:text-slate-900'
              )}
            >
              <item.icon className="h-5 w-5" />
              {item.name}
            </Link>
          );
        })}
      </nav>
    </aside>
  );
};
