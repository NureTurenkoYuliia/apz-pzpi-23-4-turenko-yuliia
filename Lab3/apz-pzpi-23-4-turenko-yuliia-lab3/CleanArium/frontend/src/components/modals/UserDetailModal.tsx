import { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { X, User } from 'lucide-react';
import { userApi } from '../../api/users';
import { UserDto } from '../../types';
import LoadingSpinner from '../common/LoadingSpinner';

interface Props {
  userId: number | null;
  onClose: () => void;
}

const UserDetailModal = ({ userId, onClose }: Props) => {
  const { t } = useTranslation();
  const [user, setUser] = useState<UserDto | null>(null);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (!userId) return;
    setLoading(true);
    userApi
      .getUserById(userId)
      .then(setUser)
      .finally(() => setLoading(false));
  }, [userId]);

  if (!userId) return null;

  const fmt = (d: string | null) =>
    d ? new Date(d).toLocaleString() : t('users.never');

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center">
      <div className="absolute inset-0 bg-primary/40 backdrop-blur-sm" onClick={onClose} />
      <div className="relative bg-white rounded-2xl shadow-modal p-6 w-full max-w-md mx-4">
        <div className="flex items-center justify-between mb-5">
          <div className="flex items-center gap-2">
            <User className="w-5 h-5 text-primary" />
            <h2 className="font-display text-lg text-primary">{t('userDetail.title')}</h2>
          </div>
          <button onClick={onClose} className="text-primary/40 hover:text-primary transition-colors">
            <X className="w-5 h-5" />
          </button>
        </div>

        {loading ? (
          <LoadingSpinner />
        ) : user ? (
          <dl className="space-y-3">
            {([
              ['userDetail.id', String(user.id)],
              ['userDetail.email', user.email],
              ['userDetail.status', user.isBlocked ? t('users.blocked') : t('users.active')],
              ['userDetail.lastLogin', fmt(user.lastLoginAt)],
              ['userDetail.createdAt', fmt(user.createdAt)],
            ] as [string, string][]).map(([key, val]) => (
              <div key={key} className="flex justify-between text-sm">
                <dt className="text-primary/50 font-body">{t(key)}</dt>
                <dd
                  className={`font-body font-medium ${
                    key === 'userDetail.status'
                      ? user.isBlocked
                        ? 'text-red-500'
                        : 'text-green-600'
                      : 'text-primary'
                  }`}
                >
                  {val}
                </dd>
              </div>
            ))}
          </dl>
        ) : (
          <p className="text-sm text-primary/50 font-body">{t('common.error')}</p>
        )}
      </div>
    </div>
  );
};

export default UserDetailModal;
