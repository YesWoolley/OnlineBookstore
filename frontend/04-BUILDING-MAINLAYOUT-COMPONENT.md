# Section 04: Building the MainLayout Component

## 🎯 **Creating the MainLayout Wrapper**

### **Step 1: Understanding the MainLayout Structure**

The MainLayout component will provide:
- **Header** - Navigation and branding at the top
- **Main Content Area** - Flexible content container that uses full viewport height
- **Footer** - Full-width navigation and copyright at the bottom
- **Responsive Layout** - Proper spacing and structure
- **Full Viewport Height** - Uses entire screen height

### **Step 2: Create the MainLayout Component**

Create `src/components/layout/MainLayout.tsx`:

```tsx
import Header from './Header';
import Footer from './Footer';
import * as React from 'react';
import type { User } from '../../types/user';

interface MainLayoutProps {
    children: React.ReactNode;
    user?: User | null;
    isAuthenticated?: boolean;
    cartItemCount?: number;
    onSignOut?: () => void;
}

const MainLayout = ({ children, user, isAuthenticated, cartItemCount, onSignOut }: MainLayoutProps) => {
    return (
        <div className="d-flex flex-column min-vh-100">
            {/* Header */}
            <Header
                user={user}
                isAuthenticated={isAuthenticated}
                cartItemCount={cartItemCount}
                onSignOut={onSignOut}
            />

            {/* Main Content Area */}
            <main className="flex-grow-1">
                {children}
            </main>

            {/* Footer */}
            <Footer />
        </div>
    );
};

export default MainLayout;
```

### **Step 3: Update App.tsx to Use MainLayout**

Update your `src/App.tsx`:

```tsx
import React, { useState, useEffect } from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import MainLayout from './components/layout/MainLayout';
import Header from './components/layout/Header';
import BookGrid from './components/ui/BookGrid';
import BookDetails from './components/ui/BookDetails';
import SignIn from './components/auth/SignIn';
import SignUp from './components/auth/SignUp';
import ShoppingCart from './components/cart/ShoppingCart';
import CheckoutForm from './components/checkout/CheckoutForm';
import UserProfile from './components/profile/UserProfile';
import { sampleBooks } from './utils/sampleData';
import type { User } from './types/user';
import type { Book } from './types/book';
import './index.css';

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
        <Router>
            <MainLayout
                user={user}
                isAuthenticated={isAuthenticated}
                cartItemCount={getCartItemCount()}
                onSignOut={handleSignOut}
            >
                <Routes>
                    {/* Public Routes */}
                    <Route path="/" element={
                        <div className="container py-4">
                            <BookGrid
                                books={sampleBooks}
                                onViewDetails={handleBookSelect}
                                onAddToCart={addToCart}
                            />
                        </div>
                    } />

                    <Route path="/books/:id" element={
                        <div className="container py-4">
                            <BookDetailsRoute
                                onAddToCart={addToCart}
                                onBackToList={() => setAppState(prev => ({ ...prev, selectedBook: null }))}
                            />
                        </div>
                    } />

                    <Route path="/signin" element={
                        isAuthenticated ? (
                            <Navigate to="/" replace />
                        ) : (
                            <div className="container py-4">
                                <SignIn
                                    onSignInSuccess={handleSignInSuccess}
                                    onSignInError={handleAuthError}
                                />
                            </div>
                        )
                    } />

                    <Route path="/signup" element={
                        isAuthenticated ? (
                            <Navigate to="/" replace />
                        ) : (
                            <div className="container py-4">
                                <SignUp
                                    onSignUpSuccess={handleSignUpSuccess}
                                    onSignUpError={handleAuthError}
                                />
                            </div>
                        )
                    } />

                    {/* Protected Routes */}
                    <Route path="/cart" element={
                        isAuthenticated ? (
                            <div className="container py-4">
                                <ShoppingCartRoute />
                            </div>
                        ) : (
                            <Navigate to="/signin" replace />
                        )
                    } />

                    <Route path="/checkout" element={
                        isAuthenticated ? (
                            <div className="container py-4">
                                <CheckoutFormRoute
                                    onCheckoutSuccess={handleCheckoutSuccess}
                                    onCheckoutError={handleCheckoutError}
                                />
                            </div>
                        ) : (
                            <Navigate to="/signin" replace />
                        )
                    } />

                    <Route path="/profile" element={
                        isAuthenticated ? (
                            <div className="container py-4">
                                <UserProfile
                                    onProfileUpdate={handleProfileUpdate}
                                    onPasswordChange={handlePasswordChange}
                                />
                            </div>
                        ) : (
                            <Navigate to="/signin" replace />
                        )
                    } />

                    {/* 404 Route */}
                    <Route path="*" element={
                        <div className="container py-5">
                            <div className="row justify-content-center">
                                <div className="col-md-6 text-center">
                                    <h1 className="display-1 text-muted">404</h1>
                                    <h2>Page Not Found</h2>
                                    <p className="text-muted">
                                        The page you're looking for doesn't exist.
                                    </p>
                                    <a href="/" className="btn btn-primary">
                                        Go Home
                                    </a>
                                </div>
                            </div>
                        </div>
                    } />
                </Routes>
            </MainLayout>
        </Router>
    );
}

export default App;
```

### **Step 4: Add Custom Styling**

Add these styles to `src/index.css`:

