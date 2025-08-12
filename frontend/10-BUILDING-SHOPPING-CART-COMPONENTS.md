# Section 10: Building Shopping Cart Components

## Overview

This section covers the essential shopping cart functionality for the e-books platform. We'll build two core components that handle the complete shopping cart experience: displaying cart items, managing quantities, calculating totals, and providing checkout functionality.

## Component Structure

### File Locations
```
src/components/cart/
├── ShoppingCart.tsx          # Main cart container
└── ShoppingCartItem.tsx      # Individual cart item
```

## Component 1: ShoppingCartItem

### File Location
```
src/components/cart/ShoppingCartItem.tsx
```

### Props Interface
```tsx:src/components/cart/ShoppingCartItem.tsx
import type { CartItem } from '../../types/book';

interface ShoppingCartItemProps {
    item: CartItem;
    onUpdateQuantity: (itemId: string, quantity: number) => void;
    onRemoveItem: (itemId: string) => void;
    isUpdating?: boolean;
}
```

### Complete ShoppingCartItem Component

```tsx:src/components/cart/ShoppingCartItem.tsx
import React from 'react';
import type { CartItem } from '../../types/book';

interface ShoppingCartItemProps {
    item: CartItem;
    onUpdateQuantity: (itemId: string, quantity: number) => void;
    onRemoveItem: (itemId: string) => void;
    isUpdating?: boolean;
}

const ShoppingCartItem: React.FC<ShoppingCartItemProps> = ({ 
    item, 
    onUpdateQuantity, 
    onRemoveItem,
    isUpdating = false
}) => {
    const handleQuantityChange = async (newQuantity: number) => {
        if (newQuantity < 1) return;
        await onUpdateQuantity(item.id, newQuantity);
    };

    const handleRemove = async () => {
        await onRemoveItem(item.id);
    };

    const totalPrice = item.price * item.quantity;

    return (
        <div className="card mb-3 shadow-sm">
            <div className="card-body">
                <div className="row align-items-center">
                    {/* Book Image */}
                    <div className="col-md-2 col-4">
                        <img 
                            src={item.book.coverImageUrl || '/placeholder-book.jpg'} 
                            alt={item.book.title}
                            className="img-fluid rounded"
                            style={{ maxHeight: '80px', objectFit: 'cover' }}
                        />
                    </div>
                    
                    {/* Book Details */}
                    <div className="col-md-4 col-8">
                        <h6 className="card-title mb-1">{item.book.title}</h6>
                        <p className="text-muted mb-0">by {item.book.authorName}</p>
                        <p className="text-muted mb-0">${item.price.toFixed(2)} each</p>
                    </div>
                    
                    {/* Quantity Controls */}
                    <div className="col-md-3 col-6">
                        <div className="d-flex align-items-center">
                            <button
                                className="btn btn-outline-secondary btn-sm"
                                onClick={() => handleQuantityChange(item.quantity - 1)}
                                disabled={isUpdating || item.quantity <= 1}
                            >
                                <i className="bi bi-dash"></i>
                            </button>
                            
                            <span className="mx-3 fw-bold">
                                {isUpdating ? (
                                    <div className="spinner-border spinner-border-sm" role="status">
                                        <span className="visually-hidden">Loading...</span>
                                    </div>
                                ) : (
                                    item.quantity
                                )}
                            </span>
                            
                            <button
                                className="btn btn-outline-secondary btn-sm"
                                onClick={() => handleQuantityChange(item.quantity + 1)}
                                disabled={isUpdating}
                            >
                                <i className="bi bi-plus"></i>
                            </button>
                        </div>
                    </div>
                    
                    {/* Total Price */}
                    <div className="col-md-2 col-3">
                        <h6 className="mb-0 text-primary">
                            ${totalPrice.toFixed(2)}
                        </h6>
                    </div>
                    
                    {/* Remove Button */}
                    <div className="col-md-1 col-3 text-end">
                        <button
                            className="btn btn-outline-danger btn-sm"
                            onClick={handleRemove}
                            disabled={isUpdating}
                            title="Remove from cart"
                        >
                            {isUpdating ? (
                                <div className="spinner-border spinner-border-sm" role="status">
                                    <span className="visually-hidden">Loading...</span>
                                </div>
                            ) : (
                                <i className="bi bi-trash"></i>
                            )}
                        </button>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default ShoppingCartItem;
```

