// cinesocial-frontend/src/services/authService.js
import axios from 'axios';

const API_URL = '/api/Auth'; // Using proxy

export const loginUser = async (email, password) => {
    return axios.post(`${API_URL}/login`, { email, password });
};

export const registerUser = async (userData) => {
    // Ensure all required fields are included as per backend:
    // Email, Password, ConfirmPassword, FirstName, LastName, UserName
    return axios.post(`${API_URL}/register`, userData);
};
