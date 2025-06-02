// src/mocks/handlers.js
import { http, HttpResponse } from 'msw';

const AUTH_API_URL = '/api/Auth'; // Renamed to be specific
const MOVIES_API_URL = '/api/Movies';


export const handlers = [
    // Mock for login
    http.post(`${AUTH_API_URL}/login`, async ({ request }) => {
        const body = await request.json();
        const email = body && typeof body === 'object' ? body.email : undefined;
        if (email === 'test@example.com') {
            return HttpResponse.json({
                isSuccess: true,
                value: {
                    accessToken: 'fake-access-token',
                    refreshToken: 'fake-refresh-token',
                    expiresAt: new Date(Date.now() + 3600 * 1000).toISOString(),
                    user: { id: '1', email: 'test@example.com', userName: 'testuser', firstName: 'Test', lastName: 'User', fullName: 'Test User', profileImageUrl: null, emailConfirmed: true },
                },
                message: 'Giriş başarılı',
                errorMessage: null,
            });
        } else {
            return HttpResponse.json({
                isSuccess: false,
                value: null,
                message: null,
                errorMessage: 'Invalid credentials',
            }, { status: 400 });
        }
    }),

    // Mock for register
    http.post(`${AUTH_API_URL}/register`, async ({ request }) => {
        const body = await request.json();
        const email = body && typeof body === 'object' ? body.email : undefined;
        if (email === 'new@example.com') {
            return HttpResponse.json({
                isSuccess: true,
                value: { 
                    accessToken: 'new-fake-access-token',
                    refreshToken: 'new-fake-refresh-token',
                    expiresAt: new Date(Date.now() + 3600 * 1000).toISOString(),
                    user: { id: '2', email: 'new@example.com', userName: 'newuser', firstName: 'New', lastName: 'User', fullName: 'New User', profileImageUrl: null, emailConfirmed: false },
                },
                message: 'Kayıt başarılı',
                errorMessage: null,
            });
        } else {
             return HttpResponse.json({
                isSuccess: false,
                value: null,
                message: null,
                errorMessage: 'User already exists',
            }, { status: 400 });
        }
    }),

    // Movie Handlers
    http.get(`${MOVIES_API_URL}`, ({ request }) => {
        const url = new URL(request.url);
        const search = url.searchParams.get('search');
        // Simulating search-based error for testing
        if (search === 'error') { 
            return HttpResponse.json({ isSuccess: false, value: null, errorMessage: 'Server error fetching movies' }, { status: 500 });
        }
        return HttpResponse.json({
            isSuccess: true,
            value: {
                items: [
                    { id: '1', title: 'Inception', posterUrl: 'poster1.jpg', overview: 'A mind-bending thriller' },
                    { id: '2', title: 'The Matrix', posterUrl: 'poster2.jpg', overview: 'A hacker discovers the truth' },
                ],
                totalCount: 2,
                page: 1, // Default page
                pageSize: 20, // Default page size
            },
            message: 'Movies fetched successfully',
            errorMessage: null,
        });
    }),

    http.get(`${MOVIES_API_URL}/:id`, ({ params }) => {
        const { id } = params;
        if (id === '1') {
            return HttpResponse.json({
                isSuccess: true,
                value: {
                    id: '1',
                    title: 'Inception', 
                    overview: 'A mind-bending thriller from Christopher Nolan.', 
                    releaseDate: '2010-07-16T00:00:00Z',
                    posterUrl: 'poster1_large.jpg',
                    genres: [{id: 'g1', name: 'Sci-Fi'}, {id: 'g2', name: 'Action'}],
                    // Add other fields as expected by MovieDto
                },
                message: 'Movie details fetched successfully',
                errorMessage: null,
            });
        }
        // Simulating ID-based error for testing
        if (id === 'error_id') { 
             return HttpResponse.json({ isSuccess: false, value: null, errorMessage: 'Server error fetching movie detail' }, { status: 500 });
        }
        // Default not found for other IDs
        return HttpResponse.json({ isSuccess: false, value: null, errorMessage: 'Movie not found' }, { status: 404 });
    }),
];
