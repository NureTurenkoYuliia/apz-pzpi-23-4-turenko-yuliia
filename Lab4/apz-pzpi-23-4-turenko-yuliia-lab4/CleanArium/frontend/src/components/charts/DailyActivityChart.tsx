import { useState } from 'react';
import { useTranslation } from 'react-i18next';
import {
  AreaChart,
  Area,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer,
} from 'recharts';
import { adminApi } from '../../api/admin';
import { UserActivityDailyDto } from '../../types';
import LoadingSpinner from '../common/LoadingSpinner';

const DailyActivityChart = () => {
  const { t } = useTranslation();
  const [days, setDays] = useState(7);
  const [data, setData] = useState<UserActivityDailyDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [fetched, setFetched] = useState(false);

  const fetchData = async () => {
    setLoading(true);
    try {
      const result = await adminApi.getDailyActivity(days);
      setData(result);
      setFetched(true);
    } catch {
      // toast handled globally
    } finally {
      setLoading(false);
    }
  };

  const chartData = data.map((d) => ({
    date: d.date,
    actions: d.actionsCount,
  }));

  return (
    <div className="bg-white rounded-2xl shadow-card p-6">
      <h2 className="font-display text-xl text-primary mb-4">{t('analytics.dailyActivity')}</h2>
      <div className="flex items-center gap-3 mb-6">
        <label className="text-sm font-body text-primary/60">{t('analytics.daysLabel')}</label>
        <input
          type="number"
          min={1}
          max={90}
          value={days}
          onChange={(e) => setDays(Number(e.target.value))}
          className="w-20 border border-secondary/40 rounded-lg px-3 py-1.5 text-sm font-body text-primary focus:outline-none focus:border-primary transition-colors"
        />
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
      ) : fetched && chartData.length === 0 ? (
        <p className="text-sm text-primary/40 font-body text-center py-8">{t('analytics.noData')}</p>
      ) : fetched ? (
        <ResponsiveContainer width="100%" height={240}>
          <AreaChart data={chartData}>
            <defs>
              <linearGradient id="actGrad" x1="0" y1="0" x2="0" y2="1">
                <stop offset="5%" stopColor="#8DA9C4" stopOpacity={0.3} />
                <stop offset="95%" stopColor="#8DA9C4" stopOpacity={0} />
              </linearGradient>
            </defs>
            <CartesianGrid strokeDasharray="3 3" stroke="#EEF4ED" />
            <XAxis dataKey="date" tick={{ fontSize: 11, fontFamily: 'DM Sans' }} />
            <YAxis tick={{ fontSize: 11, fontFamily: 'DM Sans' }} />
            <Tooltip
              contentStyle={{
                borderRadius: '10px',
                border: 'none',
                boxShadow: '0 4px 20px rgba(11,37,69,0.12)',
                fontFamily: 'DM Sans',
                fontSize: '12px',
              }}
            />
            <Area
              type="monotone"
              dataKey="actions"
              stroke="#0B2545"
              strokeWidth={2}
              fill="url(#actGrad)"
            />
          </AreaChart>
        </ResponsiveContainer>
      ) : null}
    </div>
  );
};

export default DailyActivityChart;
