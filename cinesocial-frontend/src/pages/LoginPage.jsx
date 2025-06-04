import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { loginUser } from '../services/authService';

const LoginPage = () => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(false);
    const auth = useAuth();
    const navigate = useNavigate();

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');
        setLoading(true);
        try {
            const response = await loginUser(email, password);
            if (response.data && response.data.isSuccess && response.data.value) {
                auth.login(response.data.value.user, response.data.value.accessToken);
                navigate('/');
            } else {
                setError(response.data.errorMessage || 'Login failed. Please check your credentials.');
            }
        } catch (err) {
            console.error("Login error:", err);
            setError(err.response?.data?.errorMessage || 'An unexpected error occurred during login.');
        } finally {
            setLoading(false);
        }
    };

    return (
        // The global form styling will apply to the <form> tag
        // A container div might be useful if more elements are outside the form
        <div className="container"> 
            <form onSubmit={handleSubmit}>
                <h2>Login</h2> {/* Moved h2 inside form or it could be outside with its own styling */}
                {error && <p className="error-message">{error}</p>}
                <div>
                    <label htmlFor="email">Email:</label>
                    <input
                        type="email"
                        id="email"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        required
                        autoComplete="email"
                    />
                </div>
                <div>
                    <label htmlFor="password">Password:</label>
                    <input
                        type="password"
                        id="password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        required
                        autoComplete="current-password"
                    />
                </div>
                <button type="submit" disabled={loading}>
                    {loading ? 'Logging in...' : 'Login'}
                </button>
            </form>
        </div>
    );
};

export default LoginPage;
