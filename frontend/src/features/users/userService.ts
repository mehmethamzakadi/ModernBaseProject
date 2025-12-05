import { api } from '../../lib/axios';
import { API_ROUTES } from '../../constants';
import type { User, CreateUserRequest, UpdateUserRequest } from '../../types';

export const userService = {
  getAll: async (): Promise<User[]> => {
    const { data } = await api.get<User[]>(API_ROUTES.USERS.BASE);
    return data;
  },

  getById: async (id: string): Promise<User> => {
    const { data } = await api.get<User>(API_ROUTES.USERS.BY_ID(id));
    return data;
  },

  create: async (request: CreateUserRequest): Promise<User> => {
    const { data } = await api.post<User>(API_ROUTES.USERS.BASE, request);
    return data;
  },

  update: async (id: string, request: UpdateUserRequest): Promise<User> => {
    const { data } = await api.put<User>(API_ROUTES.USERS.BY_ID(id), request);
    return data;
  },

  delete: async (id: string): Promise<void> => {
    await api.delete(API_ROUTES.USERS.BY_ID(id));
  },
};
