interface Props {
  fullPage?: boolean;
  size?: 'sm' | 'md' | 'lg';
}

const sizes = { sm: 'w-5 h-5', md: 'w-8 h-8', lg: 'w-12 h-12' };

const LoadingSpinner = ({ fullPage = false, size = 'md' }: Props) => {
  const spinner = (
    <div
      className={`${sizes[size]} border-2 border-secondary/30 border-t-primary rounded-full animate-spin`}
    />
  );

  if (fullPage) {
    return (
      <div className="min-h-screen bg-background flex items-center justify-center">
        {spinner}
      </div>
    );
  }

  return (
    <div className="flex justify-center items-center py-10">{spinner}</div>
  );
};

export default LoadingSpinner;
