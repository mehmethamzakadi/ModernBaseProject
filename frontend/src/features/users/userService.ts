import { api } from '../../lib/axios';
import type { User, CreateUserRequest, UpdateUserRequest } from '../../types';

export const userService = {
  getAll: async (): Promise<User[]> => {
    const { data } = await api.get<User[]>('/users');
    return data;
  },

  getById: async (id: string): Promise<User> => {
    const { data } = await api.get<User>(`/users/${id}`);
    return data;
  },

  create: async (request: CreateUserRequest): Promise<User> => {
    const { data } = await api.post<User>('/users', request);
    return data;
  },

  update: async (id: string, request: UpdateUserRequest): Promise<User> => {
    const { data } = await api.put<User>(`/users/${id}`, request);
    return data;
  },

  delete: async (id: string): Promise<void> => {
    await api.delete(`/users/${id}`);
  },
};
