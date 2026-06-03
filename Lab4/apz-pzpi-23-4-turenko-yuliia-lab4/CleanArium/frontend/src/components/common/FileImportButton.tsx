import { useRef } from 'react';
import { Upload } from 'lucide-react';
import { useTranslation } from 'react-i18next';

interface Props {
  onFile: (file: File) => void;
  accept?: string;
  label?: string;
  loading?: boolean;
}

const FileImportButton = ({ onFile, accept = '.json,.csv', label, loading = false }: Props) => {
  const { t } = useTranslation();
  const ref = useRef<HTMLInputElement>(null);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file) {
      onFile(file);
      e.target.value = '';
    }
  };

  return (
    <>
      <input ref={ref} type="file" accept={accept} className="hidden" onChange={handleChange} />
      <button
        onClick={() => ref.current?.click()}
        disabled={loading}
        className="inline-flex items-center gap-1.5 px-3 py-1.5 text-sm font-medium text-primary border border-secondary/40 rounded-xl hover:bg-background transition-colors disabled:opacity-50"
      >
        <Upload className="w-3.5 h-3.5" />
        {label ?? t('common.import')}
      </button>
    </>
  );
};

export default FileImportButton;
