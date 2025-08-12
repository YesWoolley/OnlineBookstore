# Section 12: Building User Profile Component

## Overview

This section covers the user profile management functionality, including profile information updates and password change capabilities. The component provides a comprehensive user account management interface.

## Component Structure

### File Locations
```
src/components/profile/
├── UserProfile.tsx          # Main profile management component
└── PasswordChange.tsx       # Password change form component
```

## Component 1: UserProfile

### File Location
```
src/components/profile/UserProfile.tsx
```

### Props Interface
```tsx:src/components/profile/UserProfile.tsx
interface UserProfileProps {
    onProfileUpdate?: (user: User) => void;
    onPasswordChange?: () => void;
}
```

### Complete UserProfile Component

```tsx:src/components/profile/UserProfile.tsx
import React, { useState, useEffect } from 'react';
import PasswordChange from './PasswordChange';
import type { User, UserProfileUpdate } from '../../types/user';

interface UserProfileProps {
    onProfileUpdate?: (user: User) => void;
    onPasswordChange?: () => void;
}

const UserProfile: React.FC<UserProfileProps> = ({ 
    onProfileUpdate, 
    onPasswordChange 
}) => {
    const [user, setUser] = useState<User | null>(null);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [isEditing, setIsEditing] = useState(false);
    const [showPasswordChange, setShowPasswordChange] = useState(false);
    const [isUpdating, setIsUpdating] = useState(false);
    const [updateError, setUpdateError] = useState<string | null>(null);
    const [updateSuccess, setUpdateSuccess] = useState(false);

    const [formData, setFormData] = useState<UserProfileUpdate>({
        firstName: '',
        lastName: '',
        phoneNumber: '',
        email: ''
    });

    const [errors, setErrors] = useState<Partial<UserProfileUpdate>>({});

    useEffect(() => {
        fetchUserProfile();
    }, []);

    const fetchUserProfile = async () => {
        setIsLoading(true);
        setError(null);
        try {
            const response = await fetch('/api/users/profile', {
                headers: {
                    'Authorization': `Bearer ${localStorage.getItem('token')}`,
                    'Content-Type': 'application/json',
                },
            });

            if (response.ok) {
                const userData = await response.json();
                setUser(userData);
                setFormData({
                    firstName: userData.firstName || '',
                    lastName: userData.lastName || '',
                    phoneNumber: userData.phoneNumber || '',
                    email: userData.email || ''
                });
            } else {
                setError('Failed to load profile');
            }
        } catch (error) {
            console.error('Error fetching profile:', error);
            setError('An unexpected error occurred');
        } finally {
            setIsLoading(false);
        }
    };

    const validateForm = () => {
        const newErrors: Partial<UserProfileUpdate> = {};

        if (!formData.firstName?.trim()) {
            newErrors.firstName = 'First name is required';
        }

        if (!formData.lastName?.trim()) {
            newErrors.lastName = 'Last name is required';
        }

        if (formData.phoneNumber && !/^[\+]?[1-9][\d]{0,15}$/.test(formData.phoneNumber)) {
            newErrors.phoneNumber = 'Please enter a valid phone number';
        }

        if (!formData.email.trim()) {
            newErrors.email = 'Email is required';
        } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(formData.email)) {
            newErrors.email = 'Please enter a valid email address';
        }

        setErrors(newErrors);
        return Object.keys(newErrors).length === 0;
    };

    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { name, value } = e.target;
        setFormData(prev => ({
            ...prev,
            [name]: value
        }));
        
        // Clear error for this field
        if (errors[name as keyof UserProfileUpdate]) {
            setErrors(prev => ({
                ...prev,
                [name]: undefined
            }));
        }
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        
        if (!validateForm()) {
            return;
        }

        setIsUpdating(true);
        setUpdateError(null);
        setUpdateSuccess(false);

        try {
            const response = await fetch('/api/users/profile', {
                method: 'PUT',
                headers: {
                    'Authorization': `Bearer ${localStorage.getItem('token')}`,
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(formData),
            });

            if (response.ok) {
                const updatedUser = await response.json();
                setUser(updatedUser);
                setIsEditing(false);
                setUpdateSuccess(true);
                onProfileUpdate?.(updatedUser);
                
                // Clear success message after 3 seconds
                setTimeout(() => setUpdateSuccess(false), 3000);
            } else {
                const errorData = await response.json();
                setUpdateError(errorData.message || 'Failed to update profile');
            }
        } catch (error) {
            console.error('Error updating profile:', error);
            setUpdateError('An unexpected error occurred');
        } finally {
            setIsUpdating(false);
        }
    };

    const handleCancel = () => {
        setIsEditing(false);
        setErrors({});
        setUpdateError(null);
        setUpdateSuccess(false);
        
        // Reset form data to original user data
        if (user) {
            setFormData({
                firstName: user.firstName || '',
                lastName: user.lastName || '',
                phoneNumber: user.phoneNumber || '',
                email: user.email || ''
            });
        }
    };

    const formatDate = (dateString: string) => {
        return new Date(dateString).toLocaleDateString('en-US', {
            year: 'numeric',
            month: 'long',
            day: 'numeric',
        });
    };

    if (isLoading) {
        return (
            <div className="container py-5">
                <div className="row justify-content-center">
                    <div className="col-md-8">
                        <div className="text-center">
                            <div className="spinner-border text-primary" role="status">
                                <span className="visually-hidden">Loading...</span>
                            </div>
                            <p className="mt-3">Loading your profile...</p>
                        </div>
                    </div>
                </div>
            </div>
        );
    }

    if (error || !user) {
        return (
            <div className="container py-5">
                <div className="row justify-content-center">
                    <div className="col-md-8">
                        <div className="alert alert-danger" role="alert">
                            <h4 className="alert-heading">Error Loading Profile</h4>
                            <p>{error || 'Profile not found'}</p>
                            <hr />
                            <button 
                                className="btn btn-outline-danger" 
                                onClick={fetchUserProfile}
                            >
                                Try Again
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        );
    }

    return (
        <div className="container py-5">
            <div className="row">
                <div className="col-12">
                    {/* Header */}
                    <div className="d-flex justify-content-between align-items-center mb-4">
                        <h2 className="mb-0">User Profile</h2>
                        {!isEditing && (
                            <div>
                                <button 
                                    className="btn btn-primary me-2"
                                    onClick={() => setIsEditing(true)}
                                >
                                    <i className="bi bi-pencil me-2"></i>
                                    Edit Profile
                                </button>
                                <button 
                                    className="btn btn-outline-secondary"
                                    onClick={() => setShowPasswordChange(true)}
                                >
                                    <i className="bi bi-key me-2"></i>
                                    Change Password
                                </button>
                            </div>
                        )}
                    </div>

                    {/* Success Message */}
                    {updateSuccess && (
                        <div className="alert alert-success alert-dismissible fade show" role="alert">
                            <i className="bi bi-check-circle me-2"></i>
                            Profile updated successfully!
                            <button 
                                type="button" 
                                className="btn-close" 
                                onClick={() => setUpdateSuccess(false)}
                            ></button>
                        </div>
                    )}

                    {/* Error Message */}
                    {updateError && (
                        <div className="alert alert-danger alert-dismissible fade show" role="alert">
                            <i className="bi bi-exclamation-triangle me-2"></i>
                            {updateError}
                            <button 
                                type="button" 
                                className="btn-close" 
                                onClick={() => setUpdateError(null)}
                            ></button>
                        </div>
                    )}

                    <div className="row">
                        {/* Profile Information */}
                        <div className="col-lg-8">
                            <div className="card shadow-sm">
                                <div className="card-header">
                                    <h5 className="mb-0">Profile Information</h5>
                                </div>
                                <div className="card-body">
                                    {isEditing ? (
                                        <form onSubmit={handleSubmit}>
                                            <div className="row">
                                                <div className="col-md-6 mb-3">
                                                    <label htmlFor="firstName" className="form-label">
                                                        First Name *
                                                    </label>
                                                    <input
                                                        type="text"
                                                        className={`form-control ${errors.firstName ? 'is-invalid' : ''}`}
                                                        id="firstName"
                                                        name="firstName"
                                                        value={formData.firstName}
                                                        onChange={handleInputChange}
                                                        required
                                                    />
                                                    {errors.firstName && (
                                                        <div className="invalid-feedback">
                                                            {errors.firstName}
                                                        </div>
                                                    )}
                                                </div>
                                                <div className="col-md-6 mb-3">
                                                    <label htmlFor="lastName" className="form-label">
                                                        Last Name *
                                                    </label>
                                                    <input
                                                        type="text"
                                                        className={`form-control ${errors.lastName ? 'is-invalid' : ''}`}
                                                        id="lastName"
                                                        name="lastName"
                                                        value={formData.lastName}
                                                        onChange={handleInputChange}
                                                        required
                                                    />
                                                    {errors.lastName && (
                                                        <div className="invalid-feedback">
                                                            {errors.lastName}
                                                        </div>
                                                    )}
                                                </div>
                                            </div>
                                            <div className="row">
                                                <div className="col-md-6 mb-3">
                                                    <label htmlFor="email" className="form-label">
                                                        Email Address *
                                                    </label>
                                                    <input
                                                        type="email"
                                                        className={`form-control ${errors.email ? 'is-invalid' : ''}`}
                                                        id="email"
                                                        name="email"
                                                        value={formData.email}
                                                        onChange={handleInputChange}
                                                        required
                                                    />
                                                    {errors.email && (
                                                        <div className="invalid-feedback">
                                                            {errors.email}
                                                        </div>
                                                    )}
                                                </div>
                                                <div className="col-md-6 mb-3">
                                                    <label htmlFor="phoneNumber" className="form-label">
                                                        Phone Number
                                                    </label>
                                                    <input
                                                        type="tel"
                                                        className={`form-control ${errors.phoneNumber ? 'is-invalid' : ''}`}
                                                        id="phoneNumber"
                                                        name="phoneNumber"
                                                        value={formData.phoneNumber}
                                                        onChange={handleInputChange}
                                                        placeholder="+1 (555) 123-4567"
                                                    />
                                                    {errors.phoneNumber && (
                                                        <div className="invalid-feedback">
                                                            {errors.phoneNumber}
                                                        </div>
                                                    )}
                                                </div>
                                            </div>
                                            <div className="d-flex gap-2">
                                                <button 
                                                    type="submit" 
                                                    className="btn btn-primary"
                                                    disabled={isUpdating}
                                                >
                                                    {isUpdating ? (
                                                        <>
                                                            <span className="spinner-border spinner-border-sm me-2" role="status"></span>
                                                            Saving...
                                                        </>
                                                    ) : (
                                                        'Save Changes'
                                                    )}
                                                </button>
                                                <button 
                                                    type="button" 
                                                    className="btn btn-outline-secondary"
                                                    onClick={handleCancel}
                                                    disabled={isUpdating}
                                                >
                                                    Cancel
                                                </button>
                                            </div>
                                        </form>
                                    ) : (
                                        <div className="row">
                                            <div className="col-md-6 mb-3">
                                                <label className="form-label fw-medium">First Name</label>
                                                <p className="mb-0">{user.firstName || 'Not provided'}</p>
                                            </div>
                                            <div className="col-md-6 mb-3">
                                                <label className="form-label fw-medium">Last Name</label>
                                                <p className="mb-0">{user.lastName || 'Not provided'}</p>
                                            </div>
                                            <div className="col-md-6 mb-3">
                                                <label className="form-label fw-medium">Email Address</label>
                                                <p className="mb-0">{user.email}</p>
                                            </div>
                                            <div className="col-md-6 mb-3">
                                                <label className="form-label fw-medium">Phone Number</label>
                                                <p className="mb-0">{user.phoneNumber || 'Not provided'}</p>
                                            </div>
                                        </div>
                                    )}
                                </div>
                            </div>
                        </div>

                        {/* Account Information */}
                        <div className="col-lg-4">
                            <div className="card shadow-sm mb-4">
                                <div className="card-header">
                                    <h5 className="mb-0">Account Information</h5>
                                </div>
                                <div className="card-body">
                                    <div className="mb-3">
                                        <label className="form-label fw-medium">Username</label>
                                        <p className="mb-0">{user.userName}</p>
                                    </div>
                                    <div className="mb-3">
                                        <label className="form-label fw-medium">Member Since</label>
                                        <p className="mb-0">{formatDate(user.createdAt)}</p>
                                    </div>
                                    <div className="mb-3">
                                        <label className="form-label fw-medium">Account Status</label>
                                        <p className="mb-0">
                                            <span className="badge bg-success">Active</span>
                                        </p>
                                    </div>
                                </div>
                            </div>

                            {/* Quick Actions */}
                            <div className="card shadow-sm">
                                <div className="card-header">
                                    <h5 className="mb-0">Quick Actions</h5>
                                </div>
                                <div className="card-body">
                                    <div className="d-grid gap-2">
                                        <button 
                                            className="btn btn-outline-primary"
                                            onClick={() => setShowPasswordChange(true)}
                                        >
                                            <i className="bi bi-key me-2"></i>
                                            Change Password
                                        </button>
                                        <button className="btn btn-outline-secondary">
                                            <i className="bi bi-download me-2"></i>
                                            Download My Data
                                        </button>
                                        <button className="btn btn-outline-danger">
                                            <i className="bi bi-trash me-2"></i>
                                            Delete Account
                                        </button>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            {/* Password Change Modal */}
            {showPasswordChange && (
                <PasswordChange 
                    onClose={() => setShowPasswordChange(false)}
                    onSuccess={() => {
                        setShowPasswordChange(false);
                        onPasswordChange?.();
                    }}
                />
            )}
        </div>
    );
};

export default UserProfile;
```

