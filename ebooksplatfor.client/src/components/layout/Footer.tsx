
import { Link } from 'react-router-dom';

const Footer = () => {
  return (
    <footer className="py-3 my-4 w-100">
      <div className="container">
        <ul className="nav justify-content-center border-bottom pb-3 mb-3">
          <li className="nav-item">
            <Link to="/" className="nav-link px-2 text-body-secondary">Home</Link>
          </li>
          <li className="nav-item">
            <Link to="/books" className="nav-link px-2 text-body-secondary">Books</Link>
          </li>
          <li className="nav-item">
            <Link to="/cart" className="nav-link px-2 text-body-secondary">Cart</Link>
          </li>
          <li className="nav-item">
            <Link to="/about" className="nav-link px-2 text-body-secondary">About</Link>
          </li>
        </ul>
        <p className="text-center text-body-secondary">© 2025 BookStore, Inc</p>
      </div>
    </footer>
  );
};

export default Footer;