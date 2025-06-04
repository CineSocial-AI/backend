import React from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';

const LogoutButton = () => {
    const auth = useAuth();
    const navigate = useNavigate();

    const handleLogout = () => {
        auth.logout();
        navigate('/login');
        // If a backend call for logout is needed, it would be called here:
        // try {
        //   await logoutUserFromBackend(); // Assuming a function in authService.js
        // } catch (error) {
        //   console.error("Backend logout failed", error);
        //   // Handle backend logout error if necessary, though local state is cleared
        // }
    };

    return (
        <button onClick={handleLogout}>
            Logout
        </button>
    );
};

export default LogoutButton;
