import axios from 'axios';
import API_BASE_URL from '../config/apiConfig';
import useAuthStore from '../store/authStore'; // Import the store

const axiosInstance = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

axiosInstance.interceptors.request.use(
  (config) => {
    // Access token directly from Zustand store's state
    // This is a synchronous way to get the latest state when the interceptor runs.
    const token = useAuthStore.getState().accessToken;
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  },
);

// Optional: Add interceptors for response (e.g., global error handling or token refresh)
// axiosInstance.interceptors.response.use(
//   response => response,
//   async error => {
//     const originalRequest = error.config;
//     // Example: Token refresh logic
//     // if (error.response.status === 401 && !originalRequest._retry && useAuthStore.getState().refreshToken) {
//     //   originalRequest._retry = true;
//     //   try {
//     //     const refreshToken = useAuthStore.getState().refreshToken;
//     //     // const response = await axiosInstance.post('/Auth/refresh', { refreshToken }); // Replace with actual refresh call
//     //     // const { accessToken, newRefreshToken } = response.data.data; // Adjust to your refresh token response
//     //     // useAuthStore.getState().setTokens(accessToken, newRefreshToken); // You'd need a setTokens action
//     //     // axiosInstance.defaults.headers.common['Authorization'] = 'Bearer ' + accessToken;
//     //     // originalRequest.headers['Authorization'] = 'Bearer ' + accessToken;
//     //     // return axiosInstance(originalRequest);
//     //     console.log("Simulating token refresh - would call refresh endpoint here");
//     //     // For now, just log out if refresh fails or is not implemented
//     //     useAuthStore.getState().logoutAction();
//     //     return Promise.reject(error);
//     //   } catch (refreshError) {
//     //     useAuthStore.getState().logoutAction();
//     //     return Promise.reject(refreshError);
//     //   }
//     // }
//     return Promise.reject(error);
//   }
// );

export default axiosInstance;
