import React, { useState, useEffect } from 'react';
import { useNavigate, Link as RouterLink } from 'react-router-dom';
import useAuthStore from '../store/authStore';
import { Container, TextField, Button, Typography, Box, CircularProgress, Alert, Grid } from '@mui/material';

const RegisterPage: React.FC = () => {
  const [formData, setFormData] = useState({
    email: '',
    password: '',
    confirmPassword: '',
    firstName: '',
    lastName: '',
    userName: '',
  });
  const [localError, setLocalError] = useState<string | null>(null);
  const navigate = useNavigate();

  const registerAction = useAuthStore((state) => state.registerAction);
  const isLoading = useAuthStore((state) => state.isLoading);
  const storeError = useAuthStore((state) => state.error);
  const isAuthenticated = useAuthStore((state) => state.isAuthenticated);
  const clearStoreError = useAuthStore((state) => state.error ? () => useAuthStore.setState({ error: null }) : () => {});

  useEffect(() => {
    if (isAuthenticated) {
      navigate('/'); // Redirect after successful registration and auto-login
    }
  }, [isAuthenticated, navigate]);

  const handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setFormData({ ...formData, [event.target.name]: event.target.value });
    if (localError) setLocalError(null);
    if (storeError) clearStoreError();
  };

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setLocalError(null);
    if (storeError) clearStoreError();

    if (formData.password !== formData.confirmPassword) {
      setLocalError("Passwords don't match!");
      return;
    }
    if (formData.password.length < 6) { // Example basic validation
        setLocalError("Password must be at least 6 characters long.");
        return;
    }

    // RegisterRequestData expects: email, password, confirmPassword, firstName, lastName, userName
    // The DTO in auth.dto.ts defined these as optional for password and confirmPassword
    // but for registration, they are effectively required.
    await registerAction({
      email: formData.email,
      password: formData.password,
      confirmPassword: formData.confirmPassword,
      firstName: formData.firstName,
      lastName: formData.lastName,
      userName: formData.userName,
    });
  };

  const displayError = localError || storeError;

  return (
    <Container component="main" maxWidth="xs">
      <Box
        sx={{
          marginTop: 8,
          marginBottom: 8,
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
          Sign Up
        </Typography>
        {displayError && (
          <Alert severity="error" sx={{ width: '100%', mt: 2, mb: 1 }}>
            {displayError}
          </Alert>
        )}
        <Box component="form" onSubmit={handleSubmit} noValidate sx={{ mt: 1, width: '100%' }}>
          <Grid container spacing={2}>
            <Grid item xs={12} sm={6}>
              <TextField
                margin="normal"
                required
                fullWidth
                id="firstName"
                label="First Name"
                name="firstName"
                autoComplete="given-name"
                autoFocus
                value={formData.firstName}
                onChange={handleChange}
                disabled={isLoading}
              />
            </Grid>
            <Grid item xs={12} sm={6}>
              <TextField
                margin="normal"
                required
                fullWidth
                id="lastName"
                label="Last Name"
                name="lastName"
                autoComplete="family-name"
                value={formData.lastName}
                onChange={handleChange}
                disabled={isLoading}
              />
            </Grid>
          </Grid>
          <TextField
            margin="normal"
            required
            fullWidth
            id="userName"
            label="Username"
            name="userName"
            autoComplete="username"
            value={formData.userName}
            onChange={handleChange}
            disabled={isLoading}
          />
          <TextField
            margin="normal"
            required
            fullWidth
            id="email"
            label="Email Address"
            name="email"
            type="email"
            autoComplete="email"
            value={formData.email}
            onChange={handleChange}
            disabled={isLoading}
          />
          <TextField
            margin="normal"
            required
            fullWidth
            name="password"
            label="Password"
            type="password"
            id="password"
            value={formData.password}
            onChange={handleChange}
            disabled={isLoading}
            error={!!localError && localError.toLowerCase().includes("password")}
          />
          <TextField
            margin="normal"
            required
            fullWidth
            name="confirmPassword"
            label="Confirm Password"
            type="password"
            id="confirmPassword"
            value={formData.confirmPassword}
            onChange={handleChange}
            disabled={isLoading}
            error={!!localError && localError === "Passwords don't match!"}
          />
          <Button
            type="submit"
            fullWidth
            variant="contained"
            color="primary"
            sx={{ mt: 3, mb: 2, py: 1.5 }}
            disabled={isLoading}
          >
            {isLoading ? <CircularProgress size={24} color="inherit" /> : 'Sign Up'}
          </Button>
          <Grid container justifyContent="flex-end">
            <Grid item>
              <RouterLink to="/login" style={{ textDecoration: 'none' }}>
                <Typography variant="body2" color="primary">
                  Already have an account? Sign in
                </Typography>
              </RouterLink>
            </Grid>
          </Grid>
        </Box>
      </Box>
    </Container>
  );
};

export default RegisterPage;
