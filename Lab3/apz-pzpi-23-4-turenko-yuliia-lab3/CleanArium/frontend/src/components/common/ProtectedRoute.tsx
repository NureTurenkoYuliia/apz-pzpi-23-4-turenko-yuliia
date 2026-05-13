import { Navigate } from 'react-router-dom';
import { useAuth } from '../../store/AuthContext';
import { UserRole } from '../../types';
import { getRoleHomePath } from '../../utils/roleRedirect';
import LoadingSpinner from './LoadingSpinner';

interface Props {
  children: React.ReactNode;
  allowedRoles: UserRole[];
}

const ProtectedRoute = ({ children, allowedRoles }: Props) => {
  const { user, isAuthenticated, isLoading } = useAuth();

  if (isLoading) return <LoadingSpinner fullPage />;
  if (!isAuthenticated) return <Navigate to="/login" replace />;

  if (!allowedRoles.includes(user!.role)) {
    return <Navigate to={getRoleHomePath(user!.role)} replace />;
  }

  return <>{children}</>;
};

export default ProtectedRoute;
