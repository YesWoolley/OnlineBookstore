import Header from './Header';
import Footer from './Footer';
import * as React from 'react';
import type { User } from '../../types/user';

interface MainLayoutProps {
    children: React.ReactNode;
    user?: User | null;
    isAuthenticated?: boolean;
    cartItemCount?: number;
    onSignOut?: () => void;
}

const MainLayout = ({ children, user, isAuthenticated, cartItemCount, onSignOut }: MainLayoutProps) => {
    return (
        <div className="d-flex flex-column min-vh-100">
            {/* Header */}
            <Header
                user={user || null}
                isAuthenticated={isAuthenticated || false}
                cartItemCount={cartItemCount || 0}
                onSignOut={onSignOut || (() => {})}
            />

            {/* Main Content Area */}
            <main className="flex-grow-1">
                {children}
            </main>

            {/* Footer */}
            <Footer />
        </div>
    );
};

export default MainLayout;