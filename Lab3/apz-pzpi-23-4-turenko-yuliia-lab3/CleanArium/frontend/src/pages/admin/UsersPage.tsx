import { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { MoreHorizontal, ShieldCheck, Ban, CheckCircle, Trash2, Eye } from 'lucide-react';
import toast from 'react-hot-toast';
import { userApi } from '../../api/users';
import { adminApi } from '../../api/admin';
import { PreviewUserDto } from '../../types';
import { useAuth, UserRole } from '../../store/AuthContext';
import LoadingSpinner from '../../components/common/LoadingSpinner';
import ConfirmModal from '../../components/modals/ConfirmModal';
import UserDetailModal from '../../components/modals/UserDetailModal';

type ActionType = 'block' | 'unblock' | 'delete' | 'makeModerator';

interface PendingAction {
  type: ActionType;
  userId: number;
}

const UsersPage = () => {
  const { t } = useTranslation();
  const { user: currentUser } = useAuth();
  const isAdmin = currentUser?.role === UserRole.Admin;

  const [users, setUsers] = useState<PreviewUserDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [actionLoading, setActionLoading] = useState(false);
  const [pending, setPending] = useState<PendingAction | null>(null);
  const [detailUserId, setDetailUserId] = useState<number | null>(null);
  const [openMenuId, setOpenMenuId] = useState<number | null>(null);

  useEffect(() => {
    load();
  }, []);

  const load = async () => {
    setLoading(true);
    try {
      const data = await userApi.getAllUsers();
      setUsers(data);
    } catch {
      toast.error(t('users.loadError'));
    } finally {
      setLoading(false);
    }
  };

  const handleConfirm = async () => {
    if (!pending) return;
    setActionLoading(true);
    try {
      const { type, userId } = pending;
      if (type === 'block') await userApi.blockUser(userId);
      else if (type === 'unblock') await userApi.unblockUser(userId);
      else if (type === 'delete') await userApi.deleteUser(userId);
      else if (type === 'makeModerator') await adminApi.makeModerator(userId);

      const successKey: Record<ActionType, string> = {
        block: 'users.blockSuccess',
        unblock: 'users.unblockSuccess',
        delete: 'users.deleteSuccess',
        makeModerator: 'users.makeModeratorSuccess',
      };
      toast.success(t(successKey[type]));
      if (type === 'delete') {
        setUsers((prev) => prev.filter((u) => u.userId !== userId));
      }
    } catch (err: unknown) {
      const msg =
        (err as { response?: { data?: { message?: string } } })?.response?.data?.message ??
        t('common.error');
      toast.error(msg);
    } finally {
      setActionLoading(false);
      setPending(null);
    }
  };

  const confirmMessages: Record<ActionType, string> = {
    block: t('users.confirmBlock'),
    unblock: t('users.confirmUnblock'),
    delete: t('users.confirmDelete'),
    makeModerator: t('users.confirmMakeModerator'),
  };

  const fmt = (d: string | null) =>
    d ? new Date(d).toLocaleDateString() : t('users.never');

  return (
    <div>
      <div className="mb-6">
        <h1 className="font-display text-3xl text-primary">{t('users.title')}</h1>
      </div>

      {loading ? (
        <LoadingSpinner />
      ) : users.length === 0 ? (
        <p className="text-sm font-body text-primary/40 py-8 text-center">{t('users.noUsers')}</p>
      ) : (
        <div className="bg-white rounded-2xl shadow-card overflow-hidden">
          <table className="w-full text-sm font-body">
            <thead>
              <tr className="border-b border-background-muted">
                {['users.userId', 'users.email', 'users.lastLogin', 'users.actions'].map((k) => (
                  <th key={k} className="text-left px-5 py-4 text-primary/50 font-medium text-xs uppercase tracking-wide">
                    {t(k)}
                  </th>
                ))}
              </tr>
            </thead>
            <tbody>
              {users.map((u) => (
                <tr key={u.userId} className="border-b border-background/60 hover:bg-background/40 transition-colors">
                  <td className="px-5 py-3.5 font-mono text-primary/70">{u.userId}</td>
                  <td className="px-5 py-3.5 text-primary">{u.email}</td>
                  <td className="px-5 py-3.5 text-primary/60">{fmt(u.lastLoginAt)}</td>
                  <td className="px-5 py-3.5">
                    <div className="relative">
                      <button
                        onClick={() => setOpenMenuId(openMenuId === u.userId ? null : u.userId)}
                        className="p-1.5 rounded-lg hover:bg-background text-primary/40 hover:text-primary transition-colors"
                      >
                        <MoreHorizontal className="w-4 h-4" />
                      </button>

                      {openMenuId === u.userId && (
                        <>
                          <div className="fixed inset-0 z-10" onClick={() => setOpenMenuId(null)} />
                          <div className="absolute right-0 top-8 z-20 bg-white rounded-xl shadow-modal border border-background-muted py-1 min-w-44">
                            <button
                              onClick={() => { setDetailUserId(u.userId); setOpenMenuId(null); }}
                              className="flex items-center gap-2.5 w-full px-4 py-2 text-sm text-primary hover:bg-background transition-colors"
                            >
                              <Eye className="w-3.5 h-3.5" /> {t('users.viewDetails')}
                            </button>
                            <button
                              onClick={() => { setPending({ type: 'block', userId: u.userId }); setOpenMenuId(null); }}
                              className="flex items-center gap-2.5 w-full px-4 py-2 text-sm text-primary hover:bg-background transition-colors"
                            >
                              <Ban className="w-3.5 h-3.5" /> {t('users.block')}
                            </button>
                            <button
                              onClick={() => { setPending({ type: 'unblock', userId: u.userId }); setOpenMenuId(null); }}
                              className="flex items-center gap-2.5 w-full px-4 py-2 text-sm text-primary hover:bg-background transition-colors"
                            >
                              <CheckCircle className="w-3.5 h-3.5" /> {t('users.unblock')}
                            </button>
                            {isAdmin && (
                              <>
                                <button
                                  onClick={() => { setPending({ type: 'makeModerator', userId: u.userId }); setOpenMenuId(null); }}
                                  className="flex items-center gap-2.5 w-full px-4 py-2 text-sm text-primary hover:bg-background transition-colors"
                                >
                                  <ShieldCheck className="w-3.5 h-3.5" /> {t('users.makeModerator')}
                                </button>
                                <div className="my-1 border-t border-background-muted" />
                                <button
                                  onClick={() => { setPending({ type: 'delete', userId: u.userId }); setOpenMenuId(null); }}
                                  className="flex items-center gap-2.5 w-full px-4 py-2 text-sm text-red-500 hover:bg-red-50 transition-colors"
                                >
                                  <Trash2 className="w-3.5 h-3.5" /> {t('users.delete')}
                                </button>
                              </>
                            )}
                          </div>
                        </>
                      )}
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      <ConfirmModal
        isOpen={!!pending}
        message={pending ? confirmMessages[pending.type] : ''}
        onConfirm={handleConfirm}
        onCancel={() => setPending(null)}
        danger={pending?.type === 'delete'}
        loading={actionLoading}
      />

      <UserDetailModal userId={detailUserId} onClose={() => setDetailUserId(null)} />
    </div>
  );
};

export default UsersPage;
