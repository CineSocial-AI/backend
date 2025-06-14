import React from 'react';
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import MainLayout from './layouts/MainLayout';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import HomePage from './pages/HomePage';
import ProtectedRoute from './components/ProtectedRoute';
import { Box, Typography } from '@mui/material';

// Placeholder for a NotFoundPage
const NotFoundPage: React.FC = () => (
  <Box sx={{ textAlign: 'center', mt: 8 }}>
    <Typography variant="h3" gutterBottom>404 - Page Not Found</Typography>
    <Typography variant="body1">Sorry, the page you are looking for does not exist.</Typography>
    <RouterLink to="/">Go to Homepage</RouterLink> {/* Corrected: RouterLink needs to be imported */}
  </Box>
);

// Correcting RouterLink import for NotFoundPage, or remove if not used yet.
// For now, I'll define it here simply.
import { Link as RouterLink } from 'react-router-dom';


function App() {
  // Zustand's persist middleware handles rehydration automatically on app load.
  // If specific actions are needed on load (like validating a token with the backend),
  // they could be triggered in a top-level component like App or MainLayout using useEffect.
  // For example:
  // useEffect(() => {
  //   const checkUserStatus = async () => {
  //     const token = useAuthStore.getState().accessToken;
  //     if (token && !useAuthStore.getState().user) {
  //       // Potentially call a service to get user details if only token is rehydrated
  //       // await useAuthStore.getState().fetchUserAction(); // hypothetical action
  //     }
  //   };
  //   checkUserStatus();
  // }, []);

  return (
    <BrowserRouter>
      <Routes>
        {/* Standalone routes (no MainLayout, or different layout) */}
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />

        {/* Protected routes that use MainLayout */}
        {/*
          The structure <Route element={<MainLayout><ProtectedRoute /></MainLayout>}>
          means MainLayout will always render ProtectedRoute.
          ProtectedRoute will then render <Outlet /> (which is HomePage, etc.)
          This is one way to do it. Another way is:
          <Route element={<ProtectedRoute />}>
            <Route path="/" element={<MainLayout><HomePage /></MainLayout>} />
          </Route>
          But the current way is fine: MainLayout wraps the protected area, and ProtectedRoute decides if Outlet (page) is shown.
        */}
        <Route element={
          <MainLayout>
            <ProtectedRoute />
          </MainLayout>
        }>
          <Route path="/" element={<HomePage />} />
          {/* Add other protected routes that use MainLayout here, e.g.: */}
          {/* <Route path="/profile" element={<ProfilePage />} /> */}
          {/* <Route path="/movies/:id" element={<MovieDetailPage />} /> */}
          {/* <Route path="/groups" element={<GroupsListPage />} /> */}
        </Route>

        {/* Fallback for non-matched routes */}
        <Route path="*" element={
          <MainLayout> {/* Optional: Wrap NotFoundPage in MainLayout too */}
            <NotFoundPage />
          </MainLayout>
        } />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
