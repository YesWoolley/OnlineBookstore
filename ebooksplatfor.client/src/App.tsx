import React, { useState, useEffect } from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate, useParams, useNavigate } from 'react-router-dom';

// Layout Components
import MainLayout from './components/layout/MainLayout';

// UI Components
import BookGrid from './components/ui/BookGrid';
import BookDetails from './components/ui/BookDetails';

// Authentication Components
import SignIn from './components/auth/SignIn';
import SignUp from './components/auth/SignUp';

// Shopping Cart Components
import ShoppingCart from './components/cart/ShoppingCart';

// Checkout Components
import CheckoutForm from './components/checkout/CheckoutForm';

// Profile Components
import UserProfile from './components/profile/UserProfile';

// Page Components
import About from './pages/About';
import SearchResults from './components/search/SearchResults';

// Types
import type { User } from './types/user';
import type { Book, CartItem } from './types/book';

// Hooks
import { useBooks, useBook } from './hooks/useBooks';

// BookGridRoute Component
const BookGridRoute: React.FC<{
    onAddToCart: (book: Book, quantity?: number) => void;
}> = ({ onAddToCart }) => {
    const navigate = useNavigate();
    const { data: books = [], isLoading: loading, error } = useBooks();
    
    const handleViewDetails = (book: Book) => {
        navigate(`/books/${book.id}`);
    };
    
    return (
        <div className="container py-4">
            <BookGrid
                books={books}
                loading={loading}
                error={error?.message || undefined}
                onViewDetails={handleViewDetails}
                onAddToCart={onAddToCart}
            />
        </div>
    );
};

// BookDetailsRoute Component
const BookDetailsRoute: React.FC<{
    onAddToCart: (book: Book, quantity?: number) => void;
    onBackToList: () => void;
    currentUserName?: string;
}> = ({ onAddToCart, currentUserName }) => {
    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();
    
    // Use the hook to fetch book data
    const { data: book, isLoading: loading, error, refetch } = useBook(parseInt(id || '0'));
    
    // Loading state
    if (loading) {
        return (
            <div className="container py-5">
                <div className="d-flex justify-content-center">
                    <div className="spinner-border text-primary" role="status">
                        <span className="visually-hidden">Loading...</span>
                    </div>
                </div>
            </div>
        );
    }
    
    // Error state
    if (error) {
        return (
            <div className="container py-5">
                <div className="row justify-content-center">
                    <div className="col-md-6 text-center">
                        <h1 className="display-1 text-muted">Error</h1>
                        <h2>Failed to Load Book</h2>
                        <p className="text-muted mb-3">{error.message}</p>
                        <button 
                            className="btn btn-primary me-2"
                            onClick={() => refetch()}
                        >
                            Try Again
                        </button>
                        <button 
                            className="btn btn-outline-secondary"
                            onClick={() => navigate('/')}
                        >
                            Go Back to Books
                        </button>
                    </div>
                </div>
            </div>
        );
    }
    
    // Book not found
    if (!book) {
        return (
            <div className="container py-5">
                <div className="row justify-content-center">
                    <div className="col-md-6 text-center">
                        <h1 className="display-1 text-muted">404</h1>
                        <h2>Book Not Found</h2>
                        <p className="text-muted">
                            The book you're looking for doesn't exist.
                        </p>
                        <button 
                            className="btn btn-primary"
                            onClick={() => navigate('/')}
                        >
                            Go Back to Books
                        </button>
                    </div>
                </div>
            </div>
        );
    }
    
    const handleBackToList = () => {
        navigate('/');
    };
    
    return (
        <div className="container py-4">
            <BookDetails
                book={book}
                onAddToCart={onAddToCart}
                onBackToList={handleBackToList}
                currentUserName={currentUserName}
            />
        </div>
    );
};

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

// CheckoutFormRoute Component
const CheckoutFormRoute: React.FC<{
    onCheckoutSuccess: (orderId: string) => void;
    onCheckoutError: (error: string) => void;
}> = ({ onCheckoutSuccess, onCheckoutError }) => {
    const navigate = useNavigate();
    
    const handleBackToCart = () => {
        navigate('/cart');
    };
    
    return (
        <CheckoutForm
            onCheckoutSuccess={onCheckoutSuccess}
            onCheckoutError={onCheckoutError}
            onBackToCart={handleBackToCart}
        />
    );
};

// App State Interface
interface AppState {
    user: User | null;
    isAuthenticated: boolean;
    cart: CartItem[];
    selectedBook: Book | null;
    isLoading: boolean;
}

