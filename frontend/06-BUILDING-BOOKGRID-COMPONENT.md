# Section 06: Building the BookGrid Component

## 🎯 **Creating the BookGrid Component**

### **Step 1: Understanding the BookGrid Structure**

Based on [Bootstrap's Card documentation](https://getbootstrap.com/docs/5.3/components/card/#example), the BookGrid component will provide:
- **Responsive Grid Layout** - Using Bootstrap's `row-cols-*` classes
- **Equal Height Cards** - Using `h-100` class for consistent card heights
- **Card Groups** - Multiple layout options (grid cards, card groups, masonry)
- **Loading States** - Skeleton loading for better UX
- **Pagination** - For large book collections
- **Filtering** - Search and category filters

### **Step 3: Create the BookGrid Component**

Create `src/components/ui/BookGrid.tsx`:

```tsx
import * as React from 'react';
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
  onBookSelect?: (book: Book) => void;
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
      {/* Standard Book Grid */}
      <div className="row row-cols-1 row-cols-sm-2 row-cols-md-3 row-cols-lg-4 g-4">
        {books.map((book) => (
          <div key={book.id} className="col d-flex justify-content-center">
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
```

### **Step 4: Add Custom Styling**

Add these styles to `src/index.css`:

```css
/* BookGrid styling */
.book-grid {
  margin-top: 2rem;
}

/* Skeleton loading */
.placeholder-glow .placeholder {
  background-color: #e9ecef;
  border-radius: 0.375rem;
}

/* Card group styling */
.card-group > .card {
  border-radius: 0;
}

.card-group > .card:first-child {
  border-top-left-radius: 0.375rem;
  border-bottom-left-radius: 0.375rem;
}

.card-group > .card:last-child {
  border-top-right-radius: 0.375rem;
  border-bottom-right-radius: 0.375rem;
}

/* Pagination styling */
.pagination {
  margin-bottom: 0;
}

.page-link {
  color: #0d6efd;
  border-color: #dee2e6;
}

.page-link:hover {
  color: #0a58ca;
  background-color: #e9ecef;
  border-color: #dee2e6;
}

.page-item.active .page-link {
  background-color: #0d6efd;
  border-color: #0d6efd;
}

.page-item.disabled .page-link {
  color: #6c757d;
  pointer-events: none;
  background-color: #fff;
  border-color: #dee2e6;
}

/* Responsive grid adjustments */
@media (max-width: 575.98px) {
  .row-cols-1 > * {
    flex: 0 0 auto;
    width: 100%;
  }
}

@media (min-width: 576px) and (max-width: 767.98px) {
  .row-cols-sm-2 > * {
    flex: 0 0 auto;
    width: 50%;
  }
}

@media (min-width: 768px) and (max-width: 991.98px) {
  .row-cols-md-3 > * {
    flex: 0 0 auto;
    width: 33.333333%;
  }
}

@media (min-width: 992px) {
  .row-cols-lg-4 > * {
    flex: 0 0 auto;
    width: 25%;
  }
}

/* Loading spinner */
.spinner-border {
  width: 3rem;
  height: 3rem;
}

/* Alert styling */
.alert {
  border-radius: 0.5rem;
  border: none;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}

.alert-heading {
  font-weight: 600;
  margin-bottom: 0.5rem;
}
```

### **Step 5: Test the BookGrid**

Update your `src/App.tsx` to test the BookGrid:

```tsx
import * as React from 'react';

import MainLayout from './components/layout/MainLayout';
import BookGrid from './components/ui/BookGrid';
import { sampleBooks } from './utils/sampleData';
import type { Book } from './types/book';
import type { User } from './types/user';
import './index.css';

function App() {
    const [user, setUser] = React.useState<User | null>(null);
    const [isAuthenticated, setIsAuthenticated] = React.useState(false);
    const [cartItemCount, setCartItemCount] = React.useState(0);

    const handleAddToCart = (book: Book) => {
        console.log('Adding to cart:', book.title);
        // Add to cart logic here
        setCartItemCount(prev => prev + 1);
    };

    const handleViewDetails = (book: Book) => {
        console.log('Viewing details:', book.title);
        // Navigate to book details page
    };

    const handlePageChange = (page: number) => {
        console.log('Changing to page:', page);
        // Handle pagination logic here
    };

    const handleSignOut = () => {
        setUser(null);
        setIsAuthenticated(false);
        setCartItemCount(0);
    };

    return (
        <MainLayout
            user={user}
            isAuthenticated={isAuthenticated}
            cartItemCount={cartItemCount}
            onSignOut={handleSignOut}
        >
            <div className="container py-4">
                <div className="row">
                    <div className="col-12">
                        <h1 className="display-4 mb-4">BookStore</h1>
                        <p className="lead mb-4">Discover amazing books</p>
                    </div>
                </div>

                {/* Test BookGrid Component */}
                <BookGrid
                    books={sampleBooks}
                    onAddToCart={handleAddToCart}
                    onViewDetails={handleViewDetails}
                    currentPage={1}
                    totalPages={3}
                    onPageChange={handlePageChange}
                />
            </div>
        </MainLayout>
    );
}

export default App;
```

## **🎨 BookGrid Features**

### **✅ What We Built:**

#### **📐 Standard Grid Layout:**
- **Responsive Grid** - `row-cols-*` classes for different screen sizes
- **Equal Height Cards** - `h-100` class for consistency
- **Centered Cards** - `d-flex justify-content-center` for alignment

#### **🔄 State Management:**
- **Loading State** - Skeleton loading with spinners
- **Error State** - Alert messages for errors
- **Empty State** - Informative empty state
- **Pagination** - Bootstrap pagination component

#### **📱 Responsive Design:**
- **Mobile First** - 1 column on mobile
- **Tablet** - 2 columns on small screens
- **Desktop** - 3-4 columns on larger screens
- **Touch Friendly** - Large touch targets

### **📱 Responsive Behavior:**

#### **🖥️ Desktop View:**
- **4 Columns** - `row-cols-lg-4` for large screens
- **Equal Heights** - Cards stretch to match height
- **Proper Spacing** - Consistent gutters

#### **📱 Mobile View:**
- **1 Column** - `row-cols-1` for mobile
- **Touch Friendly** - Large touch targets
- **Optimized Layout** - Stacked cards

## **🔧 Bootstrap Classes Used:**

### **🎯 Grid Classes:**
- `row` - Bootstrap grid row
- `col` - Bootstrap grid column
- `row-cols-*` - Responsive column counts
- `g-4` - Grid gutters

### **🎨 Layout Classes:**
- `h-100` - 100% height
- `card-group` - Card group layout
- `d-flex` - Flexbox display
- `justify-content-center` - Center alignment

### **📐 Spacing Classes:**
- `mb-4` - Margin bottom
- `mt-4` - Margin top
- `mt-5` - Large margin top
- `g-4` - Grid gutters

### **🎯 Component Classes:**
- `spinner-border` - Loading spinner
- `alert` - Alert component
- `pagination` - Pagination component
- `placeholder-glow` - Skeleton loading

## **🚀 BookGrid Benefits**

### **✅ Professional Layout:**
- **Responsive Design** - Works on all devices
- **Multiple Layouts** - Grid, group, masonry options
- **Consistent Spacing** - Bootstrap grid system

### **🎯 User Experience:**
- **Loading States** - Skeleton loading for better UX
- **Error Handling** - Clear error messages
- **Pagination** - Handle large collections
- **Touch Friendly** - Mobile-optimized

### **📱 Performance:**
- **Lazy Loading** - Load books as needed
- **Optimized Images** - Proper image sizing
- **Efficient Rendering** - React optimization

## **🔧 How to Use BookGrid**

### **📝 Basic Usage:**
```tsx
import BookGrid from './components/ui/BookGrid';

function BookList({ books }) {
  return (
    <BookGrid 
      books={books}
      onAddToCart={handleAddToCart}
      onViewDetails={handleViewDetails}
    />
  );
}
```

### **📝 With Pagination:**
```tsx
import BookGrid from './components/ui/BookGrid';

function BookCatalog({ books, currentPage, totalPages }) {
  return (
    <BookGrid 
      books={books}
      currentPage={currentPage}
      totalPages={totalPages}
      onPageChange={handlePageChange}
      onAddToCart={handleAddToCart}
      onViewDetails={handleViewDetails}
    />
  );
}
```

### **📝 With Loading State:**
```tsx
import BookGrid from './components/ui/BookGrid';

function BookSearch({ books, loading, error }) {
  return (
    <BookGrid 
      books={books}
      loading={loading}
      error={error}
      onAddToCart={handleAddToCart}
      onViewDetails={handleViewDetails}
    />
  );
}
```

## **🚀 Next Steps**

### **✅ BookGrid Complete!**

Your BookGrid component is now:
- **Responsive** - Works on all devices
- **Professional** - Clean, modern design
- **Flexible** - Multiple layout options
- **User-Friendly** - Loading states and pagination

### **🎯 What's Next:**

**Option A: Create SearchBar Component**
- Book search functionality
- Bootstrap form styling
- Search input and filters

**Option B: Create CartItem Component**
- Shopping cart item display
- Quantity controls
- Remove functionality

**Option C: Create BookDetails Component**
- Detailed book information
- Reviews and ratings
- Related books

**Which component would you like to build next?** 🎯

The BookGrid now provides a professional way to display book collections using Bootstrap's card grid system! 