## Component 2: ShoppingCart

### File Location
```
src/components/cart/ShoppingCart.tsx
```

### Props Interface
```tsx:src/components/cart/ShoppingCart.tsx
import type { CartItem } from '../../types/book';

interface ShoppingCartProps {
    cartItems: CartItem[];
    onUpdateQuantity: (itemId: string, quantity: number) => void;
    onRemoveItem: (itemId: string) => void;
    onClearCart: () => void;
    onCheckout?: () => void;
    onContinueShopping?: () => void;
}
```

### Complete ShoppingCart Component

```tsx:src/components/cart/ShoppingCart.tsx
import React, { useState } from 'react';
import ShoppingCartItem from './ShoppingCartItem';
import type { CartItem } from '../../types/book';

interface ShoppingCartProps {
    cartItems: CartItem[];
    onUpdateQuantity: (itemId: string, quantity: number) => void;
    onRemoveItem: (itemId: string) => void;
    onClearCart: () => void;
    onCheckout?: () => void;
    onContinueShopping?: () => void;
}

const ShoppingCart: React.FC<ShoppingCartProps> = ({
    cartItems,
    onUpdateQuantity,
    onRemoveItem,
    onClearCart,
    onCheckout,
    onContinueShopping
}) => {
    const [isUpdating, setIsUpdating] = useState(false);

    const handleUpdateQuantity = async (itemId: string, quantity: number) => {
        try {
            setIsUpdating(true);
            onUpdateQuantity(itemId, quantity);
        } catch (error) {
            console.error('Error updating quantity:', error);
        } finally {
            setIsUpdating(false);
        }
    };

    const handleRemoveItem = async (itemId: string) => {
        try {
            setIsUpdating(true);
            onRemoveItem(itemId);
        } catch (error) {
            console.error('Error removing item:', error);
        } finally {
            setIsUpdating(false);
        }
    };

    const handleClearCart = async () => {
        try {
            setIsUpdating(true);
            onClearCart();
        } catch (error) {
            console.error('Error clearing cart:', error);
        } finally {
            setIsUpdating(false);
        }
    };

    const getCartTotal = () => {
        return cartItems.reduce((total, item) => total + (item.price * item.quantity), 0);
    };

    const getCartItemCount = () => {
        return cartItems.reduce((count, item) => count + item.quantity, 0);
    };

    if (cartItems.length === 0) {
        return (
            <div className="container py-5">
                <div className="row justify-content-center">
                    <div className="col-md-6 text-center">
                        <div className="card shadow">
                            <div className="card-body p-5">
                                <i className="bi bi-cart-x display-1 text-muted mb-3"></i>
                                <h2>Your Cart is Empty</h2>
                                <p className="text-muted mb-4">
                                    Looks like you haven't added any books to your cart yet.
                                </p>
                                <button
                                    className="btn btn-primary btn-lg"
                                    onClick={onContinueShopping}
                                >
                                    <i className="bi bi-arrow-left me-2"></i>
                                    Continue Shopping
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        );
    }

    return (
        <div className="container py-5">
            <div className="row">
                <div className="col-lg-8">
                    <div className="card shadow">
                        <div className="card-header bg-white">
                            <h2 className="mb-0">
                                <i className="bi bi-cart me-2"></i>
                                Shopping Cart ({getCartItemCount()} items)
                            </h2>
                        </div>
                        <div className="card-body p-0">
                            {cartItems.map((item) => (
                                <ShoppingCartItem
                                    key={item.id}
                                    item={item}
                                    onUpdateQuantity={handleUpdateQuantity}
                                    onRemoveItem={handleRemoveItem}
                                    isUpdating={isUpdating}
                                />
                            ))}
                        </div>
                        <div className="card-footer bg-white">
                            <div className="d-flex justify-content-between align-items-center">
                                <button
                                    className="btn btn-outline-danger"
                                    onClick={handleClearCart}
                                    disabled={isUpdating}
                                >
                                    <i className="bi bi-trash me-2"></i>
                                    Clear Cart
                                </button>
                                <div className="text-end">
                                    <h5 className="mb-0">
                                        Total: <span className="text-primary">${getCartTotal().toFixed(2)}</span>
                                    </h5>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div className="col-lg-4">
                    <div className="card shadow">
                        <div className="card-body">
                            <h5 className="card-title mb-3">Order Summary</h5>
                            <div className="mb-3">
                                <div className="d-flex justify-content-between">
                                    <span>Subtotal ({getCartItemCount()} items):</span>
                                    <span>${getCartTotal().toFixed(2)}</span>
                                </div>
                                <div className="d-flex justify-content-between">
                                    <span>Shipping:</span>
                                    <span className="text-success">Free</span>
                                </div>
                                <hr />
                                <div className="d-flex justify-content-between">
                                    <strong>Total:</strong>
                                    <strong className="text-primary">${getCartTotal().toFixed(2)}</strong>
                                </div>
                            </div>
                            <div className="d-grid gap-2">
                                <button
                                    className="btn btn-primary btn-lg"
                                    onClick={onCheckout}
                                    disabled={isUpdating}
                                >
                                    <i className="bi bi-credit-card me-2"></i>
                                    Proceed to Checkout
                                </button>
                                <button
                                    className="btn btn-outline-secondary"
                                    onClick={onContinueShopping}
                                >
                                    <i className="bi bi-arrow-left me-2"></i>
                                    Continue Shopping
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default ShoppingCart;
```

