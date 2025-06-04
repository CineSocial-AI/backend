import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { registerUser } from '../services/authService';

const RegisterPage = () => {
    const [formData, setFormData] = useState({
        firstName: '',
        lastName: '',
        userName: '',
        email: '',
        password: '',
        confirmPassword: '',
    });
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(false);
    const auth = useAuth();
    const navigate = useNavigate();

    const handleChange = (e) => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        if (formData.password !== formData.confirmPassword) {
            setError("Passwords don't match");
            return;
        }
        setError('');
        setLoading(true);
        try {
            const response = await registerUser(formData);
            if (response.data && response.data.isSuccess) {
                if (response.data.value && response.data.value.user && response.data.value.accessToken) {
                    auth.register(response.data.value.user, response.data.value.accessToken);
                    navigate('/');
                } else {
                    // Consider showing a success message before navigating to login
                    // e.g., "Registration successful! Please log in."
                    navigate('/login');
                }
            } else {
                setError(response.data.errorMessage || 'Registration failed. Please try again.');
            }
        } catch (err) {
            console.error("Registration error:", err);
            setError(err.response?.data?.errorMessage || 'An unexpected error occurred during registration.');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="container">
            <form onSubmit={handleSubmit}>
                <h2>Register</h2>
                {error && <p className="error-message">{error}</p>}
                <div>
                    <label htmlFor="firstName">First Name:</label>
                    <input type="text" id="firstName" name="firstName" value={formData.firstName} onChange={handleChange} required autoComplete="given-name" />
                </div>
                <div>
                    <label htmlFor="lastName">Last Name:</label>
                    <input type="text" id="lastName" name="lastName" value={formData.lastName} onChange={handleChange} required autoComplete="family-name" />
                </div>
                <div>
                    <label htmlFor="userName">Username:</label>
                    <input type="text" id="userName" name="userName" value={formData.userName} onChange={handleChange} required autoComplete="username" />
                </div>
                <div>
                    <label htmlFor="email">Email:</label>
                    <input type="email" id="email" name="email" value={formData.email} onChange={handleChange} required autoComplete="email" />
                </div>
                <div>
                    <label htmlFor="password">Password:</label>
                    <input type="password" id="password" name="password" value={formData.password} onChange={handleChange} required autoComplete="new-password" />
                </div>
                <div>
                    <label htmlFor="confirmPassword">Confirm Password:</label>
                    <input type="password" id="confirmPassword" name="confirmPassword" value={formData.confirmPassword} onChange={handleChange} required autoComplete="new-password" />
                </div>
                <button type="submit" disabled={loading}>
                    {loading ? 'Registering...' : 'Register'}
                </button>
            </form>
        </div>
    );
};

export default RegisterPage;
