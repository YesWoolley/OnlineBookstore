import React, { useState, useEffect } from 'react';
import ReviewForm from './ReviewForm';
import ReviewList from './ReviewList';
import type { Review, CreateReview, UpdateReview } from '../../types/review';

interface ReviewSectionProps {
  bookId: number;
  bookTitle: string;
  currentUserName?: string;
}

const ReviewSection: React.FC<ReviewSectionProps> = ({ 
  bookId, 
  bookTitle, 
  currentUserName
}) => {
  const [reviews, setReviews] = useState<Review[]>([]);
  const [showForm, setShowForm] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [editingReview, setEditingReview] = useState<Review | null>(null);

  // Fetch reviews when component mounts
  useEffect(() => {
    fetchReviews();
  }, [bookId]);

  const fetchReviews = async () => {
    try {
      const token = localStorage.getItem('token');
      const response = await fetch(`/api/reviews/book/${bookId}`, {
        headers: {
          'Authorization': `Bearer ${token}`
        }
      });
      if (response.ok) {
        const data = await response.json();
        setReviews(data);
      } else if (response.status === 401) {
        console.log('User not authenticated, reviews will be loaded when signed in');
        setReviews([]);
      }
    } catch (error) {
      console.error('Failed to fetch reviews:', error);
    }
  };

  const handleSubmitReview = async (reviewData: CreateReview) => {
    setIsLoading(true);
    try {
      const response = await fetch('/api/reviews', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        },
        body: JSON.stringify(reviewData)
      });

      if (response.ok) {
        const newReview = await response.json();
        setReviews(prev => [newReview, ...prev]);
        setShowForm(false);
        // Show success message
        alert('Review submitted successfully!');
      } else {
        const error = await response.json();
        alert(`Failed to submit review: ${error.message}`);
      }
    } catch (error) {
      console.error('Failed to submit review:', error);
      alert('Failed to submit review. Please try again.');
    } finally {
      setIsLoading(false);
    }
  };

  const handleEditReview = async (reviewData: UpdateReview) => {
    if (!editingReview) return;
    
    setIsLoading(true);
    try {
      const response = await fetch(`/api/reviews/${editingReview.id}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        },
        body: JSON.stringify(reviewData)
      });

      if (response.ok) {
        // Update the review in the list
        setReviews(prev => prev.map(review => 
          review.id === editingReview.id 
            ? { ...review, ...reviewData }
            : review
        ));
        setEditingReview(null);
        alert('Review updated successfully!');
      } else {
        const error = await response.json();
        alert(`Failed to update review: ${error.message}`);
      }
    } catch (error) {
      console.error('Failed to update review:', error);
      alert('Failed to update review. Please try again.');
    } finally {
      setIsLoading(false);
    }
  };

  const handleDeleteReview = async (reviewId: number) => {
    if (!confirm('Are you sure you want to delete this review?')) return;

    try {
      const response = await fetch(`/api/reviews/${reviewId}`, {
        method: 'DELETE',
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        }
      });

      if (response.ok) {
        setReviews(prev => prev.filter(review => review.id !== reviewId));
        alert('Review deleted successfully!');
      } else {
        const error = await response.json();
        alert(`Failed to delete review: ${error.message}`);
      }
    } catch (error) {
      console.error('Failed to delete review:', error);
      alert('Failed to delete review. Please try again.');
    }
  };

  const handleEditClick = (review: Review) => {
    setEditingReview(review);
    setShowForm(true);
  };

  const handleCancel = () => {
    setShowForm(false);
    setEditingReview(null);
  };

  // Check if user has already reviewed this book
  const hasUserReviewed = reviews.some(review => 
    currentUserName && review.userName === currentUserName
  );

  return (
    <div className="review-section mt-4">
      <div className="d-flex justify-content-between align-items-center mb-3">
        <h4>Reviews ({reviews.length})</h4>
        {currentUserName && !hasUserReviewed && !showForm && (
          <button
            className="btn btn-primary"
            onClick={() => setShowForm(true)}
          >
            <i className="bi bi-plus-circle me-2"></i>
            Write a Review
          </button>
        )}
      </div>

      {/* Review Form */}
      {showForm && (
        <div className="mb-4">
          <ReviewForm
            bookId={bookId}
            bookTitle={bookTitle}
            onSubmit={editingReview ? handleEditReview : handleSubmitReview}
            onCancel={handleCancel}
            isLoading={isLoading}
          />
        </div>
      )}

      {/* Review List */}
      <ReviewList
        reviews={reviews}
        currentUserName={currentUserName}
        onEditReview={handleEditClick}
        onDeleteReview={handleDeleteReview}
      />
    </div>
  );
};

export default ReviewSection;
