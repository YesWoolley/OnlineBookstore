import React from 'react';

interface ErrorMessageProps {
  error: string;
  onRetry?: () => void;
  className?: string;
}

const ErrorMessage: React.FC<ErrorMessageProps> = ({ 
  error, 
  onRetry, 
  className = '' 
}) => {
  return (
    <div className={`alert alert-danger ${className}`} role="alert">
      <div className="d-flex align-items-center">
        <i className="bi bi-exclamation-triangle me-2"></i>
        <div className="flex-grow-1">
          <strong>Error:</strong> {error}
        </div>
        {onRetry && (
          <button
            type="button"
            className="btn btn-outline-danger btn-sm ms-2"
            onClick={onRetry}
          >
            <i className="bi bi-arrow-clockwise me-1"></i>
            Retry
          </button>
        )}
      </div>
    </div>
  );
};

export default ErrorMessage;

