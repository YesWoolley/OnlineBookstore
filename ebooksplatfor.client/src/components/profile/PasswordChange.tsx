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

        if (!formData.confirmNewPassword.trim()) {
            newErrors.confirmNewPassword = 'Please confirm your new password';
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
                                    <label htmlFor="confirmPassword" className="form-label">
                                        Confirm New Password *
                                    </label>
                                    <input
                                        type="password"
                                            className={`form-control ${errors.confirmNewPassword ? 'is-invalid' : ''}`}
                                        id="confirmPassword"
                                        name="confirmPassword"
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