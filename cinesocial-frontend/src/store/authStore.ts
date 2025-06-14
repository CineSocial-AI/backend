import { create } from 'zustand';
import { persist, createJSONStorage } from 'zustand/middleware';
import { loginUser, registerUser } from '../services/authService';
import type {
  UserResponseDto,
  LoginRequestData,
  RegisterRequestData,
  // ApiResponse as ServiceApiResponse // Renaming for clarity if needed, but using ApiResponse from DTOs
} from '../services/dtos/auth.dto';
// Ensure this path is correct and ApiResponse is the one from auth.dto.ts
import type { ApiResponse as ServiceApiResponse } from '../services/dtos/auth.dto';


interface AuthState {
  user: UserResponseDto | null;
  accessToken: string | null;
  refreshToken: string | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  error: string | null;
  loginAction: (data: LoginRequestData) => Promise<boolean>; // Renamed to avoid conflict with state key
  registerAction: (data: RegisterRequestData) => Promise<boolean>; // Renamed
  logoutAction: () => void; // Renamed
}

const useAuthStore = create<AuthState>()(
  persist(
    (set, get) => ({
      user: null,
      accessToken: null,
      refreshToken: null,
      isAuthenticated: false,
      isLoading: false,
      error: null,

      loginAction: async (data: LoginRequestData) => {
        set({ isLoading: true, error: null });
        try {
          const response: ServiceApiResponse<UserResponseDto | any> = await loginUser(data); // Using any for response.value to match service
          // Assuming response.data contains AuthTokenResponseDto as per authService
          if (response.isSuccess && response.data) {
            const { accessToken, refreshToken, user } = response.data; // response.data is AuthTokenResponseDto
            set({
              user,
              accessToken,
              refreshToken,
              isAuthenticated: true,
              isLoading: false,
            });
            return true;
          } else {
            set({
              error: response.message || response.errors?.[0] || 'Login failed',
              isLoading: false,
              isAuthenticated: false,
              user: null,
              accessToken: null,
              refreshToken: null,
            });
            return false;
          }
        } catch (err: any) {
          // This catch block might be redundant if authService already transforms errors
          // into the ApiResponse structure. However, it's a safety net.
          const errorMessage = err.response?.data?.message || err.response?.data?.errors?.[0] || err.message || 'An unexpected error occurred during login.';
          set({
            error: errorMessage,
            isLoading: false,
            isAuthenticated: false,
            user: null,
            accessToken: null,
            refreshToken: null,
          });
          return false;
        }
      },

      registerAction: async (data: RegisterRequestData) => {
        set({ isLoading: true, error: null });
        try {
          const response: ServiceApiResponse<UserResponseDto | any> = await registerUser(data);
           // Assuming response.data contains AuthTokenResponseDto
          if (response.isSuccess && response.data) {
            const { accessToken, refreshToken, user } = response.data;
            set({
              user,
              accessToken,
              refreshToken,
              isAuthenticated: true,
              isLoading: false,
            });
            return true;
          } else {
            set({
              error: response.message || response.errors?.[0] || 'Registration failed',
              isLoading: false,
              isAuthenticated: false,
              user: null,
              accessToken: null,
              refreshToken: null,
            });
            return false;
          }
        } catch (err: any) {
          const errorMessage = err.response?.data?.message || err.response?.data?.errors?.[0] || err.message || 'An unexpected error occurred during registration.';
          set({
            error: errorMessage,
            isLoading: false,
            isAuthenticated: false,
            user: null,
            accessToken: null,
            refreshToken: null,
          });
          return false;
        }
      },

      logoutAction: () => {
        // Optional: Call backend logout endpoint
        // import { axiosInstance } from './axiosInstance'; // if making a call
        // axiosInstance.post('/Auth/logout').catch(e => console.error("Logout failed on backend", e));

        set({
          user: null,
          accessToken: null,
          refreshToken: null,
          isAuthenticated: false,
          error: null,
          isLoading: false,
        });
        // The persist middleware should handle clearing the persisted state (localStorage)
        // when these primary state keys are set to null.
        // For more explicit control, one might call `localStorage.removeItem('auth-storage')`
        // but this is often not needed with `persist`.
      },
    }),
    {
      name: 'auth-storage', // Name for localStorage item
      storage: createJSONStorage(() => localStorage),
      // By default, all state is persisted.
      // Use `partialize` if you want to persist only specific parts of the store:
      partialize: (state) => ({
        user: state.user,
        accessToken: state.accessToken,
        refreshToken: state.refreshToken,
        isAuthenticated: state.isAuthenticated, // Important to persist for session continuity
      }),
    },
  ),
);

export default useAuthStore;

// The persist middleware handles rehydration from localStorage automatically on store creation/app load.
// No explicit hydrateAuth function is typically needed unless there's complex logic
// beyond what persist provides (e.g., token validation on load).
// For token validation on load, you might do it in App.tsx or a similar top-level component:
// useEffect(() => {
//   const token = useAuthStore.getState().accessToken;
//   if (token) {
//     // validateTokenAndFetchUser(); // A function you'd write
//   }
// }, []);

// Note on `response.value` vs `response.data` in the original example:
// The `authService` was implemented to return the backend's `ApiResponse` structure directly.
// In that structure, the payload is in `data` (mapping to C# `Data` property).
// The example store code used `response.value`. I've adjusted it to `response.data`
// to align with `authService.ts` and `auth.dto.ts` (`ServiceApiResponse`).
// Also, I've renamed action functions to `loginAction`, `registerAction`, `logoutAction`
// to avoid potential naming conflicts with state properties if they were named `login`, `register`, `logout`.
