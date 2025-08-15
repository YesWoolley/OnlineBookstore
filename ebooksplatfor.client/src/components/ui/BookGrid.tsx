
import BookCard from './BookCard';
import type { Book } from '../../types/book';

interface BookGridProps {
    books: Book[];
    loading?: boolean;
    error?: string;
    onAddToCart?: (book: Book) => void;
    onViewDetails?: (book: Book) => void;
    currentPage?: number;
    totalPages?: number;
    onPageChange?: (page: number) => void;
}

const BookGrid = ({
    books,
    loading = false,
    error,
    onAddToCart,
    onViewDetails,
    currentPage = 1,
    totalPages = 1,
    onPageChange
}: BookGridProps) => {

    // Loading skeleton
    const renderSkeletonCards = () => {
        return Array.from({ length: 8 }).map((_, index) => (
            <div key={`skeleton-${index}`} className="col-6 col-md-3 mb-4">
                <div className="card h-100">
                    <div className="card-img-top bg-light" style={{ height: '200px' }}>
                        <div className="placeholder-glow">
                            <div className="placeholder" style={{ height: '100%' }}></div>
                        </div>
                    </div>
                    <div className="card-body">
                        <div className="placeholder-glow">
                            <div className="placeholder col-8 mb-2"></div>
                            <div className="placeholder col-6 mb-2"></div>
                            <div className="placeholder col-4"></div>
                        </div>
                    </div>
                </div>
            </div>
        ));
    };

    // Error state
    if (error) {
        return (
            <div className="alert alert-danger mt-4">
                <h5 className="alert-heading">Error Loading Books</h5>
                <p>{error}</p>
            </div>
        );
    }

    // Loading state
    if (loading) {
        return (
            <div className="mt-4">
                <div className="d-flex justify-content-center mb-4">
                    <div className="spinner-border" role="status">
                        <span className="visually-hidden">Loading...</span>
                    </div>
                </div>
                <div className="row">
                    {renderSkeletonCards()}
                </div>
            </div>
        );
    }

    // Empty state
    if (books.length === 0) {
        return (
            <div className="alert alert-info mt-4">
                <h5 className="alert-heading">No Books Found</h5>
                <p>Try adjusting your search criteria or browse our full collection.</p>
            </div>
        );
    }

    return (
        <div className="book-grid">
            {/* Book Grid */}
            <div className="row row-cols-1 row-cols-sm-2 row-cols-md-3 row-cols-lg-4 g-4">
                {books.map((book) => (
                    <div key={book.id} className="col mb-4">
                        <BookCard
                            book={book}
                            onAddToCart={onAddToCart}
                            onViewDetails={onViewDetails}
                        />
                    </div>
                ))}
            </div>

            {/* Pagination */}
            {totalPages > 1 && (
                <div className="d-flex justify-content-center mt-5">
                    <nav>
                        <ul className="pagination">
                            <li className="page-item">
                                <button 
                                    className="page-link" 
                                    onClick={() => onPageChange?.(1)}
                                    disabled={currentPage === 1}
                                >
                                    First
                                </button>
                            </li>
                            <li className="page-item">
                                <button 
                                    className="page-link" 
                                    onClick={() => onPageChange?.(currentPage - 1)}
                                    disabled={currentPage === 1}
                                >
                                    Previous
                                </button>
                            </li>

                            {/* Page Numbers */}
                            {Array.from({ length: Math.min(5, totalPages) }, (_, i) => {
                                const page = Math.max(1, Math.min(totalPages - 4, currentPage - 2)) + i;
                                return (
                                    <li key={page} className={`page-item ${page === currentPage ? 'active' : ''}`}>
                                        <button 
                                            className="page-link"
                                            onClick={() => onPageChange?.(page)}
                                        >
                                            {page}
                                        </button>
                                    </li>
                                );
                            })}

                            <li className="page-item">
                                <button 
                                    className="page-link" 
                                    onClick={() => onPageChange?.(currentPage + 1)}
                                    disabled={currentPage === totalPages}
                                >
                                    Next
                                </button>
                            </li>
                            <li className="page-item">
                                <button 
                                    className="page-link" 
                                    onClick={() => onPageChange?.(totalPages)}
                                    disabled={currentPage === totalPages}
                                >
                                    Last
                                </button>
                            </li>
                        </ul>
                    </nav>
                </div>
            )}
        </div>
    );
};

export default BookGrid;