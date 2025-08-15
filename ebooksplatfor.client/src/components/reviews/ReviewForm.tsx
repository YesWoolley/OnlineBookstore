import React, { useState } from 'react';
import type { CreateReview } from '../../types/review';

interface ReviewFormProps {
  bookId: number;
  bookTitle: string;
  onSubmit: (review: CreateReview) => void;
  onCancel: () => void;
  isLoading?: boolean;
}

const ReviewForm: React.FC<ReviewFormProps> = ({ 
  bookId, 
  bookTitle, 
  onSubmit, 
  onCancel, 
  isLoading = false 
}) => {
  const [rating, setRating] = useState(5);
  const [comment, setComment] = useState('');
  const [errors, setErrors] = useState<{ rating?: string; comment?: string }>({});

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    
    // Validation
    const newErrors: { rating?: string; comment?: string } = {};
    
    if (rating < 1 || rating > 5) {
      newErrors.rating = 'Rating must be between 1 and 5';
    }
    
    if (comment.length > 1000) {
      newErrors.comment = 'Comment cannot exceed 1000 characters';
    }
    
    if (Object.keys(newErrors).length > 0) {
      setErrors(newErrors);
      return;
    }
    
    onSubmit({
      bookId,
      rating,
      comment: comment.trim() || undefined
    });
  };

  const renderStars = (rating: number) => {
    const stars = [];
    for (let i = 1; i <= 5; i++) {
      stars.push(
        <button
          key={i}
          type="button"
          className={`btn btn-link p-0 me-1 ${i <= rating ? 'text-warning' : 'text-muted'}`}
          onClick={() => setRating(i)}
          disabled={isLoading}
        >
          <i className={`bi ${i <= rating ? 'bi-star-fill' : 'bi-star'}`} style={{ fontSize: '1.5rem' }}></i>
        </button>
      );
    }
    return stars;
  };

  return (
    <div className="card">
      <div className="card-header">
        <h5 className="mb-0">Write a Review for "{bookTitle}"</h5>
      </div>
      <div className="card-body">
        <form onSubmit={handleSubmit}>
          {/* Rating Selection */}
          <div className="mb-3">
            <label className="form-label">Rating *</label>
            <div className="d-flex align-items-center">
              {renderStars(rating)}
              <span className="ms-2 fw-bold">{rating}/5</span>
            </div>
            {errors.rating && (
              <div className="text-danger small mt-1">{errors.rating}</div>
            )}
          </div>

          {/* Comment Input */}
          <div className="mb-3">
            <label htmlFor="comment" className="form-label">Comment (Optional)</label>
            <textarea
              id="comment"
              className={`form-control ${errors.comment ? 'is-invalid' : ''}`}
              rows={4}
              value={comment}
              onChange={(e) => setComment(e.target.value)}
              placeholder="Share your thoughts about this book..."
              disabled={isLoading}
            />
            {errors.comment && (
              <div className="text-danger small mt-1">{errors.comment}</div>
            )}
            <div className="form-text">
              {comment.length}/1000 characters
            </div>
          </div>

          {/* Action Buttons */}
          <div className="d-flex gap-2">
            <button
              type="submit"
              className="btn btn-primary"
              disabled={isLoading}
            >
              {isLoading ? (
                <>
                  <span className="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>
                  Submitting...
                </>
              ) : (
                'Submit Review'
              )}
            </button>
            <button
              type="button"
              className="btn btn-outline-secondary"
              onClick={onCancel}
              disabled={isLoading}
            >
              Cancel
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default ReviewForm;






