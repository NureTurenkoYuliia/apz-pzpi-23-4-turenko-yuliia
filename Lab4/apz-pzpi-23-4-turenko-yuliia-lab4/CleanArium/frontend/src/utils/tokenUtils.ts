import { jwtDecode } from 'jwt-decode';
import { DecodedToken, UserRole } from '../types';

const ROLE_CLAIM = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';
const EMAIL_CLAIM = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress';
const ID_CLAIM = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier';

export interface TokenUser {
  id: string;
  email: string;
  role: UserRole;
}

export const decodeToken = (token: string): TokenUser | null => {
  try {
    const decoded = jwtDecode<DecodedToken>(token);
    const roleRaw = decoded[ROLE_CLAIM as keyof DecodedToken] as string;
    const role = roleRaw as UserRole;
    return {
      id: decoded[ID_CLAIM as keyof DecodedToken] as string,
      email: decoded[EMAIL_CLAIM as keyof DecodedToken] as string,
      role,
    };
  } catch {
    return null;
  }
};

export const isTokenExpired = (token: string): boolean => {
  try {
    const { exp } = jwtDecode<{ exp: number }>(token);
    return Date.now() >= exp * 1000;
  } catch {
    return true;
  }
};

export const saveTokens = (accessToken: string, refreshToken: string) => {
  localStorage.setItem('accessToken', accessToken);
  localStorage.setItem('refreshToken', refreshToken);
};

export const clearTokens = () => {
  localStorage.removeItem('accessToken');
  localStorage.removeItem('refreshToken');
};

export const getAccessToken = () => localStorage.getItem('accessToken');
export const getRefreshToken = () => localStorage.getItem('refreshToken');
