# Section 03: Building the Footer Component

## 🎯 **Creating the Footer (Full-Width Bootstrap Design)**

### **Step 1: Understanding the Footer Structure**

> **📚 Reference:** This footer structure is based on the official [Bootstrap Headers Examples](https://getbootstrap.com/docs/5.3/examples/footers/) from getbootstrap.com, specifically the "Simple footer" example with full-width design.

The Footer component will provide:
- **Full-Width Design** - Occupies the entire viewport width
- **Navigation Links** - Centered horizontal navigation
- **Copyright** - Centered copyright text
- **Clean Design** - Simple, elegant Bootstrap footer

### **Step 2: Create the Footer Component**

Create `src/components/layout/Footer.tsx`:

```tsx
import * as React from 'react';

const Footer = () => {
  return (
    <footer className="py-3 my-4 w-100">
      <div className="container">
        <ul className="nav justify-content-center border-bottom pb-3 mb-3">
          <li className="nav-item">
            <a href="/" className="nav-link px-2 text-body-secondary">Home</a>
          </li>
          <li className="nav-item">
            <a href="/books" className="nav-link px-2 text-body-secondary">Books</a>
          </li>
          <li className="nav-item">
            <a href="/cart" className="nav-link px-2 text-body-secondary">Cart</a>
          </li>
          <li className="nav-item">
            <a href="/signin" className="nav-link px-2 text-body-secondary">Login</a>
          </li>
          <li className="nav-item">
            <a href="/about" className="nav-link px-2 text-body-secondary">About</a>
          </li>
        </ul>
        <p className="text-center text-body-secondary">© 2025 BookStore, Inc</p>
      </div>
    </footer>
  );
};

export default Footer;
```

### **Step 3: Add Custom Styling**

Add these styles to `src/index.css`:

```css
/* Custom footer styling */
footer {
  background: linear-gradient(135deg, #f8f9fa 0%, #e9ecef 100%);
  border-radius: 0;
  box-shadow: 0 -2px 4px rgba(0,0,0,0.1);
  width: 100%;
  margin: 0;
}

footer .nav-link {
  font-weight: 500;
  transition: color 0.2s ease;
}

footer .nav-link:hover {
  color: #0d6efd !important;
}

footer .border-bottom {
  border-color: rgba(0,0,0,0.1) !important;
}

/* Responsive adjustments */
@media (max-width: 767.98px) {
  footer .nav {
    flex-wrap: wrap;
  }
  
  footer .nav-item {
    margin-bottom: 0.5rem;
  }
}
```

### **Step 4: Test the Footer**

Update your `src/App.tsx` to include the Footer:

```tsx
import * as React from 'react';
import Header from './components/layout/Header';
import Footer from './components/layout/Footer';
import './index.css';

function App() {
    return (
        <div className="App d-flex flex-column min-vh-100">
            <Header />
            <main className="flex-grow-1">
                <div className="container py-4">
                    <h1>Welcome to BookStore</h1>
                    <p>Your footer is now working with full-width design!</p>
                </div>
            </main>
            <Footer />
        </div>
    );
}

export default App;
```

## **🎨 Footer Features (Simple Bootstrap Design)**

### **✅ What We Built:**

#### **🔗 Navigation Links:**
- **Home** - Main page
- **Books** - Browse all books
- **Cart** - Shopping cart
- **Login** - User authentication
- **About** - Company information

#### **© Copyright:**
- **Legal Text** - "© 2025 BookStore, Inc"
- **Centered** - Professional layout
- **Clean Design** - Simple and elegant

#### **🎨 Design Elements:**
- **Centered Navigation** - Horizontal nav with justify-content-center
- **Border Bottom** - Subtle separator line
- **Light Background** - Gradient from light gray
- **Hover Effects** - Blue color on hover

### **📱 Responsive Behavior:**

#### **🖥️ Desktop View:**
- **Centered Navigation** - All links in one row
- **Clean Layout** - Simple, professional design
- **Proper Spacing** - Consistent margins and padding

#### **📱 Mobile View:**
- **Wrapped Navigation** - Links wrap to multiple lines
- **Touch-Friendly** - Large touch targets
- **Responsive Spacing** - Adjusted margins

## **🔧 Bootstrap Classes Used:**

### **🎯 Layout Classes:**
- `container` - Centered content
- `py-3` - Vertical padding
- `my-4` - Vertical margin
- `justify-content-center` - Center navigation

### **🎨 Styling Classes:**
- `text-body-secondary` - Secondary text color
- `border-bottom` - Bottom border
- `pb-3` - Padding bottom
- `mb-3` - Margin bottom

### **📐 Spacing Classes:**
- `px-2` - Horizontal padding
- `text-center` - Center alignment
- `nav-item` - Navigation item styling

### **🎯 Interactive Classes:**
- `nav` - Navigation component
- `nav-link` - Navigation links
- `nav-item` - Navigation items

## **🚀 Next Steps**

### **✅ Footer Complete!**

Your footer is now:
- **Clean & Simple** - Elegant Bootstrap design
- **Responsive** - Works on all devices
- **Professional** - Centered navigation and copyright
- **Accessible** - Screen reader friendly

### **🎯 What's Next:**

**Option A: Create MainLayout Component**
- Wrap pages with header and footer
- Consistent page structure
- Proper layout management

**Option B: Create BookCard Component**
- Individual book display
- Bootstrap card structure
- Book information layout

**Option C: Create BookGrid Component**
- Responsive book grid
- Multiple book cards
- Grid layout system

**Which component would you like to build next?** 🎯

The Footer now uses the clean, simple Bootstrap design you provided! 