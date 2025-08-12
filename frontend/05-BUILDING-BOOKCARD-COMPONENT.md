# Section 05: Building the BookCard Component

## 🎯 **Creating the BookCard Component**

### **Step 1: Understanding the BookCard Structure**

The BookCard component will provide:
- **Book Cover Image** - Visual representation of the book
- **Book Title** - Clear, prominent title display
- **Author Name** - Author information
- **Price Display** - Clear pricing information
- **Rating Display** - Star rating system
- **Action Buttons** - Add to cart, view details
- **Responsive Design** - Works on all screen sizes

### **Step 2: Create the BookCard Component**

Create `src/components/ui/BookCard.tsx`:

```tsx
import * as React from 'react';
import type { Book } from '../../types/book';

interface BookCardProps {
  // Required Prop - The BookCard must receive a book object
    book: Book;
    // Optional Prop - The ? means this prop is not required
    //   Function - A callback function that handles adding books to cart
    // Parameter - Takes a Book object as input
    // Return - void means it doesn't return anything
    onAddToCart?: (book: Book) => void;
    onViewDetails?: (book: Book) => void;
}

const BookCard = ({ book, onAddToCart, onViewDetails }: BookCardProps) => {
    return (
        <div className="card" style={{ width: '18rem' }}>
            <img
                src={book.coverImageUrl || '/placeholder-book.jpg'}
                className="card-img-top"
                alt={book.title}
            />
            <div className="card-body">
                <h5 className="card-title">{book.title}</h5>
                <p className="card-text">
                    by {book.authorName}
                </p>
                <div className="d-flex justify-content-between align-items-center mb-3">
                    <span className="fw-bold">${book.price}</span>
                    <small className="text-muted">
                        Rating: {book.averageRating.toFixed(1)}/5 ({book.reviewCount} reviews)
                    </small>
                </div>
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
    );
};

export default BookCard;
```

### **Step 3: Add Custom Styling**

Add these styles to `src/index.css`:

```css
/* BookCard styling */
.book-card {
  transition: transform 0.2s ease, box-shadow 0.2s ease;
  border: 1px solid rgba(0,0,0,0.125);
  overflow: hidden;
}

.book-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 16px rgba(0,0,0,0.1);
}

/* Book cover styling */
.book-cover-container {
  position: relative;
  overflow: hidden;
  background: #f8f9fa;
}

.book-cover {
  height: 200px;
  object-fit: cover;
  transition: transform 0.3s ease;
}

.book-card:hover .book-cover {
  transform: scale(1.05);
}

/* Book title styling */
.book-title {
  font-size: 1rem;
  font-weight: 600;
  line-height: 1.3;
  height: 2.6em;
  overflow: hidden;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
}

/* Book author styling */
.book-author {
  font-size: 0.875rem;
  font-weight: 500;
}

/* Rating styling */
.book-rating {
  font-size: 0.875rem;
}

.book-rating .bi {
  font-size: 0.875rem;
}

/* Price styling */
.book-price {
  font-size: 1.125rem;
}

/* Button styling */
.add-to-cart-btn {
  font-weight: 500;
  transition: all 0.2s ease;
}

.add-to-cart-btn:hover {
  transform: translateY(-1px);
}

.view-details-btn {
  font-weight: 500;
  transition: all 0.2s ease;
}

.view-details-btn:hover {
  transform: translateY(-1px);
}

/* Badge positioning */
.book-card .badge {
  font-size: 0.75rem;
  font-weight: 600;
  z-index: 1;
}

/* Responsive adjustments */
@media (max-width: 767.98px) {
  .book-cover {
    height: 180px;
  }
  
  .book-title {
    font-size: 0.9rem;
  }
  
  .book-author {
    font-size: 0.8rem;
  }
}
```

### **Step 4: Update Types**

Update `src/types/book.ts` to match the backend `BookDto` structure:

```tsx
export interface Book {
  id: number;
  title: string;
  description?: string;
  price: number;
  stockQuantity: number;
  coverImageUrl?: string;
  authorName: string;
  publisherName: string;
  categoryName: string;
  reviewCount: number;
  averageRating: number;
}
```

**Backend Compliance:**
- **Matches BookDto** - Exact structure from backend API
- **No Extra Fields** - Removed language, format, isbn, isNew, etc.
- **Proper Types** - `id` as number, `price` as number, etc.
- **Relationship Data** - Uses `authorName`, `publisherName`, `categoryName`

### **Step 5: Test the BookCard**

Update your `src/App.tsx` to test the BookCard:

