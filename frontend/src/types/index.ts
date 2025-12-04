export interface User {
  id: string;
  username: string;
  email: string;
  isActive: boolean;
  createdAt: string;
  roles: Role[];
}

export interface Role {
  id: string;
  name: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  expiresIn: number;
  user: User;
}

export interface RefreshTokenRequest {
  refreshToken: string;
}

export interface CreateUserRequest {
  username: string;
  email: string;
  password: string;
  roleIds: string[];
}

export interface UpdateUserRequest {
  username: string;
  email: string;
  isActive: boolean;
  roleIds: string[];
}

export interface ApiError {
  message: string;
  errors?: Record<string, string[]>;
}
