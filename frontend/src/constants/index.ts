/**
 * Application constants
 */

// API Routes
export const API_ROUTES = {
  AUTH: {
    LOGIN: "/auth/login",
    REFRESH: "/auth/refresh",
  },
  USERS: {
    BASE: "/users",
    BY_ID: (id: string) => `/users/${id}`,
  },
  FILES: {
    UPLOAD: "/files/upload",
  },
  ROLES: {
    BASE: "/roles",
  },
} as const;

// LocalStorage Keys
export const STORAGE_KEYS = {
  ACCESS_TOKEN: "accessToken",
  REFRESH_TOKEN: "refreshToken",
} as const;

// Application Routes
export const APP_ROUTES = {
  LOGIN: "/login",
  DASHBOARD: "/dashboard",
  USERS: "/users",
  ROLES: "/roles",
} as const;

// HTTP Headers
export const HTTP_HEADERS = {
  AUTHORIZATION: "Authorization",
  CONTENT_TYPE: "application/json",
  BEARER_PREFIX: "Bearer ",
} as const;

// HTTP Status Codes
export const HTTP_STATUS = {
  UNAUTHORIZED: 401,
} as const;

// SignalR
export const SIGNALR = {
  HUB_PATH: "/hubs/notifications",
  ACCESS_TOKEN_QUERY_PARAM: "access_token",
} as const;

// Default URLs
export const DEFAULT_URLS = {
  API_URL: "http://localhost:5000/api",
  SIGNALR_URL: "http://localhost:5000/hubs/notifications",
} as const;

// Permissions (should match backend Permissions constants)
export const PERMISSIONS = {
  USER_CREATE: "User.Create",
  USER_READ: "User.Read",
  USER_UPDATE: "User.Update",
  USER_DELETE: "User.Delete",
} as const;
