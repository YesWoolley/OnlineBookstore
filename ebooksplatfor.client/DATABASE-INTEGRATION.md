# Database Integration Guide

## Overview

This guide explains how the frontend application has been updated to integrate with the backend database instead of using mock data.

## What Changed

### 1. **API Service Layer** (`src/services/api.ts`)
- Created a centralized service for all HTTP requests
- Handles GET, POST, PUT, DELETE operations
- Provides methods for all major entities (books, authors, categories, etc.)
- Built-in error handling and logging

### 2. **React Query Integration** (`src/hooks/useBooks.ts`)
- Replaced custom state management with React Query
- Automatic caching and background updates
- Built-in loading and error states
- Optimistic updates and retry logic

### 3. **Configuration** (`src/config/api.ts`)
- Centralized API configuration
- Environment-specific settings
- Easy to modify for different deployment environments

## Key Benefits

✅ **Real Data**: No more mock data - everything comes from your database  
✅ **Better Performance**: React Query provides intelligent caching  
✅ **Error Handling**: Graceful error states with retry functionality  
✅ **Loading States**: Professional loading indicators  
✅ **Type Safety**: Full TypeScript support  
✅ **Maintainable**: Centralized API logic  

## Usage Examples

### Fetching All Books
```typescript
import { useBooks } from '../hooks/useBooks';

const BookGrid = () => {
  const { data: books, isLoading, error } = useBooks();
  
  if (isLoading) return <LoadingSpinner />;
  if (error) return <ErrorMessage error={error.message} />;
  
  return <BookGrid books={books} />;
};
```

### Fetching Single Book
```typescript
import { useBook } from '../hooks/useBooks';

const BookDetails = ({ bookId }) => {
  const { data: book, isLoading, error } = useBook(bookId);
  
  if (isLoading) return <LoadingSpinner />;
  if (error) return <ErrorMessage error={error.message} />;
  
  return <BookCard book={book} />;
};
```

### Searching Books
```typescript
import { useBookSearch } from '../hooks/useBooks';

const SearchResults = ({ query }) => {
  const { data: books, isLoading } = useBookSearch(query);
  
  return (
    <div>
      {isLoading && <LoadingSpinner />}
      <BookGrid books={books || []} />
    </div>
  );
};
```

## API Endpoints

The following endpoints are available through the API service:

### Books
- `GET /api/books` - Get all books
- `GET /api/books/{id}` - Get book by ID
- `GET /api/books/{id}/detail` - Get detailed book information
- `GET /api/books/search?q={query}` - Search books
- `GET /api/books/category/{categoryId}` - Get books by category

### Authors
- `GET /api/authors` - Get all authors
- `GET /api/authors/{id}` - Get author by ID

### Categories
- `GET /api/categories` - Get all categories
- `GET /api/categories/{id}` - Get category by ID

### Publishers
- `GET /api/publishers` - Get all publishers
- `GET /api/publishers/{id}` - Get publisher by ID

### Reviews
- `GET /api/reviews/book/{bookId}` - Get reviews for a book
- `POST /api/reviews` - Create a new review

### Shopping Cart
- `GET /api/shoppingcart/user/{userId}` - Get user's cart items
- `POST /api/shoppingcart` - Add item to cart
- `PUT /api/shoppingcart/{id}` - Update cart item
- `DELETE /api/shoppingcart/{id}` - Remove item from cart

### Orders
- `POST /api/orders` - Create new order
- `GET /api/orders/user/{userId}` - Get user's orders

### Authentication
- `POST /api/auth/signin` - User sign in
- `POST /api/auth/signup` - User sign up
- `GET /api/auth/me` - Get current user

## Configuration

### Development
- API Base URL: `https://localhost:7273/api`
- Configured in `src/config/api.ts`

### Production
- Set `VITE_API_BASE_URL` environment variable
- Or modify `src/config/api.ts` directly

## Error Handling

The application now handles errors gracefully:

1. **Network Errors**: Automatic retry with exponential backoff
2. **API Errors**: User-friendly error messages with retry buttons
3. **Loading States**: Professional loading indicators
4. **Fallbacks**: Graceful degradation when data is unavailable

## Performance Features

- **Automatic Caching**: React Query caches responses intelligently
- **Background Updates**: Data refreshes automatically in background
- **Stale Time**: Configurable cache invalidation
- **Garbage Collection**: Automatic cleanup of unused data
- **Optimistic Updates**: Immediate UI updates for better UX

## Testing

To test the database integration:

1. **Start Backend**: Ensure your .NET backend is running on `https://localhost:7273`
2. **Start Frontend**: Run `npm run dev` in the client directory
3. **Verify Data**: Check that books are loaded from the database instead of mock data
4. **Test Features**: Try searching, viewing book details, etc.

## Troubleshooting

### Common Issues

1. **CORS Errors**: Ensure backend has proper CORS configuration
2. **Connection Refused**: Check if backend is running on correct port
3. **Data Not Loading**: Check browser console for API errors
4. **Authentication Issues**: Verify JWT token handling

### Debug Mode

Enable debug logging by checking the browser console for API calls and responses.

## Next Steps

With database integration complete, consider:

1. **Search Functionality**: Implement advanced search with filters
2. **Pagination**: Add server-side pagination for large datasets
3. **Real-time Updates**: Implement WebSocket connections for live data
4. **Offline Support**: Add service worker for offline functionality
5. **Performance Monitoring**: Add analytics and performance tracking

## Support

For issues or questions about the database integration:
1. Check the browser console for error messages
2. Verify backend API endpoints are working
3. Review the React Query documentation
4. Check network tab for failed requests
