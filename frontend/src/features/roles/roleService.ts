import { api } from '../../lib/axios';
import { API_ROUTES } from '../../constants';
import type { Role } from '../../types';

export const roleService = {
  getAll: async (): Promise<Role[]> => {
    const { data } = await api.get<Role[]>(API_ROUTES.ROLES.BASE);
    return data;
  },
};
