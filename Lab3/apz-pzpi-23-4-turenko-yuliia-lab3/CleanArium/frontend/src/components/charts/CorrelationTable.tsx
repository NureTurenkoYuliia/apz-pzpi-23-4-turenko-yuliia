import { useState } from 'react';
import { useTranslation } from 'react-i18next';
import DatePicker from 'react-datepicker';
import 'react-datepicker/dist/react-datepicker.css';
import { subDays } from 'date-fns';
import { adminApi } from '../../api/admin';
import { CommandAlarmCorrelationDto } from '../../types';
import LoadingSpinner from '../common/LoadingSpinner';
import toast from 'react-hot-toast';

const CorrelationTable = () => {
  const { t } = useTranslation();
  const [from, setFrom] = useState<Date>(subDays(new Date(), 30));
  const [to, setTo] = useState<Date>(new Date());
  const [data, setData] = useState<CommandAlarmCorrelationDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [fetched, setFetched] = useState(false);

  const fetchData = async () => {
    setLoading(true);
    try {
      const result = await adminApi.getCorrelation(from, to);
      setData(result);
      setFetched(true);
    } catch {
      toast.error(t('analytics.loadError'));
    } finally {
      setLoading(false);
    }
  };

  const formatDelay = (ts: string) => {
    return ts ?? '—';
  };

  const cmdLabel = (type: number) =>
    t(`analytics.commandTypes.${type}`, { defaultValue: String(type) });

  return (
    <div className="bg-white rounded-2xl shadow-card p-6">
      <h2 className="font-display text-xl text-primary mb-4">{t('analytics.correlation')}</h2>

      <div className="flex flex-wrap items-center gap-4 mb-6">
        <div className="flex items-center gap-2">
          <label className="text-sm font-body text-primary/60">{t('analytics.from')}</label>
          <DatePicker
            selected={from}
            onChange={(d) => d && setFrom(d)}
            selectsStart
            startDate={from}
            endDate={to}
            className="border border-secondary/40 rounded-lg px-3 py-1.5 text-sm font-body text-primary focus:outline-none focus:border-primary w-36"
            dateFormat="yyyy-MM-dd"
          />
        </div>
        <div className="flex items-center gap-2">
          <label className="text-sm font-body text-primary/60">{t('analytics.to')}</label>
          <DatePicker
            selected={to}
            onChange={(d) => d && setTo(d)}
            selectsEnd
            startDate={from}
            endDate={to}
            minDate={from}
            className="border border-secondary/40 rounded-lg px-3 py-1.5 text-sm font-body text-primary focus:outline-none focus:border-primary w-36"
            dateFormat="yyyy-MM-dd"
          />
        </div>
        <button
          onClick={fetchData}
          disabled={loading}
          className="px-4 py-1.5 bg-primary text-white text-sm font-body font-medium rounded-xl hover:bg-primary-light transition-colors disabled:opacity-50"
        >
          {loading ? t('common.loading') : t('analytics.fetch')}
        </button>
      </div>

      {loading ? (
        <LoadingSpinner />
      ) : fetched && data.length === 0 ? (
        <p className="text-sm text-primary/40 font-body text-center py-8">{t('analytics.noData')}</p>
      ) : fetched ? (
        <div className="overflow-x-auto">
          <table className="w-full text-sm font-body">
            <thead>
              <tr className="border-b border-background-muted">
                {[
                  'analytics.deviceId',
                  'analytics.commandType',
                  'analytics.commandCount',
                  'analytics.alarmCount',
                  'analytics.avgDelay',
                  'analytics.recommendation',
                ].map((key) => (
                  <th key={key} className="text-left py-3 px-3 text-primary/50 font-medium text-xs uppercase tracking-wide">
                    {t(key)}
                  </th>
                ))}
              </tr>
            </thead>
            <tbody>
              {data.map((row, i) => (
                <tr key={i} className="border-b border-background/60 hover:bg-background/50 transition-colors">
                  <td className="py-3 px-3 font-mono text-primary">{row.deviceId}</td>
                  <td className="py-3 px-3 text-primary">{cmdLabel(row.commandType)}</td>
                  <td className="py-3 px-3 text-primary">{row.commandCount}</td>
                  <td className="py-3 px-3 text-primary">{row.alarmCount}</td>
                  <td className="py-3 px-3 font-mono text-primary/70">{formatDelay(row.avgDelayBetweenCommandAndAlarm)}</td>
                  <td className="py-3 px-3 text-primary/70 max-w-xs">{row.recommendation}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      ) : null}
    </div>
  );
};

export default CorrelationTable;
