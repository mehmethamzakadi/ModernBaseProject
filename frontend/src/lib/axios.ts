import axios from 'axios';
import {
  API_ROUTES,
  STORAGE_KEYS,
  HTTP_HEADERS,
  HTTP_STATUS,
  APP_ROUTES,
  DEFAULT_URLS,
} from '../constants';

const API_URL = import.meta.env.VITE_API_URL || DEFAULT_URLS.API_URL;

export const api = axios.create({
  baseURL: API_URL,
  headers: {
    'Content-Type': HTTP_HEADERS.CONTENT_TYPE,
  },
});

api.interceptors.request.use((config) => {
  const token = localStorage.getItem(STORAGE_KEYS.ACCESS_TOKEN);
  if (token) {
    config.headers.Authorization = `${HTTP_HEADERS.BEARER_PREFIX}${token}`;
  }
  return config;
});

api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    if (error.response?.status === HTTP_STATUS.UNAUTHORIZED && !originalRequest._retry) {
      originalRequest._retry = true;

      try {
        const refreshToken = localStorage.getItem(STORAGE_KEYS.REFRESH_TOKEN);
        if (!refreshToken) throw new Error('No refresh token');

        const { data } = await axios.post(`${API_URL}${API_ROUTES.AUTH.REFRESH}`, {
          refreshToken,
        });

        localStorage.setItem(STORAGE_KEYS.ACCESS_TOKEN, data.accessToken);
        localStorage.setItem(STORAGE_KEYS.REFRESH_TOKEN, data.refreshToken);

        originalRequest.headers.Authorization = `${HTTP_HEADERS.BEARER_PREFIX}${data.accessToken}`;
        return api(originalRequest);
      } catch {
        localStorage.clear();
        window.location.href = APP_ROUTES.LOGIN;
      }
    }

    return Promise.reject(error);
  }
);
