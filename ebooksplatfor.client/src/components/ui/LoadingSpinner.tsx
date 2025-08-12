import React from 'react';

interface LoadingSpinnerProps {
  size?: 'small' | 'medium' | 'large';
  message?: string;
  className?: string;
}

const LoadingSpinner: React.FC<LoadingSpinnerProps> = ({ 
  size = 'medium', 
  message = 'Loading...', 
  className = '' 
}) => {
  const sizeClasses = {
    small: 'spinner-border-sm',
    medium: '',
    large: 'spinner-border-lg'
  };

  return (
    <div className={`d-flex justify-content-center align-items-center ${className}`}>
      <div className="text-center">
        <div className={`spinner-border text-primary ${sizeClasses[size]}`} role="status">
          <span className="visually-hidden">Loading...</span>
        </div>
        {message && (
          <p className="mt-2 text-muted">{message}</p>
        )}
      </div>
    </div>
  );
};

export default LoadingSpinner;

