import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import useAuthStore from '../store/authStore';
import { Container, TextField, Button, Typography, Box, CircularProgress, Alert } from '@mui/material';
import { Link as RouterLink } from 'react-router-dom'; // For navigation links

const LoginPage: React.FC = () => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const navigate = useNavigate();

  // Selectors from Zustand store
  const loginAction = useAuthStore((state) => state.loginAction);
  const isLoading = useAuthStore((state) => state.isLoading);
  const error = useAuthStore((state) => state.error);
  const isAuthenticated = useAuthStore((state) => state.isAuthenticated);
  const clearError = useAuthStore((state) => state.error ? () => useAuthStore.setState({ error: null }) : () => {});


  useEffect(() => {
    // Redirect if already authenticated
    if (isAuthenticated) {
      navigate('/');
    }
    // Clear errors when component mounts or unmounts if needed, or on input change
    return () => {
      // Potentially clear error on unmount if it's a global error meant for this page
      // For now, let's clear error when email/password changes (see below)
    };
  }, [isAuthenticated, navigate]);

  const handleInputChange = () => {
    if (error) {
      clearError();
    }
  };

  const handleEmailChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setEmail(e.target.value);
    handleInputChange();
  };

  const handlePasswordChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setPassword(e.target.value);
    handleInputChange();
  };

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    // The LoginRequestData DTO in auth.dto.ts expects:
    // email: string; password?: string; rememberMe?: boolean;
    // We'll pass email and password. rememberMe is optional.
    await loginAction({ email, password: password });
    // No need to check isAuthenticated here directly after, useEffect will handle redirect.
  };

  return (
    <Container component="main" maxWidth="xs">
      <Box
        sx={{
          marginTop: 8,
          display: 'flex',
          flexDirection: 'column',
          alignItems: 'center',
          padding: 3,
          borderRadius: 2,
          boxShadow: '0px 3px 15px rgba(0,0,0,0.2)',
          backgroundColor: 'white',
        }}
      >
        <Typography component="h1" variant="h5" color="primary">
          Sign In
        </Typography>
        {error && (
          <Alert severity="error" sx={{ width: '100%', mt: 2, mb: 1 }}>
            {error}
          </Alert>
        )}
        <Box component="form" onSubmit={handleSubmit} noValidate sx={{ mt: 1, width: '100%' }}>
          <TextField
            margin="normal"
            required
            fullWidth
            id="email"
            label="Email Address"
            name="email"
            autoComplete="email"
            autoFocus
            value={email}
            onChange={handleEmailChange}
            disabled={isLoading}
            error={!!error && error.toLowerCase().includes('email')} // Example of field-specific error indication
          />
          <TextField
            margin="normal"
            required
            fullWidth
            name="password"
            label="Password"
            type="password"
            id="password"
            autoComplete="current-password"
            value={password}
            onChange={handlePasswordChange}
            disabled={isLoading}
            error={!!error && error.toLowerCase().includes('password')} // Example
          />
          <Button
            type="submit"
            fullWidth
            variant="contained"
            color="primary"
            sx={{ mt: 3, mb: 2, py: 1.5 }}
            disabled={isLoading}
          >
            {isLoading ? <CircularProgress size={24} color="inherit" /> : 'Sign In'}
          </Button>
          <Box sx={{ textAlign: 'center' }}>
            <Typography variant="body2">
              Don't have an account?{' '}
              <RouterLink to="/register" style={{ textDecoration: 'none' }}>
                <Typography component="span" variant="body2" color="primary">
                  Sign Up
                </Typography>
              </RouterLink>
            </Typography>
          </Box>
        </Box>
      </Box>
    </Container>
  );
};

export default LoginPage;
