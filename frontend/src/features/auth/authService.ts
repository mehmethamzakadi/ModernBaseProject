import { api } from '../../lib/axios';
import { API_ROUTES } from '../../constants';
import type { LoginRequest, LoginResponse, RefreshTokenRequest } from '../../types';

export const authService = {
  login: async (credentials: LoginRequest): Promise<LoginResponse> => {
    const { data } = await api.post<LoginResponse>(API_ROUTES.AUTH.LOGIN, credentials);
    return data;
  },

  refresh: async (request: RefreshTokenRequest): Promise<LoginResponse> => {
    const { data } = await api.post<LoginResponse>(API_ROUTES.AUTH.REFRESH, request);
    return data;
  },
};
