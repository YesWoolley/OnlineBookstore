import * as React from 'react';
import { useState } from 'react';
import type { User, UserCredentials } from '../../types/user';

interface SignInProps {
    onSignInSuccess?: (user: User, token: string) => void;
    onSignInError?: (error: string) => void;
    onNavigateToSignUp?: () => void;
    onNavigateToForgotPassword?: () => void;
    isAuthenticated?: boolean;
    onSignOut?: () => void;
}

const SignIn = ({
    onSignInSuccess,
    onSignInError
}: SignInProps) => {
    const [formData, setFormData] = useState<UserCredentials>({
        userName: '',
        password: '',
        rememberMe: false
    });

    const [errors, setErrors] = useState<Partial<UserCredentials>>({});
    const [isLoading, setIsLoading] = useState(false);

    // Check if user is already authenticated by looking for token in localStorage
    const existingToken = localStorage.getItem('token');
    if (existingToken) {
        return (
            <div className="d-flex align-items-center py-4 bg-body-tertiary w-100">
                <div className="container">
                    <div className="row justify-content-center">
                        <div className="col-md-6 col-lg-4">
                            <div className="text-center">
                                <i className="bi bi-check-circle text-success" style={{ fontSize: '3rem' }}></i>
                                <h2 className="h3 mb-3 fw-normal text-success">You are already logged in!</h2>
                                <p className="text-muted mb-4">
                                    You are currently signed in. If you want to sign in with a different account, 
                                    please sign out first.
                                </p>
                                <div className="d-grid gap-2">
                                    <button 
                                        className="btn btn-outline-primary" 
                                        onClick={() => {
                                            localStorage.removeItem('token');
                                            localStorage.removeItem('user');
                                            window.location.href = '/';
                                        }}
                                    >
                                        <i className="bi bi-box-arrow-right me-2"></i>
                                        Sign Out
                                    </button>
                                    <a href="/" className="btn btn-primary">
                                        <i className="bi bi-house me-2"></i>
                                        Go to Home
                                    </a>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        );
    }

    const validateUserName = (userName: string): string => {
        if (!userName) return 'Username is required';
        return '';
    };

    const validatePassword = (password: string): string => {
        if (!password) return 'Password is required';
        //if (password.length < 6) return 'Password must be at least 6 characters';
        return '';
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        
        // Clear previous errors
        setErrors({});

        // Validate form
        const userNameError = validateUserName(formData.userName);
        const passwordError = validatePassword(formData.password);

        if (userNameError || passwordError) {
            setErrors({ userName: userNameError, password: passwordError });
            return;
        }

        setIsLoading(true);

        try {
            // API call to backend
            const response = await fetch('/api/auth/login', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    userName: formData.userName,
                    password: formData.password
                })
            });

            if (response.ok) {
                const authResponse = await response.json();
                if (authResponse.success && authResponse.user && authResponse.token) {
                    // Pass both user and token to the success handler
                    onSignInSuccess?.(authResponse.user, authResponse.token);
                } else {
                    // Handle authentication failure
                    const errorMessage = authResponse.message || 'Sign in failed';
                    // Show generic error under username field for authentication failures
                    setErrors({ userName: errorMessage });
                    onSignInError?.(errorMessage);
                }
            } else {
                // Handle different HTTP status codes with specific messages
                if (response.status === 400) {
                    try {
                        const errorData = await response.json();
                        const errorMessage = errorData.message || 'Invalid credentials';
                        
                        // For "Invalid username or password" messages, we'll show it under username
                        // since the backend doesn't distinguish between username/password errors for security
                        if (errorMessage.includes('Invalid username or password')) {
                            setErrors({ userName: 'Username not found or password is incorrect. Please check your credentials and try again.' });
                        } else {
                            setErrors({ userName: errorMessage });
                        }
                        onSignInError?.(errorMessage);
                    } catch {
                        // Generic error handling
                        setErrors({ userName: 'Invalid username or password. Please check your credentials and try again.' });
                        onSignInError?.('Invalid username or password. Please check your credentials and try again.');
                    }
                } else if (response.status === 404) {
                    setErrors({ userName: 'Username not found. Please check your username or create a new account.' });
                    onSignInError?.('Username not found. Please check your username or create a new account.');
                } else if (response.status === 0) {
                    // Backend not available - development fallback
                    console.log('Backend not available, creating mock user for development');
                    const mockUser = {
                        id: Date.now().toString(),
                        userName: formData.userName,
                        email: `${formData.userName}@example.com`,
                        firstName: 'Demo',
                        lastName: 'User',
                        createdAt: new Date().toISOString(),  
                        roles: ['User']      
                    };
                    onSignInSuccess?.(mockUser, 'mock-token'); // Mock token for development
                } else {
                    const error = await response.text();
                    setErrors({ userName: `Sign in failed: ${error}` });
                    onSignInError?.(`Sign in failed: ${error}`);
                }
            }
        } catch (error) {
            // Network error handling
            if (error instanceof TypeError && error.message.includes('fetch')) {
                // Backend not running - development fallback
                console.log('Network error, creating mock user for development');
                const mockUser = {
                    id: Date.now().toString(),
                    userName: formData.userName,
                    email: `${formData.userName}@example.com`,
                    firstName: 'Demo',
                    lastName: 'User',
                    createdAt: new Date().toISOString(),
                    roles: ['User']      
                };
                onSignInSuccess?.(mockUser, 'mock-token'); // Mock token for development
            } else {
                const errorMessage = 'Network error. Please check your connection and try again.';
                setErrors({ userName: errorMessage });
                onSignInError?.(errorMessage);
            }
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <div className="d-flex align-items-center py-4 bg-body-tertiary w-100">
            <div className="container">
                <div className="row justify-content-center">
                    <div className="col-md-6 col-lg-4">
                        <div className="form-signin w-100 m-auto">
                            <form onSubmit={handleSubmit}>
                                <div className="text-center mb-4">
                                    <i className="bi bi-book" style={{ fontSize: '3rem', color: '#0d6efd' }}></i>
                                    <h1 className="h3 mb-3 fw-normal">Please sign in</h1>
                                </div>

                                <div className="form-floating mb-3">
                                    <input
                                        type="text"
                                        className={`form-control ${errors.userName ? 'is-invalid' : ''}`}
                                        id="floatingInput"
                                        placeholder="Username"
                                        value={formData.userName}
                                        onChange={(e) => {
                                            setFormData({ ...formData, userName: e.target.value });
                                            // Clear username error when user types
                                            if (errors.userName) {
                                                setErrors(prev => ({ ...prev, userName: undefined }));
                                            }
                                        }}
                                    />
                                    <label htmlFor="floatingInput">Username</label>
                                    {errors.userName && <div className="invalid-feedback">{errors.userName}</div>}
                                </div>

                                <div className="form-floating mb-3">
                                    <input
                                        type="password"
                                        className={`form-control ${errors.password ? 'is-invalid' : ''}`}
                                        id="floatingPassword"
                                        placeholder="Password"
                                        value={formData.password}
                                        onChange={(e) => {
                                            setFormData({ ...formData, password: e.target.value });
                                            // Clear password error when user types
                                            if (errors.password) {
                                                setErrors(prev => ({ ...prev, password: undefined }));
                                            }
                                        }}
                                    />
                                    <label htmlFor="floatingPassword">Password</label>
                                    {errors.password && <div className="invalid-feedback">{errors.password}</div>}
                                </div>

                                <div className="form-check text-start my-3">
                                    <input
                                        className="form-check-input"
                                        type="checkbox"
                                        id="flexCheckDefault"
                                        checked={formData.rememberMe}
                                        onChange={(e) => setFormData({ ...formData, rememberMe: e.target.checked })}
                                    />
                                    <label className="form-check-label" htmlFor="flexCheckDefault">
                                        Remember me
                                    </label>
                                </div>

                                <button
                                    className="btn btn-primary w-100 py-2"
                                    type="submit"
                                    disabled={isLoading}
                                >
                                    {isLoading ? (
                                        <>
                                            <span className="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>
                                            Signing in...
                                        </>
                                    ) : (
                                        'Sign in'
                                    )}
                                </button>

                                <div className="text-center mt-3">
                                    <a href="/signup" className="text-decoration-none">Don't have an account? Sign up</a>
                                </div>
                            </form>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default SignIn;