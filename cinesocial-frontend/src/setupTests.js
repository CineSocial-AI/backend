// src/setupTests.js
import '@testing-library/jest-dom';
import { server } from './mocks/server.js'; // Ensure this path is correct

// Establish API mocking before all tests.
// Fail on unhandled requests to prevent accidental real API calls.
beforeAll(() => server.listen({ onUnhandledRequest: 'error' }));

// Reset any request handlers that we may add during the tests,
// so they don't affect other tests.
afterEach(() => server.resetHandlers());

// Clean up after the tests are finished.
afterAll(() => server.close());