## Recent Updates (Latest Version)

### **🔧 Critical Fixes Implemented (Latest)**

The shopping cart components have been completely refactored to resolve critical integration issues:

#### **1. Type System Alignment**
- **Before**: ShoppingCart used local `CartItem` interface that conflicted with App.tsx
- **After**: Now properly imports and uses `CartItem` from `types/book.ts`
- **Impact**: Eliminates type mismatch errors and ensures data consistency

#### **2. Props-Based Architecture**
- **Before**: ShoppingCart managed its own state and made direct API calls
- **After**: Now receives all data and handlers as props from parent App.tsx
- **Impact**: Centralized state management, no duplicate API calls

#### **3. Navigation Integration**
- **Before**: Header used regular `<a href="/cart">` causing page reloads
- **After**: Now uses React Router `<Link to="/cart">` for smooth navigation
- **Impact**: Proper SPA behavior, no more navigation errors

#### **4. Route Protection**
- **Before**: Cart route was accessible without authentication
- **After**: Cart route now protected, redirects to signin if not authenticated
- **Impact**: Prevents unauthorized access and cart state conflicts

#### **5. Component Integration**
- **Before**: ShoppingCart was isolated and couldn't access App's cart state
- **After**: Fully integrated with App.tsx cart management system
- **Impact**: Seamless cart functionality across the entire application

### **📋 Key Changes Made**

#### **ShoppingCart.tsx**
```diff
- interface CartItem { id: number; bookId: number; ... }
+ import type { CartItem } from '../../types/book';

- const [cartItems, setCartItems] = useState<CartItem[]>([]);
+ interface ShoppingCartProps {
+   cartItems: CartItem[];
+   onUpdateQuantity: (itemId: string, quantity: number) => void;
+   onRemoveItem: (itemId: string) => void;
+   onClearCart: () => void;
+ }

- const fetchCartItems = async () => { /* API calls */ }
+ // Removed - now receives data via props

- const updateCartItemQuantity = async (itemId: number, quantity: number) => {
+ const handleUpdateQuantity = async (itemId: string, quantity: number) => {
+   onUpdateQuantity(itemId, quantity);
+ }
```

