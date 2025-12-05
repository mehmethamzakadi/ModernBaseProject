import { create } from "zustand";
import { STORAGE_KEYS } from "../constants";
import type { User } from "../types";

interface AuthState {
  user: User | null;
  accessToken: string | null;
  isAuthenticated: boolean;
  setAuth: (user: User, accessToken: string, refreshToken: string) => void;
  logout: () => void;
  hasPermission: (permission: string) => boolean;
}

export const useAuthStore = create<AuthState>((set) => ({
  user: null,
  accessToken: localStorage.getItem(STORAGE_KEYS.ACCESS_TOKEN),
  isAuthenticated: !!localStorage.getItem(STORAGE_KEYS.ACCESS_TOKEN),

  setAuth: (user, accessToken, refreshToken) => {
    localStorage.setItem(STORAGE_KEYS.ACCESS_TOKEN, accessToken);
    localStorage.setItem(STORAGE_KEYS.REFRESH_TOKEN, refreshToken);
    set({ user, accessToken, isAuthenticated: true });
  },

  logout: () => {
    localStorage.clear();
    set({ user: null, accessToken: null, isAuthenticated: false });
  },

  hasPermission: () => {
    return true; // Şimdilik basit implementasyon
  },
}));
