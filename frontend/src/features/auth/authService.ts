import { api } from '../../lib/axios';
import type { LoginRequest, LoginResponse, RefreshTokenRequest } from '../../types';

export const authService = {
  login: async (credentials: LoginRequest): Promise<LoginResponse> => {
    const { data } = await api.post<LoginResponse>('/auth/login', credentials);
    return data;
  },

  refresh: async (request: RefreshTokenRequest): Promise<LoginResponse> => {
    const { data } = await api.post<LoginResponse>('/auth/refresh', request);
    return data;
  },
};
