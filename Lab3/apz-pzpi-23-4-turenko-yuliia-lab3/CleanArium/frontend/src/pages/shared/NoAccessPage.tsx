import { useTranslation } from 'react-i18next';
import { useNavigate } from 'react-router-dom';
import { ShieldOff, LogOut } from 'lucide-react';
import { useAuth } from '../../store/AuthContext';
import toast from 'react-hot-toast';

const NoAccessPage = () => {
  const { t } = useTranslation();
  const { logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = async () => {
    await logout();
    toast.success(t('auth.logoutSuccess'));
    navigate('/login');
  };

  return (
    <div className="min-h-screen bg-background flex items-center justify-center px-4">
      <div className="absolute inset-0 overflow-hidden pointer-events-none">
        <div className="absolute top-1/4 left-1/4 w-64 h-64 bg-accent/5 rounded-full blur-3xl" />
        <div className="absolute bottom-1/4 right-1/4 w-64 h-64 bg-secondary/10 rounded-full blur-3xl" />
      </div>

      <div className="relative text-center max-w-md">
        <div className="w-20 h-20 bg-accent/10 rounded-2xl flex items-center justify-center mx-auto mb-6">
          <ShieldOff className="w-10 h-10 text-accent" />
        </div>
        <h1 className="font-display text-4xl text-primary mb-3">{t('access.denied')}</h1>
        <p className="font-body text-primary/60 leading-relaxed mb-2">
          {t('access.deniedMessage')}
        </p>
        <p className="font-body text-sm text-primary/40 mb-8">{t('access.contactAdmin')}</p>
        <button
          onClick={handleLogout}
          className="inline-flex items-center gap-2 px-6 py-2.5 bg-primary text-white font-body font-medium rounded-xl hover:bg-primary-light transition-colors"
        >
          <LogOut className="w-4 h-4" />
          {t('nav.logout')}
        </button>
      </div>
    </div>
  );
};

export default NoAccessPage;
