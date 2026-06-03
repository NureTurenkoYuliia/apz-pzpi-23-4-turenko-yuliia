import { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import toast from 'react-hot-toast';
import { adminApi } from '../../api/admin';
import { SystemSettingsDto } from '../../types';
import LoadingSpinner from '../../components/common/LoadingSpinner';

const schema = z.object({
  maxAquariumsPerUser: z.coerce.number().int().min(1).max(1000),
  maxDevicesPerAquarium: z.coerce.number().int().min(1).max(1000),
  maxAlarmRulesPerDevice: z.coerce.number().int().min(1).max(1000),
  maxScheduledCommandsPerDevice: z.coerce.number().int().min(1).max(1000),
});

type FormValues = z.infer<typeof schema>;

const FIELDS: Array<{ key: keyof FormValues; label: string }> = [
  { key: 'maxAquariumsPerUser', label: 'settings.maxAquariumsPerUser' },
  { key: 'maxDevicesPerAquarium', label: 'settings.maxDevicesPerAquarium' },
  { key: 'maxAlarmRulesPerDevice', label: 'settings.maxAlarmRulesPerDevice' },
  { key: 'maxScheduledCommandsPerDevice', label: 'settings.maxScheduledCommandsPerDevice' },
];

const SystemSettingsPage = () => {
  const { t } = useTranslation();
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);

  const { register, handleSubmit, reset, formState: { errors } } = useForm<FormValues>({
    resolver: zodResolver(schema),
  });

  useEffect(() => {
    adminApi
      .getSystemSettings()
      .then((data: SystemSettingsDto) => reset(data))
      .catch(() => toast.error(t('settings.loadError')))
      .finally(() => setLoading(false));
  }, []);

  const onSubmit = async (values: FormValues) => {
    setSaving(true);
    try {
      await adminApi.updateSystemSettings(values);
      toast.success(t('settings.saveSuccess'));
    } catch (err: unknown) {
      const msg =
        (err as { response?: { data?: { message?: string } } })?.response?.data?.message ??
        t('common.error');
      toast.error(msg);
    } finally {
      setSaving(false);
    }
  };

  if (loading) return <LoadingSpinner fullPage />;

  return (
    <div>
      <div className="mb-6">
        <h1 className="font-display text-3xl text-primary">{t('settings.title')}</h1>
      </div>

      <div className="bg-white rounded-2xl shadow-card p-8 max-w-lg">
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-5">
          {FIELDS.map(({ key, label }) => (
            <div key={key}>
              <label className="block text-sm font-body font-medium text-primary/70 mb-1.5">
                {t(label)}
              </label>
              <input
                {...register(key)}
                type="number"
                min={1}
                className="w-full border border-secondary/40 rounded-xl px-4 py-2.5 text-sm font-body text-primary focus:outline-none focus:border-primary transition-colors"
              />
              {errors[key] && (
                <p className="text-red-500 text-xs font-body mt-1">{errors[key]?.message}</p>
              )}
            </div>
          ))}

          <button
            type="submit"
            disabled={saving}
            className="w-full bg-primary text-white font-body font-medium py-2.5 rounded-xl hover:bg-primary-light transition-colors disabled:opacity-60 mt-2"
          >
            {saving ? t('settings.saving') : t('settings.save')}
          </button>
        </form>
      </div>
    </div>
  );
};

export default SystemSettingsPage;
