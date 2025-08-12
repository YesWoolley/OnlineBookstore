import React from 'react';
import '../components/About.css';

const About: React.FC = () => {
  return (
    <div className="about-container">
      <div className="about-header">
        <h1>About Our Online Bookstore</h1>
        <p className="subtitle">Your Gateway to Literary Excellence</p>
      </div>

      <div className="about-content">
        <section className="about-section">
          <h2>Welcome to Our Digital Library</h2>
          <p>
            We are passionate about connecting readers with their next great adventure. 
            Our online bookstore offers a carefully curated collection of books across all genres, 
            from timeless classics to contemporary bestsellers, academic texts to leisure reading.
          </p>
        </section>

        <section className="about-section">
          <h2>Our Mission</h2>
          <p>
            To make quality literature accessible to everyone, everywhere. We believe that 
            every book has the power to educate, inspire, and transform lives. Our platform 
            is designed to provide a seamless reading experience while supporting authors, 
            publishers, and the literary community.
          </p>
        </section>

        <section className="about-section">
          <h2>What We Offer</h2>
          <div className="features-grid">
            <div className="feature-card">
              <h3>📚 Extensive Collection</h3>
              <p>Thousands of books across fiction, non-fiction, academic, and specialized subjects</p>
            </div>
            <div className="feature-card">
              <h3>🔍 Smart Search</h3>
              <p>Advanced filtering by author, publisher, category, and price range</p>
            </div>
            <div className="feature-card">
              <h3>⭐ Community Reviews</h3>
              <p>Read and write reviews to help other readers make informed choices</p>
            </div>
            <div className="feature-card">
              <h3>🛒 Secure Shopping</h3>
              <p>Safe payment processing with multiple payment options</p>
            </div>
            <div className="feature-card">
              <h3>📱 Mobile Friendly</h3>
              <p>Optimized for all devices - read and shop anywhere, anytime</p>
            </div>
            <div className="feature-card">
              <h3>🚚 Fast Delivery</h3>
              <p>Quick and reliable shipping to get your books to you promptly</p>
            </div>
          </div>
        </section>

        <section className="about-section">
          <h2>Our Story</h2>
          <p>
            Founded with a love for literature and technology, our online bookstore was born 
            from the idea that the digital age should enhance, not replace, the joy of reading. 
            We've built a platform that combines the convenience of online shopping with the 
            personal touch of a neighborhood bookstore.
          </p>
          <p>
            Our team consists of book enthusiasts, technology experts, and customer service 
            professionals who are committed to providing you with the best possible reading 
            experience. We continuously work to improve our platform based on your feedback 
            and the evolving needs of the literary community.
          </p>
        </section>

        <section className="about-section">
          <h2>Why Choose Us?</h2>
          <ul className="benefits-list">
            <li><strong>Quality Assurance:</strong> Every book in our collection meets our high standards</li>
            <li><strong>Customer Support:</strong> Dedicated team ready to help with any questions</li>
            <li><strong>Competitive Pricing:</strong> Best prices guaranteed with regular deals and discounts</li>
            <li><strong>Author Support:</strong> We promote both established and emerging authors</li>
            <li><strong>Community Focus:</strong> Building a community of readers and book lovers</li>
            <li><strong>Environmental Responsibility:</strong> Supporting sustainable publishing practices</li>
          </ul>
        </section>

        <section className="about-section">
          <h2>Get in Touch</h2>
          <p>
            We'd love to hear from you! Whether you have questions about our books, 
            need help with your order, or want to share your reading recommendations, 
            our team is here to help. Your feedback helps us improve and grow.
          </p>
          <div className="contact-info">
            <p><strong>Email:</strong> support@onlinebookstore.com</p>
            <p><strong>Phone:</strong> +1 (555) 123-4567</p>
            <p><strong>Hours:</strong> Monday - Friday, 9:00 AM - 6:00 PM EST</p>
          </div>
        </section>

        <section className="about-section">
          <h2>Join Our Community</h2>
          <p>
            Become part of our growing community of book lovers. Follow us on social media 
            for the latest releases, author interviews, reading recommendations, and exclusive 
            offers. Share your reading journey with fellow book enthusiasts and discover 
            your next favorite book.
          </p>
        </section>
      </div>
    </div>
  );
};

export default About;
