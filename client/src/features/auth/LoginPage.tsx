import { useState } from 'react';
import { useForm, Controller } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { useMutation } from '@tanstack/react-query';
import { useNavigate } from 'react-router-dom';
import {
  Box,
  Button,
  TextField,
  Typography,
  Alert,
  IconButton,
  InputAdornment,
  CircularProgress,
} from '@mui/material';
import { authApi } from '../../api/auth';
import { useAuthStore } from '../../store/authStore';
import { loginSchema, type LoginFormValues } from './schemas';
import { getTenantSlug } from '../../utils/tenant';

const EyeOpenIcon = () => (
  <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.75" strokeLinecap="round" strokeLinejoin="round">
    <path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z" />
    <circle cx="12" cy="12" r="3" />
  </svg>
);

const EyeClosedIcon = () => (
  <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.75" strokeLinecap="round" strokeLinejoin="round">
    <path d="M17.94 17.94A10.07 10.07 0 0 1 12 20c-7 0-11-8-11-8a18.45 18.45 0 0 1 5.06-5.94M9.9 4.24A9.12 9.12 0 0 1 12 4c7 0 11 8 11 8a18.5 18.5 0 0 1-2.16 3.19m-6.72-1.07a3 3 0 1 1-4.24-4.24" />
    <line x1="1" y1="1" x2="23" y2="23" />
  </svg>
);

const ShopWaveLogo = () => (
  <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.75" strokeLinecap="round" strokeLinejoin="round">
    <path d="M6 2L3 6v14a2 2 0 002 2h14a2 2 0 002-2V6l-3-4z" />
    <line x1="3" y1="6" x2="21" y2="6" />
    <path d="M16 10a4 4 0 01-8 0" />
  </svg>
);

const GridOverlay = () => (
  <Box
    sx={{
      position: 'absolute',
      inset: 0,
      opacity: 0.03,
      backgroundImage: `
        linear-gradient(rgba(99,102,241,1) 1px, transparent 1px),
        linear-gradient(90deg, rgba(99,102,241,1) 1px, transparent 1px)
      `,
      backgroundSize: '40px 40px',
      pointerEvents: 'none',
    }}
  />
);

const RadialGlow = () => (
  <Box
    sx={{
      position: 'absolute',
      top: '38%',
      left: '50%',
      transform: 'translate(-50%, -50%)',
      width: 480,
      height: 480,
      borderRadius: '50%',
      background: 'radial-gradient(circle, rgba(99,102,241,0.12) 0%, transparent 65%)',
      pointerEvents: 'none',
    }}
  />
);

