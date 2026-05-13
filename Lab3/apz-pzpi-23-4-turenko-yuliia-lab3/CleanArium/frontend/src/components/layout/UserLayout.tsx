import { useEffect, useState } from 'react';
import { NavLink, Outlet, useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { Bell, LogOut, Layers } from 'lucide-react';
import toast from 'react-hot-toast';
import { useAuth } from '../../store/AuthContext';
import { notificationApi } from '../../api/notification';

const UserLayout = () => {
  const { t, i18n } = useTranslation();
  const { logout } = useAuth();
  const navigate = useNavigate();
  const [unread, setUnread] = useState(0);

  useEffect(() => {
    notificationApi.getUnreadCount().then(setUnread).catch(() => {});
    const interval = setInterval(() => {
      notificationApi.getUnreadCount().then(setUnread).catch(() => {});
    }, 30_000);
    return () => clearInterval(interval);
  }, []);

  const handleLogout = async () => {
    await logout();
    toast.success(t('auth.logoutSuccess'));
    navigate('/login');
  };

  const toggleLang = () => i18n.changeLanguage(i18n.language === 'en' ? 'uk' : 'en');

  return (
    <div className="min-h-screen bg-background font-body flex flex-col">
      <header className="bg-primary shadow-md sticky top-0 z-30">
        <div className="max-w-5xl mx-auto px-6 h-14 flex items-center justify-between">
          <div className="flex items-center gap-2">
            <span className="font-display text-lg text-white tracking-wide">{t('app.name')}</span>
          </div>

          <nav className="flex items-center gap-1">
            <NavLink
              to="aquariums"
              className={({ isActive }) =>
                `flex items-center gap-1.5 px-3 py-1.5 rounded-lg text-sm font-medium transition-colors ${
                  isActive ? 'bg-secondary/20 text-white' : 'text-secondary/70 hover:text-white hover:bg-white/10'
                }`
              }
            >
              <Layers className="w-3.5 h-3.5" />
              {t('userNav.aquariums')}
            </NavLink>

            <NavLink
              to="notifications"
              className={({ isActive }) =>
                `relative flex items-center gap-1.5 px-3 py-1.5 rounded-lg text-sm font-medium transition-colors ${
                  isActive ? 'bg-secondary/20 text-white' : 'text-secondary/70 hover:text-white hover:bg-white/10'
                }`
              }
            >
              <Bell className="w-3.5 h-3.5" />
              {t('userNav.notifications')}
              {unread > 0 && (
                <span className="absolute -top-1 -right-1 min-w-[18px] h-[18px] bg-accent text-white text-[10px] font-bold rounded-full flex items-center justify-center px-1">
                  {unread > 99 ? '99+' : unread}
                </span>
              )}
            </NavLink>
          </nav>

          <div className="flex items-center gap-1">
            <button
              onClick={toggleLang}
              className="px-2.5 py-1.5 rounded-lg text-xs font-mono font-medium text-secondary/70 hover:text-white hover:bg-white/10 transition-colors uppercase tracking-widest"
            >
              {i18n.language === 'en' ? 'EN' : 'UK'}
            </button>
            <button
              onClick={handleLogout}
              className="flex items-center gap-1.5 px-3 py-1.5 rounded-lg text-sm text-secondary/70 hover:text-white hover:bg-white/10 transition-colors"
            >
              <LogOut className="w-3.5 h-3.5" />
              {t('userNav.logout')}
            </button>
          </div>
        </div>
      </header>

      <main className="flex-1">
        <div className="max-w-5xl mx-auto px-6 py-8">
          <Outlet />
        </div>
      </main>
    </div>
  );
};

export default UserLayout;
