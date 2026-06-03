import { useTranslation } from 'react-i18next';
import { Users, UserMinus, BarChart2 } from 'lucide-react';
import { Link } from 'react-router-dom';

const cards = [
  { icon: Users, label: 'nav.users', to: '/admin/users', color: 'bg-secondary/20 text-primary' },
  { icon: UserMinus, label: 'nav.inactiveUsers', to: '/admin/inactive-users', color: 'bg-accent/10 text-accent' },
  { icon: BarChart2, label: 'nav.analytics', to: '/admin/analytics', color: 'bg-primary/10 text-primary' },
];

const ModeratorDashboard = () => {
  const { t } = useTranslation();

  return (
    <div>
      <div className="mb-8">
        <h1 className="font-display text-3xl text-primary">{t('dashboard.moderatorTitle')}</h1>
        <p className="font-body text-primary/50 mt-1">{t('dashboard.welcome')}</p>
      </div>

      <div className="grid grid-cols-2 gap-4 sm:grid-cols-3">
        {cards.map(({ icon: Icon, label, to, color }) => (
          <Link
            key={to}
            to={to}
            className="bg-white rounded-2xl shadow-card p-6 flex flex-col items-start gap-4 hover:shadow-card-hover transition-shadow group"
          >
            <div className={`w-10 h-10 rounded-xl flex items-center justify-center ${color}`}>
              <Icon className="w-5 h-5" />
            </div>
            <span className="font-body font-medium text-primary group-hover:text-primary-light transition-colors">
              {t(label)}
            </span>
          </Link>
        ))}
      </div>
    </div>
  );
};

export default ModeratorDashboard;
