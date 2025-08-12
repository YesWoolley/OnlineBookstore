# Section 07: Building BookDetails Component

## Overview

The `BookDetails` component provides a comprehensive view of individual books with detailed information, ratings, and purchase options. It's designed to give users all the information they need to make a purchase decision.

## Component Structure

### File Location
```
src/components/ui/BookDetails.tsx
```

### Props Interface
```tsx
interface BookDetailsProps {
    book: Book;                           // Book data to display
    onAddToCart?: (book: Book) => void;   // Add to cart handler
    onBackToList?: () => void;            // Navigation back to book list
}
```

## Features

### 1. **Book Cover Display**
- Large, high-quality book cover image
- Responsive sizing with consistent aspect ratio
- Fallback to placeholder image if cover not available

### 2. **Book Information**
- **Title**: Large, prominent display
- **Author**: With person icon
- **Publisher**: With building icon
- **Category**: With tag icon
- **Price**: Large, highlighted display
- **Stock Status**: Color-coded badges (In Stock/Out of Stock)

### 3. **Rating System**
- **Star Rating**: Visual star display using Bootstrap Icons
- **Rating Score**: Numerical rating (e.g., 4.5/5)
- **Review Count**: Number of reviews
- **Half-Star Support**: Handles decimal ratings

### 4. **Interactive Elements**
- **Back Button**: Navigation to book list
- **Add to Cart**: Primary action button (positioned in book information section)
- **Wishlist**: Secondary action button (positioned in book information section)
- **Stock-Aware**: Buttons disabled when out of stock
- **Compact Text**: Shortened button text for better layout
- **Narrow Width**: Buttons sized to content, not full width

### 5. **Responsive Design**
- **Desktop**: Two-column layout (cover + details)
- **Mobile**: Single-column stacked layout
- **Tablet**: Adaptive grid system
- **Bottom Alignment**: Image and information sections perfectly aligned at the bottom

## Code Implementation

### Star Rating Function
```tsx
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
```

### Main Component Structure
```tsx
import * as React from 'react';
import type { Book } from '../../types/book';

interface BookDetailsProps {
    book: Book;
    onAddToCart?: (book: Book) => void;
    onBackToList?: () => void;
}

const BookDetails = ({ book, onAddToCart, onBackToList }: BookDetailsProps) => {
    // Star rating function here...

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
                    <div className="card border-0 shadow-sm">
                        <img
                            src={book.coverImageUrl || '/placeholder-book.jpg'}
                            className="card-img-top"
                            alt={book.title}
                            style={{ height: '400px', objectFit: 'cover' }}
                        />
                    </div>
                </div>

                {/* Book Information */}
                <div className="col-md-8">
                    <div className="card border-0 shadow-sm h-100">
                        <div className="card-body">
                            {/* Title, Author, Rating, Price, Actions */}
                            
                            {/* Action Buttons - Narrow width */}
                            <div className="d-flex gap-2">
                                <button className="btn btn-primary btn-lg">
                                    <i className="bi bi-cart-plus me-1"></i>
                                    Add to Cart
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
                {/* Detailed book information */}
            </div>
        </div>
    );
};

export default BookDetails;
```

## Bootstrap Classes Used

### Layout & Spacing
- `container` - Main container
- `row` - Bootstrap grid row
- `col-md-4`, `col-md-8` - Responsive columns
- `py-4`, `mb-4`, `mt-5` - Spacing utilities

### Cards & Shadows
- `card` - Card container
- `card-body` - Card content area
- `card-header` - Card header
- `border-0` - Remove borders
- `shadow-sm` - Subtle shadow

### Typography
- `display-5` - Large title
- `text-muted` - Muted text color
- `fw-bold` - Bold font weight
- `text-primary` - Primary color

### Buttons & Badges
- `btn btn-primary` - Primary button (Add to Cart)
- `btn btn-outline-secondary` - Secondary button (Wishlist)
- `btn-lg` - Large button size
- `d-flex gap-2` - Flexbox layout with gap spacing
- `badge bg-success` - Success badge
- `badge bg-danger` - Danger badge

### Icons & Utilities
- `bi bi-star-fill` - Filled star icon
- `bi bi-star-half` - Half star icon
- `bi bi-star` - Empty star icon
- `d-flex align-items-center` - Flexbox alignment
- `text-warning` - Warning color for stars

## Usage Example

### In App.tsx
```tsx
import BookDetails from './components/ui/BookDetails';
import type { Book } from './types/book';

function App() {
    const handleAddToCart = (book: Book) => {
        console.log('Adding to cart:', book.title);
        // Add to cart logic
    };

    const handleBackToList = () => {
        console.log('Going back to book list');
        // Navigation logic
    };

    return (
        <MainLayout>
            <BookDetails
                book={selectedBook}
                onAddToCart={handleAddToCart}
                onBackToList={handleBackToList}
            />
        </MainLayout>
    );
}
```

### In BookCard Component
```tsx
const handleViewDetails = (book: Book) => {
    // Navigate to book details page
    // This would typically use React Router
    navigate(`/book/${book.id}`);
};
```

## Key Features

### 1. **Comprehensive Information Display**
- All book details from the backend model
- Visual rating system with stars
- Stock status with color coding
- Publisher and category information

### 2. **User-Friendly Navigation**
- Clear back button
- Intuitive layout
- Responsive design for all devices

### 3. **Purchase Flow Integration**
- Direct add to cart functionality
- Wishlist option
- Stock-aware button states

### 4. **Visual Appeal**
- Clean, modern design
- Consistent with Bootstrap theme
- Professional book store appearance
- Optimized button sizing for better proportions

## Integration Points

### 1. **With BookCard Component**
- BookCard's "View Details" button navigates to BookDetails
- Consistent data flow between components

### 2. **With Shopping Cart**
- Add to cart functionality
- Stock quantity validation

### 3. **With Navigation**
- Back button integration
- URL routing support

### 4. **With Backend API**
- Uses Book interface matching BookDto
- Ready for API integration

## Next Steps

1. **Add to App.tsx** - Integrate BookDetails into the main app
2. **Routing Setup** - Implement React Router for navigation
3. **API Integration** - Connect to backend book details endpoint
4. **State Management** - Handle selected book state
5. **Error Handling** - Add loading and error states

The BookDetails component provides a complete, professional book detail view that enhances the user experience and drives conversions! 📚✨ 