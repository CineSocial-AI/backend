import React from 'react';
import { Typography, Container, Box, Paper } from '@mui/material';
import useAuthStore from '../store/authStore';

const HomePage: React.FC = () => {
  const user = useAuthStore((state) => state.user);
  const isAuthenticated = useAuthStore((state) => state.isAuthenticated);

  return (
    <Container maxWidth="md">
      <Paper elevation={3} sx={{ padding: 4, marginTop: 4, backgroundColor: 'white' }}>
        <Typography variant="h4" component="h1" gutterBottom color="primary">
          Welcome to CineSocial, {user?.firstName || 'Guest'}!
        </Typography>
        {isAuthenticated ? (
          <Typography variant="body1">
            This is your personalized dashboard. Explore movies, join groups, and share your thoughts!
            {user && (
              <Box sx={{ marginTop: 2 }}>
                <Typography variant="h6">Your Profile:</Typography>
                <Typography><strong>Username:</strong> {user.userName}</Typography>
                <Typography><strong>Email:</strong> {user.email}</Typography>
                <Typography><strong>Name:</strong> {user.fullName || `${user.firstName} ${user.lastName}`}</Typography>
              </Box>
            )}
          </Typography>
        ) : (
          <Typography variant="body1">
            Please log in or register to enjoy the full features of CineSocial.
          </Typography>
        )}
        {/* More content will be added here later */}
      </Paper>
    </Container>
  );
};

export default HomePage;
