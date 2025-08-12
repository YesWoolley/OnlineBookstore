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