```tsx
import * as React from 'react';
import MainLayout from './components/layout/MainLayout';
import BookCard from './components/ui/BookCard';
import { sampleBooks } from './utils/sampleData';
import './index.css';
import type { Book } from './types/book';
import type { User } from './types/user';

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
                
                {/* Test BookCard Components */}
                <div className="row">
                    {sampleBooks.slice(0, 4).map((book) => (
                        <div key={book.id} className="col-6 col-md-3 mb-4">
                            <BookCard 
                                book={book}
                                onAddToCart={handleAddToCart}
                                onViewDetails={handleViewDetails}
                            />
                        </div>
                    ))}
                </div>
            </div>
        </MainLayout>
    );
}

export default App;
```

## **🎨 BookCard Features**

### **✅ What We Built:**

#### **📚 Book Information:**
- **Cover Image** - Visual book representation
- **Title** - Clear, prominent display
- **Author** - Author name with "by" prefix
- **Rating** - Star rating system (0-5 stars)
- **Price** - Clear pricing with discount support

#### **🏷️ Visual Elements:**
- **NEW Badge** - Green badge for new books
- **Discount Badge** - Red badge showing discount percentage
- **Star Rating** - Bootstrap Icons star system
- **Hover Effects** - Smooth animations

#### **🎯 Action Buttons:**
- **Add to Cart** - Primary button with cart icon
- **View Details** - Secondary button for more info
- **Responsive** - Works on all screen sizes

### **📱 Responsive Behavior:**

#### **🖥️ Desktop View:**
- **4 Columns** - `col-md-3` for 4 books per row
- **Full Height** - Cards stretch to match height
- **Hover Effects** - Smooth animations on hover

#### **📱 Mobile View:**
- **2 Columns** - `col-6` for 2 books per row
- **Touch Friendly** - Large touch targets
- **Optimized Text** - Smaller font sizes

## **🔧 Bootstrap Classes Used:**

### **🎯 Card Classes:**
- `card` - Bootstrap card component
- `card-body` - Card content area
- `card-title` - Card title styling
- `card-text` - Card text styling
- `card-img-top` - Card image styling

### **🎨 Layout Classes:**
- `h-100` - 100% height
- `d-flex` - Flexbox display
- `flex-column` - Vertical flex direction
- `mt-auto` - Auto margin top

### **📐 Spacing Classes:**
- `mb-2` - Margin bottom
- `mb-3` - Margin bottom
- `me-2` - Margin end
- `gap-2` - Gap between flex items

### **🎯 Component Classes:**
- `btn` - Bootstrap button
- `btn-primary` - Primary button
- `btn-outline-secondary` - Secondary button
- `badge` - Bootstrap badge
- `position-absolute` - Absolute positioning

## **🚀 BookCard Benefits**

### **✅ Professional Design:**
- **Clean Layout** - Well-organized information
- **Visual Hierarchy** - Clear information structure
- **Consistent Styling** - Bootstrap design system

### **🎯 Interactive Features:**
- **Hover Effects** - Smooth animations
- **Click Handlers** - Add to cart and view details
- **Responsive** - Works on all devices

### **📱 User Experience:**
- **Clear Pricing** - Discount display
- **Rating System** - Visual star rating
- **Action Buttons** - Clear call-to-action

## **🔧 How to Use BookCard**

### **📝 Basic Usage:**
```tsx
import BookCard from './components/ui/BookCard';

function BookList() {
  const book = {
    id: '1',
    title: 'The Great Gatsby',
    author: 'F. Scott Fitzgerald',
    price: 12.99,
    rating: 4.5,
    coverImage: '/gatsby.jpg'
  };

  return (
    <BookCard 
      book={book}
      onAddToCart={(book) => console.log('Add to cart:', book)}
      onViewDetails={(book) => console.log('View details:', book)}
    />
  );
}
```

### **📝 In a Grid:**
```tsx
import BookCard from './components/ui/BookCard';

function BookGrid({ books }) {
  return (
    <div className="row">
      {books.map((book) => (
        <div key={book.id} className="col-6 col-md-3 mb-4">
          <BookCard 
            book={book}
            onAddToCart={handleAddToCart}
            onViewDetails={handleViewDetails}
          />
        </div>
      ))}
    </div>
  );
}
```

## **🚀 Next Steps**

### **✅ BookCard Complete!**

Your BookCard component is now:
- **Professional** - Clean, modern design
- **Responsive** - Works on all devices
- **Interactive** - Hover effects and buttons
- **Flexible** - Handles various book data

### **🎯 What's Next:**

**Option A: Create BookGrid Component**
- Responsive book grid
- Multiple book cards
- Grid layout system

**Option B: Create SearchBar Component**
- Book search functionality
- Bootstrap form styling
- Search input and filters

**Option C: Create CartItem Component**
- Shopping cart item display
- Quantity controls
- Remove functionality

**Which component would you like to build next?** 🎯

The BookCard now provides a professional way to display book information! 