## Component 2: PasswordChange

### File Location
```
src/components/profile/PasswordChange.tsx
```

### Props Interface
```tsx:src/components/profile/PasswordChange.tsx
interface PasswordChangeProps {
    onClose: () => void;
    onSuccess?: () => void;
}
```

### Complete PasswordChange Component

```tsx:src/components/profile/PasswordChange.tsx
import React, { useState } from 'react';
import type { PasswordChange } from '../../types/user';

interface PasswordChangeProps {
    onClose: () => void;
    onSuccess?: () => void;
}

const PasswordChange: React.FC<PasswordChangeProps> = ({ onClose, onSuccess }) => {
    const [formData, setFormData] = useState<PasswordChange>({
        currentPassword: '',
        newPassword: '',
        confirmNewPassword: ''
    });

    const [errors, setErrors] = useState<Partial<PasswordChange>>({});
    const [isLoading, setIsLoading] = useState(false);
    const [success, setSuccess] = useState(false);

    const validateForm = () => {
        const newErrors: Partial<PasswordChange> = {};

        if (!formData.currentPassword.trim()) {
            newErrors.currentPassword = 'Current password is required';
        }

        if (!formData.newPassword.trim()) {
            newErrors.newPassword = 'New password is required';
        } else if (formData.newPassword.length < 8) {
            newErrors.newPassword = 'Password must be at least 8 characters long';
        } else if (!/(?=.*[a-z])(?=.*[A-Z])(?=.*\d)/.test(formData.newPassword)) {
            newErrors.newPassword = 'Password must contain at least one uppercase letter, one lowercase letter, and one number';
        }

        if (!formData.confirmNewPassword
            newErrors.confirmNewPasswordm your new password';
        } else if (formData.newPassword !== formData.confirmNewPassword) {
            newErrors.confirmNewPassword = 'Passwords do not match';
        }

        setErrors(newErrors);
        return Object.keys(newErrors).length === 0;
    };

    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { name, value } = e.target;
        setFormData(prev => ({
            ...prev,
            [name]: value
        }));
        
        // Clear error for this field
        if (errors[name as keyof PasswordChange]) {
            setErrors(prev => ({
                ...prev,
                [name]: undefined
            }));
        }
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        
        if (!validateForm()) {
            return;
        }

        setIsLoading(true);
        setErrors({});

        try {
            const response = await fetch('/api/users/change-password', {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${localStorage.getItem('token')}`,
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    currentPassword: formData.currentPassword,
                    newPassword: formData.newPassword
                }),
            });

            if (response.ok) {
                setSuccess(true);
                setTimeout(() => {
                    onSuccess?.();
                }, 2000);
            } else {
                const errorData = await response.json();
                setErrors({
                    currentPassword: errorData.message || 'Failed to change password'
                });
            }
        } catch (error) {
            console.error('Error changing password:', error);
            setErrors({
                currentPassword: 'An unexpected error occurred'
            });
        } finally {
            setIsLoading(false);
        }
    };

    const handleClose = () => {
        if (!isLoading) {
            onClose();
        }
    };

    return (
        <div className="modal fade show d-block" style={{ backgroundColor: 'rgba(0,0,0,0.5)' }}>
            <div className="modal-dialog modal-dialog-centered">
                <div className="modal-content">
                    <div className="modal-header">
                        <h5 className="modal-title">Change Password</h5>
                        <button 
                            type="button" 
                            className="btn-close" 
                            onClick={handleClose}
                            disabled={isLoading}
                        ></button>
                    </div>
                    
                    {success ? (
                        <div className="modal-body text-center">
                            <div className="mb-3">
                                <i className="bi bi-check-circle text-success" style={{ fontSize: '3rem' }}></i>
                            </div>
                            <h5>Password Changed Successfully!</h5>
                            <p className="text-muted">Your password has been updated. You'll be redirected shortly.</p>
                        </div>
                    ) : (
                        <form onSubmit={handleSubmit}>
                            <div className="modal-body">
                                <div className="mb-3">
                                    <label htmlFor="currentPassword" className="form-label">
                                        Current Password *
                                    </label>
                                    <input
                                        type="password"
                                        className={`form-control ${errors.currentPassword ? 'is-invalid' : ''}`}
                                        id="currentPassword"
                                        name="currentPassword"
                                        value={formData.currentPassword}
                                        onChange={handleInputChange}
                                        required
                                    />
                                    {errors.currentPassword && (
                                        <div className="invalid-feedback">
                                            {errors.currentPassword}
                                        </div>
                                    )}
                                </div>
                                
                                <div className="mb-3">
                                    <label htmlFor="newPassword" className="form-label">
                                        New Password *
                                    </label>
                                    <input
                                        type="password"
                                        className={`form-control ${errors.newPassword ? 'is-invalid' : ''}`}
                                        id="newPassword"
                                        name="newPassword"
                                        value={formData.newPassword}
                                        onChange={handleInputChange}
                                        required
                                    />
                                    {errors.newPassword && (
                                        <div className="invalid-feedback">
                                            {errors.newPassword}
                                        </div>
                                    )}
                                    <div className="form-text">
                                        Password must be at least 8 characters long and contain uppercase, lowercase, and number.
                                    </div>
                                </div>
                                
                                <div className="mb-3">
                                    <label htmlFor="confirmNewPassword" className="form-label">
                                        Confirm New Password *
                                    </label>
                                    <input
                                        type="password"
                                        className={`form-control ${errors.confirmNewPassword ? 'is-invalid' : ''}`}
                                        id="confirmNewPassword"
                                        name="confirmNewPassword"
                                        value={formData.confirmNewPassword}
                                        onChange={handleInputChange}
                                        required
                                    />
                                    {errors.confirmNewPassword && (
                                        <div className="invalid-feedback">
                                            {errors.confirmNewPassword}
                                        </div>
                                    )}
                                </div>
                            </div>
                            
                            <div className="modal-footer">
                                <button 
                                    type="button" 
                                    className="btn btn-secondary" 
                                    onClick={handleClose}
                                    disabled={isLoading}
                                >
                                    Cancel
                                </button>
                                <button 
                                    type="submit" 
                                    className="btn btn-primary"
                                    disabled={isLoading}
                                >
                                    {isLoading ? (
                                        <>
                                            <span className="spinner-border spinner-border-sm me-2" role="status"></span>
                                            Changing Password...
                                        </>
                                    ) : (
                                        'Change Password'
                                    )}
                                </button>
                            </div>
                        </form>
                    )}
                </div>
            </div>
        </div>
    );
};