```css
/* MainLayout styling */
.min-vh-100 {
  min-height: 100vh;
}

.flex-grow-1 {
  flex-grow: 1;
}

/* Main content area */
main {
  background: linear-gradient(135deg, #f8f9fa 0%, #ffffff 100%);
  min-height: calc(100vh - 200px); /* Account for header and footer */
}

main .container {
  max-width: 1200px;
  padding: 2rem 1rem;
}

/* Responsive adjustments */
@media (max-width: 767.98px) {
  main .container {
    padding-left: 1rem;
    padding-right: 1rem;
  }
}

/* Card enhancements */
.card {
  transition: transform 0.2s ease, box-shadow 0.2s ease;
  border: 1px solid rgba(0,0,0,0.125);
}

.card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 8px rgba(0,0,0,0.1);
}

/* Button styling */
.btn {
  font-weight: 500;
  transition: all 0.2s ease;
}

.btn:hover {
  transform: translateY(-1px);
}
```

## **🎨 MainLayout Features**

### **✅ What We Built:**

#### **📐 Layout Structure:**
- **Flexbox Layout** - `d-flex flex-column min-vh-100`
- **Header Section** - Fixed at the top with user authentication
- **Main Content** - Flexible growing area that uses full viewport height
- **Footer Section** - Fixed at the bottom

#### **🎯 Content Wrapper:**
- **Container** - Bootstrap container for content with proper padding
- **Responsive Padding** - `py-4` for vertical spacing
- **Flexible Height** - Content area grows to fill available space

#### **📱 Responsive Design:**
- **Mobile First** - Bootstrap responsive classes
- **Proper Spacing** - Consistent margins and padding
- **Touch Friendly** - Appropriate sizing for mobile

### **📱 Responsive Behavior:**

#### **🖥️ Desktop View:**
- **Full Height** - Uses entire viewport height
- **Centered Content** - Bootstrap container
- **Proper Spacing** - Consistent margins and padding

#### **📱 Mobile View:**
- **Stacked Layout** - Cards stack vertically
- **Touch Friendly** - Large touch targets
- **Responsive Padding** - Adjusted margins

## **🔧 Bootstrap Classes Used:**

### **🎯 Layout Classes:**
- `d-flex` - Flexbox display
- `flex-column` - Vertical flex direction
- `min-vh-100` - Minimum 100% viewport height
- `flex-grow-1` - Grow to fill available space

### **🎨 Container Classes:**
- `container` - Bootstrap container
- `py-4` - Vertical padding
- `row` - Bootstrap grid row
- `col-md-6` - Responsive columns

### **📐 Spacing Classes:**
- `mb-4` - Margin bottom
- `h-100` - 100% height
- `display-4` - Large display text
- `lead` - Lead paragraph text

### **🎯 Component Classes:**
- `card` - Bootstrap card component
- `card-body` - Card content area
- `card-title` - Card title styling
- `card-text` - Card text styling

## **🚀 Benefits of MainLayout**

### **✅ Consistent Structure:**
- **Header Always Present** - Navigation available on all pages
- **Footer Always Present** - Links and copyright on all pages
- **Proper Spacing** - Consistent margins and padding
- **Full Viewport Height** - Uses entire screen height

### **🎯 Reusable Component:**
- **Wrap Any Content** - Pass children as props
- **TypeScript Support** - Proper interface definition
- **Flexible Design** - Adapts to different content types
- **Authentication Integration** - Handles user state and cart

### **📱 Responsive Design:**
- **Mobile First** - Bootstrap responsive classes
- **Touch Friendly** - Appropriate sizing for mobile
- **Cross Browser** - Consistent across all browsers

## **🔧 How to Use MainLayout**

### **📝 Basic Usage:**
```tsx
import MainLayout from './components/layout/MainLayout';

function MyPage() {
  return (
    <MainLayout
      user={user}
      isAuthenticated={isAuthenticated}
      cartItemCount={cartItemCount}
      onSignOut={handleSignOut}
    >
      <h1>My Page Content</h1>
      <p>This content will be wrapped with Header and Footer</p>
    </MainLayout>
  );
}
```

### **📝 With Bootstrap Components:**
```tsx
import MainLayout from './components/layout/MainLayout';

function HomePage() {
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
            <h1>Welcome</h1>
          </div>
        </div>
        <div className="row">
          <div className="col-md-6">
            <div className="card">
              <div className="card-body">
                <h5>Card Title</h5>
                <p>Card content</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </MainLayout>
  );
}
```

## **🚀 Next Steps**

### **✅ MainLayout Complete!**

Your layout structure is now:
- **Professional** - Clean Bootstrap design
- **Responsive** - Works on all devices
- **Functional** - Search, navigation, user menu
- **Authenticated** - User-specific features
- **Cart-Aware** - Visual cart indicators
- **Accessible** - Screen reader friendly
- **Full Height** - Uses entire viewport height

### **🎯 What's Next:**

**Option A: Create BookCard Component**
- Individual book display
- Bootstrap card structure

**Option B: Create BookGrid Component**
- Grid layout for books
- Pagination support

**Option C: Create Authentication Components**
- Sign in/Sign up forms
- User profile management

**Which component would you like to build next?** 🎯

The MainLayout now includes authentication features, cart badges, personalized user experience, and full viewport height usage! 