export const LoginPage = () => {
  const navigate = useNavigate();
  const { setAccessToken, setUser } = useAuthStore();
  const [showPassword, setShowPassword] = useState(false);
  const [apiError, setApiError] = useState<string | null>(null);

  const tenantSlug = getTenantSlug();

  const {
    control,
    handleSubmit,
    formState: { errors },
  } = useForm<LoginFormValues>({
    resolver: zodResolver(loginSchema),
    defaultValues: { email: '', password: '' },
  });

  const { mutate: login, isPending } = useMutation({
    mutationFn: authApi.login,
    onSuccess: (data) => {
      setAccessToken(data.accessToken);
      setUser(data.user);
      navigate('/dashboard');
    },
    onError: () => {
      setApiError('Invalid email or password. Please try again.');
    },
  });

  const onSubmit = (values: LoginFormValues) => {
    setApiError(null);
    login(values);
  };

  return (
    <Box sx={{ minHeight: '100vh', display: 'flex', fontFamily: 'var(--font-body)' }}>
      {/* Left panel */}
      <Box
        sx={{
          display: { xs: 'none', lg: 'flex' },
          width: '45%',
          flexDirection: 'column',
          justifyContent: 'space-between',
          p: '56px',
          position: 'relative',
          overflow: 'hidden',
          background: '#0d0d10',
          borderRight: '1px solid #1a1a22',
        }}
      >
        <GridOverlay />
        <RadialGlow />

        {/* Brand mark */}
        <Box className="animate-fade-up" sx={{ position: 'relative', display: 'flex', alignItems: 'center', gap: 1.5 }}>
          <Box
            sx={{
              width: 36,
              height: 36,
              borderRadius: '8px',
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'center',
              background: 'linear-gradient(135deg, #6366f1 0%, #4f46e5 100%)',
              color: '#fff',
              flexShrink: 0,
            }}
          >
            <ShopWaveLogo />
          </Box>
          <Typography sx={{ fontSize: '0.9375rem', fontWeight: 600, color: '#f8f8ff', letterSpacing: '-0.01em' }}>
            ShopWave
          </Typography>
        </Box>

        {/* Headline */}
        <Box className="animate-fade-up delay-100" sx={{ position: 'relative' }}>
          <Box
            sx={{
              display: 'inline-flex',
              alignItems: 'center',
              gap: 1,
              px: 1.5,
              py: 0.625,
              mb: 4,
              borderRadius: '999px',
              border: '1px solid rgba(99,102,241,0.25)',
              background: 'rgba(99,102,241,0.06)',
            }}
          >
            <Box sx={{ width: 6, height: 6, borderRadius: '50%', background: '#6366f1', animation: 'pulse 2s infinite' }} />
            <Typography sx={{ fontSize: '0.6875rem', fontWeight: 500, letterSpacing: '0.08em', color: '#818cf8', textTransform: 'uppercase' }}>
              Commerce Platform
            </Typography>
          </Box>

          <Typography
            component="h1"
            sx={{
              fontFamily: 'var(--font-display)',
              fontSize: '2.75rem',
              lineHeight: 1.08,
              fontWeight: 400,
              color: '#f8f8ff',
              letterSpacing: '-0.02em',
              mb: 3,
            }}
          >
            Control every<br />
            corner of<br />
            <Box component="span" sx={{ color: '#818cf8' }}>commerce.</Box>
          </Typography>

          <Typography sx={{ fontSize: '0.875rem', lineHeight: 1.7, color: '#4b5563', maxWidth: 280 }}>
            Inventory, orders, analytics, and growth — unified in a single operator dashboard.
          </Typography>
        </Box>

        {/* Stats */}
        <Box
          className="animate-fade-up delay-200"
          sx={{
            position: 'relative',
            display: 'grid',
            gridTemplateColumns: 'repeat(3, 1fr)',
            gap: 3,
            pt: 4,
            borderTop: '1px solid #1a1a22',
          }}
        >
          {[
            { value: '12k+', label: 'Active stores' },
            { value: '99.9%', label: 'Uptime SLA' },
            { value: '4.2M', label: 'Orders / mo' },
          ].map(({ value, label }) => (
            <Box key={label}>
              <Typography
                sx={{
                  fontFamily: 'var(--font-display)',
                  fontSize: '1.375rem',
                  fontWeight: 400,
                  color: '#f8f8ff',
                  letterSpacing: '-0.02em',
                  lineHeight: 1,
                  mb: 0.5,
                }}
              >
                {value}
              </Typography>
              <Typography sx={{ fontSize: '0.6875rem', color: '#374151', letterSpacing: '0.02em' }}>
                {label}
              </Typography>
            </Box>
          ))}
        </Box>
      </Box>

      {/* Right panel */}
      <Box
        sx={{
          flex: 1,
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'center',
          px: { xs: 3, sm: 6 },
          py: 6,
          background: '#111116',
        }}
      >
        <Box sx={{ width: '100%', maxWidth: 400 }}>
          {/* Mobile brand */}
          <Box
            className="animate-fade-up"
            sx={{ display: { xs: 'flex', lg: 'none' }, alignItems: 'center', gap: 1.5, mb: 5 }}
          >
            <Box
              sx={{
                width: 32,
                height: 32,
                borderRadius: '7px',
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
                background: 'linear-gradient(135deg, #6366f1 0%, #4f46e5 100%)',
                color: '#fff',
              }}
            >
              <ShopWaveLogo />
            </Box>
            <Typography sx={{ fontSize: '0.9rem', fontWeight: 600, color: '#f8f8ff' }}>ShopWave</Typography>
          </Box>

          {/* Workspace badge */}
          {tenantSlug && (
            <Box
              className="animate-fade-up"
              sx={{
                display: 'inline-flex',
                alignItems: 'center',
                gap: 1,
                px: 1.5,
                py: 0.75,
                mb: 4,
                borderRadius: '6px',
                border: '1px solid #1f1f27',
                background: '#0d0d10',
              }}
            >
              <Box sx={{ width: 7, height: 7, borderRadius: '50%', background: '#6366f1', flexShrink: 0 }} />
              <Typography sx={{ fontSize: '0.75rem', color: '#6b7280', fontFamily: "'Space Grotesk', monospace", letterSpacing: '0.02em' }}>
                <Box component="span" sx={{ color: '#f8f8ff' }}>{tenantSlug}</Box>
                {' · shopwave.com'}
              </Typography>
            </Box>
          )}

          {/* Heading */}
          <Box className="animate-fade-up delay-100" sx={{ mb: 4 }}>
            <Typography
              sx={{
                fontSize: '1.5rem',
                fontWeight: 600,
                color: '#f8f8ff',
                letterSpacing: '-0.03em',
                lineHeight: 1.2,
                mb: 0.75,
              }}
            >
              {tenantSlug ? `Sign in to ${tenantSlug}` : 'Welcome back'}
            </Typography>
            <Typography sx={{ fontSize: '0.8125rem', color: '#4b5563' }}>
              Enter your credentials to access your dashboard
            </Typography>
          </Box>

          {/* Error alert */}
          {apiError && (
            <Alert severity="error" className="animate-fade-in" sx={{ mb: 3 }}>
              {apiError}
            </Alert>
          )}

          <Box
            component="form"
            onSubmit={handleSubmit(onSubmit)}
            noValidate
            className="animate-fade-up delay-200"
            sx={{ display: 'flex', flexDirection: 'column', gap: 2.5 }}
          >
            {/* Email */}
            <Controller
              name="email"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  label="Email address"
                  type="email"
                  autoComplete="email"
                  error={!!errors.email}
                  helperText={errors.email?.message}
                  inputProps={{ style: { fontSize: '0.875rem' } }}
                />
              )}
            />

            {/* Password */}
            <Controller
              name="password"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  label="Password"
                  type={showPassword ? 'text' : 'password'}
                  autoComplete="current-password"
                  error={!!errors.password}
                  helperText={errors.password?.message}
                  inputProps={{ style: { fontSize: '0.875rem' } }}
                  InputProps={{
                    endAdornment: (
                      <InputAdornment position="end">
                        <IconButton
                          onClick={() => setShowPassword((v) => !v)}
                          edge="end"
                          size="small"
                          sx={{ color: '#374151', '&:hover': { color: '#6b7280' } }}
                        >
                          {showPassword ? <EyeClosedIcon /> : <EyeOpenIcon />}
                        </IconButton>
                      </InputAdornment>
                    ),
                  }}
                />
              )}
            />

            {/* Forgot password */}
            <Box sx={{ display: 'flex', justifyContent: 'flex-end', mt: -1.5 }}>
              <Typography
                component="button"
                type="button"
                sx={{
                  fontSize: '0.75rem',
                  color: '#6366f1',
                  background: 'none',
                  border: 'none',
                  cursor: 'pointer',
                  fontFamily: 'inherit',
                  p: 0,
                  '&:hover': { color: '#818cf8' },
                  transition: 'color 0.15s',
                }}
              >
                Forgot password?
              </Typography>
            </Box>

            {/* Submit */}
            <Button
              type="submit"
              variant="contained"
              color="primary"
              disabled={isPending}
              fullWidth
              sx={{ mt: 0.5, py: 1.375 }}
              endIcon={
                isPending
                  ? <CircularProgress size={14} color="inherit" />
                  : (
                    <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                      <line x1="5" y1="12" x2="19" y2="12" />
                      <polyline points="12 5 19 12 12 19" />
                    </svg>
                  )
              }
            >
              {isPending ? 'Signing in…' : 'Sign in'}
            </Button>
          </Box>

          {/* Divider */}
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 2, my: 3.5 }} className="animate-fade-up delay-300">
            <Box sx={{ flex: 1, height: '1px', background: '#1a1a22' }} />
            <Typography sx={{ fontSize: '0.6875rem', color: '#2d2d38', letterSpacing: '0.04em' }}>OR</Typography>
            <Box sx={{ flex: 1, height: '1px', background: '#1a1a22' }} />
          </Box>

          {/* SSO */}
          <Button
            variant="outlined"
            fullWidth
            className="animate-fade-up delay-400"
            sx={{
              borderColor: '#1f1f27',
              color: '#6b7280',
              '&:hover': { borderColor: '#2e2e3a', color: '#f8f8ff', background: 'transparent' },
              gap: 1,
            }}
            startIcon={
              <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" strokeLinejoin="round">
                <rect x="3" y="11" width="11" height="11" rx="2" />
                <path d="M7 11V7a5 5 0 0 1 9.9-1" />
              </svg>
            }
          >
            Continue with SSO
          </Button>

          {/* Sign up */}
          <Typography
            className="animate-fade-up delay-500"
            sx={{ mt: 4, textAlign: 'center', fontSize: '0.8125rem', color: '#374151' }}
          >
            Don't have an account?{' '}
            <Box
              component="button"
              type="button"
              sx={{
                background: 'none',
                border: 'none',
                cursor: 'pointer',
                fontFamily: 'inherit',
                fontSize: 'inherit',
                color: '#6366f1',
                p: 0,
                '&:hover': { color: '#818cf8' },
                transition: 'color 0.15s',
              }}
            >
              Request access
            </Box>
          </Typography>
        </Box>
      </Box>
    </Box>
  );
};
