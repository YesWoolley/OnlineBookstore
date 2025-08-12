import type { Book } from '../../types/book';

interface BookCardProps {
  book: Book;
  onAddToCart?: (book: Book) => void;
  onViewDetails?: (book: Book) => void;
}

const BookCard = ({ book, onAddToCart, onViewDetails }: BookCardProps) => {
  return (
    <div className="card h-100 d-flex flex-column" style={{ width: '100%' }}>
      {/* Fixed height image container */}
      <div className="position-relative" style={{ height: '280px', overflow: 'hidden' }}>
        <img 
          src={book.coverImageUrl || '/placeholder-book.jpg'} 
          className="card-img-top w-100 h-100"
          alt={book.title}
          style={{ objectFit: 'cover' }}
        />
      </div>
      
      <div className="card-body d-flex flex-column flex-grow-1">
        {/* Title with fixed height and truncation */}
        <h5 className="card-title" style={{ 
          height: '48px', 
          overflow: 'hidden',
          display: '-webkit-box',
          WebkitLineClamp: 2,
          WebkitBoxOrient: 'vertical'
        }}>
          {book.title}
        </h5>
        
        {/* Author with fixed height */}
        <p className="card-text mb-2" style={{ 
          height: '24px',
          overflow: 'hidden',
          textOverflow: 'ellipsis',
          whiteSpace: 'nowrap'
        }}>
          by {book.authorName}
        </p>
        
        {/* Price and rating section */}
        <div className="d-flex justify-content-between align-items-center mb-3">
          <span className="fw-bold fs-5">${book.price}</span>
          <small className="text-muted text-end" style={{ fontSize: '0.8rem' }}>
            Rating: {book.averageRating.toFixed(1)}/5<br/>
            <span style={{ fontSize: '0.75rem' }}>({book.reviewCount} reviews)</span>
          </small>
        </div>
        
        {/* Buttons section - pushed to bottom */}
        <div className="mt-auto">
          <div className="d-grid gap-2">
            <button 
              className="btn btn-primary"
              onClick={() => onAddToCart?.(book)}
              disabled={book.stockQuantity === 0}
            >
              {book.stockQuantity === 0 ? 'Out of Stock' : 'Add to Cart'}
            </button>
            <button 
              className="btn btn-outline-secondary"
              onClick={() => onViewDetails?.(book)}
            >
              View Details
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default BookCard;