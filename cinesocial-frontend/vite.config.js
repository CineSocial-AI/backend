import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      '/api': {
        target: 'http://localhost:5108',
        changeOrigin: true,
        secure: false,
      }
    }
  },
  // Add this 'test' section for Vitest
  test: {
    globals: true, // Enables global APIs like describe, it, expect
    environment: 'jsdom', // Simulates a browser environment
    setupFiles: './src/setupTests.js', // Path to your setup file
    css: true, // If you want to process CSS during tests
  },
})
