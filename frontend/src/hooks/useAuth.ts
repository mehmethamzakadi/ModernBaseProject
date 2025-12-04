import { useAuthStore } from '../stores/authStore';

export const useAuth = () => {
  const { user, isAuthenticated, logout, hasPermission } = useAuthStore();

  return {
    user,
    isAuthenticated,
    logout,
    hasPermission,
  };
};
