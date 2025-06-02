// src/services/authService.test.js
import { describe, it, expect, afterEach, beforeAll, afterAll } from 'vitest';
import { loginUser, registerUser } from './authService'; 
import { server } from '../mocks/server'; // Import the msw server

// Establish API mocking before all tests.
// Using onUnhandledRequest: 'error' to catch any unmocked requests.
beforeAll(() => server.listen({ onUnhandledRequest: 'error' }) );

// Reset any request handlers that we may add during the tests,
// so they don't affect other tests.
afterEach(() => server.resetHandlers());

// Clean up after the tests are finished.
afterAll(() => server.close());


describe('authService', () => {
    describe('loginUser', () => {
        it('should return user data and token on successful login', async () => {
            const response = await loginUser('test@example.com', 'password');
            // Axios wraps the response in a 'data' object.
            expect(response.data.isSuccess).toBe(true);
            expect(response.data.value.accessToken).toBe('fake-access-token');
            expect(response.data.value.user.email).toBe('test@example.com');
        });

        it('should throw an error with response data on failed login', async () => {
            try {
                await loginUser('wrong@example.com', 'password');
            } catch (error) {
                expect(error.isAxiosError).toBe(true); // Verify it's an axios error
                expect(error.response).toBeDefined();
                expect(error.response.data.isSuccess).toBe(false);
                expect(error.response.data.errorMessage).toBe('Invalid credentials');
                expect(error.response.status).toBe(400);
            }
        });
    });

    describe('registerUser', () => {
        it('should return user data and token on successful registration', async () => {
            const userData = {
                email: 'new@example.com',
                password: 'password123',
                confirmPassword: 'password123',
                firstName: 'New',
                lastName: 'User',
                userName: 'newuser',
            };
            const response = await registerUser(userData);
            expect(response.data.isSuccess).toBe(true);
            expect(response.data.value.accessToken).toBe('new-fake-access-token');
            expect(response.data.value.user.email).toBe('new@example.com');
        });

        it('should throw an error with response data if user already exists', async () => {
            const userData = {
                email: 'existing@example.com', // This email will trigger the "User already exists" path
                password: 'password123',
                confirmPassword: 'password123',
                firstName: 'Existing',
                lastName: 'User',
                userName: 'existinguser',
            };
            try {
                await registerUser(userData);
            } catch (error) {
                expect(error.isAxiosError).toBe(true);
                expect(error.response).toBeDefined();
                expect(error.response.data.isSuccess).toBe(false);
                expect(error.response.data.errorMessage).toBe('User already exists');
                expect(error.response.status).toBe(400);
            }
        });
    });
});
