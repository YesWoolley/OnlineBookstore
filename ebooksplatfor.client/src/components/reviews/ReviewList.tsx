import React from 'react';
import type { Review } from '../../types/review';

interface ReviewListProps {
  reviews: Review[];
  currentUserName?: string;
  onEditReview?: (review: Review) => void;
  onDeleteReview?: (reviewId: number) => void;
}

const ReviewList: React.FC<ReviewListProps> = ({ 
  reviews, 
  currentUserName, 
  onEditReview, 
  onDeleteReview 
}) => {
  const renderStars = (rating: number) => {
    const stars = [];
    for (let i = 1; i <= 5; i++) {
      stars.push(
        <i 
          key={i} 
          className={`bi ${i <= rating ? 'bi-star-fill text-warning' : 'bi-star text-muted'}`}
        ></i>
      );
    }
    return stars;
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    });
  };

  if (reviews.length === 0) {
    return (
      <div className="text-center py-4">
        <i className="bi bi-chat-dots text-muted" style={{ fontSize: '3rem' }}></i>
        <p className="text-muted mt-2">No reviews yet. Be the first to review this book!</p>
      </div>
    );
  }

  return (
    <div className="review-list">
      {reviews.map((review) => (
        <div key={review.id} className="card mb-3">
          <div className="card-body">
            {/* Review Header */}
            <div className="d-flex justify-content-between align-items-start mb-2">
              <div>
                <h6 className="mb-1 fw-bold">{review.userName}</h6>
                <div className="d-flex align-items-center">
                  {renderStars(review.rating)}
                  <span className="ms-2 text-muted small">
                    {review.rating}/5
                  </span>
                </div>
              </div>
              <div className="text-end">
                <small className="text-muted d-block">
                  {formatDate(review.createdAt)}
                </small>
                {/* Edit/Delete buttons for user's own reviews */}
                {currentUserName && review.userName === currentUserName && (
                  <div className="mt-1">
                    <button
                      className="btn btn-sm btn-outline-primary me-1"
                      onClick={() => onEditReview?.(review)}
                    >
                      <i className="bi bi-pencil"></i>
                    </button>
                    <button
                      className="btn btn-sm btn-outline-danger"
                      onClick={() => onDeleteReview?.(review.id)}
                    >
                      <i className="bi bi-trash"></i>
                    </button>
                  </div>
                )}
              </div>
            </div>

            {/* Review Comment */}
            {review.comment && (
              <p className="mb-0">{review.comment}</p>
            )}
          </div>
        </div>
      ))}
    </div>
  );
};

export default ReviewList;





