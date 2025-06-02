// src/pages/MovieDetailPage.test.js
import { render, screen, waitFor } from '@testing-library/react';
import { MemoryRouter, Routes, Route } from 'react-router-dom';
import MovieDetailPage from './MovieDetailPage';
import { server } from '../mocks/server';
import { http, HttpResponse } from 'msw';
import { vi } from 'vitest'; // Import vi for mocking if needed

const renderWithRouterAndId = (id) => {
    return render(
        <MemoryRouter initialEntries={[`/movies/${id}`]}>
            <Routes>
                <Route path="/movies/:id" element={<MovieDetailPage />} />
            </Routes>
        </MemoryRouter>
    );
};

describe('MovieDetailPage', () => {
    afterEach(() => {
        server.resetHandlers(); // Reset MSW handlers after each test
        vi.clearAllMocks(); // Clear all Vitest mocks
    });

    it('shows loading state initially', () => {
        renderWithRouterAndId('1'); // Use a valid ID that would normally fetch data
        expect(screen.getByText(/loading movie details.../i)).toBeInTheDocument();
    });

    it('renders movie details on successful fetch for ID "1"', async () => {
        renderWithRouterAndId('1'); // This ID is handled by the default success mock
        // Wait for details to be loaded (e.g., title and overview)
        expect(await screen.findByText('Inception')).toBeInTheDocument();
        expect(await screen.findByText(/A mind-bending thriller from Christopher Nolan./i)).toBeInTheDocument();
        // Check for genres if they are part of the mock and component rendering
        expect(await screen.findByText('Sci-Fi')).toBeInTheDocument();
        expect(await screen.findByText('Action')).toBeInTheDocument();
    });

    it('shows an error message if fetching movie details fails for ID "error_id"', async () => {
        // The 'error_id' is specifically mocked to return a server error in handlers.js
        renderWithRouterAndId('error_id');
        expect(await screen.findByText(/Error: Server error fetching movie detail/i)).toBeInTheDocument();
    });

    it('shows "Movie not found" message for a non-existent movie ID', async () => {
        // 'nonexistent_id' should trigger the default 404 mock in handlers.js
        renderWithRouterAndId('nonexistent_id'); 
        expect(await screen.findByText(/Movie not found/i)).toBeInTheDocument();
    });
    
    it('handles cases where no ID is provided in the route (though Route setup should prevent this)', async () => {
        // This tests the component's internal check for `id` if route was `/movies/`
        // For this to work, the route in MemoryRouter needs to allow it.
        // The current setup with path="/movies/:id" will not match "/movies/"
        // However, if the component was rendered without an id param due to a routing misconfiguration,
        // this tests the component's robustness.
        render(
            <MemoryRouter initialEntries={[`/movies/`]}> 
                <Routes>
                    {/* A route that might accidentally pass no id if not careful with route definitions */}
                    <Route path="/movies/" element={<MovieDetailPage />} /> 
                </Routes>
            </MemoryRouter>
        );
        // The component itself has a check: if (!id) setError("No movie ID provided.");
        expect(await screen.findByText(/Error: No movie ID provided/i)).toBeInTheDocument();
    });
});
