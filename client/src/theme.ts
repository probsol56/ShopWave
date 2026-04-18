import { createTheme } from '@mui/material/styles';

export const theme = createTheme({
  palette: {
    mode: 'dark',
    primary: {
      main: '#6366f1',
      light: '#818cf8',
      dark: '#4f46e5',
    },
    background: {
      default: '#0d0d10',
      paper: '#111116',
    },
    text: {
      primary: '#f8f8ff',
      secondary: '#6b7280',
    },
    error: {
      main: '#f87171',
    },
    divider: '#1f1f27',
  },
  typography: {
    fontFamily: "'Space Grotesk', sans-serif",
    h1: { fontFamily: "'DM Serif Display', serif" },
    h2: { fontFamily: "'DM Serif Display', serif" },
    h3: { fontFamily: "'DM Serif Display', serif" },
  },
  shape: {
    borderRadius: 6,
  },
  components: {
    MuiTextField: {
      defaultProps: {
        variant: 'outlined',
        fullWidth: true,
      },
      styleOverrides: {
        root: {
          '& .MuiOutlinedInput-root': {
            backgroundColor: '#0d0d10',
            fontSize: '0.875rem',
            '& fieldset': {
              borderColor: '#1f1f27',
            },
            '&:hover fieldset': {
              borderColor: '#2e2e3a',
            },
            '&.Mui-focused fieldset': {
              borderColor: '#6366f1',
              borderWidth: '1px',
            },
          },
          '& .MuiInputLabel-root': {
            fontSize: '0.8125rem',
            color: '#4b5563',
            '&.Mui-focused': {
              color: '#6366f1',
            },
          },
          '& .MuiOutlinedInput-input': {
            color: '#f8f8ff',
            '&::placeholder': {
              color: '#374151',
              opacity: 1,
            },
          },
          '& .MuiFormHelperText-root': {
            marginLeft: 0,
            fontSize: '0.75rem',
          },
        },
      },
    },
    MuiButton: {
      defaultProps: {
        disableElevation: true,
      },
      styleOverrides: {
        root: {
          textTransform: 'none',
          fontFamily: "'Space Grotesk', sans-serif",
          fontWeight: 500,
          letterSpacing: '0.01em',
          borderRadius: 6,
          padding: '10px 20px',
          fontSize: '0.875rem',
        },
        containedPrimary: {
          background: 'linear-gradient(135deg, #6366f1 0%, #4f46e5 100%)',
          '&:hover': {
            background: 'linear-gradient(135deg, #818cf8 0%, #6366f1 100%)',
          },
          '&:disabled': {
            background: '#1f1f27',
            color: '#374151',
          },
        },
      },
    },
    MuiAlert: {
      styleOverrides: {
        root: {
          fontSize: '0.8125rem',
          borderRadius: 6,
        },
        standardError: {
          backgroundColor: 'rgba(248,113,113,0.08)',
          border: '1px solid rgba(248,113,113,0.2)',
          color: '#fca5a5',
        },
      },
    },
    MuiCssBaseline: {
      styleOverrides: {
        body: {
          backgroundColor: '#0d0d10',
        },
      },
    },
  },
});
