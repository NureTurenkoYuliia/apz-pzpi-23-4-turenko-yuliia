import { ReactNode } from 'react';
import { X } from 'lucide-react';
import { useTranslation } from 'react-i18next';

interface Props {
  isOpen: boolean;
  title: string;
  onClose: () => void;
  children: ReactNode;
  width?: string;
}

const FormModal = ({ isOpen, title, onClose, children, width = 'max-w-md' }: Props) => {
  const { t } = useTranslation();
  if (!isOpen) return null;
  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center">
      <div className="absolute inset-0 bg-primary/40 backdrop-blur-sm" onClick={onClose} />
      <div
        className={`relative bg-white rounded-2xl shadow-modal p-6 w-full ${width} mx-4 max-h-[90vh] overflow-y-auto animate-[fadeInUp_0.18s_ease]`}
      >
        <div className="flex items-center justify-between mb-5">
          <h2 className="font-display text-xl text-primary">{title}</h2>
          <button
            onClick={onClose}
            aria-label={t('common.close')}
            className="text-primary/30 hover:text-primary transition-colors"
          >
            <X className="w-5 h-5" />
          </button>
        </div>
        {children}
      </div>
    </div>
  );
};

export default FormModal;
