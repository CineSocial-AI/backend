// src/pages/LoginPage.test.js
import { render, screen, waitFor }
from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { BrowserRouter } from 'react-router-dom';
import { AuthProvider } from '../contexts/AuthContext'; 
import LoginPage from './LoginPage'; 
import { server } from '../mocks/server'; // MSW server
import { http, HttpResponse } from 'msw'; // For overriding handlers if needed
import { vi } from 'vitest'; // Import vi for mocking

// Mock react-router-dom's useNavigate
const mockNavigate = vi.fn();
vi.mock('react-router-dom', async (importOriginal) => {
    const actual = await importOriginal();
    return {
        ...actual,
        useNavigate: () => mockNavigate,
    };
});

// Helper to render with providers
const renderWithProviders = (ui) => {
    return render(
        <BrowserRouter>
            <AuthProvider>
                {ui}
            </AuthProvider>
        </BrowserRouter>
    );
};

describe('LoginPage', () => {
    beforeEach(() => { // Use beforeEach to clear localStorage for token check consistency
        localStorage.clear();
    });
    afterEach(() => {
        vi.clearAllMocks(); // Clear mocks after each test
        server.resetHandlers(); // Reset MSW handlers
    });

    it('renders login form correctly', () => {
        renderWithProviders(<LoginPage />);
        expect(screen.getByLabelText(/email/i)).toBeInTheDocument();
        expect(screen.getByLabelText(/password/i)).toBeInTheDocument();
        expect(screen.getByRole('button', { name: /login/i })).toBeInTheDocument();
    });

    it('allows user to type into form fields', async () => {
        const user = userEvent.setup();
        renderWithProviders(<LoginPage />);
        await user.type(screen.getByLabelText(/email/i), 'test@example.com');
        await user.type(screen.getByLabelText(/password/i), 'password');
        expect(screen.getByLabelText(/email/i)).toHaveValue('test@example.com');
        expect(screen.getByLabelText(/password/i)).toHaveValue('password');
    });

    it('shows error message on failed login', async () => {
        const user = userEvent.setup();
        // Override MSW handler for this specific test to ensure failure
        server.use(
            http.post('/api/Auth/login', () => {
                return HttpResponse.json({
                    isSuccess: false, value: null, errorMessage: 'Invalid test credentials'
                }, { status: 400 });
            })
        );

        renderWithProviders(<LoginPage />);
        await user.type(screen.getByLabelText(/email/i), 'wrong@example.com');
        await user.type(screen.getByLabelText(/password/i), 'wrongpassword');
        await user.click(screen.getByRole('button', { name: /login/i }));

        expect(await screen.findByText('Invalid test credentials')).toBeInTheDocument();
    });

    it('calls login context function and navigates on successful login', async () => {
        const user = userEvent.setup();
        // MSW will use the default handler from src/mocks/handlers.js for test@example.com
        
        renderWithProviders(<LoginPage />);
        
        await user.type(screen.getByLabelText(/email/i), 'test@example.com');
        await user.type(screen.getByLabelText(/password/i), 'password');
        await user.click(screen.getByRole('button', { name: /login/i }));

        await waitFor(() => {
            expect(mockNavigate).toHaveBeenCalledWith('/');
        });
        // Check localStorage for the token after successful login
        await waitFor(() => expect(localStorage.getItem('token')).toBe('fake-access-token'));
    });
});
