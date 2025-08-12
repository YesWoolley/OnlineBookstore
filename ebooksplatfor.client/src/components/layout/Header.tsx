import React from 'react';
import { Link } from 'react-router-dom';
import { useSearch } from '../../hooks/useSearch';
import type { User } from '../../types/user';

interface HeaderProps {
  user: User | null;
  isAuthenticated: boolean;
  cartItemCount: number;
  onSignOut: () => void;
}

const Header: React.FC<HeaderProps> = ({ 
  user, 
  isAuthenticated, 
  cartItemCount, 
  onSignOut 
}) => {
  const { 
    searchQuery, 
    isSearching, 
    handleSearchSubmit, 
    handleInputChange, 
    clearSearch 
  } = useSearch();

  return (
    <header className="p-3 mb-3 border-bottom">
      <div className="container">
        <div className="d-flex flex-wrap align-items-center justify-content-center justify-content-lg-start">
          {/* Logo/Brand */}
          <Link to="/" className="d-flex align-items-center mb-2 mb-lg-0 link-body-emphasis text-decoration-none">
            <i className="bi bi-book me-2" style={{ fontSize: '2rem', color: '#0d6efd' }}></i>
            <span className="fw-bold fs-4">BookStore</span>
          </Link>
          
          {/* Navigation Links */}
          <ul className="nav col-12 col-lg-auto me-lg-auto mb-2 justify-content-center mb-md-0 ms-4">
            <li><Link to="/" className="nav-link px-2 link-body-emphasis">Home</Link></li>
            <li><Link to="/books" className="nav-link px-2 link-body-emphasis">Books</Link></li>
            <li>
              <Link to="/cart" className="nav-link px-2 link-body-emphasis position-relative">
                Cart
                {cartItemCount > 0 && (
                  <span className="position-absolute top-0 start-100 translate-middle badge rounded-pill bg-danger">
                    {cartItemCount}
                  </span>
                )}
              </Link>
            </li>
          </ul>
          
          {/* Search Bar */}
          <form className="col-12 col-lg-auto mb-3 mb-lg-0 me-lg-3" role="search" onSubmit={handleSearchSubmit}>
            <div className="input-group">
              <input
                type="search"
                className="form-control"
                placeholder="Search books, authors..."
                aria-label="Search"
                value={searchQuery}
                onChange={handleInputChange}
                disabled={isSearching}
              />
              {searchQuery && (
                <button
                  type="button"
                  className="btn btn-outline-secondary"
                  onClick={clearSearch}
                  title="Clear search"
                >
                  <i className="bi bi-x"></i>
                </button>
              )}
              <button
                type="submit"
                className="btn btn-primary"
                disabled={isSearching || !searchQuery.trim()}
              >
                {isSearching ? (
                  <span className="spinner-border spinner-border-sm" role="status"></span>
                ) : (
                  <i className="bi bi-search"></i>
                )}
              </button>
            </div>
          </form>
          
          {/* User Dropdown */}
          <div className="dropdown text-end">
            <a
              href="#"
              className="d-block link-body-emphasis text-decoration-none dropdown-toggle"
              data-bs-toggle="dropdown"
              aria-expanded="false"
            >
              <i className="bi bi-person-circle" style={{ fontSize: '2rem' }}></i>
            </a>
            <ul className="dropdown-menu text-small">
              {isAuthenticated ? (
                <>
                  <li>
                    <div className="dropdown-item-text">
                      <strong>Welcome, {user?.firstName || user?.userName || 'User'}!</strong>
                    </div>
                  </li>
                  <li><hr className="dropdown-divider" /></li>
                  <li><Link className="dropdown-item" to="/profile">Profile</Link></li>
                  <li><hr className="dropdown-divider" /></li>
                  <li><button className="dropdown-item" onClick={onSignOut}>Sign out</button></li>
                </>
              ) : (
                <>
                  <li><Link className="dropdown-item" to="/signin">Sign in</Link></li>
                  <li><Link className="dropdown-item" to="/signup">Sign up</Link></li>
                </>
              )}
            </ul>
          </div>
        </div>
      </div>
    </header>
  );
};

export default Header;