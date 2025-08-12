import React from 'react';
import { useUserStats } from '../../hooks/useUserStats';
import type { User } from '../../types/user';

interface UserProfileProps {
    user: User;
}

const UserProfile: React.FC<UserProfileProps> = ({ user }) => {
    const { stats, isLoading: statsLoading, invalidateAndRefetch } = useUserStats(user.id);

    const formatDate = (dateString: string) => {
        return new Date(dateString).toLocaleDateString('en-US', {
            year: 'numeric',
            month: 'long',
            day: 'numeric',
        });
    };

    return (
        <div className="w-100 py-5">
            <div className="container">
                <div className="row justify-content-center">
                    <div className="col-lg-8">
                        {/* Header */}
                        <div className="text-center mb-5">
                            <div className="mb-4">
                                <i className="bi bi-person-circle" style={{ fontSize: '4rem', color: '#0d6efd' }}></i>
                            </div>
                            <h2 className="mb-2">My Profile</h2>
                            <p className="text-muted">Welcome to your personal dashboard</p>
                        </div>

                        {/* Profile Information Card */}
                        <div className="card shadow-sm mb-4">
                            <div className="card-header bg-primary text-white">
                                <h5 className="mb-0">
                                    <i className="bi bi-person me-2"></i>
                                    Personal Information
                                </h5>
                            </div>
                            <div className="card-body">
                                <div className="row">
                                    <div className="col-md-6 mb-3">
                                        <label className="form-label fw-medium text-muted">First Name</label>
                                        <p className="mb-0 fs-5">{user.firstName || 'Not provided'}</p>
                                    </div>
                                    <div className="col-md-6 mb-3">
                                        <label className="form-label fw-medium text-muted">Last Name</label>
                                        <p className="mb-0 fs-5">{user.lastName || 'Not provided'}</p>
                                    </div>
                                    <div className="col-md-6 mb-3">
                                        <label className="form-label fw-medium text-muted">Username</label>
                                        <p className="mb-0 fs-5">{user.userName}</p>
                                    </div>
                                    <div className="col-md-6 mb-3">
                                        <label className="form-label fw-medium text-muted">Email Address</label>
                                        <p className="mb-0 fs-5">{user.email}</p>
                                    </div>
                                </div>
                            </div>
                        </div>

                        {/* Account Information Card */}
                        <div className="card shadow-sm mb-4">
                            <div className="card-header bg-info text-white">
                                <h5 className="mb-0">
                                    <i className="bi bi-shield-check me-2"></i>
                                    Account Details
                                </h5>
                            </div>
                            <div className="card-body">
                                <div className="row">
                                    <div className="col-md-6 mb-3">
                                        <label className="form-label fw-medium text-muted">Account Status</label>
                                        <p className="mb-0">
                                            <span className="badge bg-success fs-6">Active</span>
                                        </p>
                                    </div>
                                    <div className="col-md-6 mb-3">
                                        <label className="form-label fw-medium text-muted">Member Since</label>
                                        <p className="mb-0 fs-6">{user.createdAt ? formatDate(user.createdAt) : 'N/A'}</p>
                                    </div>
                                </div>
                            </div>
                        </div>

                        {/* Quick Stats Card */}
                        <div className="card shadow-sm">
                            <div className="card-header bg-success text-white d-flex justify-content-between align-items-center">
                                <h5 className="mb-0">
                                    <i className="bi bi-graph-up me-2"></i>
                                    Quick Overview
                                </h5>
                                <button 
                                    className="btn btn-sm btn-outline-light"
                                    onClick={() => invalidateAndRefetch()}
                                    disabled={statsLoading}
                                >
                                    {statsLoading ? (
                                        <span className="spinner-border spinner-border-sm" role="status"></span>
                                    ) : (
                                        <i className="bi bi-arrow-clockwise"></i>
                                    )}
                                </button>
                            </div>
                            <div className="card-body">
                                <div className="row text-center">
                                    <div className="col-md-4 mb-3">
                                        <div className="p-3">
                                            <i className="bi bi-book text-primary" style={{ fontSize: '2rem' }}></i>
                                            <h4 className="mt-2 mb-1">
                                                {statsLoading ? (
                                                    <span className="spinner-border spinner-border-sm text-primary" role="status"></span>
                                                ) : (
                                                    stats.booksPurchased
                                                )}
                                            </h4>
                                            <p className="text-muted mb-0">Books Purchased</p>
                                        </div>
                                    </div>
                                    <div className="col-md-4 mb-3">
                                        <div className="p-3">
                                            <i className="bi bi-star text-warning" style={{ fontSize: '2rem' }}></i>
                                            <h4 className="mt-2 mb-1">
                                                {statsLoading ? (
                                                    <span className="spinner-border spinner-border-sm text-warning" role="status"></span>
                                                ) : (
                                                    stats.reviewCount
                                                )}
                                            </h4>
                                            <p className="text-muted mb-0">Reviews Written</p>
                                        </div>
                                    </div>
                                    <div className="col-md-4 mb-3">
                                        <div className="p-3">
                                            <i className="bi bi-calendar-check text-info" style={{ fontSize: '2rem' }}></i>
                                            <h4 className="mt-2 mb-1">
                                                {statsLoading ? (
                                                    <span className="spinner-border spinner-border-sm text-info" role="status"></span>
                                                ) : (
                                                    stats.orderCount
                                                )}
                                            </h4>
                                            <p className="text-muted mb-0">Orders Placed</p>
                                        </div>
                                    </div>
                                </div>
                                
                                {/* Last Updated Info */}
                                <div className="text-center mt-3">
                                    <small className="text-muted">
                                        <i className="bi bi-info-circle me-1"></i>
                                        Stats update automatically. Click refresh to update now.
                                    </small>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default UserProfile;