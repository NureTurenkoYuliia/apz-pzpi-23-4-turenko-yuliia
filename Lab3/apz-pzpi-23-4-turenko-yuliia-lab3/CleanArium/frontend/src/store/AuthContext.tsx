import React, { createContext, useContext, useEffect, useState, ReactNode } from 'react';
import { UserRole } from '../types';
import { decodeToken, getAccessToken, getRefreshToken, saveTokens, clearTokens, TokenUser } from '../utils/tokenUtils';
import { authApi } from '../api/auth';

interface AuthContextValue {
  user: TokenUser | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  login: (accessToken: string, refreshToken: string) => void;
  logout: () => Promise<void>;
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

export const AuthProvider = ({ children }: { children: ReactNode }) => {
  const [user, setUser] = useState<TokenUser | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const token = getAccessToken();
    if (token) {
      const decoded = decodeToken(token);
      setUser(decoded);
    }
    setIsLoading(false);
  }, []);

  const login = (accessToken: string, refreshToken: string) => {
    saveTokens(accessToken, refreshToken);
    const decoded = decodeToken(accessToken);
    setUser(decoded);
  };

  const logout = async () => {
    const refreshToken = getRefreshToken();
    try {
      if (refreshToken) await authApi.logout({ refreshToken });
    } catch {
      // silently ignore
    } finally {
      clearTokens();
      setUser(null);
    }
  };

  return (
    <AuthContext.Provider value={{ user, isAuthenticated: !!user, isLoading, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth must be used inside AuthProvider');
  return ctx;
};

export { UserRole };
