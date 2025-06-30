import React from 'react';

interface LoadingProps {
  size?: 'sm' | 'md' | 'lg';
  className?: string;
  text?: string;
}

const Loading: React.FC<LoadingProps> = ({ 
  size = 'md', 
  className = '', 
  text 
}) => {
  const sizeClasses = {
    sm: 'w-4 h-4',
    md: 'w-8 h-8',
    lg: 'w-12 h-12'
  };

  return (
    <div className={`flex flex-col items-center justify-center space-y-2 ${className}`}>
      <div 
        className={`animate-spin rounded-full border-2 border-secondary-300 border-t-primary-600 ${sizeClasses[size]}`}
        data-testid="loading-spinner"
        role="status"
        aria-label="Loading"
      ></div>
      {text && (
        <p className="text-secondary-600 text-sm">{text}</p>
      )}
    </div>
  );
};

export default Loading;