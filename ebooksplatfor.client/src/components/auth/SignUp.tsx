import * as React from 'react';
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import type { User, UserRegistration } from '../../types/user';

interface SignUpProps {
    onSignUpSuccess?: (user: User, token: string) => void;
    onSignUpError?: (error: string) => void;
    onNavigateToTerms?: () => void;
}

const SignUp: React.FC<SignUpProps> = ({
    onSignUpSuccess,
    onSignUpError,
    onNavigateToTerms
}) => {
    const navigate = useNavigate();
    // State definitions
    const [formData, setFormData] = useState<UserRegistration>({
        userName: '',
        email: '',
        password: '',
        confirmPassword: '',
        firstName: '',
        lastName: '',
        phoneNumber: ''
    });

    const [currentStep, setCurrentStep] = useState<number>(1);
    const [errors, setErrors] = useState<Partial<UserRegistration>>({});
    const [fieldTouched, setFieldTouched] = useState<Partial<Record<keyof UserRegistration, boolean>>>({});
    const [isLoading, setIsLoading] = useState<boolean>(false);
    const [showNotification, setShowNotification] = useState<boolean>(false);
    const [notificationMessage, setNotificationMessage] = useState<string>('');
    const [notificationType, setNotificationType] = useState<'error' | 'success' | 'warning'>('error');

    // Show notification function
    const showNotificationMessage = (message: string, type: 'error' | 'success' | 'warning' = 'error') => {
        setNotificationMessage(message);
        setNotificationType(type);
        setShowNotification(true);
        
        // Auto-hide after 5 seconds
        setTimeout(() => {
            setShowNotification(false);
        }, 5000);
    };

    // Real-time validation functions
    const validateUserNameRealTime = (userName: string): string => {
        if (!userName || userName.trim() === '') {
            return 'Username is required';
        }
        if (userName.length < 3) {
            return 'Username must be at least 3 characters';
        }
        if (!/^[a-zA-Z0-9._@+-]+$/.test(userName)) {
            return 'Username contains invalid characters';
        }
        return '';
    };

    const validateEmailRealTime = (email: string): string => {
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!email || email.trim() === '') {
            return 'Email is required';
        }
        if (!emailRegex.test(email)) {
            return 'Please enter a valid email';
        }
        return '';
    };

    const validatePasswordRealTime = (password: string): string => {
        if (!password || password.trim() === '') {
            return 'Password is required';
        }
        if (password.length < 8) {
            return 'Password must be at least 8 characters';
        }
        if (!/(?=.*[a-z])/.test(password)) {
            return 'Password must contain at least one lowercase letter';
        }
        if (!/(?=.*[A-Z])/.test(password)) {
            return 'Password must contain at least one uppercase letter';
        }
        if (!/(?=.*\d)/.test(password)) {
            return 'Password must contain at least one number';
        }
        return '';
    };

    const validateConfirmPasswordRealTime = (password: string, confirmPassword: string): string => {
        if (!confirmPassword || confirmPassword.trim() === '') {
            return 'Please confirm your password';
        }
        if (password !== confirmPassword) {
            return 'Passwords do not match';
        }
        return '';
    };

    const validateFirstNameRealTime = (firstName: string): string => {
        if (!firstName || firstName.trim() === '') {
            return 'First name is required';
        }
        if (firstName.length < 2) {
            return 'First name must be at least 2 characters';
        }
        return '';
    };

    const validateLastNameRealTime = (lastName: string): string => {
        if (!lastName || lastName.trim() === '') {
            return 'Last name is required';
        }
        if (lastName.length < 2) {
            return 'Last name must be at least 2 characters';
        }
        return '';
    };

    const validatePhoneNumberRealTime = (phoneNumber: string): string => {
        if (!phoneNumber || phoneNumber.trim() === '') {
            return 'Phone number is required';
        }
        if (!/^[\+]?[1-9][\d]{0,15}$/.test(phoneNumber.replace(/[\s\-\(\)]/g, ''))) {
            return 'Please enter a valid phone number';
        }
        return '';
    };

    // Handle input changes with real-time validation
    const handleInputChange = (field: keyof UserRegistration, value: string) => {
        setFormData(prev => ({
            ...prev,
            [field]: value
        }));

        // Mark field as touched
        setFieldTouched(prev => ({
            ...prev,
            [field]: true
        }));

        // Real-time validation
        let error = '';
        switch (field) {
            case 'userName':
                error = validateUserNameRealTime(value);
                break;
            case 'email':
                error = validateEmailRealTime(value);
                break;
            case 'password':
                error = validatePasswordRealTime(value);
                break;
            case 'confirmPassword':
                error = validateConfirmPasswordRealTime(formData.password || '', value);
                break;
            case 'firstName':
                error = validateFirstNameRealTime(value);
                break;
            case 'lastName':
                error = validateLastNameRealTime(value);
                break;
            case 'phoneNumber':
                error = validatePhoneNumberRealTime(value);
                break;
        }

        // Update errors immediately
        setErrors(prev => ({
            ...prev,
            [field]: error
        }));
    };

    // Handle field blur (when user leaves input)
    const handleFieldBlur = (field: keyof UserRegistration) => {
        setFieldTouched(prev => ({
            ...prev,
            [field]: true
        }));

        // Validate field on blur
        let error = '';
        switch (field) {
            case 'userName':
                error = validateUserNameRealTime(formData.userName || '');
                break;
            case 'email':
                error = validateEmailRealTime(formData.email || '');
                break;
            case 'password':
                error = validatePasswordRealTime(formData.password || '');
                break;
            case 'confirmPassword':
                error = validateConfirmPasswordRealTime(formData.password || '', formData.confirmPassword || '');
                break;
            case 'firstName':
                error = validateFirstNameRealTime(formData.firstName || '');
                break;
            case 'lastName':
                error = validateLastNameRealTime(formData.lastName || '');
                break;
            case 'phoneNumber':
                error = validatePhoneNumberRealTime(formData.phoneNumber || '');
                break;
        }

        setErrors(prev => ({
            ...prev,
            [field]: error
        }));
    };

    // Get password requirements status
    const getPasswordRequirements = (password: string | undefined) => {
        const pwd = password || '';
        return {
            length: pwd.length >= 8,
            lowercase: /(?=.*[a-z])/.test(pwd),
            uppercase: /(?=.*[A-Z])/.test(pwd),
            number: /(?=.*\d)/.test(pwd)
        };
    };

    // Async validation functions for server-side checks
    const validateUserName = async (userName: string): Promise<string> => {
        const realTimeError = validateUserNameRealTime(userName);
        if (realTimeError) return realTimeError;

        // Check if username already exists
        try {
            const response = await fetch(`https://onlinebookstore-backend-f4ejgsdudbghhkfz.australiaeast-01.azurewebsites.net/api/auth/check-username?userName=${userName}`);
            if (response.ok) {
                const { exists } = await response.json();
                if (exists) {
                    return 'Username is already taken';
                }
            } else {
                console.error('Username check failed:', response.status, response.statusText);
                return 'Unable to verify username availability';
            }
        } catch (error) {
            console.error('Username check failed:', error);
            return 'Unable to verify username availability';
        }

        return '';
    };

    const validateEmail = async (email: string): Promise<string> => {
        const realTimeError = validateEmailRealTime(email);
        if (realTimeError) return realTimeError;

        // Check if email already exists
        try {
            const response = await fetch(`https://onlinebookstore-backend-f4ejgsdudbghhkfz.australiaeast-01.azurewebsites.net/api/auth/check-email?email=${email}`);
            if (response.ok) {
                const { exists } = await response.json();
                if (exists) {
                    return 'Email already registered';
                }
            } else {
                console.error('Email check failed:', response.status, response.statusText);
                return 'Unable to verify email availability';
            }
        } catch (error) {
            console.error('Email check failed:', error);
            return 'Unable to verify email availability';
        }

        return '';
    };

    const validatePassword = (password: string): string => {
        return validatePasswordRealTime(password);
    };

    const validateConfirmPassword = (password: string, confirmPassword: string): string => {
        return validateConfirmPasswordRealTime(password, confirmPassword);
    };

    const validateFirstName = (firstName: string): string => {
        if (!firstName || firstName.trim() === '') {
            return 'First name is required';
        }
        if (firstName.length < 2) {
            return 'First name must be at least 2 characters';
        }
        return '';
    };

    const validateLastName = (lastName: string): string => {
        if (!lastName || lastName.trim() === '') {
            return 'Last name is required';
        }
        if (lastName.length < 2) {
            return 'Last name must be at least 2 characters';
        }
        return '';
    };

    const validatePhoneNumber = (phoneNumber: string): string => {
        if (!phoneNumber || phoneNumber.trim() === '') {
            return 'Phone number is required';
        }
        if (!/^[\+]?[1-9][\d]{0,15}$/.test(phoneNumber.replace(/[\s\-\(\)]/g, ''))) {
            return 'Please enter a valid phone number';
        }
        return '';
    };

    // Form submission handler
    const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        console.log('Form submitted!'); // Debug log

        // Clear previous errors and notifications
        setErrors({});
        setShowNotification(false);

        // Validate all fields
        console.log('Starting validation...'); // Debug log
        const userNameError = await validateUserName(formData.userName);
        const emailError = await validateEmail(formData.email);
        const passwordError = validatePassword(formData.password);
        const confirmPasswordError = validateConfirmPassword(formData.password, formData.confirmPassword);
        const firstNameError = validateFirstName(formData.firstName || '');
        const lastNameError = validateLastName(formData.lastName || '');
        const phoneNumberError = validatePhoneNumber(formData.phoneNumber || '');

        console.log('Validation results:', { userNameError, emailError, passwordError, confirmPasswordError, firstNameError, lastNameError, phoneNumberError }); // Debug log

        if (userNameError || emailError || passwordError || confirmPasswordError || firstNameError || lastNameError || phoneNumberError) {
            console.log('Validation failed, setting errors'); // Debug log
            
            // Set field-specific errors
            setErrors({
                userName: userNameError || undefined,
                email: emailError || undefined,
                password: passwordError || undefined,
                confirmPassword: confirmPasswordError || undefined,
                firstName: firstNameError || undefined,
                lastName: lastNameError || undefined,
                phoneNumber: phoneNumberError || undefined
            });

            // Show notification with first error
            const firstError = userNameError || emailError || passwordError || confirmPasswordError || firstNameError || lastNameError || phoneNumberError;
            if (firstError) {
                showNotificationMessage(`Please fix the following issues: ${firstError}`, 'error');
            }

            // Scroll to first error field
            const firstErrorField = document.querySelector('.is-invalid');
            if (firstErrorField) {
                firstErrorField.scrollIntoView({ behavior: 'smooth', block: 'center' });
            }

            return;
        }

        console.log('Validation passed, proceeding with registration...'); // Debug log
        setIsLoading(true);

        try {
            const response = await fetch('https://onlinebookstore-backend-f4ejgsdudbghhkfz.australiaeast-01.azurewebsites.net/api/auth/register', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(formData)
            });

            if (response.ok) {
                const authResponse = await response.json();
                if (authResponse.success && authResponse.user) {
                    showNotificationMessage('Account created successfully! Welcome aboard!', 'success');
                    setTimeout(() => {
                        onSignUpSuccess?.(authResponse.user, authResponse.token);
                    }, 1500);
                } else {
                    const errorMsg = authResponse.message || 'Registration failed';
                    showNotificationMessage(errorMsg, 'error');
                    onSignUpError?.(errorMsg);
                }
            } else {
                const error = await response.text();
                showNotificationMessage(`Registration failed: ${error}`, 'error');
                onSignUpError?.(error);
            }
        } catch (error) {
            const errorMsg = 'Network error. Please try again.';
            showNotificationMessage(errorMsg, 'error');
            onSignUpError?.(errorMsg);
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <div className="w-100 py-5">
            {/* Notification Banner */}
            {showNotification && (
                <div className={`alert alert-${notificationType === 'error' ? 'danger' : notificationType === 'success' ? 'success' : 'warning'} alert-dismissible fade show position-fixed`} 
                     style={{ top: '20px', right: '20px', zIndex: 1050, minWidth: '300px' }}>
                    <strong>{notificationType === 'error' ? '❌ Error' : notificationType === 'success' ? '✅ Success' : '⚠️ Warning'}:</strong> {notificationMessage}
                    <button type="button" className="btn-close" onClick={() => setShowNotification(false)}></button>
                </div>
            )}

            <div className="container">
                <div className="row justify-content-center">
                    <div className="col-md-8 col-lg-6">
                        <div className="card shadow">
                            <div className="card-body p-4">
                                {/* Progress indicator */}
                                <div className="mb-4">
                                    <div className="progress" style={{ height: '4px' }}>
                                        <div
                                            className="progress-bar"
                                            style={{ width: `${(currentStep / 4) * 100}%` }}
                                        ></div>
                                    </div>
                                    <div className="d-flex justify-content-between mt-2">
                                        <span className={`badge ${currentStep >= 1 ? 'bg-primary' : 'bg-secondary'}`}>1</span>
                                        <span className={`badge ${currentStep >= 2 ? 'bg-primary' : 'bg-secondary'}`}>2</span>
                                        <span className={`badge ${currentStep >= 3 ? 'bg-primary' : 'bg-secondary'}`}>3</span>
                                        <span className={`badge ${currentStep >= 4 ? 'bg-primary' : 'bg-secondary'}`}>4</span>
                                    </div>
                                </div>

                                {/* Form */}
                                <form onSubmit={handleSubmit}>
                                    <h2 className="text-center mb-4">Create Account</h2>

                                    {/* Step 1: Basic Information */}
                                    {currentStep === 1 && (
                                        <div>
                                            <h5 className="mb-3">Step 1: Basic Information</h5>

                                            <div className="mb-3">
                                                <label htmlFor="userName" className="form-label">Username *</label>
                                                <input
                                                    type="text"
                                                    className={`form-control ${errors.userName ? 'is-invalid' : ''}`}
                                                    id="userName"
                                                    value={formData.userName}
                                                    onChange={(e) => handleInputChange('userName', e.target.value)}
                                                    onBlur={() => handleFieldBlur('userName')}
                                                    placeholder="Enter username"
                                                />
                                                {errors.userName && fieldTouched.userName && <div className="invalid-feedback">{errors.userName}</div>}
                                                {fieldTouched.userName && !errors.userName && (
                                                    <div className="valid-feedback">Looks good!</div>
                                                )}
                                            </div>

                                            <div className="mb-3">
                                                <label htmlFor="email" className="form-label">Email *</label>
                                                <input
                                                    type="email"
                                                    className={`form-control ${errors.email ? 'is-invalid' : ''}`}
                                                    id="email"
                                                    value={formData.email}
                                                    onChange={(e) => handleInputChange('email', e.target.value)}
                                                    onBlur={() => handleFieldBlur('email')}
                                                    placeholder="Enter email"
                                                />
                                                {errors.email && fieldTouched.email && <div className="invalid-feedback">{errors.email}</div>}
                                                {fieldTouched.email && !errors.email && (
                                                    <div className="valid-feedback">Looks good!</div>
                                                )}
                                            </div>

                                            <div className="d-flex justify-content-between">
                                                <button
                                                    type="button"
                                                    className="btn btn-secondary"
                                                    onClick={() => navigate('/signin')}
                                                >
                                                    Back to Sign In
                                                </button>
                                                <button
                                                    type="button"
                                                    className="btn btn-primary"
                                                    onClick={() => setCurrentStep(2)}
                                                >
                                                    Next
                                                </button>
                                            </div>
                                        </div>
                                    )}

                                    {/* Step 2: Password */}
                                    {currentStep === 2 && (
                                        <div>
                                            <h5 className="mb-3">Step 2: Password</h5>

                                            <div className="mb-3">
                                                <label htmlFor="password" className="form-label">Password *</label>
                                                <input
                                                    type="password"
                                                    className={`form-control ${errors.password ? 'is-invalid' : ''}`}
                                                    id="password"
                                                    value={formData.password}
                                                    onChange={(e) => handleInputChange('password', e.target.value)}
                                                    onBlur={() => handleFieldBlur('password')}
                                                    placeholder="Enter password"
                                                />
                                                {errors.password && fieldTouched.password && <div className="invalid-feedback">{errors.password}</div>}
                                                {fieldTouched.password && !errors.password && (
                                                    <div className="valid-feedback">Looks good!</div>
                                                )}

                                                {/* Password strength meter */}
                                                {formData.password && (
                                                    <div className="mt-2">
                                                        <div className="progress" style={{ height: '4px' }}>
                                                            <div
                                                                className={`progress-bar bg-${getPasswordRequirements(formData.password).length ? 'success' : 'warning'}`}
                                                                style={{ width: `${(getPasswordRequirements(formData.password).length ? 100 : 0)}%` }}
                                                            ></div>
                                                        </div>
                                                        <small className="text-muted">
                                                            Length: {getPasswordRequirements(formData.password).length ? '✓' : '✗'}
                                                        </small>
                                                        <br />
                                                        <small className="text-muted">
                                                            Lowercase: {getPasswordRequirements(formData.password).lowercase ? '✓' : '✗'}
                                                        </small>
                                                        <br />
                                                        <small className="text-muted">
                                                            Uppercase: {getPasswordRequirements(formData.password).uppercase ? '✓' : '✗'}
                                                        </small>
                                                        <br />
                                                        <small className="text-muted">
                                                            Number: {getPasswordRequirements(formData.password).number ? '✓' : '✗'}
                                                        </small>
                                                    </div>
                                                )}
                                            </div>

                                            <div className="mb-3">
                                                <label htmlFor="confirmPassword" className="form-label">Confirm Password *</label>
                                                <input
                                                    type="password"
                                                    className={`form-control ${errors.confirmPassword ? 'is-invalid' : ''}`}
                                                    id="confirmPassword"
                                                    value={formData.confirmPassword}
                                                    onChange={(e) => handleInputChange('confirmPassword', e.target.value)}
                                                    onBlur={() => handleFieldBlur('confirmPassword')}
                                                    placeholder="Confirm password"
                                                />
                                                {errors.confirmPassword && fieldTouched.confirmPassword && <div className="invalid-feedback">{errors.confirmPassword}</div>}
                                                {fieldTouched.confirmPassword && !errors.confirmPassword && (
                                                    <div className="valid-feedback">Looks good!</div>
                                                )}
                                            </div>

                                            <div className="d-flex justify-content-between">
                                                <button
                                                    type="button"
                                                    className="btn btn-secondary"
                                                    onClick={() => setCurrentStep(1)}
                                                >
                                                    Previous
                                                </button>
                                                <button
                                                    type="button"
                                                    className="btn btn-primary"
                                                    onClick={() => setCurrentStep(3)}
                                                >
                                                    Next
                                                </button>
                                            </div>
                                        </div>
                                    )}

                                    {/* Step 3: Personal Information */}
                                    {currentStep === 3 && (
                                        <div>
                                            <h5 className="mb-3">Step 3: Personal Information</h5>

                                            <div className="row">
                                                <div className="col-md-6 mb-3">
                                                    <label htmlFor="firstName" className="form-label">First Name</label>
                                                    <input
                                                        type="text"
                                                        className="form-control"
                                                        id="firstName"
                                                        value={formData.firstName}
                                                        onChange={(e) => handleInputChange('firstName', e.target.value)}
                                                        onBlur={() => handleFieldBlur('firstName')}
                                                        placeholder="Enter first name"
                                                    />
                                                    {errors.firstName && fieldTouched.firstName && <div className="invalid-feedback">{errors.firstName}</div>}
                                                    {fieldTouched.firstName && !errors.firstName && (
                                                        <div className="valid-feedback">Looks good!</div>
                                                    )}
                                                </div>
                                                <div className="col-md-6 mb-3">
                                                    <label htmlFor="lastName" className="form-label">Last Name</label>
                                                    <input
                                                        type="text"
                                                        className="form-control"
                                                        id="lastName"
                                                        value={formData.lastName}
                                                        onChange={(e) => handleInputChange('lastName', e.target.value)}
                                                        onBlur={() => handleFieldBlur('lastName')}
                                                        placeholder="Enter last name"
                                                    />
                                                    {errors.lastName && fieldTouched.lastName && <div className="invalid-feedback">{errors.lastName}</div>}
                                                    {fieldTouched.lastName && !errors.lastName && (
                                                        <div className="valid-feedback">Looks good!</div>
                                                    )}
                                                </div>
                                            </div>

                                            <div className="mb-3">
                                                <label htmlFor="phoneNumber" className="form-label">Phone Number</label>
                                                <input
                                                    type="tel"
                                                    className="form-control"
                                                    id="phoneNumber"
                                                    value={formData.phoneNumber}
                                                    onChange={(e) => handleInputChange('phoneNumber', e.target.value)}
                                                    onBlur={() => handleFieldBlur('phoneNumber')}
                                                    placeholder="Enter phone number"
                                                />
                                                {errors.phoneNumber && fieldTouched.phoneNumber && <div className="invalid-feedback">{errors.phoneNumber}</div>}
                                                {fieldTouched.phoneNumber && !errors.phoneNumber && (
                                                    <div className="valid-feedback">Looks good!</div>
                                                )}
                                            </div>

                                            <div className="d-flex justify-content-between">
                                                <button
                                                    type="button"
                                                    className="btn btn-secondary"
                                                    onClick={() => setCurrentStep(2)}
                                                >
                                                    Previous
                                                </button>
                                                <button
                                                    type="button"
                                                    className="btn btn-primary"
                                                    onClick={() => setCurrentStep(4)}
                                                >
                                                    Next
                                                </button>
                                            </div>
                                        </div>
                                    )}

                                    {/* Step 4: Review and Submit */}
                                    {currentStep === 4 && (
                                        <div>
                                            <h5 className="mb-3">Step 4: Review and Submit</h5>

                                            <div className="card mb-3">
                                                <div className="card-body">
                                                    <h6>Review Your Information</h6>
                                                    <p><strong>Username:</strong> {formData.userName}</p>
                                                    <p><strong>Email:</strong> {formData.email}</p>
                                                    <p><strong>First Name:</strong> {formData.firstName || 'Not provided'}</p>
                                                    <p><strong>Last Name:</strong> {formData.lastName || 'Not provided'}</p>
                                                    <p><strong>Phone Number:</strong> {formData.phoneNumber || 'Not provided'}</p>
                                                </div>
                                            </div>

                                            <div className="form-check mb-3">
                                                <input
                                                    className="form-check-input"
                                                    type="checkbox"
                                                    id="agreeTerms"
                                                    required
                                                />
                                                <label className="form-check-label" htmlFor="agreeTerms">
                                                    I agree to the <a href="#" onClick={(e) => { e.preventDefault(); onNavigateToTerms?.(); }}>Terms of Service</a> and <a href="#" onClick={(e) => { e.preventDefault(); onNavigateToTerms?.(); }}>Privacy Policy</a>
                                                </label>
                                            </div>

                                            <div className="d-flex justify-content-between">
                                                <button
                                                    type="button"
                                                    className="btn btn-secondary"
                                                    onClick={() => setCurrentStep(3)}
                                                >
                                                    Previous
                                                </button>
                                                <button
                                                    type="submit"
                                                    className="btn btn-primary"
                                                    disabled={isLoading}
                                                >
                                                    {isLoading ? (
                                                        <>
                                                            <span className="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>
                                                            Creating Account...
                                                        </>
                                                    ) : (
                                                        'Create Account'
                                                    )}
                                                </button>
                                            </div>
                                        </div>
                                    )}
                                </form>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default SignUp;