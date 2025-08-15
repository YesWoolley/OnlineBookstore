import React, { useEffect, useState } from 'react';
import { useSearchParams, Link, useNavigate } from 'react-router-dom';
import { apiService } from '../../services/api';
import BookGrid from '../ui/BookGrid';
import type { Book } from '../../types/book';

interface SearchResultsProps {
  onAddToCart?: (book: Book) => void;
}

const SearchResults: React.FC<SearchResultsProps> = ({ onAddToCart }) => {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const query = searchParams.get('q') || '';
  const [searchResults, setSearchResults] = useState<Book[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (query.trim()) {
      performSearch(query);
    }
  }, [query]);

  const performSearch = async (searchQuery: string) => {
    setIsLoading(true);
    setError(null);
    
    try {
      const results = await apiService.searchBooks(searchQuery);
      setSearchResults(results);
    } catch (error) {
      console.error('Search error:', error);
      setError('Failed to perform search. Please try again.');
    } finally {
      setIsLoading(false);
    }
  };

  const handleAddToCart = (book: Book) => {
    if (onAddToCart) {
      onAddToCart(book);
    }
  };

  const handleViewDetails = (book: Book) => {
    navigate(`/books/${book.id}`);
  };

  if (!query.trim()) {
    return (
      <div className="container py-5">
        <div className="row justify-content-center">
          <div className="col-md-6 text-center">
            <h2>Search Books</h2>
            <p className="text-muted">Enter a search term to find books, authors, or categories.</p>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="container py-4">
      {/* Search Header */}
      <div className="row mb-4">
        <div className="col-12">
          <div className="d-flex justify-content-between align-items-center">
            <div>
              <h2>Search Results</h2>
              <p className="text-muted mb-0">
                {isLoading ? 'Searching...' : `Found ${searchResults.length} result${searchResults.length !== 1 ? 's' : ''} for "${query}"`}
              </p>
            </div>
            <Link to="/" className="btn btn-outline-secondary">
              <i className="bi bi-arrow-left me-2"></i>
              Back to Books
            </Link>
          </div>
        </div>
      </div>

      {/* Search Results */}
      {error ? (
        <div className="alert alert-danger" role="alert">
          <h5 className="alert-heading">Search Error</h5>
          <p>{error}</p>
          <button 
            className="btn btn-outline-danger"
            onClick={() => performSearch(query)}
          >
            Try Again
          </button>
        </div>
      ) : (
        <BookGrid
          books={searchResults}
          loading={isLoading}
          error={error || undefined}
          onAddToCart={handleAddToCart}
          onViewDetails={handleViewDetails}
        />
      )}

      {/* No Results Message */}
      {!isLoading && !error && searchResults.length === 0 && query.trim() && (
        <div className="text-center py-5">
          <i className="bi bi-search" style={{ fontSize: '3rem', color: '#6c757d' }}></i>
          <h3 className="mt-3">No Results Found</h3>
          <p className="text-muted">
            We couldn't find any books matching "{query}". Try different keywords or browse our full collection.
          </p>
          <Link to="/" className="btn btn-primary">
            Browse All Books
          </Link>
        </div>
      )}
    </div>
  );
};

export default SearchResults;
