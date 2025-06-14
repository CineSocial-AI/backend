import axios from 'axios'; // Imported for isAxiosError type checking
import axiosInstance from './axiosInstance';
import {
  LoginRequestData,
  RegisterRequestData,
  AuthTokenResponseDto,
  ApiResponse, // This is ServiceApiResponse<T> due to the export alias in auth.dto.ts
} from './dtos/auth.dto';

export const registerUser = async (
  data: RegisterRequestData,
): Promise<ApiResponse<AuthTokenResponseDto>> => {
  try {
    // Assuming the backend's ApiResponse<T> is directly in response.data
    const response = await axiosInstance.post<ApiResponse<AuthTokenResponseDto>>(
      '/Auth/register', // Endpoint from AuthController.cs
      data,
    );
    return response.data;
  } catch (error: any) {
    if (axios.isAxiosError(error) && error.response) {
      // If the error response itself is in the ApiResponse<T> format from the backend
      return error.response.data as ApiResponse<AuthTokenResponseDto>;
    }
    // Fallback for network errors or other unexpected issues
    return {
      isSuccess: false,
      message: 'An unexpected error occurred during registration.', // Or use error.message
      errors: [error.message || 'Network error or unexpected issue'],
    };
  }
};

export const loginUser = async (
  data: LoginRequestData,
): Promise<ApiResponse<AuthTokenResponseDto>> => {
  try {
    const response = await axiosInstance.post<ApiResponse<AuthTokenResponseDto>>(
      '/Auth/login', // Endpoint from AuthController.cs
      data,
    );
    return response.data;
  } catch (error: any) {
    if (axios.isAxiosError(error) && error.response) {
      return error.response.data as ApiResponse<AuthTokenResponseDto>;
    }
    return {
      isSuccess: false,
      message: 'An unexpected error occurred during login.', // Or use error.message
      errors: [error.message || 'Network error or unexpected issue'],
    };
  }
};
