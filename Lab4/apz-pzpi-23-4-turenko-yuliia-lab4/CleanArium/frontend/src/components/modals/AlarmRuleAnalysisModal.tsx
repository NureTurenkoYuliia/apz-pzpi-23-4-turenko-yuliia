import { useState } from 'react';
import { useTranslation } from 'react-i18next';
import DatePicker from 'react-datepicker';
import 'react-datepicker/dist/react-datepicker.css';
import { subDays } from 'date-fns';
import { alarmRuleApi } from '../../api/alarmRule';
import { AlarmRuleAnalysisDto } from '../../types';
import FormModal from '../common/FormModal';
import LoadingSpinner from '../common/LoadingSpinner';
import toast from 'react-hot-toast';

interface Props {
  ruleId: number | null;
  onClose: () => void;
}

const AlarmRuleAnalysisModal = ({ ruleId, onClose }: Props) => {
  const { t } = useTranslation();
  const [from, setFrom] = useState<Date>(subDays(new Date(), 30));
  const [to, setTo] = useState<Date>(new Date());
  const [result, setResult] = useState<AlarmRuleAnalysisDto | null>(null);
  const [loading, setLoading] = useState(false);

  const run = async () => {
    if (!ruleId) return;
    setLoading(true);
    try {
      const data = await alarmRuleApi.analyze(ruleId, from, to);
      setResult(data);
    } catch {
      toast.error(t('alarmRule.analysis.loadError'));
    } finally {
      setLoading(false);
    }
  };

  const handleClose = () => { setResult(null); onClose(); };

  return (
    <FormModal isOpen={!!ruleId} title={t('alarmRule.analysis.title')} onClose={handleClose}>
      <div className="space-y-4">
        <div className="flex flex-wrap gap-4">
          <div className="flex items-center gap-2">
            <label className="text-sm font-body text-primary/60">{t('alarmRule.analysis.from')}</label>
            <DatePicker selected={from} onChange={d => d && setFrom(d)} selectsStart startDate={from} endDate={to}
              className="border border-secondary/40 rounded-lg px-3 py-1.5 text-sm font-body text-primary focus:outline-none focus:border-primary w-32" dateFormat="yyyy-MM-dd" />
          </div>
          <div className="flex items-center gap-2">
            <label className="text-sm font-body text-primary/60">{t('alarmRule.analysis.to')}</label>
            <DatePicker selected={to} onChange={d => d && setTo(d)} selectsEnd startDate={from} endDate={to} minDate={from}
              className="border border-secondary/40 rounded-lg px-3 py-1.5 text-sm font-body text-primary focus:outline-none focus:border-primary w-32" dateFormat="yyyy-MM-dd" />
          </div>
        </div>

        <button onClick={run} disabled={loading}
          className="px-4 py-1.5 bg-primary text-white text-sm font-medium rounded-xl hover:bg-primary-light transition-colors disabled:opacity-50">
          {loading ? t('common.loading') : t('alarmRule.analysis.run')}
        </button>

        {loading && <LoadingSpinner />}

        {!loading && result && (
          <dl className="space-y-3 pt-2 border-t border-background-muted">
            {([
              ['alarmRule.analysis.avgValue', result.averageValue.toFixed(2)],
              ['alarmRule.analysis.trendPerDay', result.trendPerDay.toFixed(4)],
              ['alarmRule.analysis.estimatedDays', String(result.estimatedDaysToTrigger)],
              ['alarmRule.analysis.recommendation', result.recommendation],
            ] as [string, string][]).map(([key, val]) => (
              <div key={key} className="flex justify-between gap-4 text-sm">
                <dt className="text-primary/50 font-body shrink-0">{t(key)}</dt>
                <dd className="font-body text-primary text-right">{val}</dd>
              </div>
            ))}
          </dl>
        )}

        {!loading && !result && (
          <p className="text-sm text-primary/40 font-body text-center py-4">{t('alarmRule.analysis.noData')}</p>
        )}
      </div>
    </FormModal>
  );
};

export default AlarmRuleAnalysisModal;
