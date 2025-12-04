import { useAuth } from '../hooks/useAuth';

interface PermissionGuardProps {
  permission: string;
  children: React.ReactNode;
}

export const PermissionGuard = ({ permission, children }: PermissionGuardProps) => {
  const { hasPermission } = useAuth();

  if (!hasPermission(permission)) {
    return null;
  }

  return <>{children}</>;
};
