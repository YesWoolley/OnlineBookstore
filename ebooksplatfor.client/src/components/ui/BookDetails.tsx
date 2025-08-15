
import type { Book } from '../../types/book';
import ReviewSection from '../reviews/ReviewSection';

interface BookDetailsProps {
    book: Book;
    onAddToCart?: (book: Book) => void;
    onBackToList?: () => void;
    currentUserName?: string;
}

const BookDetails = ({ book, onAddToCart, onBackToList, currentUserName }: BookDetailsProps) => {
    const renderStars = (rating: number) => {
        const stars = [];
        const fullStars = Math.floor(rating);
        const hasHalfStar = rating % 1 !== 0;
        
        // Full stars
        for (let i = 0; i < fullStars; i++) {
            stars.push(<i key={`full-${i}`} className="bi bi-star-fill text-warning"></i>);
        }
        
        // Half star
        if (hasHalfStar) {
            stars.push(<i key="half" className="bi bi-star-half text-warning"></i>);
        }
        
        // Empty stars
        const emptyStars = 5 - fullStars - (hasHalfStar ? 1 : 0);
        for (let i = 0; i < emptyStars; i++) {
            stars.push(<i key={`empty-${i}`} className="bi bi-star text-warning"></i>);
        }
        
        return stars;
    };

    return (
        <div className="container py-4">
            {/* Back Button */}
            <div className="mb-4">
                <button 
                    className="btn btn-outline-secondary"
                    onClick={onBackToList}
                >
                    <i className="bi bi-arrow-left me-2"></i>
                    Back to Books
                </button>
            </div>

            <div className="row">
                {/* Book Cover */}
                <div className="col-md-4 mb-4">
                    <div className="card border-0 shadow-sm h-100 d-flex flex-column">
                        <img
                            src={book.coverImageUrl || '/placeholder-book.jpg'}
                            className="card-img-top flex-grow-1"
                            alt={book.title}
                            style={{ objectFit: 'cover', minHeight: '400px' }}
                        />
                    </div>
                </div>

                {/* Book Information */}
                <div className="col-md-8">
                    <div className="card border-0 shadow-sm h-100">
                        <div className="card-body">
                            {/* Title */}
                            <h1 className="card-title display-5 mb-3">{book.title}</h1>
                            
                            {/* Author */}
                            <p className="text-muted mb-3">
                                <i className="bi bi-person me-2"></i>
                                by <strong>{book.authorName}</strong>
                            </p>

                            {/* Rating */}
                            <div className="mb-3">
                                <div className="d-flex align-items-center mb-2">
                                    <div className="me-2">
                                        {renderStars(book.averageRating)}
                                    </div>
                                    <span className="fw-bold me-2">{book.averageRating.toFixed(1)}</span>
                                    <span className="text-muted">({book.reviewCount} reviews)</span>
                                </div>
                            </div>

                            {/* Price and Stock */}
                            <div className="mb-4">
                                <div className="d-flex align-items-center justify-content-between">
                                    <div>
                                        <span className="display-6 fw-bold text-primary">${book.price}</span>
                                    </div>
                                    <div>
                                        {book.stockQuantity > 0 ? (
                                            <span className="badge bg-success">
                                                <i className="bi bi-check-circle me-1"></i>
                                                In Stock ({book.stockQuantity} available)
                                            </span>
                                        ) : (
                                            <span className="badge bg-danger">
                                                <i className="bi bi-x-circle me-1"></i>
                                                Out of Stock
                                            </span>
                                        )}
                                    </div>
                                </div>
                            </div>

                            {/* Book Details */}
                            <div className="row mb-4">
                                <div className="col-sm-6">
                                    <div className="d-flex align-items-center mb-2">
                                        <i className="bi bi-building me-2 text-muted"></i>
                                        <span><strong>Publisher:</strong> {book.publisherName}</span>
                                    </div>
                                </div>
                                <div className="col-sm-6">
                                    <div className="d-flex align-items-center mb-2">
                                        <i className="bi bi-tag me-2 text-muted"></i>
                                        <span><strong>Category:</strong> {book.categoryName}</span>
                                    </div>
                                </div>
                            </div>

                            {/* Description */}
                            {book.description && (
                                <div className="mb-4">
                                    <h5>Description</h5>
                                    <p className="text-muted">{book.description}</p>
                                </div>
                            )}

                            {/* Action Buttons */}
                            <div className="d-flex gap-2">
                                <button
                                    className="btn btn-primary btn-lg"
                                    onClick={() => onAddToCart?.(book)}
                                    disabled={book.stockQuantity === 0}
                                >
                                    <i className="bi bi-cart-plus me-1"></i>
                                    {book.stockQuantity === 0 ? 'Out of Stock' : 'Add to Cart'}
                                </button>
                                <button className="btn btn-outline-secondary btn-lg">
                                    <i className="bi bi-heart me-1"></i>
                                    Wishlist
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            {/* Additional Information Section */}
            <div className="row mt-5">
                <div className="col-12">
                    <div className="card border-0 shadow-sm">
                        <div className="card-header bg-light">
                            <h5 className="mb-0">
                                <i className="bi bi-info-circle me-2"></i>
                                Book Information
                            </h5>
                        </div>
                        <div className="card-body">
                            <div className="row">
                                <div className="col-md-6">
                                    <ul className="list-unstyled">
                                        <li className="mb-2">
                                            <strong>Author:</strong> {book.authorName}
                                        </li>
                                        <li className="mb-2">
                                            <strong>Publisher:</strong> {book.publisherName}
                                        </li>
                                        <li className="mb-2">
                                            <strong>Category:</strong> {book.categoryName}
                                        </li>
                                    </ul>
                                </div>
                                <div className="col-md-6">
                                    <ul className="list-unstyled">
                                        <li className="mb-2">
                                            <strong>Price:</strong> ${book.price}
                                        </li>
                                        <li className="mb-2">
                                            <strong>Stock:</strong> {book.stockQuantity} copies
                                        </li>
                                        <li className="mb-2">
                                            <strong>Rating:</strong> {book.averageRating.toFixed(1)}/5 ({book.reviewCount} reviews)
                                        </li>
                                    </ul>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            {/* Reviews Section */}
            <ReviewSection
                bookId={book.id}
                bookTitle={book.title}
                currentUserName={currentUserName}
            />
        </div>
    );
};

export default BookDetails; 