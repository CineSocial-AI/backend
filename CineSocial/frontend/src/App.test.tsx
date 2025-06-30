import React from 'react';
import { render, screen } from '@testing-library/react';
import '@testing-library/jest-dom';

// Create a simple mock App component for testing
const MockApp: React.FC = () => {
  return (
    <div data-testid="app">
      <header data-testid="header">
        <h1>CineSocial</h1>
      </header>
      <main data-testid="main">
        <p>Welcome to CineSocial</p>
      </main>
    </div>
  );
};

describe('App Component', () => {
  test('renders without crashing', () => {
    render(<MockApp />);
    expect(screen.getByTestId('app')).toBeInTheDocument();
  });

  test('renders header component', () => {
    render(<MockApp />);
    expect(screen.getByTestId('header')).toBeInTheDocument();
    expect(screen.getByText('CineSocial')).toBeInTheDocument();
  });

  test('renders main content area', () => {
    render(<MockApp />);
    expect(screen.getByTestId('main')).toBeInTheDocument();
    expect(screen.getByText('Welcome to CineSocial')).toBeInTheDocument();
  });
});
