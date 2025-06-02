// src/pages/RegisterPage.test.js
import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { BrowserRouter } from 'react-router-dom';
import { AuthProvider } from '../contexts/AuthContext'; 
import RegisterPage from './RegisterPage'; 
import { server } from '../mocks/server';
import { http, HttpResponse } from 'msw';
import { vi } from 'vitest'; // Import vi for mocking

const mockNavigate = vi.fn();
vi.mock('react-router-dom', async (importOriginal) => {
    const actual = await importOriginal();
    return {
        ...actual,
        useNavigate: () => mockNavigate,
    };
});

const renderWithProviders = (ui) => {
    return render(
        <BrowserRouter>
            <AuthProvider>
                {ui}
            </AuthProvider>
        </BrowserRouter>
    );
};

describe('RegisterPage', () => {
    beforeEach(() => { // Use beforeEach to clear localStorage for token check consistency
        localStorage.clear();
    });
    afterEach(() => {
        vi.clearAllMocks();
        server.resetHandlers();
    });

    it('renders registration form correctly', () => {
        renderWithProviders(<RegisterPage />);
        expect(screen.getByLabelText(/first name/i)).toBeInTheDocument();
        expect(screen.getByLabelText(/last name/i)).toBeInTheDocument();
        expect(screen.getByLabelText(/username/i)).toBeInTheDocument();
        expect(screen.getByLabelText(/email/i)).toBeInTheDocument();
        expect(screen.getByLabelText(/^password$/i)).toBeInTheDocument(); // Use regex for exact match
        expect(screen.getByLabelText(/confirm password/i)).toBeInTheDocument();
        expect(screen.getByRole('button', { name: /register/i })).toBeInTheDocument();
    });

    it('shows error message on failed registration (e.g., user exists)', async () => {
        const user = userEvent.setup();
        server.use(
            http.post('/api/Auth/register', async () => { // made async to match original handler structure
                return HttpResponse.json({
                    isSuccess: false, value: null, errorMessage: 'Test user already exists'
                }, { status: 400 });
            })
        );

        renderWithProviders(<RegisterPage />);
        await user.type(screen.getByLabelText(/first name/i), 'Test');
        await user.type(screen.getByLabelText(/last name/i), 'User');
        await user.type(screen.getByLabelText(/username/i), 'testuser');
        await user.type(screen.getByLabelText(/email/i), 'existing@example.com');
        await user.type(screen.getByLabelText(/^password$/i), 'password123');
        await user.type(screen.getByLabelText(/confirm password/i), 'password123');
        await user.click(screen.getByRole('button', { name: /register/i }));

        expect(await screen.findByText('Test user already exists')).toBeInTheDocument();
    });

    it('navigates on successful registration and logs in user', async () => {
        const user = userEvent.setup();
        // MSW will use default handler for new@example.com from src/mocks/handlers.js
        
        renderWithProviders(<RegisterPage />);
        await user.type(screen.getByLabelText(/first name/i), 'New');
        await user.type(screen.getByLabelText(/last name/i), 'User');
        await user.type(screen.getByLabelText(/username/i), 'newuser');
        await user.type(screen.getByLabelText(/email/i), 'new@example.com');
        await user.type(screen.getByLabelText(/^password$/i), 'password123');
        await user.type(screen.getByLabelText(/confirm password/i), 'password123');
        await user.click(screen.getByRole('button', { name: /register/i }));

        await waitFor(() => {
            // Check if the AuthContext's register/login function was called, which updates localStorage
            // And then navigation occurs
            expect(localStorage.getItem('token')).toBe('new-fake-access-token');
            expect(mockNavigate).toHaveBeenCalledWith('/'); 
        });
    });

    it('shows error if passwords do not match', async () => {
        const user = userEvent.setup();
        renderWithProviders(<RegisterPage />);
        
        await user.type(screen.getByLabelText(/first name/i), 'Test');
        await user.type(screen.getByLabelText(/last name/i), 'User');
        await user.type(screen.getByLabelText(/username/i), 'testuser');
        await user.type(screen.getByLabelText(/email/i), 'test@example.com');
        await user.type(screen.getByLabelText(/^password$/i), 'password123');
        await user.type(screen.getByLabelText(/confirm password/i), 'password456'); // Mismatched password
        await user.click(screen.getByRole('button', { name: /register/i }));

        expect(await screen.findByText("Passwords don't match")).toBeInTheDocument();
        expect(mockNavigate).not.toHaveBeenCalled(); // Ensure navigation did not occur
        expect(localStorage.getItem('token')).toBeNull(); // Ensure no token was set
    });
});
