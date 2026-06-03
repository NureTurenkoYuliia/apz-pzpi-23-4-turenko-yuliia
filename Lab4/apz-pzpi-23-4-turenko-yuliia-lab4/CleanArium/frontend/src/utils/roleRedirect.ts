import { UserRole } from '../types';

export const getRoleHomePath = (role: UserRole): string => {
  switch (role) {
    case UserRole.Admin:
      return '/admin/dashboard';
    case UserRole.Moderator:
      return '/moderator/dashboard';
    case UserRole.User:
      return '/user/aquariums';
    default:
      return '/no-access';
  }
};

export const canAccess = (role: UserRole, allowedRoles: UserRole[]): boolean => {
  return allowedRoles.includes(role);
};