const App: React.FC = () => {
    // Main App State
    const [appState, setAppState] = useState<AppState>({
        user: null,
        isAuthenticated: false,
        cart: [],
        selectedBook: null,
        isLoading: true
    });

    // Authentication State
    const [authError, setAuthError] = useState<string | null>(null);
    const [authSuccess, setAuthSuccess] = useState<string | null>(null);

    // Check authentication on app load
    useEffect(() => {
        checkAuthentication();
    }, []);

    const checkAuthentication = async () => {
        const token = localStorage.getItem('token');
        if (!token) {
            setAppState(prev => ({ ...prev, isLoading: false }));
            return;
        }

        try {
            const response = await fetch('/api/auth/me', {
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json',
                },
            });

            if (response.ok) {
                const userData = await response.json();
                setAppState(prev => ({
                    ...prev,
                    user: userData,
                    isAuthenticated: true,
                    isLoading: false
                }));
            } else {
                localStorage.removeItem('token');
                setAppState(prev => ({
                    ...prev,
                    user: null,
                    isAuthenticated: false,
                    isLoading: false
                }));
            }
        } catch (error) {
            console.error('Error checking authentication:', error);
            localStorage.removeItem('token');
            setAppState(prev => ({
                ...prev,
                user: null,
                isAuthenticated: false,
                isLoading: false
            }));
        }
    };

    // Authentication Handlers
    const handleSignIn = (user: User, token: string) => {
        localStorage.setItem('token', token);
        setAppState(prev => ({
            ...prev,
            user,
            isAuthenticated: true
        }));
        setAuthSuccess('Successfully signed in!');
        setTimeout(() => setAuthSuccess(null), 3000);
    };

    const handleSignUp = (user: User, token: string) => {
        localStorage.setItem('token', token);
        setAppState(prev => ({
            ...prev,
            user,
            isAuthenticated: true
        }));
        setAuthSuccess('Account created successfully!');
        setTimeout(() => setAuthSuccess(null), 3000);
    };

    // Wrapper functions for component props
    const handleSignInSuccess = (user: User, token: string) => {
        // Now we receive both user and token from the SignIn component
        handleSignIn(user, token);
    };

    const handleSignUpSuccess = (user: User, token: string) => {
        // Now we receive both user and token from the SignUp component
        handleSignUp(user, token);
    };

    const handleSignOut = () => {
        localStorage.removeItem('token');
        setAppState(prev => ({
            ...prev,
            user: null,
            isAuthenticated: false,
            cart: []
        }));
        setAuthSuccess('Successfully signed out!');
        setTimeout(() => setAuthSuccess(null), 3000);
    };

    const handleAuthError = (error: string) => {
        setAuthError(error);
        setTimeout(() => setAuthError(null), 5000);
    };

    // Cart Handlers
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
                item.id === itemId
                    ? { ...item, quantity }
                    : item
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



    const getCartItemCount = () => {
        return appState.cart.reduce((count, item) => count + item.quantity, 0);
    };



    // Checkout Handler
    const handleCheckoutSuccess = (orderId: string) => {
        clearCart();
        setAuthSuccess(`Order #${orderId} placed successfully!`);
        setTimeout(() => setAuthSuccess(null), 5000);
    };

    const handleCheckoutError = (error: string) => {
        setAuthError(`Checkout failed: ${error}`);
        setTimeout(() => setAuthError(null), 5000);
    };

    // Loading State
    if (appState.isLoading) {
        return (
            <div className="d-flex justify-content-center align-items-center" style={{ height: '100vh' }}>
                <div className="text-center">
                    <div className="spinner-border text-primary" role="status">
                        <span className="visually-hidden">Loading...</span>
                    </div>
                    <p className="mt-3">Loading E-Books Platform...</p>
                </div>
            </div>
        );
    }

    return (
        <Router>
            <MainLayout
                user={appState.user}
                isAuthenticated={appState.isAuthenticated}
                cartItemCount={getCartItemCount()}
                onSignOut={handleSignOut}
            >
                {/* Global Notifications */}
                {authSuccess && (
                    <div className="alert alert-success alert-dismissible fade show position-fixed top-0 start-50 translate-middle-x mt-3" style={{ zIndex: 1050 }}>
                        <i className="bi bi-check-circle me-2"></i>
                        {authSuccess}
                        <button
                            type="button"
                            className="btn-close"
                            onClick={() => setAuthSuccess(null)}
                        ></button>
                    </div>
                )}

                {authError && (
                    <div className="alert alert-danger alert-dismissible fade show position-fixed top-0 start-50 translate-middle-x mt-3" style={{ zIndex: 1050 }}>
                        <i className="bi bi-exclamation-triangle me-2"></i>
                        {authError}
                        <button
                            type="button"
                            className="btn-close"
                            onClick={() => setAuthError(null)}
                        ></button>
                    </div>
                )}

                <Routes>
                    {/* Public Routes */}
                    <Route path="/" element={
                        <BookGridRoute
                            onAddToCart={addToCart}
                        />
                    } />

                    <Route path="/books" element={
                        <BookGridRoute
                            onAddToCart={addToCart}
                        />
                    } />

                    <Route path="/books/:id" element={
                        <BookDetailsRoute
                            onAddToCart={addToCart}
                            onBackToList={() => setAppState(prev => ({ ...prev, selectedBook: null }))}
                            currentUserName={appState.user?.userName}
                        />
                    } />

                    <Route path="/about" element={
                        <div className="container py-4">
                            <About />
                        </div>
                    } />

                    <Route path="/search" element={
                        <SearchResults onAddToCart={addToCart} />
                    } />

                    <Route path="/signin" element={
                        appState.isAuthenticated ? (
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
                        appState.isAuthenticated ? (
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

                    <Route path="/checkout" element={
                        appState.isAuthenticated ? (
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
                        appState.isAuthenticated ? (
                            <div className="container py-4">
                                <UserProfile user={appState.user!} />
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
};

export default App;