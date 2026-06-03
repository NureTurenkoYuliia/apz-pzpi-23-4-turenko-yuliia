import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import { Toaster } from "react-hot-toast";
import { AuthProvider } from "./store/AuthContext";
import { UserRole } from "./types";

import ProtectedRoute from "./components/common/ProtectedRoute";
import DashboardLayout from "./components/layout/DashboardLayout";
import UserLayout from "./components/layout/UserLayout";

import LoginPage from "./pages/shared/LoginPage";
import RegisterPage from "./pages/shared/RegisterPage";
import NoAccessPage from "./pages/shared/NoAccessPage";

import AdminDashboard from "./pages/admin/AdminDashboard";
import UsersPage from "./pages/admin/UsersPage";
import InactiveUsersPage from "./pages/admin/InactiveUsersPage";
import ModeratorsPage from "./pages/admin/ModeratorsPage";
import SystemSettingsPage from "./pages/admin/SystemSettingsPage";
import AnalyticsPage from "./pages/admin/AnalyticsPage";

import ModeratorDashboard from "./pages/moderator/ModeratorDashboard";

import AquariumsPage from "./pages/user/AquariumsPage";
import DevicesPage from "./pages/user/DevicesPage";
import DeviceDetailPage from "./pages/user/DeviceDetailPage";
import NotificationsPage from "./pages/user/NotificationsPage";

import KubernetesPage from './pages/admin/KubernetesPage';

const ADMIN_MOD = [UserRole.Admin, UserRole.Moderator];
const ADMIN_ONLY = [UserRole.Admin];
const MOD_ONLY = [UserRole.Moderator];
const USER_ONLY = [UserRole.User];

function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Toaster
          position="top-right"
          toastOptions={{
            style: {
              fontFamily: "DM Sans, sans-serif",
              fontSize: "13px",
              borderRadius: "12px",
              color: "#0B2545",
            },
          }}
        />
        <Routes>
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />
          <Route path="/no-access" element={<NoAccessPage />} />

          <Route
            path="/user"
            element={
              <ProtectedRoute allowedRoles={USER_ONLY}>
                <UserLayout />
              </ProtectedRoute>
            }
          >
            <Route index element={<Navigate to="aquariums" replace />} />

            <Route path="aquariums" element={<AquariumsPage />} />
            <Route path="aquariums/:aquariumId" element={<DevicesPage />} />
            <Route
              path="aquariums/:aquariumId/devices/:deviceId"
              element={<DeviceDetailPage />}
            />
            <Route path="notifications" element={<NotificationsPage />} />
          </Route>

          <Route
            path="/admin"
            element={
              <ProtectedRoute allowedRoles={ADMIN_ONLY}>
                <DashboardLayout />
              </ProtectedRoute>
            }
          >
            <Route index element={<Navigate to="dashboard" replace />} />
            <Route path="dashboard" element={<AdminDashboard />} />
            <Route path="moderators" element={<ModeratorsPage />} />
            <Route path="system-settings" element={<SystemSettingsPage />} />
            <Route path="kubernetes" element={<KubernetesPage />} />
          </Route>

          <Route
            path="/admin/users"
            element={
              <ProtectedRoute allowedRoles={ADMIN_MOD}>
                <DashboardLayout />
              </ProtectedRoute>
            }
          >
            <Route index element={<UsersPage />} />
          </Route>

          <Route
            path="/admin/inactive-users"
            element={
              <ProtectedRoute allowedRoles={ADMIN_MOD}>
                <DashboardLayout />
              </ProtectedRoute>
            }
          >
            <Route index element={<InactiveUsersPage />} />
          </Route>

          <Route
            path="/admin/analytics"
            element={
              <ProtectedRoute allowedRoles={ADMIN_MOD}>
                <DashboardLayout />
              </ProtectedRoute>
            }
          >
            <Route index element={<AnalyticsPage />} />
          </Route>

          <Route
            path="/moderator"
            element={
              <ProtectedRoute allowedRoles={MOD_ONLY}>
                <DashboardLayout />
              </ProtectedRoute>
            }
          >
            <Route index element={<Navigate to="dashboard" replace />} />
            <Route path="dashboard" element={<ModeratorDashboard />} />
          </Route>

          <Route path="/" element={<Navigate to="/login" replace />} />
          <Route path="*" element={<Navigate to="/login" replace />} />
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  );
}

export default App;
