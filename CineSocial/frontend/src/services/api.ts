import axios, { AxiosInstance, AxiosResponse } from 'axios';

// API Configuration
const API_BASE_URL = process.env.REACT_APP_API_URL || 'http://localhost:5000/api';

// Types
export interface ApiResponse<T> {
  success: boolean;
  data: T;
  message?: string;
  errors?: string[];
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
  confirmPassword: string;
  firstName: string;
  lastName: string;
}

export interface AuthResponse {
  token: string;
  refreshToken: string;
  user: User;
}

export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  profileImageUrl?: string;
  bio?: string;
  isActive: boolean;
  createdAt: string;
}

export interface Movie {
  id: string;
  title: string;
  description: string;
  releaseDate: string;
  duration: number;
  rating?: number;
  posterUrl?: string;
  backdropUrl?: string;
  genres: Genre[];
  director?: string;
  cast?: string[];
  tmdbId?: number;
  createdAt: string;
  updatedAt?: string;
}

export interface Genre {
  id: string;
  name: string;
  description?: string;
  tmdbId?: number;
}

export interface MovieSummary {
  id: string;
  title: string;
  releaseDate: string;
  rating?: number;
  posterUrl?: string;
  genres: Genre[];
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
}

export interface CreateMovieRequest {
  title: string;
  description: string;
  releaseDate: string;
  duration: number;
  posterUrl?: string;
  backdropUrl?: string;
  director?: string;
  cast?: string[];
  genreIds: string[];
  tmdbId?: number;
}

// API Service Class
class ApiService {
  private api: AxiosInstance;

  constructor() {
    this.api = axios.create({
      baseURL: API_BASE_URL,
      headers: {
        'Content-Type': 'application/json',
      },
    });

    // Request interceptor to add auth token
    this.api.interceptors.request.use(
      (config) => {
        const token = localStorage.getItem('accessToken');
        if (token) {
          config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
      },
      (error) => {
        return Promise.reject(error);
      }
    );

    // Response interceptor to handle token refresh
    this.api.interceptors.response.use(
      (response) => response,
      async (error) => {
        const originalRequest = error.config;
        
        if (error.response?.status === 401 && !originalRequest._retry) {
          originalRequest._retry = true;
          
          try {
            const refreshToken = localStorage.getItem('refreshToken');
            if (refreshToken) {
              const response = await this.refreshToken(refreshToken);
              localStorage.setItem('accessToken', response.token);
              localStorage.setItem('refreshToken', response.refreshToken);
              
              // Retry the original request
              originalRequest.headers.Authorization = `Bearer ${response.token}`;
              return this.api(originalRequest);
            }
          } catch (refreshError) {
            // Refresh failed, redirect to login
            this.logout();
            window.location.href = '/login';
          }
        }
        
        return Promise.reject(error);
      }
    );
  }

  // Auth Methods
  async login(credentials: LoginRequest): Promise<AuthResponse> {
    const response: AxiosResponse<AuthResponse> = await this.api.post('/auth/login', credentials);
    return response.data;
  }

  async register(userData: RegisterRequest): Promise<AuthResponse> {
    const response: AxiosResponse<AuthResponse> = await this.api.post('/auth/register', userData);
    return response.data;
  }

  async refreshToken(refreshToken: string): Promise<AuthResponse> {
    const response: AxiosResponse<AuthResponse> = await this.api.post('/auth/refresh', {
      refreshToken,
    });
    return response.data;
  }

  async logout(): Promise<void> {
    try {
      await this.api.post('/auth/logout');
    } catch (error) {
      // Ignore errors on logout
    } finally {
      localStorage.removeItem('accessToken');
      localStorage.removeItem('refreshToken');
      localStorage.removeItem('user');
    }
  }

  async forgotPassword(email: string): Promise<void> {
    await this.api.post('/auth/forgot-password', { email });
  }

  async resetPassword(token: string, password: string, confirmPassword: string): Promise<void> {
    await this.api.post('/auth/reset-password', {
      token,
      password,
      confirmPassword,
    });
  }

  async changePassword(currentPassword: string, newPassword: string, confirmPassword: string): Promise<void> {
    await this.api.post('/auth/change-password', {
      currentPassword,
      newPassword,
      confirmPassword,
    });
  }

  // Movie Methods
  async getMovies(
    page: number = 1,
    pageSize: number = 20,
    search?: string,
    genreIds?: string[],
    sortBy?: string
  ): Promise<PagedResult<MovieSummary>> {
    const params = new URLSearchParams();
    params.append('page', page.toString());
    params.append('pageSize', pageSize.toString());
    
    if (search) {
      params.append('search', search);
    }
    
    if (genreIds && genreIds.length > 0) {
      genreIds.forEach(id => params.append('genreIds', id));
    }
    
    if (sortBy) {
      params.append('sortBy', sortBy);
    }

    const response: AxiosResponse<PagedResult<MovieSummary>> = await this.api.get(`/movies?${params}`);
    return response.data;
  }

  async getMovieById(id: string): Promise<Movie> {
    const response: AxiosResponse<Movie> = await this.api.get(`/movies/${id}`);
    return response.data;
  }

  async createMovie(movieData: CreateMovieRequest): Promise<Movie> {
    const response: AxiosResponse<Movie> = await this.api.post('/movies', movieData);
    return response.data;
  }

  async updateMovie(id: string, movieData: Partial<CreateMovieRequest>): Promise<Movie> {
    const response: AxiosResponse<Movie> = await this.api.put(`/movies/${id}`, movieData);
    return response.data;
  }

  async deleteMovie(id: string): Promise<void> {
    await this.api.delete(`/movies/${id}`);
  }

  async getPopularMovies(count: number = 10): Promise<MovieSummary[]> {
    const response: AxiosResponse<MovieSummary[]> = await this.api.get(`/movies/popular?count=${count}`);
    return response.data;
  }

  async getTopRatedMovies(count: number = 10): Promise<MovieSummary[]> {
    const response: AxiosResponse<MovieSummary[]> = await this.api.get(`/movies/top-rated?count=${count}`);
    return response.data;
  }

  async getRecentMovies(count: number = 10): Promise<MovieSummary[]> {
    const response: AxiosResponse<MovieSummary[]> = await this.api.get(`/movies/recent?count=${count}`);
    return response.data;
  }

  // Genre Methods
  async getGenres(): Promise<Genre[]> {
    const response: AxiosResponse<Genre[]> = await this.api.get('/movies/genres');
    return response.data;
  }

  async createGenre(name: string, description?: string): Promise<Genre> {
    const response: AxiosResponse<Genre> = await this.api.post('/movies/genres', {
      name,
      description,
    });
    return response.data;
  }

  // Health Check
  async healthCheck(): Promise<{ status: string }> {
    const response: AxiosResponse<{ status: string }> = await this.api.get('/health');
    return response.data;
  }
}

// Export singleton instance
export const apiService = new ApiService();
export default apiService;