export default PasswordChange;
```

## Usage Example

### In App.tsx
```tsx:src/App.tsx
import UserProfile from './components/profile/UserProfile';

function App() {
    const handleProfileUpdate = (updatedUser: User) => {
        console.log('Profile updated:', updatedUser);
        // Update global user state if needed
    };

    const handlePasswordChange = () => {
        console.log('Password changed successfully');
        // Show success notification
    };

    return (
        <MainLayout>
            <UserProfile 
                onProfileUpdate={handleProfileUpdate}
                onPasswordChange={handlePasswordChange}
            />
        </MainLayout>
    );
}
```

## Key Features

### UserProfile
- ✅ **Profile Display** - View current profile information
- ✅ **Profile Editing** - Edit personal information
- ✅ **Form Validation** - Client-side validation for all fields
- ✅ **Password Change** - Modal for password updates
- ✅ **Account Information** - Display account details
- ✅ **Quick Actions** - Easy access to common tasks
- ✅ **Success/Error Handling** - Clear feedback for all actions
- ✅ **Responsive Design** - Mobile-friendly layout

### PasswordChange
- ✅ **Modal Interface** - Clean modal dialog
- ✅ **Password Validation** - Strong password requirements
- ✅ **Current Password** - Verify current password
- ✅ **Password Confirmation** - Double-check new password
- ✅ **Security Feedback** - Clear password requirements
- ✅ **Success Animation** - Visual confirmation

## API Integration

### Backend Endpoints Used:
- `GET /api/users/profile` - Fetch user profile
- `PUT /api/users/profile` - Update profile information
- `POST /api/users/change-password` - Change password

### Request/Response Examples:
```typescript
// Update Profile
PUT /api/users/profile
{
    "firstName": "John",
    "lastName": "Doe",
    "email": "john.doe@example.com",
    "phoneNumber": "+1234567890"
}

// Change Password
POST /api/users/change-password
{
    "currentPassword": "oldPassword123",
    "newPassword": "newPassword123"
}
```

## Next Steps

1. **Add to App.tsx** - Integrate profile component
2. **API Integration** - Connect to backend endpoints
3. **Avatar Upload** - Add profile picture functionality
4. **Email Verification** - Email change verification
5. **Account Deletion** - Implement account deletion
6. **Data Export** - Download user data functionality

The user profile component provides comprehensive account management capabilities! 👤✨ 