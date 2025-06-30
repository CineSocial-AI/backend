import React from 'react';
import { render, screen } from '@testing-library/react';
import '@testing-library/jest-dom';
import Loading from './Loading';

describe('Loading Component', () => {
  test('renders loading spinner with default props', () => {
    render(<Loading />);
    
    const loadingElement = screen.getByTestId('loading-spinner');
    expect(loadingElement).toBeInTheDocument();
    expect(loadingElement).toHaveClass('w-8', 'h-8');
  });

  test('renders with small size', () => {
    render(<Loading size="sm" />);
    
    const loadingElement = screen.getByTestId('loading-spinner');
    expect(loadingElement).toHaveClass('w-4', 'h-4');
  });

  test('renders with large size', () => {
    render(<Loading size="lg" />);
    
    const loadingElement = screen.getByTestId('loading-spinner');
    expect(loadingElement).toHaveClass('w-12', 'h-12');
  });

  test('renders with custom text', () => {
    const customText = 'Loading movies...';
    render(<Loading text={customText} />);
    
    expect(screen.getByText(customText)).toBeInTheDocument();
  });

  test('renders without text when not provided', () => {
    render(<Loading />);
    
    // Should not have any text element
    expect(screen.queryByText(/loading/i)).not.toBeInTheDocument();
  });

  test('renders with both text and custom size', () => {
    const customText = 'Please wait...';
    render(<Loading size="lg" text={customText} />);
    
    const loadingElement = screen.getByTestId('loading-spinner');
    expect(loadingElement).toHaveClass('w-12', 'h-12');
    expect(screen.getByText(customText)).toBeInTheDocument();
  });

  test('has correct accessibility attributes', () => {
    render(<Loading text="Loading content" />);
    
    const loadingElement = screen.getByTestId('loading-spinner');
    expect(loadingElement).toHaveAttribute('role', 'status');
    expect(loadingElement).toHaveAttribute('aria-label', 'Loading');
  });
});