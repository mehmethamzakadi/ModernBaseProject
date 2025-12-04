import { api } from '../../lib/axios';
import type { Role } from '../../types';

export const roleService = {
  getAll: async (): Promise<Role[]> => {
    const { data } = await api.get<Role[]>('/roles');
    return data;
  },
};