#### **ShoppingCartItem.tsx**
```diff
- interface CartItem { id: number; bookId: number; ... }
+ import type { CartItem } from '../../types/book';

- const [isUpdating, setIsUpdating] = useState(false);
+ interface ShoppingCartItemProps {
+   isUpdating?: boolean; // Now received as prop
+ }

- item.bookTitle → item.book.title
- item.bookPrice → item.price
- item.totalPrice → calculated totalPrice
```

#### **Header.tsx**
```diff
- <a href="/cart">Cart</a>
+ import { Link } from 'react-router-dom';
+ <Link to="/cart">Cart</Link>

- <a href="/">Home</a>
+ <Link to="/">Home</Link>

- <a href="/books">Books</a>
+ <Link to="/books">Books</Link>
```

#### **App.tsx**
```diff
+ <Route path="/cart" element={
+   appState.isAuthenticated ? (
+     <ShoppingCartRoute
+       cartItems={appState.cart}
+       onUpdateQuantity={updateCartItem}
+       onRemoveItem={removeFromCart}
+       onClearCart={clearCart}
+     />
+   ) : (
+     <Navigate to="/signin" replace />
+   )
+ } />
```

## Integration with App.tsx

### **Cart State Management**

The shopping cart is now fully integrated with the main App state:

```tsx:src/App.tsx
interface AppState {
    user: User | null;
    isAuthenticated: boolean;
    cart: CartItem[];           // Centralized cart state
    selectedBook: Book | null;
    isLoading: boolean;
}

// Cart handlers in App component
const addToCart = (book: Book, quantity: number = 1) => {
    setAppState(prev => {
        const existingItem = prev.cart.find(item => item.book.id === book.id);
        if (existingItem) {
            // Update existing item quantity
            const updatedCart = prev.cart.map(item =>
                item.book.id === book.id
                    ? { ...item, quantity: item.quantity + quantity }
                    : item
            );
            return { ...prev, cart: updatedCart };
        } else {
            // Add new item
            const newItem: CartItem = {
                id: Date.now().toString(),
                book,
                quantity,
                price: book.price
            };
            return { ...prev, cart: [...prev.cart, newItem] };
        }
    });
};

const updateCartItem = (itemId: string, quantity: number) => {
    if (quantity <= 0) {
        removeFromCart(itemId);
        return;
    }
    setAppState(prev => ({
        ...prev,
        cart: prev.cart.map(item =>
            item.id === itemId ? { ...item, quantity } : item
        )
    }));
};

const removeFromCart = (itemId: string) => {
    setAppState(prev => ({
        ...prev,
        cart: prev.cart.filter(item => item.id !== itemId)
    }));
};

const clearCart = () => {
    setAppState(prev => ({ ...prev, cart: [] }));
};
```

### **Route Integration**

```tsx:src/App.tsx
// ShoppingCartRoute Component
const ShoppingCartRoute: React.FC<{
    cartItems: CartItem[];
    onUpdateQuantity: (itemId: string, quantity: number) => void;
    onRemoveItem: (itemId: string) => void;
    onClearCart: () => void;
}> = ({ cartItems, onUpdateQuantity, onRemoveItem, onClearCart }) => {
    const navigate = useNavigate();
    
    const handleCheckout = () => {
        navigate('/checkout');
    };
    
    const handleContinueShopping = () => {
        navigate('/');
    };
    
    return (
        <ShoppingCart
            cartItems={cartItems}
            onUpdateQuantity={onUpdateQuantity}
            onRemoveItem={onRemoveItem}
            onClearCart={onClearCart}
            onCheckout={handleCheckout}
            onContinueShopping={handleContinueShopping}
        />
    );
};

// Protected Cart Route
<Route path="/cart" element={
    appState.isAuthenticated ? (
        <ShoppingCartRoute
            cartItems={appState.cart}
            onUpdateQuantity={updateCartItem}
            onRemoveItem={removeFromCart}
            onClearCart={clearCart}
        />
    ) : (
        <Navigate to="/signin" replace />
    )
} />
```

## Key Features & Improvements

### **✅ What We Built:**

