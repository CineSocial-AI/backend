// src/pages/MoviesListPage.test.js
import { render, screen, waitFor } from '@testing-library/react';
import { BrowserRouter } from 'react-router-dom';
import MoviesListPage from './MoviesListPage';
import { server } from '../mocks/server';
import { http, HttpResponse } from 'msw';
import { vi } from 'vitest'; // Import vi for mocking if needed, not strictly for this file

const renderWithRouter = (ui) => {
    return render(<BrowserRouter>{ui}</BrowserRouter>);
};

describe('MoviesListPage', () => {
    afterEach(() => {
        server.resetHandlers(); // Reset MSW handlers after each test
        vi.clearAllMocks(); // Clear all Vitest mocks
    });

    it('shows loading state initially', () => {
        renderWithRouter(<MoviesListPage />);
        expect(screen.getByText(/loading movies.../i)).toBeInTheDocument();
    });

    it('renders a list of movies on successful fetch', async () => {
        renderWithRouter(<MoviesListPage />);
        // Wait for movies to be loaded and displayed
        // Default MSW handler returns 'Inception' and 'The Matrix'
        expect(await screen.findByText('Inception')).toBeInTheDocument();
        expect(await screen.findByText('The Matrix')).toBeInTheDocument();
    });

    it('shows an error message if fetching movies fails', async () => {
        // Override the default handler to simulate an error
        server.use(
            http.get('/api/Movies', ({request}) => { // Ensure the path matches exactly what's called
                 // Check for query params if your component sends them by default
                const url = new URL(request.url);
                if (!url.searchParams.has('search')) { // Only fail if it's the default call without specific error search
                    return HttpResponse.json({ isSuccess: false, value: null, errorMessage: 'Custom API Error' }, { status: 500 });
                }
            })
        );
        renderWithRouter(<MoviesListPage />);
        expect(await screen.findByText(/error: Custom API Error/i)).toBeInTheDocument();
    });

    it('shows "No movies found." if API returns empty list', async () => {
        server.use(
            http.get('/api/Movies', () => {
                return HttpResponse.json({
                    isSuccess: true,
                    value: { items: [], totalCount: 0, page: 1, pageSize: 20 },
                    message: 'No movies found',
                    errorMessage: null,
                });
            })
        );
        renderWithRouter(<MoviesListPage />);
        expect(await screen.findByText(/no movies found/i)).toBeInTheDocument();
    });
});
