import { useTranslation } from 'react-i18next';
import { AlertTriangle } from 'lucide-react';

interface Props {
  isOpen: boolean;
  message: string;
  onConfirm: () => void;
  onCancel: () => void;
  danger?: boolean;
  loading?: boolean;
}

const ConfirmModal = ({ isOpen, message, onConfirm, onCancel, danger = false, loading = false }: Props) => {
  const { t } = useTranslation();
  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center">
      <div className="absolute inset-0 bg-primary/40 backdrop-blur-sm" onClick={onCancel} />
      <div className="relative bg-white rounded-2xl shadow-modal p-6 w-full max-w-md mx-4 animate-[fadeInUp_0.18s_ease]">
        <div className="flex items-start gap-4">
          <div className={`flex-shrink-0 w-10 h-10 rounded-full flex items-center justify-center ${danger ? 'bg-red-100' : 'bg-secondary/20'}`}>
            <AlertTriangle className={`w-5 h-5 ${danger ? 'text-red-500' : 'text-primary'}`} />
          </div>
          <p className="text-primary/80 font-body text-sm leading-relaxed pt-2">{message}</p>
        </div>
        <div className="flex justify-end gap-3 mt-6">
          <button
            onClick={onCancel}
            className="px-4 py-2 text-sm font-body font-medium text-primary/70 hover:text-primary transition-colors"
          >
            {t('common.cancel')}
          </button>
          <button
            onClick={onConfirm}
            disabled={loading}
            className={`px-5 py-2 text-sm font-body font-medium rounded-xl text-white transition-all ${
              danger
                ? 'bg-red-500 hover:bg-red-600 disabled:bg-red-300'
                : 'bg-primary hover:bg-primary-light disabled:bg-secondary'
            }`}
          >
            {loading ? t('common.loading') : t('common.confirm')}
          </button>
        </div>
      </div>
    </div>
  );
};

export default ConfirmModal;
