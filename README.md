# 📚 EbooksPlatfor - Online Bookstore Platform

A full-stack online bookstore application built with **ASP.NET Core Web API** backend and **React TypeScript** frontend.

## 🎯 What It Is

A complete e-commerce platform for selling books online, featuring user authentication, book browsing, shopping cart functionality, order management, and review systems.

## 🏗️ Architecture

- **Backend**: ASP.NET Core Web API with Entity Framework Core
- **Frontend**: React 18+ with TypeScript and Bootstrap
- **Database**: SQL Server with Entity Framework migrations
- **Authentication**: JWT-based with role-based authorization

## 🚀 Key Features

- **User Management**: Registration, login, profiles, password changes
- **Book Catalog**: Browse, search, and filter books by category/author/publisher
- **Shopping Cart**: Add/remove items, quantity management, persistent cart
- **Order System**: Checkout process, order history, status tracking
- **Reviews & Ratings**: User-generated book reviews and ratings
- **Admin Panel**: Book, author, category, and order management

## 🛠️ Tech Stack

### Backend
- ASP.NET Core 7.0+
- Entity Framework Core
- JWT Authentication
- AutoMapper for DTOs
- SQL Server Database

### Frontend
- React 18+ with TypeScript
- Bootstrap for styling and components
- TanStack Query for data fetching
- React Hook Form + Zod validation
- React Router for navigation

## 📁 Project Structure

```
EbooksPlatfor/
├── EbooksPlatfor.Server/          # ASP.NET Core Backend
│   ├── Controllers/               # API endpoints
│   ├── Models/                   # Entity models
│   ├── Services/                 # Business logic
│   ├── Data/                    # Database context & migrations
│   └── DTOs/                    # Data transfer objects
├── ebooksplatfor.client/         # React Frontend
│   ├── src/components/           # UI components
│   ├── src/pages/               # Page components
│   ├── src/hooks/               # Custom React hooks
│   └── src/services/            # API integration
└── EbooksPlatfor.Server.Tests/   # Backend unit tests
```

## 🔧 Getting Started

### Prerequisites
- .NET 7.0+ SDK
- SQL Server
- Node.js 18+

### Backend Setup
```bash
cd EbooksPlatfor.Server
dotnet restore
dotnet ef database update
dotnet run
```

### Frontend Setup
```bash
cd ebooksplatfor.client
npm install
npm run dev
```

## 🌐 API Overview

The backend provides RESTful APIs for:
- **Authentication**: `/api/auth/*`
- **Books**: `/api/books/*`
- **Authors**: `/api/authors/*`
- **Categories**: `/api/categories/*`
- **Shopping Cart**: `/api/cart/*`
- **Orders**: `/api/orders/*`
- **Reviews**: `/api/reviews/*`
- **Users**: `/api/users/*`

## 🗄️ Database

Uses Entity Framework Core with SQL Server, featuring:
- User management and authentication
- Book catalog with authors, publishers, and categories
- Shopping cart and order management
- Review and rating system
- Proper relationships and constraints

## 🧪 Testing

- Backend: Unit tests using xUnit
- Frontend: Component testing with React Testing Library
- Run with `dotnet test` (backend) and `npm test` (frontend)

## 🚀 Deployment

- **Backend**: Azure App Service or Docker containers
- **Frontend**: Netlify, Vercel, or Azure Static Web Apps
- **Database**: Azure SQL Database or SQL Server

## 📚 Learning Focus

This project demonstrates:
- Full-stack development with modern technologies
- RESTful API design and implementation
- Database design with Entity Framework
- Modern React patterns and TypeScript
- Authentication and authorization systems
- E-commerce functionality implementation
