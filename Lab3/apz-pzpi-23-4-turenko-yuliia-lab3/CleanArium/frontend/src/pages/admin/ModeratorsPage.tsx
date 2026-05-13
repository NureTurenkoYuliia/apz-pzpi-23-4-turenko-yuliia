import { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { ShieldOff } from 'lucide-react';
import toast from 'react-hot-toast';
import { adminApi } from '../../api/admin';
import { ModeratorDto } from '../../types';
import LoadingSpinner from '../../components/common/LoadingSpinner';
import ConfirmModal from '../../components/modals/ConfirmModal';

const ModeratorsPage = () => {
  const { t } = useTranslation();
  const [moderators, setModerators] = useState<ModeratorDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [actionLoading, setActionLoading] = useState(false);
  const [pendingId, setPendingId] = useState<number | null>(null);

  useEffect(() => { load(); }, []);

  const load = async () => {
    setLoading(true);
    try {
      setModerators(await adminApi.getModerators());
    } catch {
      toast.error(t('moderators.loadError'));
    } finally {
      setLoading(false);
    }
  };

  const handleRemove = async () => {
    if (!pendingId) return;
    setActionLoading(true);
    try {
      await adminApi.removeModerator(pendingId);
      setModerators((prev) => prev.filter((m) => m.id !== pendingId));
      toast.success(t('moderators.removeSuccess'));
    } catch (err: unknown) {
      const msg =
        (err as { response?: { data?: { message?: string } } })?.response?.data?.message ??
        t('common.error');
      toast.error(msg);
    } finally {
      setActionLoading(false);
      setPendingId(null);
    }
  };

  const fmt = (d: string | null) =>
    d ? new Date(d).toLocaleDateString() : t('users.never');

  return (
    <div>
      <div className="mb-6">
        <h1 className="font-display text-3xl text-primary">{t('moderators.title')}</h1>
      </div>

      {loading ? (
        <LoadingSpinner />
      ) : moderators.length === 0 ? (
        <p className="text-sm font-body text-primary/40 text-center py-8">{t('moderators.noModerators')}</p>
      ) : (
        <div className="bg-white rounded-2xl shadow-card overflow-hidden">
          <table className="w-full text-sm font-body">
            <thead>
              <tr className="border-b border-background-muted">
                {[
                  'moderators.name',
                  'moderators.email',
                  'moderators.lastLogin',
                  'moderators.createdAt',
                  'moderators.actions',
                ].map((k) => (
                  <th key={k} className="text-left px-5 py-4 text-primary/50 font-medium text-xs uppercase tracking-wide">
                    {t(k)}
                  </th>
                ))}
              </tr>
            </thead>
            <tbody>
              {moderators.map((m) => (
                <tr key={m.id} className="border-b border-background/60 hover:bg-background/40 transition-colors">
                  <td className="px-5 py-3.5 font-medium text-primary">{m.name}</td>
                  <td className="px-5 py-3.5 text-primary">{m.email}</td>
                  <td className="px-5 py-3.5 text-primary/60">{fmt(m.lastLoginAt)}</td>
                  <td className="px-5 py-3.5 text-primary/60">{fmt(m.createdAt)}</td>
                  <td className="px-5 py-3.5">
                    <button
                      onClick={() => setPendingId(m.id)}
                      className="inline-flex items-center gap-1.5 px-3 py-1.5 text-xs font-medium text-red-500 border border-red-200 rounded-lg hover:bg-red-50 transition-colors"
                    >
                      <ShieldOff className="w-3.5 h-3.5" />
                      {t('moderators.removeModerator')}
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      <ConfirmModal
        isOpen={!!pendingId}
        message={t('moderators.confirmRemove')}
        onConfirm={handleRemove}
        onCancel={() => setPendingId(null)}
        danger
        loading={actionLoading}
      />
    </div>
  );
};

export default ModeratorsPage;
