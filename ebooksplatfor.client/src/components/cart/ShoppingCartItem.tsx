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
                        <p className="text-muted mb-0">${item.price.toFixed(2)} each</p>
                        <small className="text-muted">by {item.book.authorName}</small>
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