#### **🛒 ShoppingCart Component:**
- **Props-Based Design**: Receives all data and handlers from parent
- **Empty State Handling**: Beautiful empty cart display with call-to-action
- **Responsive Layout**: Bootstrap grid system for mobile and desktop
- **Order Summary**: Right sidebar with totals and checkout button
- **Clear Cart**: Option to remove all items at once

#### **📦 ShoppingCartItem Component:**
- **Book Information**: Title, author, price, and cover image
- **Quantity Controls**: Increase/decrease buttons with validation
- **Real-time Updates**: Immediate UI feedback during operations
- **Remove Functionality**: Individual item removal
- **Price Calculation**: Dynamic total price per item

#### **🔗 Integration Features:**
- **Centralized State**: All cart data managed in App.tsx
- **Type Safety**: Proper TypeScript interfaces throughout
- **Navigation**: Smooth React Router integration
- **Authentication**: Protected cart access
- **Error Handling**: Graceful error management

### **🎨 UI/UX Improvements:**

#### **Visual Design:**
- **Card-based Layout**: Clean, modern card design
- **Bootstrap Icons**: Professional iconography
- **Responsive Grid**: Mobile-first responsive design
- **Shadow Effects**: Subtle depth and visual hierarchy
- **Color Coding**: Consistent color scheme

#### **User Experience:**
- **Loading States**: Visual feedback during operations
- **Empty State**: Helpful guidance when cart is empty
- **Quantity Validation**: Prevents invalid quantities
- **Smooth Navigation**: No page reloads
- **Clear Actions**: Intuitive buttons and controls

## Troubleshooting Guide

### **🚨 Common Issues & Solutions**

#### **1. Cart Page Shows "Error Loading Cart"**
- **Cause**: Type mismatch between components or missing props
- **Solution**: Ensure all components use the same `CartItem` type from `types/book.ts`

#### **2. Navigation to Cart Causes Page Reload**
- **Cause**: Header using `<a href="/cart">` instead of React Router
- **Solution**: Use `<Link to="/cart">` for proper SPA navigation

#### **3. Cart Items Not Displaying**
- **Cause**: Props not properly passed from App.tsx to ShoppingCart
- **Solution**: Verify ShoppingCartRoute receives and passes all required props

#### **4. Authentication Required for Cart**
- **Cause**: Cart route now protected for security
- **Solution**: Sign in to access cart functionality

#### **5. Type Errors in Cart Components**
- **Cause**: Inconsistent CartItem interface definitions
- **Solution**: Import `CartItem` from `types/book.ts` in all cart components

### **🔍 Debugging Steps**

1. **Check Browser Console**: Look for JavaScript errors
2. **Verify Props**: Ensure ShoppingCart receives all required props
3. **Check Authentication**: Confirm user is signed in
4. **Verify Types**: Ensure consistent CartItem interface usage
5. **Check Navigation**: Verify React Router Links are used

### **📱 Testing Checklist**

- [ ] Add book to cart from book details page
- [ ] Navigate to cart via header link
- [ ] Verify cart items display correctly
- [ ] Test quantity increase/decrease
- [ ] Test remove individual items
- [ ] Test clear cart functionality
- [ ] Verify total calculations
- [ ] Test checkout navigation
- [ ] Test continue shopping navigation
- [ ] Verify responsive design on mobile

## Next Steps

### **🚀 Future Enhancements**

1. **Cart Persistence**: Save cart to localStorage or backend
2. **Real-time Updates**: WebSocket integration for live cart updates
3. **Advanced Features**: Wishlist, save for later, bulk operations
4. **Performance**: Virtual scrolling for large cart lists
5. **Analytics**: Track cart abandonment and conversion rates

### **🔗 Related Components**

- **BookDetails**: Add to cart functionality
- **Header**: Cart count display and navigation
- **CheckoutForm**: Proceed to checkout
- **UserProfile**: Order history and preferences

---

**Note**: This documentation reflects the latest implementation with all critical fixes applied. The shopping cart is now fully functional and integrated with the main application state. 