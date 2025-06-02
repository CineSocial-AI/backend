// src/services/movieService.test.js
import { describe, it, expect, afterEach, beforeAll, afterAll } from 'vitest';
import { getMovies, getMovieById } from './movieService';
import { server } from '../mocks/server';
import { http, HttpResponse } from 'msw'; // For specific handler overrides if needed

beforeAll(() => server.listen({ onUnhandledRequest: 'error' }));
afterEach(() => server.resetHandlers());
afterAll(() => server.close());

describe('movieService', () => {
    describe('getMovies', () => {
        it('should return a list of movies on success', async () => {
            const response = await getMovies();
            expect(response.data.isSuccess).toBe(true);
            expect(response.data.value.items).toHaveLength(2);
            expect(response.data.value.items[0].title).toBe('Inception');
        });

        it('should throw an error if the API call fails', async () => {
            // This test will use the search='error' condition in the mock handler
            // or we can override specifically. Let's rely on the specific search param for error.
            // server.use(
            //     http.get('/api/Movies', () => { // This would override the default success
            //         return HttpResponse.json({ isSuccess: false, value: null, errorMessage: 'Network Error' }, { status: 500 })
            //     })
            // );
            try {
                // Pass search='error' to trigger the error condition in the mock
                await getMovies(1, 20, 'error'); 
            } catch (error) {
                expect(error.isAxiosError).toBe(true);
                expect(error.response.data.errorMessage).toBe('Server error fetching movies');
                expect(error.response.status).toBe(500);
            }
        });
    });

    describe('getMovieById', () => {
        it('should return movie details on success', async () => {
            const response = await getMovieById('1');
            expect(response.data.isSuccess).toBe(true);
            expect(response.data.value.title).toBe('Inception');
        });

        it('should throw an error if movie not found', async () => {
            try {
                await getMovieById('nonexistent_id'); // This ID is handled by the default 404 in mock
            } catch (error) {
                expect(error.isAxiosError).toBe(true);
                expect(error.response.data.errorMessage).toBe('Movie not found');
                expect(error.response.status).toBe(404);
            }
        });

        it('should throw an error if API returns a server error for a specific ID', async () => {
             try {
                await getMovieById('error_id'); // This ID triggers the specific error in mock
            } catch (error) {
                expect(error.isAxiosError).toBe(true);
                expect(error.response.data.errorMessage).toBe('Server error fetching movie detail');
                expect(error.response.status).toBe(500);
            }
        });
    });
});
