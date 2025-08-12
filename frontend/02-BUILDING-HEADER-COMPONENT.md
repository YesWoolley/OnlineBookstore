# Section 02: Building the Header Component

## 🎯 **Creating the Navigation Bar**

> **📚 Reference:** This header structure is based on the official [Bootstrap Headers Examples](https://getbootstrap.com/docs/5.3/examples/headers/) from getbootstrap.com, specifically the "Simple header" example with dropdown functionality and authentication features.

### **Step 1: Understanding the Header Structure**

The Header component will be our main navigation bar with:
- **Logo/Brand** - BookStore name and icon
- **Navigation Links** - Home, Books, Cart (with item count badge)
- **Search Bar** - Find books quickly
- **User Dropdown** - Personalized welcome message, profile, settings, logout

### **Step 2: Create the Header Component**

Create `src/components/layout/Header.tsx`:

```tsx:src/components/layout/Header.tsx
import React from 'react';
import type { User } from '../../types/user';

interface HeaderProps {
  user: User | null;
  isAuthenticated: boolean;
  cartItemCount: number;
  onSignOut: () => void;
}

const Header: React.FC<HeaderProps> = ({ 
  user, 
  isAuthenticated, 
  cartItemCount, 
  onSignOut 
}) => {
  return (
    <header className="p-3 mb-3 border-bottom">
      <div className="container">
        <div className="d-flex flex-wrap align-items-center justify-content-center justify-content-lg-start">
          
          {/* Logo/Brand */}
          <a href="/" className="d-flex align-items-center mb-2 mb-lg-0 link-body-emphasis text-decoration-none">
            <i className="bi bi-book me-2" style={{ fontSize: '2rem', color: '#0d6efd' }}></i>
            <span className="fw-bold fs-4">BookStore</span>
          </a>
          
          {/* Navigation Links */}
          <ul className="nav col-12 col-lg-auto me-lg-auto mb-2 justify-content-center mb-md-0 ms-4">
            <li><a href="/" className="nav-link px-2 link-body-emphasis">Home</a></li>
            <li><a href="/books" className="nav-link px-2 link-body-emphasis">Books</a></li>
            <li>
              <a href="/cart" className="nav-link px-2 link-body-emphasis position-relative">
                Cart
                {cartItemCount > 0 && (
                  <span className="position-absolute top-0 start-100 translate-middle badge rounded-pill bg-danger">
                    {cartItemCount}
                  </span>
                )}
              </a>
            </li>
          </ul>
          
          {/* Search Bar */}
          <form className="col-12 col-lg-auto mb-3 mb-lg-0 me-lg-3" role="search">
            <input 
              type="search" 
              className="form-control" 
              placeholder="Search books, authors..." 
              aria-label="Search"
            />
          </form>
          
          {/* User Dropdown */}
          <div className="dropdown text-end">
            <a 
              href="#" 
              className="d-block link-body-emphasis text-decoration-none dropdown-toggle" 
              data-bs-toggle="dropdown" 
              aria-expanded="false"
            >
              <i className="bi bi-person-circle" style={{ fontSize: '2rem' }}></i>
            </a>
            <ul className="dropdown-menu text-small">
              {isAuthenticated ? (
                <>
                  <li>
                    <div className="dropdown-item-text">
                      <strong>Welcome, {user?.firstName || user?.userName || 'User'}!</strong>
                    </div>
                  </li>
                  <li><hr className="dropdown-divider" /></li>
                  <li><a className="dropdown-item" href="/profile">Profile</a></li>
                  <li><a className="dropdown-item" href="/orders">My Orders</a></li>
                  <li><hr className="dropdown-divider" /></li>
                  <li><button className="dropdown-item" onClick={onSignOut}>Sign out</button></li>
                </>
              ) : (
                <>
                  <li><a className="dropdown-item" href="/signin">Sign in</a></li>
                  <li><a className="dropdown-item" href="/signup">Sign up</a></li>
                </>
              )}
            </ul>
          </div>
        </div>
      </div>
    </header>
  );
};

export default Header;
```

### **Step 3: Navigation Spacing and Layout**

The Header component includes proper spacing between the logo and navigation links:

- **Logo/Brand**: BookStore logo with book icon
- **Navigation Links**: Home, Books, Cart with `ms-4` class for left margin spacing
- **Search Bar**: Centered search functionality
- **User Dropdown**: Profile and authentication options

#### **Key Spacing Features:**
- **`ms-4`**: Adds left margin to navigation links, creating visual separation from the logo
- **`me-lg-auto`**: Pushes navigation links to the left and other elements to the right on large screens
- **`justify-content-center`**: Centers navigation links on smaller screens
- **Responsive Design**: Adapts layout for mobile and desktop views

### **Step 4: Add Custom Styling**

Add these styles to `src/index.css`:

```css:src/index.css
/* Custom header styling */
header {
  background: linear-gradient(135deg, #f8f9fa 0%, #e9ecef 100%);
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}

/* Logo styling */
header .bi-book {
  color: #0d6efd !important;
}

header .fw-bold {
  color: #212529;
}

/* Navigation links */
header .nav-link {
  font-weight: 500;
  transition: color 0.2s ease;
}

header .nav-link:hover {
  color: #0d6efd !important;
}

/* Cart badge styling */
header .badge {
  font-size: 0.75rem;
  padding: 0.25rem 0.5rem;
}

/* Search bar styling */
header .form-control {
  border-radius: 20px;
  border: 1px solid #dee2e6;
  transition: border-color 0.2s ease;
}

header .form-control:focus {
  border-color: #0d6efd;
  box-shadow: 0 0 0 0.2rem rgba(13, 110, 253, 0.25);
}

/* Dropdown styling */
header .dropdown-toggle {
  transition: color 0.2s ease;
}

header .dropdown-toggle:hover {
  color: #0d6efd !important;
}

header .dropdown-menu {
  border-radius: 8px;
  box-shadow: 0 4px 12px rgba(0,0,0,0.15);
}

header .dropdown-item:hover {
  background-color: #f8f9fa;
  color: #0d6efd;
}

header .dropdown-item-text {
  color: #6c757d;
  font-size: 0.875rem;
}

/* Responsive adjustments */
@media (max-width: 991.98px) {
  header .nav {
    margin-bottom: 1rem !important;
  }
  
  header .form-control {
    margin-bottom: 1rem;
  }
}
```

### **Step 5: Usage in App.tsx**

Here's how to use the Header component in your main App:

```tsx:src/App.tsx
import React, { useState, useEffect } from 'react';
import Header from './components/layout/Header';
import type { User } from './types/user';
import type { CartItem } from './types/book';

function App() {
  const [user, setUser] = useState<User | null>(null);
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [cart, setCart] = useState<CartItem[]>([]);

  const handleSignOut = () => {
    setUser(null);
    setIsAuthenticated(false);
    setCart([]);
    localStorage.removeItem('token');
  };

  const getCartItemCount = () => {
    return cart.reduce((total, item) => total + item.quantity, 0);
  };

  return (
    <div className="App">
      <Header 
        user={user}
        isAuthenticated={isAuthenticated}
        cartItemCount={getCartItemCount()}
        onSignOut={handleSignOut}
      />
      <div className="container mt-4">
        <h1>Welcome to BookStore</h1>
        <p>Your header is now working with authentication and cart features!</p>
      </div>
    </div>
  );
}

export default App;
```

### **Step 6: Add Bootstrap Icons**

Add Bootstrap Icons to your `index.html`:

```html:index.html
<!-- Add this in the <head> section -->
<link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.0/font/bootstrap-icons.css">
```

## **🎨 Header Features**

### **✅ What We Built:**

#### **🏷️ Brand Section:**
- **Book Icon** - Bootstrap Icons book icon
- **Brand Name** - "BookStore" with bold styling
- **Clickable** - Links to home page

#### **🔍 Search Functionality:**
- **Search Input** - Rounded search bar
- **Responsive** - Adapts to screen size
- **Focus Effects** - Blue border on focus

#### **📚 Navigation Links:**
- **Home** - Main page
- **Books** - Browse all books
- **Cart** - Shopping cart with item count badge

#### **👤 User Dropdown:**
- **Personalized Welcome** - Shows user's first name or username
- **Profile** - User account management
- **My Orders** - Order history
- **Sign in/Sign up** - Authentication (when not logged in)
- **Sign out** - Logout functionality (when authenticated)

### **🛒 Cart Badge Feature:**
- **Visual Indicator** - Red badge shows number of items
- **Conditional Display** - Only shows when cart has items
- **Real-time Updates** - Updates as items are added/removed

### **📱 Responsive Behavior:**

#### **🖥️ Desktop View:**
- **Full Navigation** - All items visible
- **Search Bar** - Integrated in header
- **User Dropdown** - Profile menu with welcome message

#### **📱 Mobile View:**
- **Stacked Layout** - Items stack vertically
- **Centered Alignment** - Mobile-friendly
- **Touch-Friendly** - Large touch targets

## **🔧 Bootstrap Classes Used:**

### **🎯 Layout Classes:**
- `p-3` - Padding
- `mb-3` - Margin bottom
- `border-bottom` - Bottom border
- `container` - Centered content

### **📐 Flexbox Classes:**
- `d-flex` - Flexbox layout
- `flex-wrap` - Wrap items
- `align-items-center` - Vertical alignment
- `justify-content-center` - Center alignment
- `justify-content-lg-start` - Start alignment on large screens

### **🎨 Styling Classes:**
- `link-body-emphasis` - Link styling
- `text-decoration-none` - No underline
- `position-relative` - For badge positioning
- `position-absolute` - Badge positioning
- `badge` - Badge styling
- `rounded-pill` - Pill-shaped badge
- `bg-danger` - Red background for cart badge

### **🎯 Interactive Classes:**
- `dropdown` - Dropdown container
- `dropdown-toggle` - Dropdown trigger
- `dropdown-menu` - Dropdown content
- `dropdown-item` - Dropdown links
- `dropdown-item-text` - Non-clickable dropdown text
- `dropdown-divider` - Separator line

## **🚀 Next Steps**

### **✅ Header Complete!**

Your navigation bar is now:
- **Professional** - Clean Bootstrap design
- **Responsive** - Works on all devices
- **Functional** - Search, navigation, user menu
- **Authenticated** - User-specific features
- **Cart-Aware** - Visual cart indicators
- **Accessible** - Screen reader friendly

### **🎯 What's Next:**

**Option A: Create Footer Component**
- Contact information
- Social media links
- Quick navigation

**Option B: Create MainLayout Component**
- Wrap pages with header and footer
- Consistent page structure

**Option C: Create BookCard Component**
- Individual book display
- Bootstrap card structure

**Which component would you like to build next?** 🎯

The Header now includes authentication features, cart badges, and personalized user experience! 

## **🔧 TypeScript Integration**

### **📋 Props Interface:**
```typescript
interface HeaderProps {
  user: User | null;
  isAuthenticated: boolean;
  cartItemCount: number;
  onSignOut: () => void;
}
```

### **🎯 Key Features:**
- **Type Safety** - Full TypeScript support
- **User Authentication** - Conditional rendering based on auth state
- **Cart Integration** - Real-time cart item count display
- **Personalization** - Welcome message with user's name
- **Event Handling** - Sign out functionality

### **🔗 Integration Points:**
- **User Types** - Uses `User` interface from `./types/user`
- **Cart Management** - Integrates with cart state from parent component
- **Authentication** - Works with auth state and token management
- **Routing** - Links to various pages in the application 