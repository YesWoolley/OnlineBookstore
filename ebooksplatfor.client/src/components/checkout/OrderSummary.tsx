import React from 'react';

interface OrderSummaryProps {
    items: Array<{
        id: number;
        bookTitle: string;
        bookPrice: number;
        quantity: number;
        totalPrice: number;
    }>;
    subtotal: number;
    tax: number;
    shipping: number;
    total: number;
}

const OrderSummary: React.FC<OrderSummaryProps> = ({
    items,
    subtotal,
    tax,
    shipping,
    total
}) => {
    return (
        <div className="card shadow-sm">
            <div className="card-header">
                <h5 className="mb-0">Order Summary</h5>
            </div>
            <div className="card-body">
                {/* Items List */}
                <div className="mb-3">
                    <h6>Items ({items.length})</h6>
                    {items.map(item => (
                        <div key={item.id} className="d-flex justify-content-between align-items-center mb-2">
                            <div className="flex-grow-1">
                                <div className="fw-medium">{item.bookTitle}</div>
                                <small className="text-muted">
                                    ${item.bookPrice.toFixed(2)} × {item.quantity}
                                </small>
                            </div>
                            <div className="text-end">
                                <div className="fw-medium">${item.totalPrice.toFixed(2)}</div>
                            </div>
                        </div>
                    ))}
                </div>

                <hr />

                {/* Totals */}
                <div className="space-y-2">
                    <div className="d-flex justify-content-between">
                        <span>Subtotal:</span>
                        <span>${subtotal.toFixed(2)}</span>
                    </div>
                    <div className="d-flex justify-content-between">
                        <span>Tax (8%):</span>
                        <span>${tax.toFixed(2)}</span>
                    </div>
                    <div className="d-flex justify-content-between">
                        <span>Shipping:</span>
                        <span>
                            {shipping === 0 ? (
                                <span className="text-success">FREE</span>
                            ) : (
                                `$${shipping.toFixed(2)}`
                            )}
                        </span>
                    </div>
                </div>

                <hr />

                {/* Total */}
                <div className="d-flex justify-content-between">
                    <strong>Total:</strong>
                    <strong className="text-primary fs-5">
                        ${total.toFixed(2)}
                    </strong>
                </div>

                {/* Security Notice */}
                <div className="mt-3 p-3 bg-light rounded">
                    <div className="d-flex align-items-center">
                        <i className="bi bi-shield-check text-success me-2"></i>
                        <small className="text-muted">
                            Your payment information is secure and encrypted
                        </small>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default OrderSummary;