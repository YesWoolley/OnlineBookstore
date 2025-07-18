# 📚 Online Bookstore
## ASP.NET Core Web API + React Full-Stack Application

A complete online bookstore platform built with ASP.NET Core Web API backend and React frontend. This project demonstrates modern full-stack development practices with a focus on e-commerce functionality.

## 🎯 Project Overview

This application allows users to:
- Browse and search for books
- Purchase physical books
- Track order status and shipping
- Manage personal account
- Review and rate books
- Receive book recommendations

## 🏗️ Architecture

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   React App     │    │  ASP.NET Core   │    │   SQL Server    │
│   (Frontend)    │◄──►│   Web API       │◄──►│   Database      │
│                 │    │   (Backend)     │    │                 │
└─────────────────┘    └─────────────────┘    └─────────────────┘
```

## 🛠️ Technology Stack

### Backend
- **ASP.NET Core 7.0+** - Web API framework
- **Entity Framework Core** - ORM for database operations
- **JWT Authentication** - Token-based authentication
- **SQL Server** - Database (can be PostgreSQL)
- **AutoMapper** - Object mapping
- **FluentValidation** - Input validation

### Frontend (Modern Stack)
- **React 18+ with TypeScript** - Type-safe frontend development
- **Tailwind CSS** - Utility-first CSS for rapid styling
- **shadcn/ui** - Beautiful, accessible component library
- **TanStack Query** - Powerful data fetching and caching
- **React Hook Form + Zod** - Type-safe form handling
- **Lucide React** - Beautiful, consistent icons

### Frontend
- **React 18+** - Frontend framework
- **TypeScript** - Type safety and better development experience
- **Tailwind CSS** - Utility-first CSS framework
- **shadcn/ui** - Beautiful, accessible component library
- **React Router** - Client-side routing
- **TanStack Query** - Data fetching and caching
- **React Hook Form** - Form handling with validation
- **Zod** - Schema validation
- **Lucide React** - Beautiful icons

### Development Tools
- **Visual Studio 2022** / **VS Code** - IDE
- **SQL Server Management Studio** - Database management
- **Postman** - API testing
- **Git** - Version control

## 📁 Project Structure

```
OnlineBookstore/
├── Backend/                    # ASP.NET Core Web API
│   ├── Controllers/           # API endpoints
│   ├── Models/               # Entity models
│   ├── Services/             # Business logic
│   ├── Data/                # Database context
│   ├── DTOs/                # Data transfer objects
│   └── Middleware/           # Custom middleware
├── Frontend/                 # React application
│   ├── src/
│   │   ├── components/      # Reusable components
│   │   ├── pages/           # Page components
│   │   ├── services/        # API services
│   │   ├── hooks/           # Custom hooks
│   │   ├── context/         # React context
│   │   └── utils/           # Utility functions
│   └── public/              # Static assets
└── Database/                # Database scripts
```

## 🚀 Features

### 🔐 Authentication & Authorization
- User registration and login
- JWT token-based authentication
- Role-based authorization (User, Admin)
- Password reset functionality

### 📚 Book Management
- Browse books by category
- Search books by title, author, or publisher
- Book details with descriptions and images
- Book reviews and ratings
- Inventory tracking

### 🛒 Shopping Cart
- Add/remove books from cart
- Update quantities
- Cart persistence across sessions
- Secure checkout process

### 📦 Order Management
- Order history and tracking
- Shipping address management
- Order status updates
- Invoice generation

### 👤 User Profile
- Personal information management
- Order history
- Wishlist management
- Address book

### ⭐ Reviews & Ratings
- Book rating system
- User reviews
- Review moderation (Admin)
- Rating analytics

## 🔧 API Endpoints

### Authentication
```
POST   /api/auth/register     # User registration
POST   /api/auth/login        # User login
POST   /api/auth/refresh      # Refresh token
POST   /api/auth/logout       # User logout
```

### Books
```
GET    /api/books             # Get all books
GET    /api/books/{id}        # Get book by id
POST   /api/books             # Create book (Admin)
PUT    /api/books/{id}        # Update book (Admin)
DELETE /api/books/{id}        # Delete book (Admin)
GET    /api/books/search      # Search books by title/author/category
GET    /api/books/search/advanced  # Advanced search with filters
```

### Authors
```
GET    /api/authors           # Get all authors
GET    /api/authors/{id}      # Get author by id
POST   /api/authors           # Create author (Admin)
PUT    /api/authors/{id}      # Update author (Admin)
DELETE /api/authors/{id}      # Delete author (Admin)
```

### Categories
```
GET    /api/categories        # Get all categories
GET    /api/categories/{id}   # Get category by id
POST   /api/categories        # Create category (Admin)
PUT    /api/categories/{id}   # Update category (Admin)
DELETE /api/categories/{id}   # Delete category (Admin)
```

### Shopping Cart
```
GET    /api/cart              # Get cart items
POST   /api/cart/add          # Add item to cart
PUT    /api/cart/{id}         # Update cart item
DELETE /api/cart/{id}         # Remove from cart
POST   /api/cart/clear        # Clear cart
```

### Orders
```
GET    /api/orders            # Get user orders
GET    /api/orders/{id}       # Get order by id
POST   /api/orders            # Create order
GET    /api/orders/admin      # Get all orders (Admin)
PUT    /api/orders/{id}       # Update order status (Admin)
```

## 🗄️ Database Schema

### Core Entities
- **Users** - User accounts and profiles
- **Books** - Book information and metadata
- **Authors** - Book authors
- **Publishers** - Book publishers
- **Categories** - Book categories
- **Orders** - Purchase orders
- **OrderItems** - Individual items in orders
- **ShoppingCartItems** - Cart items
- **Reviews** - Book reviews and ratings

## 🔒 Security Features

- **JWT Authentication** - Secure token-based authentication
- **Password Hashing** - BCrypt password hashing
- **CORS Configuration** - Cross-origin resource sharing
- **Input Validation** - Server-side validation
- **SQL Injection Prevention** - Entity Framework protection
- **XSS Protection** - Content Security Policy

## 🧪 Testing

### Backend Testing
```bash
cd Backend
dotnet test
```

### Frontend Testing
```bash
cd Frontend
npm test
```

## 📦 Deployment

### Backend Deployment
- Azure App Service
- Docker containerization
- Environment configuration

### Frontend Deployment
- Netlify
- Vercel
- Azure Static Web Apps

## 🎓 Learning Outcomes

By completing this project, you'll learn:

### Backend Development
- ASP.NET Core Web API development
- Entity Framework Core with SQL Server
- JWT authentication and authorization
- RESTful API design principles
- Dependency injection and service patterns
- Data validation and error handling

### Frontend Development
- React with TypeScript for type safety
- Modern React patterns (hooks, context, custom hooks)
- Tailwind CSS for utility-first styling
- shadcn/ui for beautiful, accessible components
- TanStack Query for efficient data fetching
- React Hook Form with Zod validation
- Modern state management patterns
- Responsive and accessible UI design

### Full-Stack Integration
- API design and documentation
- CORS and security configuration
- Error handling across the stack
- Deployment and hosting strategies
- Database design and relationships

## 🚀 Getting Started

1. **Clone the repository**
2. **Set up the backend** (see [Section 3: Getting Started](./03-GETTING-STARTED.md))
3. **Set up the frontend** (see [Section 7: Understanding UI Design](./07-UNDERSTANDING-UI-DESIGN.md))
4. **Configure the database** (see [Section 5: Database Configuration](./05-DATABASE-CONFIGURATION.md))
5. **Run the application**

## 📚 Guide Sections

- [Section 1: Before You Get Started](./01-BEFORE-YOU-GET-STARTED.md)
- [Section 2: Welcome](./02-WELCOME.md)
- [Section 3: Getting Started](./03-GETTING-STARTED.md)
- [Section 4: Building the Data Structure](./04-BUILDING-DATA-STRUCTURE.md)
- [Section 5: Database Configuration](./05-DATABASE-CONFIGURATION.md)
- [Section 6: Building and Managing Controllers](./06-BUILDING-CONTROLLERS.md)
- [Section 7: Managing Author Data](./08-MANAGING-AUTHOR-DATA.md)
- [Section 8: Managing Publisher Data](./10-MANAGING-PUBLISHER-DATA.md)
- [Section 9: Managing Publisher Data](./10-MANAGING-PUBLISHER-DATA.md)
- [Section 10: Managing Category Data](./11-MANAGING-CATEGORY-DATA.md)
- [Section 11: Managing Book Data](./12-MANAGING-BOOK-DATA.md)
- [Section 12: Managing Shopping Cart and Orders](./13-MANAGING-SHOPPING-CART-ORDERS.md)
- [Section 13: Integrating Payments](./14-INTEGRATING-PAYMENTS.md)
- [Section 14: ASP.NET Identity Framework](./15-ASP-NET-IDENTITY-FRAMEWORK.md)
- [Section 15: Deploying the Application](./16-DEPLOYING-THE-APPLICATION.md)
- [Section 16: Understanding UI Design (React + TypeScript + Tailwind CSS + shadcn/ui)](./07-UNDERSTANDING-UI-DESIGN.md)

---

**Happy coding! 📚🛒** 