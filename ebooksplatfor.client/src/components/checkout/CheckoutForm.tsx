import React, { useState } from 'react';

interface ShippingAddress {
    firstName: string;
    lastName: string;
    email: string;
    phone: string;
    address: string;
    address2?: string;
    city: string;
    state: string;
    zipCode: string;
    country: string;
}

interface PaymentInfo {
    cardNumber: string;
    cardHolderName: string;
    expiryMonth: string;
    expiryYear: string;
    cvv: string;
    paymentMethod: 'credit' | 'debit' | 'paypal';
}

interface CheckoutFormProps {
    onCheckoutSuccess?: (orderId: string) => void;
    onCheckoutError?: (error: string) => void;
    onBackToCart?: () => void;
}

const CheckoutForm: React.FC<CheckoutFormProps> = ({
    onCheckoutSuccess,
    onCheckoutError
}) => {
    const [isLoading, setIsLoading] = useState(false);
    const [errors, setErrors] = useState<{ [key: string]: string }>({});

    // Form data
    const [shippingAddress, setShippingAddress] = useState<ShippingAddress>({
        firstName: '',
        lastName: '',
        email: '',
        phone: '',
        address: '',
        address2: '',
        city: '',
        state: '',
        zipCode: '',
        country: 'US'
    });

    const [paymentInfo, setPaymentInfo] = useState<PaymentInfo>({
        cardNumber: '',
        cardHolderName: '',
        expiryMonth: '',
        expiryYear: '',
        cvv: '',
        paymentMethod: 'credit'
    });

    // Validation functions
    const validateForm = (): boolean => {
        const newErrors: { [key: string]: string } = {};

        // Shipping validation
        if (!shippingAddress.firstName.trim()) newErrors.firstName = 'Valid first name is required';
        if (!shippingAddress.lastName.trim()) newErrors.lastName = 'Valid last name is required';
        if (!shippingAddress.email.trim()) newErrors.email = 'Email is required';
        if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(shippingAddress.email)) newErrors.email = 'Please enter a valid email address for shipping updates';
        if (!shippingAddress.address.trim()) newErrors.address = 'Please enter your shipping address';
        if (!shippingAddress.city.trim()) newErrors.city = 'City is required';
        if (!shippingAddress.state.trim()) newErrors.state = 'Please provide a valid state';
        if (!shippingAddress.zipCode.trim()) newErrors.zipCode = 'Zip code required';
        if (!shippingAddress.country.trim()) newErrors.country = 'Please select a valid country';

        // Payment validation
        if (!paymentInfo.cardHolderName.trim()) newErrors.cardHolderName = 'Name on card is required';
        if (!paymentInfo.cardNumber.trim()) newErrors.cardNumber = 'Credit card number is required';
        if (paymentInfo.cardNumber.replace(/\s/g, '').length !== 16) newErrors.cardNumber = 'Credit card number is required';
        if (!paymentInfo.expiryMonth || !paymentInfo.expiryYear) newErrors.expiryMonth = 'Expiration date required';
        if (!paymentInfo.cvv.trim()) newErrors.cvv = 'Security code required';
        if (paymentInfo.cvv.length !== 3) newErrors.cvv = 'Security code required';

        setErrors(newErrors);
        return Object.keys(newErrors).length === 0;
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!validateForm()) return;

        setIsLoading(true);
        try {
            const response = await fetch('/api/orders', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    shippingAddress,
                    paymentInfo
                }),
            });

            if (response.ok) {
                const result = await response.json();
                onCheckoutSuccess?.(result.orderId);
            } else {
                const error = await response.text();
                onCheckoutError?.(error);
            }
        } catch (error) {
            console.error('Checkout error:', error);
            onCheckoutError?.('An unexpected error occurred during checkout.');
        } finally {
            setIsLoading(false);
        }
    };

    const handleInputChange = (field: keyof ShippingAddress | keyof PaymentInfo, value: string) => {
        if (field in shippingAddress) {
            setShippingAddress(prev => ({ ...prev, [field]: value }));
        } else {
            setPaymentInfo(prev => ({ ...prev, [field]: value }));
        }

        // Clear error when user starts typing
        if (errors[field]) {
            setErrors(prev => ({ ...prev, [field]: '' }));
        }
    };

    const formatCardNumber = (value: string) => {
        return value.replace(/\s/g, '').replace(/(\d{4})/g, '$1 ').trim();
    };

    return (
        <div className="bg-body-tertiary w-100">
            {/* Theme Toggle SVG Icons */}
            <svg xmlns="http://www.w3.org/2000/svg" className="d-none">
                <symbol id="check2" viewBox="0 0 16 16">
                    <path d="M13.854 3.646a.5.5 0 0 1 0 .708l-7 7a.5.5 0 0 1-.708 0l-3.5-3.5a.5.5 0 1 1 .708-.708L6.5 10.293l6.646-6.647a.5.5 0 0 1 .708 0z" />
                </symbol>
                <symbol id="circle-half" viewBox="0 0 16 16">
                    <path d="M8 15A7 7 0 1 0 8 1v14zm0 1A8 8 0 1 1 8 0a8 8 0 0 1 0 16z" />
                </symbol>
                <symbol id="moon-stars-fill" viewBox="0 0 16 16">
                    <path d="M6 .278a.768.768 0 0 1 .08.858 7.208 7.208 0 0 0-.878 3.46c0 4.021 3.278 7.277 7.318 7.277.527 0 1.04-.055 1.533-.16a.787.787 0 0 1 .81.316.733.733 0 0 1-.031.893A8.349 8.349 0 0 1 8.344 16C3.734 16 0 12.286 0 7.71 0 4.266 2.114 1.312 5.124.06A.752.752 0 0 1 6 .278z" />
                    <path d="M10.794 3.148a.217.217 0 0 1 .412 0l.387 1.162c.173.518.579.924 1.097 1.097l1.162.387a.217.217 0 0 1 0 .412l-1.162.387a1.734 1.734 0 0 0-1.097 1.097l-.387 1.162a.217.217 0 0 1-.412 0l-.387-1.162A1.734 1.734 0 0 0 9.31 6.593l-1.162-.387a.217.217 0 0 1 0-.412l1.162-.387a1.734 1.734 0 0 0 1.097-1.097l.387-1.162zM13.863.099a.145.145 0 0 1 .274 0l.258.774c.115.346.386.617.732.732l.774.258a.145.145 0 0 1 0 .274l-.774.258a1.156 1.156 0 0 0-.732.732l-.258.774a.145.145 0 0 1-.274 0l-.258-.774a1.156 1.156 0 0 0-.732-.732l-.774-.258a.145.145 0 0 1 0-.274l.774-.258c.346-.115.617-.386.732-.732L13.863.1z" />
                </symbol>
                <symbol id="sun-fill" viewBox="0 0 16 16">
                    <path d="M8 12a4 4 0 1 0 0-8 4 4 0 0 0 0 8zM8 0a.5.5 0 0 1 .5.5v2a.5.5 0 0 1-1 0v-2A.5.5 0 0 1 8 0zm0 13a.5.5 0 0 1 .5.5v2a.5.5 0 0 1-1 0v-2A.5.5 0 0 1 8 13zm8-5a.5.5 0 0 1-.5.5h-2a.5.5 0 0 1 0-1h2a.5.5 0 0 1 .5.5zM3 8a.5.5 0 0 1-.5.5h-2a.5.5 0 0 1 0-1h2A.5.5 0 0 1 3 8zm10.657-5.657a.5.5 0 0 1 0 .707l-1.414 1.415a.5.5 0 1 1-.707-.708l1.414-1.414a.5.5 0 0 1 .707 0zm-9.193 9.193a.5.5 0 0 1 0 .707L3.05 13.657a.5.5 0 0 1-.707-.707l1.414-1.414a.5.5 0 0 1 .707 0zm9.193 2.121a.5.5 0 0 1-.707 0l-1.414-1.414a.5.5 0 0 1 .707-.707l1.414 1.414a.5.5 0 0 1 0 .707zM4.464 4.465a.5.5 0 0 1-.707 0L2.343 3.05a.5.5 0 1 1 .707-.707l1.414 1.414a.5.5 0 0 1 0 .708z" />
                </symbol>
            </svg>

            {/* Theme Toggle */}
            <div className="dropdown position-fixed bottom-0 end-0 mb-3 me-3 bd-mode-toggle">
                <button className="btn btn-bd-primary py-2 dropdown-toggle d-flex align-items-center"
                    id="bd-theme"
                    type="button"
                    aria-expanded="false"
                    data-bs-toggle="dropdown"
                    aria-label="Toggle theme (dark)">
                    <svg className="bi my-1 theme-icon-active" aria-hidden="true"><use href="#moon-stars-fill"></use></svg>
                    <span className="visually-hidden" id="bd-theme-text">Toggle theme</span>
                </button>
                <ul className="dropdown-menu dropdown-menu-end shadow" aria-labelledby="bd-theme-text">
                    <li>
                        <button type="button" className="dropdown-item d-flex align-items-center" data-bs-theme-value="light" aria-pressed="false">
                            <svg className="bi me-2 opacity-50" aria-hidden="true"><use href="#sun-fill"></use></svg>
                            Light
                            <svg className="bi ms-auto d-none" aria-hidden="true"><use href="#check2"></use></svg>
                        </button>
                    </li>
                    <li>
                        <button type="button" className="dropdown-item d-flex align-items-center active" data-bs-theme-value="dark" aria-pressed="true">
                            <svg className="bi me-2 opacity-50" aria-hidden="true"><use href="#moon-stars-fill"></use></svg>
                            Dark
                            <svg className="bi ms-auto d-none" aria-hidden="true"><use href="#check2"></use></svg>
                        </button>
                    </li>
                    <li>
                        <button type="button" className="dropdown-item d-flex align-items-center" data-bs-theme-value="auto" aria-pressed="false">
                            <svg className="bi me-2 opacity-50" aria-hidden="true"><use href="#circle-half"></use></svg>
                            Auto
                            <svg className="bi ms-auto d-none" aria-hidden="true"><use href="#check2"></use></svg>
                        </button>
                    </li>
                </ul>
            </div>

            <div className="container">
                <main>
                    <div className="py-5 text-center">     
                    </div>

                    <div className="row g-5">
                        <div className="col-md-5 col-lg-4 order-md-last">
                            <h4 className="d-flex justify-content-between align-items-center mb-3">
                                <span className="text-primary">Your cart</span>
                                <span className="badge bg-primary rounded-pill">3</span>
                            </h4>
                            <ul className="list-group mb-3">
                                <li className="list-group-item d-flex justify-content-between lh-sm">
                                    <div>
                                        <h6 className="my-0">Product name</h6>
                                        <small className="text-body-secondary">Brief description</small>
                                    </div>
                                    <span className="text-body-secondary">$12</span>
                                </li>
                                <li className="list-group-item d-flex justify-content-between lh-sm">
                                    <div>
                                        <h6 className="my-0">Second product</h6>
                                        <small className="text-body-secondary">Brief description</small>
                                    </div>
                                    <span className="text-body-secondary">$8</span>
                                </li>
                                <li className="list-group-item d-flex justify-content-between lh-sm">
                                    <div>
                                        <h6 className="my-0">Third item</h6>
                                        <small className="text-body-secondary">Brief description</small>
                                    </div>
                                    <span className="text-body-secondary">$5</span>
                                </li>
                                <li className="list-group-item d-flex justify-content-between bg-body-tertiary">
                                    <div className="text-success">
                                        <h6 className="my-0">Promo code</h6>
                                        <small>EXAMPLECODE</small>
                                    </div>
                                    <span className="text-success">?$5</span>
                                </li>
                                <li className="list-group-item d-flex justify-content-between">
                                    <span>Total (USD)</span>
                                    <strong>$20</strong>
                                </li>
                            </ul>
                            <form className="card p-2">
                                <div className="input-group">
                                    <input type="text" className="form-control" placeholder="Promo code" />
                                    <button type="submit" className="btn btn-secondary">Redeem</button>
                                </div>
                            </form>
                        </div>

                        <div className="col-md-7 col-lg-8">
                            <h4 className="mb-3">Billing address</h4>
                            <form className="needs-validation" onSubmit={handleSubmit} noValidate>
                                <div className="row g-3">
                                    <div className="col-sm-6">
                                        <label htmlFor="firstName" className="form-label">First name</label>
                                        <input
                                            type="text"
                                            className={`form-control ${errors.firstName ? 'is-invalid' : ''}`}
                                            id="firstName"
                                            placeholder=""
                                            value={shippingAddress.firstName}
                                            onChange={(e) => handleInputChange('firstName', e.target.value)}
                                            required
                                        />
                                        <div className="invalid-feedback">
                                            {errors.firstName || 'Valid first name is required.'}
                                        </div>
                                    </div>

                                    <div className="col-sm-6">
                                        <label htmlFor="lastName" className="form-label">Last name</label>
                                        <input
                                            type="text"
                                            className={`form-control ${errors.lastName ? 'is-invalid' : ''}`}
                                            id="lastName"
                                            placeholder=""
                                            value={shippingAddress.lastName}
                                            onChange={(e) => handleInputChange('lastName', e.target.value)}
                                            required
                                        />
                                        <div className="invalid-feedback">
                                            {errors.lastName || 'Valid last name is required.'}
                                        </div>
                                    </div>

                                    <div className="col-12">
                                        <label htmlFor="username" className="form-label">Username</label>
                                        <div className="input-group has-validation">
                                            <span className="input-group-text">@</span>
                                            <input
                                                type="text"
                                                className="form-control"
                                                id="username"
                                                placeholder="Username"
                                                required
                                            />
                                            <div className="invalid-feedback">
                                                Your username is required.
                                            </div>
                                        </div>
                                    </div>

                                    <div className="col-12">
                                        <label htmlFor="email" className="form-label">Email <span className="text-body-secondary">(Optional)</span></label>
                                        <input
                                            type="email"
                                            className={`form-control ${errors.email ? 'is-invalid' : ''}`}
                                            id="email"
                                            placeholder="you@example.com"
                                            value={shippingAddress.email}
                                            onChange={(e) => handleInputChange('email', e.target.value)}
                                        />
                                        <div className="invalid-feedback">
                                            {errors.email || 'Please enter a valid email address for shipping updates.'}
                                        </div>
                                    </div>

                                    <div className="col-12">
                                        <label htmlFor="address" className="form-label">Address</label>
                                        <input
                                            type="text"
                                            className={`form-control ${errors.address ? 'is-invalid' : ''}`}
                                            id="address"
                                            placeholder="1234 Main St"
                                            value={shippingAddress.address}
                                            onChange={(e) => handleInputChange('address', e.target.value)}
                                            required
                                        />
                                        <div className="invalid-feedback">
                                            {errors.address || 'Please enter your shipping address.'}
                                        </div>
                                    </div>

                                    <div className="col-12">
                                        <label htmlFor="address2" className="form-label">Address 2 <span className="text-body-secondary">(Optional)</span></label>
                                        <input
                                            type="text"
                                            className="form-control"
                                            id="address2"
                                            placeholder="Apartment or suite"
                                            value={shippingAddress.address2}
                                            onChange={(e) => handleInputChange('address2', e.target.value)}
                                        />
                                    </div>

                                    <div className="col-md-5">
                                        <label htmlFor="country" className="form-label">Country</label>
                                        <select
                                            className={`form-select ${errors.country ? 'is-invalid' : ''}`}
                                            id="country"
                                            value={shippingAddress.country}
                                            onChange={(e) => handleInputChange('country', e.target.value)}
                                            required
                                        >
                                            <option value="">Choose...</option>
                                            <option value="US">United States</option>
                                        </select>
                                        <div className="invalid-feedback">
                                            {errors.country || 'Please select a valid country.'}
                                        </div>
                                    </div>

                                    <div className="col-md-4">
                                        <label htmlFor="state" className="form-label">State</label>
                                        <select
                                            className={`form-select ${errors.state ? 'is-invalid' : ''}`}
                                            id="state"
                                            value={shippingAddress.state}
                                            onChange={(e) => handleInputChange('state', e.target.value)}
                                            required
                                        >
                                            <option value="">Choose...</option>
                                            <option value="CA">California</option>
                                        </select>
                                        <div className="invalid-feedback">
                                            {errors.state || 'Please provide a valid state.'}
                                        </div>
                                    </div>

                                    <div className="col-md-3">
                                        <label htmlFor="zip" className="form-label">Zip</label>
                                        <input
                                            type="text"
                                            className={`form-control ${errors.zipCode ? 'is-invalid' : ''}`}
                                            id="zip"
                                            placeholder=""
                                            value={shippingAddress.zipCode}
                                            onChange={(e) => handleInputChange('zipCode', e.target.value)}
                                            required
                                        />
                                        <div className="invalid-feedback">
                                            {errors.zipCode || 'Zip code required.'}
                                        </div>
                                    </div>
                                </div>

                                <hr className="my-4" />

                                <div className="form-check">
                                    <input type="checkbox" className="form-check-input" id="same-address" />
                                    <label className="form-check-label" htmlFor="same-address">Shipping address is the same as my billing address</label>
                                </div>
                                <div className="form-check">
                                    <input type="checkbox" className="form-check-input" id="save-info" />
                                    <label className="form-check-label" htmlFor="save-info">Save this information for next time</label>
                                </div>

                                <hr className="my-4" />

                                <h4 className="mb-3">Payment</h4>
                                <div className="my-3">
                                    <div className="form-check">
                                        <input
                                            id="credit"
                                            name="paymentMethod"
                                            type="radio"
                                            className="form-check-input"
                                            checked={paymentInfo.paymentMethod === 'credit'}
                                            onChange={() => handleInputChange('paymentMethod', 'credit')}
                                            required
                                        />
                                        <label className="form-check-label" htmlFor="credit">Credit card</label>
                                    </div>
                                    <div className="form-check">
                                        <input
                                            id="debit"
                                            name="paymentMethod"
                                            type="radio"
                                            className="form-check-input"
                                            checked={paymentInfo.paymentMethod === 'debit'}
                                            onChange={() => handleInputChange('paymentMethod', 'debit')}
                                            required
                                        />
                                        <label className="form-check-label" htmlFor="debit">Debit card</label>
                                    </div>
                                    <div className="form-check">
                                        <input
                                            id="paypal"
                                            name="paymentMethod"
                                            type="radio"
                                            className="form-check-input"
                                            checked={paymentInfo.paymentMethod === 'paypal'}
                                            onChange={() => handleInputChange('paymentMethod', 'paypal')}
                                            required
                                        />
                                        <label className="form-check-label" htmlFor="paypal">PayPal</label>
                                    </div>
                                </div>

                                <div className="row gy-3">
                                    <div className="col-md-6">
                                        <label htmlFor="cc-name" className="form-label">Name on card</label>
                                        <input
                                            type="text"
                                            className={`form-control ${errors.cardHolderName ? 'is-invalid' : ''}`}
                                            id="cc-name"
                                            placeholder=""
                                            value={paymentInfo.cardHolderName}
                                            onChange={(e) => handleInputChange('cardHolderName', e.target.value)}
                                            required
                                        />
                                        <small className="text-body-secondary">Full name as displayed on card</small>
                                        <div className="invalid-feedback">
                                            {errors.cardHolderName || 'Name on card is required'}
                                        </div>
                                    </div>

                                    <div className="col-md-6">
                                        <label htmlFor="cc-number" className="form-label">Credit card number</label>
                                        <input
                                            type="text"
                                            className={`form-control ${errors.cardNumber ? 'is-invalid' : ''}`}
                                            id="cc-number"
                                            placeholder=""
                                            value={paymentInfo.cardNumber}
                                            onChange={(e) => handleInputChange('cardNumber', formatCardNumber(e.target.value))}
                                            required
                                        />
                                        <div className="invalid-feedback">
                                            {errors.cardNumber || 'Credit card number is required'}
                                        </div>
                                    </div>

                                    <div className="col-md-3">
                                        <label htmlFor="cc-expiration" className="form-label">Expiration</label>
                                        <input
                                            type="text"
                                            className={`form-control ${errors.expiryMonth ? 'is-invalid' : ''}`}
                                            id="cc-expiration"
                                            placeholder=""
                                            value={`${paymentInfo.expiryMonth}/${paymentInfo.expiryYear}`}
                                            onChange={(e) => {
                                                const [month, year] = e.target.value.split('/');
                                                handleInputChange('expiryMonth', month || '');
                                                handleInputChange('expiryYear', year || '');
                                            }}
                                            required
                                        />
                                        <div className="invalid-feedback">
                                            {errors.expiryMonth || 'Expiration date required'}
                                        </div>
                                    </div>

                                    <div className="col-md-3">
                                        <label htmlFor="cc-cvv" className="form-label">CVV</label>
                                        <input
                                            type="text"
                                            className={`form-control ${errors.cvv ? 'is-invalid' : ''}`}
                                            id="cc-cvv"
                                            placeholder=""
                                            value={paymentInfo.cvv}
                                            onChange={(e) => handleInputChange('cvv', e.target.value)}
                                            required
                                        />
                                        <div className="invalid-feedback">
                                            {errors.cvv || 'Security code required'}
                                        </div>
                                    </div>
                                </div>

                                <hr className="my-4" />

                                <button
                                    className="w-100 btn btn-primary btn-lg"
                                    type="submit"
                                    disabled={isLoading}
                                >
                                    {isLoading ? (
                                        <>
                                            <span className="spinner-border spinner-border-sm me-2"></span>
                                            Processing...
                                        </>
                                    ) : (
                                        'Continue to checkout'
                                    )}
                                </button>
                            </form>
                        </div>
                    </div>
                </main>

                <footer className="my-5 pt-5 text-body-secondary text-center text-small">
                    <p className="mb-1">� 2017�2025 Company Name</p>
                    <ul className="list-inline">
                        <li className="list-inline-item"><a href="#">Privacy</a></li>
                        <li className="list-inline-item"><a href="#">Terms</a></li>
                        <li className="list-inline-item"><a href="#">Support</a></li>
                    </ul>
                </footer>
            </div>
        </div>
    );
};

export default CheckoutForm;