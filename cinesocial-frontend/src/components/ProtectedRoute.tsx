import React from 'react';
import { Navigate, Outlet, useLocation } from 'react-router-dom';
import useAuthStore from '../store/authStore';
import { CircularProgress, Box } from '@mui/material';

const ProtectedRoute: React.FC = () => {
  const isAuthenticated = useAuthStore((state) => state.isAuthenticated);
  // The `isLoading` in authStore is more for login/register actions.
  // Persist middleware rehydrates synchronously, so `isAuthenticated` should be correct on first render
  // unless there's an async check needed post-hydration (e.g., validating token with backend).
  // For now, we assume rehydration is sufficient for this check.
  // If we had a global "initializing" state for auth, it would be checked here.

  const location = useLocation();

  // A simple check for initial loading if the store had an explicit "hydrated" or "checkedOnLoad" flag
  // For example, if `isLoading` was true until first check:
  // const isLoadingAuthState = useAuthStore((state) => state.isLoadingInitial); // Fictional state
  // if (isLoadingAuthState) {
  //   return (
  //     <Box display="flex" justifyContent="center" alignItems="center" minHeight="100vh">
  //       <CircularProgress />
  //     </Box>
  //   );
  // }

  if (!isAuthenticated) {
    // Redirect them to the /login page, but save the current location they were
    // trying to go to when they were redirected. This allows us to send them
    // along to that page after they login, which is a nicer user experience
    // than dropping them off on the home page.
    return <Navigate to="/login" state={{ from: location }} replace />;
  }

  return <Outlet />; // Renders the child route's element
};

export default ProtectedRoute;
