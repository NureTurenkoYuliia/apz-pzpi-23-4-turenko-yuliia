import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { useTranslation } from 'react-i18next';
import toast from 'react-hot-toast';
import { authApi } from '../../api/auth';
import { useAuth } from '../../store/AuthContext';
import { getRoleHomePath } from '../../utils/roleRedirect';
import { decodeToken } from '../../utils/tokenUtils';
import { UserRole } from '../../types';

const schema = (t: (k: string) => string) =>
  z.object({
    email: z.string().min(1, t('validation.required')).email(t('validation.emailInvalid')),
    password: z
      .string()
      .min(8, t('validation.passwordMin'))
      .max(64, t('validation.passwordMax')),
  });

type FormValues = { email: string; password: string };

const LoginPage = () => {
  const { t } = useTranslation();
  const { login } = useAuth();
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<FormValues>({ resolver: zodResolver(schema(t)) });

  const onSubmit = async (values: FormValues) => {
    setLoading(true);
    try {
      const res = await authApi.login(values);
      login(res.accessToken, res.refreshToken);
      toast.success(t('auth.loginSuccess'));
      const decoded = decodeToken(res.accessToken);
      const role = (decoded?.role ?? UserRole.User) as UserRole;
      navigate(getRoleHomePath(role), { replace: true });
    } catch (err: unknown) {
      const msg =
        (err as { response?: { data?: { message?: string } } })?.response?.data?.message ??
        t('common.error');
      toast.error(msg);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-background flex items-center justify-center px-4">
      <div className="absolute inset-0 overflow-hidden pointer-events-none">
        <div className="absolute -top-32 -right-32 w-96 h-96 bg-secondary/10 rounded-full blur-3xl" />
        <div className="absolute -bottom-32 -left-32 w-80 h-80 bg-primary/5 rounded-full blur-3xl" />
      </div>

      <div className="relative w-full max-w-sm">
        <div className="flex flex-col items-center mb-8">
          <h1 className="font-display text-3xl text-primary">{t('app.name')}</h1>
          <p className="font-body text-sm text-primary/50 mt-1">{t('app.tagline')}</p>
        </div>

        {/* Card */}
        <div className="bg-white rounded-2xl shadow-card p-8">
          <h2 className="font-display text-2xl text-primary mb-6">{t('auth.login')}</h2>

          <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
            <div>
              <label className="block text-sm font-body font-medium text-primary/70 mb-1.5">
                {t('auth.email')}
              </label>
              <input
                {...register('email')}
                type="email"
                autoComplete="email"
                className="w-full border border-secondary/40 rounded-xl px-4 py-2.5 text-sm font-body text-primary placeholder-primary/30 focus:outline-none focus:border-primary transition-colors"
                placeholder="you@example.com"
              />
              {errors.email && (
                <p className="text-red-500 text-xs font-body mt-1">{errors.email.message}</p>
              )}
            </div>

            <div>
              <label className="block text-sm font-body font-medium text-primary/70 mb-1.5">
                {t('auth.password')}
              </label>
              <input
                {...register('password')}
                type="password"
                autoComplete="current-password"
                className="w-full border border-secondary/40 rounded-xl px-4 py-2.5 text-sm font-body text-primary placeholder-primary/30 focus:outline-none focus:border-primary transition-colors"
                placeholder="••••••••"
              />
              {errors.password && (
                <p className="text-red-500 text-xs font-body mt-1">{errors.password.message}</p>
              )}
            </div>

            <button
              type="submit"
              disabled={loading}
              className="w-full bg-primary text-white font-body font-medium py-2.5 rounded-xl hover:bg-primary-light transition-colors disabled:opacity-60 mt-2"
            >
              {loading ? t('auth.loggingIn') : t('auth.login')}
            </button>
          </form>

          <p className="text-center text-sm font-body text-primary/50 mt-5">
            {t('auth.noAccount')}{' '}
            <Link to="/register" className="text-primary font-medium hover:text-accent transition-colors">
              {t('auth.signUpLink')}
            </Link>
          </p>
        </div>
      </div>
    </div>
  );
};

export default LoginPage;
