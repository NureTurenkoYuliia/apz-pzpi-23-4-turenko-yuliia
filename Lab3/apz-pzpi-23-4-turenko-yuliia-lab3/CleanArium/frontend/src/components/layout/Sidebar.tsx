import { NavLink, useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { useAuth } from '../../store/AuthContext';
import { UserRole } from '../../types';
import {
  LayoutDashboard,
  Users,
  UserMinus,
  ShieldCheck,
  Settings,
  BarChart2,
  LogOut,
} from 'lucide-react';
import toast from 'react-hot-toast';

interface NavItem {
  to: string;
  label: string;
  icon: React.ReactNode;
  roles: UserRole[];
}

const Sidebar = () => {
  const { t, i18n } = useTranslation();
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const navItems: NavItem[] = [
    {
      to: '/admin/dashboard',
      label: t('nav.dashboard'),
      icon: <LayoutDashboard className="w-4 h-4" />,
      roles: [UserRole.Admin],
    },
    {
      to: '/moderator/dashboard',
      label: t('nav.dashboard'),
      icon: <LayoutDashboard className="w-4 h-4" />,
      roles: [UserRole.Moderator],
    },
    {
      to: '/admin/users',
      label: t('nav.users'),
      icon: <Users className="w-4 h-4" />,
      roles: [UserRole.Admin, UserRole.Moderator],
    },
    {
      to: '/admin/inactive-users',
      label: t('nav.inactiveUsers'),
      icon: <UserMinus className="w-4 h-4" />,
      roles: [UserRole.Admin, UserRole.Moderator],
    },
    {
      to: '/admin/moderators',
      label: t('nav.moderators'),
      icon: <ShieldCheck className="w-4 h-4" />,
      roles: [UserRole.Admin],
    },
    {
      to: '/admin/system-settings',
      label: t('nav.systemSettings'),
      icon: <Settings className="w-4 h-4" />,
      roles: [UserRole.Admin],
    },
    {
      to: '/admin/analytics',
      label: t('nav.analytics'),
      icon: <BarChart2 className="w-4 h-4" />,
      roles: [UserRole.Admin, UserRole.Moderator],
    },
  ];

  const visibleItems = navItems.filter((item) => user && item.roles.includes(user.role));

  const handleLogout = async () => {
    await logout();
    toast.success(t('auth.logoutSuccess'));
    navigate('/login');
  };

  const toggleLang = () => {
    i18n.changeLanguage(i18n.language === 'en' ? 'uk' : 'en');
  };

  return (
    <aside className="w-60 min-h-screen bg-primary flex flex-col flex-shrink-0">
      <div className="px-6 py-7 border-b border-primary-light/30">
        <div className="flex items-center gap-2.5">
          <span className="font-display text-xl text-white tracking-wide">
            {t('app.name')}
          </span>
        </div>
      </div>

      <nav className="flex-1 px-3 py-5 space-y-0.5">
        {visibleItems.map((item) => (
          <NavLink
            key={item.to}
            to={item.to}
            className={({ isActive }) =>
              `flex items-center gap-3 px-3 py-2.5 rounded-xl text-sm font-body font-medium transition-all ${
                isActive
                  ? 'bg-secondary/20 text-white'
                  : 'text-secondary/70 hover:bg-primary-light/50 hover:text-white'
              }`
            }
          >
            {item.icon}
            {item.label}
          </NavLink>
        ))}
      </nav>

      <div className="px-3 py-4 border-t border-primary-light/30 space-y-1">
        <button
          onClick={toggleLang}
          className="w-full flex items-center gap-3 px-3 py-2.5 rounded-xl text-sm font-body text-secondary/70 hover:bg-primary-light/50 hover:text-white transition-all"
        >
          <span className="text-xs font-mono uppercase tracking-widest">
            {i18n.language === 'en' ? 'EN' : 'UK'}
          </span>
          <span>{t('common.language')}</span>
        </button>

        {user && (
          <div className="px-3 py-1.5">
            <p className="text-xs font-mono text-secondary/40 truncate">{user.email}</p>
          </div>
        )}
        <button
          onClick={handleLogout}
          className="w-full flex items-center gap-3 px-3 py-2.5 rounded-xl text-sm font-body text-secondary/70 hover:bg-accent/20 hover:text-white transition-all"
        >
          <LogOut className="w-4 h-4" />
          {t('nav.logout')}
        </button>
      </div>
    </aside>
  );
};

export default Sidebar;
