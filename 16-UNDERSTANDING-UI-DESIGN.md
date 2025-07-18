# Section 16: Understanding UI Design (React + TypeScript + Tailwind CSS + shadcn/ui)

Welcome to the frontend development phase! In this section, we'll explore modern React development with TypeScript, set up Tailwind CSS for styling, integrate shadcn/ui components, implement routing with React Router, and connect our React frontend to the ASP.NET Core API we built in the previous section.

---

## 🎯 What You'll Learn

- How to structure React components with TypeScript for type safety
- How to set up and configure Tailwind CSS for modern styling
- How to integrate shadcn/ui components for beautiful UI
- How to set up routing with React Router
- How to manage state with Context API and modern patterns
- How to connect React to your ASP.NET Core API
- Best practices for modern React development and UI/UX design

---

## 🏗️ Step 1: Understanding React Component Structure

### **🎯 What are React Components?**

React Components are like **"building blocks"** for your user interface. Think of them as **Lego pieces** that you can combine to build a complete application.

### **🔧 Component Types:**

| Component Type | Purpose | Example |
|----------------|---------|---------|
| **Page Components** | Full pages | HomePage, BookListPage |
| **Layout Components** | Page structure | Header, Footer, Sidebar |
| **UI Components** | Reusable elements | Button, Card, Modal |
| **Container Components** | Data management | BookList, ShoppingCart |

### **📁 Recommended Folder Structure:**
```
src/
├── components/          # Reusable UI components
│   ├── ui/            # Basic UI elements (Button, Card, etc.)
│   ├── layout/        # Layout components (Header, Footer, etc.)
│   └── forms/         # Form components
├── pages/              # Page components
├── hooks/              # Custom React hooks
├── context/            # State management
├── services/           # API communication
├── utils/              # Helper functions
└── styles/             # CSS/styling
```

---

## 🎮 Step 2: Set Up Modern Frontend Stack

### **Install Required Dependencies:**
```bash
cd ebooksplatfor.client

# Install Tailwind CSS
npm install -D tailwindcss postcss autoprefixer

# Install React Router
npm install react-router-dom

# Install shadcn/ui CLI
npm install -D @shadcn/ui

# Install additional modern tools
npm install @tanstack/react-query
npm install react-hook-form
npm install zod
npm install @hookform/resolvers
npm install lucide-react
```

### **Configure Tailwind CSS:**
```bash
# Initialize Tailwind CSS
npx tailwindcss init -p
```

### **Update tailwind.config.js:**
```javascript
/** @type {import('tailwindcss').Config} */
module.exports = {
  darkMode: ["class"],
  content: [
    './pages/**/*.{ts,tsx}',
    './components/**/*.{ts,tsx}',
    './app/**/*.{ts,tsx}',
    './src/**/*.{ts,tsx}',
  ],
  prefix: "",
  theme: {
    container: {
      center: true,
      padding: "2rem",
      screens: {
        "2xl": "1400px",
      },
    },
    extend: {
      colors: {
        border: "hsl(var(--border))",
        input: "hsl(var(--input))",
        ring: "hsl(var(--ring))",
        background: "hsl(var(--background))",
        foreground: "hsl(var(--foreground))",
        primary: {
          DEFAULT: "hsl(var(--primary))",
          foreground: "hsl(var(--primary-foreground))",
        },
        secondary: {
          DEFAULT: "hsl(var(--secondary))",
          foreground: "hsl(var(--secondary-foreground))",
        },
        destructive: {
          DEFAULT: "hsl(var(--destructive))",
          foreground: "hsl(var(--destructive-foreground))",
        },
        muted: {
          DEFAULT: "hsl(var(--muted))",
          foreground: "hsl(var(--muted-foreground))",
        },
        accent: {
          DEFAULT: "hsl(var(--accent))",
          foreground: "hsl(var(--accent-foreground))",
        },
        popover: {
          DEFAULT: "hsl(var(--popover))",
          foreground: "hsl(var(--popover-foreground))",
        },
        card: {
          DEFAULT: "hsl(var(--card))",
          foreground: "hsl(var(--card-foreground))",
        },
      },
      borderRadius: {
        lg: "var(--radius)",
        md: "calc(var(--radius) - 2px)",
        sm: "calc(var(--radius) - 4px)",
      },
      keyframes: {
        "accordion-down": {
          from: { height: "0" },
          to: { height: "var(--radix-accordion-content-height)" },
        },
        "accordion-up": {
          from: { height: "var(--radix-accordion-content-height)" },
          to: { height: "0" },
        },
      },
      animation: {
        "accordion-down": "accordion-down 0.2s ease-out",
        "accordion-up": "accordion-up 0.2s ease-out",
      },
    },
  },
  plugins: [require("tailwindcss-animate")],
}
```

### **Update src/index.css:**
```css
@tailwind base;
@tailwind components;
@tailwind utilities;

@layer base {
  :root {
    --background: 0 0% 100%;
    --foreground: 222.2 84% 4.9%;
    --card: 0 0% 100%;
    --card-foreground: 222.2 84% 4.9%;
    --popover: 0 0% 100%;
    --popover-foreground: 222.2 84% 4.9%;
    --primary: 221.2 83.2% 53.3%;
    --primary-foreground: 210 40% 98%;
    --secondary: 210 40% 96%;
    --secondary-foreground: 222.2 84% 4.9%;
    --muted: 210 40% 96%;
    --muted-foreground: 215.4 16.3% 46.9%;
    --accent: 210 40% 96%;
    --accent-foreground: 222.2 84% 4.9%;
    --destructive: 0 84.2% 60.2%;
    --destructive-foreground: 210 40% 98%;
    --border: 214.3 31.8% 91.4%;
    --input: 214.3 31.8% 91.4%;
    --ring: 221.2 83.2% 53.3%;
    --radius: 0.5rem;
  }

  .dark {
    --background: 222.2 84% 4.9%;
    --foreground: 210 40% 98%;
    --card: 222.2 84% 4.9%;
    --card-foreground: 210 40% 98%;
    --popover: 222.2 84% 4.9%;
    --popover-foreground: 210 40% 98%;
    --primary: 217.2 91.2% 59.8%;
    --primary-foreground: 222.2 84% 4.9%;
    --secondary: 217.2 32.6% 17.5%;
    --secondary-foreground: 210 40% 98%;
    --muted: 217.2 32.6% 17.5%;
    --muted-foreground: 215 20.2% 65.1%;
    --accent: 217.2 32.6% 17.5%;
    --accent-foreground: 210 40% 98%;
    --destructive: 0 62.8% 30.6%;
    --destructive-foreground: 210 40% 98%;
    --border: 217.2 32.6% 17.5%;
    --input: 217.2 32.6% 17.5%;
    --ring: 224.3 76.3% 94.1%;
  }
}

@layer base {
  * {
    @apply border-border;
  }
  body {
    @apply bg-background text-foreground;
  }
}
```

### **Initialize shadcn/ui:**
```bash
# Initialize shadcn/ui
npx shadcn@latest init
```

### **Create App.tsx with Modern Stack:**
```tsx
import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { ThemeProvider } from './components/theme-provider';
import Header from './components/layout/Header';
import Footer from './components/layout/Footer';
import HomePage from './pages/HomePage';
import BookListPage from './pages/BookListPage';
import BookDetailPage from './pages/BookDetailPage';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import ShoppingCartPage from './pages/ShoppingCartPage';
import { AuthProvider } from './context/AuthContext';
import { CartProvider } from './context/CartContext';

// Create a client for React Query
const queryClient = new QueryClient();

function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <ThemeProvider defaultTheme="light" storageKey="vite-ui-theme">
        <AuthProvider>                    {/* Provides authentication state to all components */}
          <CartProvider>                  {/* Provides shopping cart state to all components */}
            <Router>                      {/* Enables client-side routing */}
              <div className="min-h-screen bg-background">
                <Header />                {/* Navigation bar with menu and user info */}
                <main className="container mx-auto px-4 py-8">
                  <Routes>                {/* Defines all application routes */}
                    <Route path="/" element={<HomePage />} />                    {/* Landing page */}
                    <Route path="/books" element={<BookListPage />} />           {/* Browse all books */}
                    <Route path="/books/:id" element={<BookDetailPage />} />     {/* Individual book details */}
                    <Route path="/login" element={<LoginPage />} />              {/* User login form */}
                    <Route path="/register" element={<RegisterPage />} />        {/* User registration form */}
                    <Route path="/cart" element={<ShoppingCartPage />} />        {/* Shopping cart view */}
                  </Routes>
                </main>
                <Footer />                {/* Site footer with links and info */}
              </div>
            </Router>
          </CartProvider>
        </AuthProvider>
      </ThemeProvider>
    </QueryClientProvider>
  );
}

export default App;
```

---

## 🎨 Step 3: Set Up shadcn/ui Components

### **Install shadcn/ui Components:**
```bash
# Install essential shadcn/ui components
npx shadcn@latest add button
npx shadcn@latest add card
npx shadcn@latest add input
npx shadcn@latest add label
npx shadcn@latest add form
npx shadcn@latest add select
npx shadcn@latest add dialog
npx shadcn@latest add dropdown-menu
npx shadcn@latest add toast
npx shadcn@latest add theme-provider
npx shadcn@latest add avatar
npx shadcn@latest add badge
npx shadcn@latest add separator
npx shadcn@latest add skeleton
```

### **Create components/ui/BookCard.tsx with shadcn/ui:**
```tsx
import React from 'react';
import { Link } from 'react-router-dom';
import { Card, CardContent, CardFooter, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { BookOpen, ShoppingCart, Star } from 'lucide-react';

interface Book {
  id: number;
  title: string;
  authorName: string;
  price: number;
  coverImageUrl?: string;
  stockQuantity: number;
  averageRating?: number;
  reviewCount?: number;
}

interface BookCardProps {
  book: Book;
  onAddToCart?: (bookId: number) => void;
}

const BookCard: React.FC<BookCardProps> = ({ book, onAddToCart }) => {
  return (
    <Card className="h-full flex flex-col hover:shadow-lg transition-shadow">
      <CardHeader className="pb-3">
        <div className="aspect-[3/4] relative overflow-hidden rounded-md">
          <img
            src={book.coverImageUrl || '/placeholder-book.jpg'}
            alt={book.title}
            className="object-cover w-full h-full"
          />
          {book.stockQuantity === 0 && (
            <Badge variant="destructive" className="absolute top-2 right-2">
              Out of Stock
            </Badge>
          )}
        </div>
      </CardHeader>
      
      <CardContent className="flex-grow">
        <CardTitle className="text-lg font-semibold line-clamp-2 mb-2">
          {book.title}
        </CardTitle>
        <p className="text-muted-foreground text-sm mb-2">
          by {book.authorName}
        </p>
        
        {book.averageRating && (
          <div className="flex items-center gap-1 mb-2">
            <Star className="h-4 w-4 fill-yellow-400 text-yellow-400" />
            <span className="text-sm text-muted-foreground">
              {book.averageRating.toFixed(1)} ({book.reviewCount} reviews)
            </span>
          </div>
        )}
        
        <p className="text-2xl font-bold text-primary">
          ${book.price.toFixed(2)}
        </p>
      </CardContent>
      
      <CardFooter className="pt-3">
        <div className="flex gap-2 w-full">
          <Button 
            variant="outline" 
            size="sm" 
            className="flex-1"
            asChild
          >
            <Link to={`/books/${book.id}`}>
              <BookOpen className="h-4 w-4 mr-1" />
              Details
            </Link>
          </Button>
          
          <Button 
            variant="default" 
            size="sm"
            onClick={() => onAddToCart?.(book.id)}
            disabled={book.stockQuantity === 0}
            className="flex-1"
          >
            <ShoppingCart className="h-4 w-4 mr-1" />
            {book.stockQuantity > 0 ? 'Add to Cart' : 'Out of Stock'}
          </Button>
        </div>
      </CardFooter>
    </Card>
  );
};

export default BookCard;
```

### **Create components/ui/FormTemplate.tsx with shadcn/ui:**
```tsx
import React from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import * as z from 'zod';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Textarea } from '@/components/ui/textarea';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Form, FormControl, FormDescription, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';

// Form validation schema
const formSchema = z.object({
  name: z.string().min(2, 'Name must be at least 2 characters'),
  email: z.string().email('Invalid email address'),
  message: z.string().min(10, 'Message must be at least 10 characters'),
});

type FormData = z.infer<typeof formSchema>;

interface FormTemplateProps {
  onSubmit: (data: FormData) => void;
  title?: string;
  description?: string;
}

const FormTemplate: React.FC<FormTemplateProps> = ({ 
  onSubmit, 
  title = 'Contact Form',
  description = 'Send us a message and we\'ll get back to you.'
}) => {
  const form = useForm<FormData>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      name: '',
      email: '',
      message: '',
    },
  });

  const handleSubmit = async (data: FormData) => {
    try {
      await onSubmit(data);
      form.reset();
    } catch (error) {
      console.error('Form submission error:', error);
    }
  };

  return (
    <Card className="w-full max-w-md mx-auto">
      <CardHeader>
        <CardTitle>{title}</CardTitle>
        <CardDescription>{description}</CardDescription>
      </CardHeader>
      <CardContent>
        <Form {...form}>
          <form onSubmit={form.handleSubmit(handleSubmit)} className="space-y-4">
            <FormField
              control={form.control}
              name="name"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Name</FormLabel>
                  <FormControl>
                    <Input placeholder="Enter your name" {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            
            <FormField
              control={form.control}
              name="email"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Email</FormLabel>
                  <FormControl>
                    <Input placeholder="Enter your email" type="email" {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            
            <FormField
              control={form.control}
              name="message"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Message</FormLabel>
                  <FormControl>
                    <Textarea 
                      placeholder="Enter your message" 
                      className="min-h-[100px]"
                      {...field} 
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            
            <Button type="submit" className="w-full" disabled={form.formState.isSubmitting}>
              {form.formState.isSubmitting ? 'Sending...' : 'Send Message'}
            </Button>
          </form>
        </Form>
      </CardContent>
    </Card>
  );
};

export default FormTemplate;
```

---

## 🎮 Step 4: Create Layout Components

### **Create components/layout/Header.tsx with shadcn/ui:**
```tsx
import React from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../../hooks/useAuth';
import { useCart } from '../../hooks/useCart';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar';
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuLabel, DropdownMenuSeparator, DropdownMenuTrigger } from '@/components/ui/dropdown-menu';
import { ShoppingCart, User, LogOut, Settings, BookOpen, Menu } from 'lucide-react';
import { ThemeToggle } from '@/components/theme-toggle';

const Header: React.FC = () => {
  const { user, logout } = useAuth();
  const { cartItems } = useCart();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/');
  };

  return (
    <header className="border-b bg-background/95 backdrop-blur supports-[backdrop-filter]:bg-background/60">
      <div className="container mx-auto px-4 py-4">
        <div className="flex items-center justify-between">
          <Link to="/" className="flex items-center space-x-2">
            <BookOpen className="h-6 w-6 text-primary" />
            <span className="text-xl font-bold">Online Bookstore</span>
          </Link>
          
          <nav className="hidden md:flex items-center space-x-6">
            <Link to="/books" className="text-sm font-medium transition-colors hover:text-primary">
              Books
            </Link>
            <Link to="/categories" className="text-sm font-medium transition-colors hover:text-primary">
              Categories
            </Link>
            <Link to="/authors" className="text-sm font-medium transition-colors hover:text-primary">
              Authors
            </Link>
          </nav>
          
          <div className="flex items-center space-x-4">
            <ThemeToggle />
            
            <Link to="/cart" className="relative">
              <Button variant="ghost" size="icon">
                <ShoppingCart className="h-5 w-5" />
                {cartItems.length > 0 && (
                  <Badge variant="destructive" className="absolute -top-2 -right-2 h-5 w-5 rounded-full p-0 flex items-center justify-center text-xs">
                    {cartItems.length}
                  </Badge>
                )}
              </Button>
            </Link>
            
            {user ? (
              <DropdownMenu>
                <DropdownMenuTrigger asChild>
                  <Button variant="ghost" className="relative h-8 w-8 rounded-full">
                    <Avatar className="h-8 w-8">
                      <AvatarImage src={user.avatarUrl} alt={user.fullName} />
                      <AvatarFallback>{user.fullName.charAt(0)}</AvatarFallback>
                    </Avatar>
                  </Button>
                </DropdownMenuTrigger>
                <DropdownMenuContent className="w-56" align="end" forceMount>
                  <DropdownMenuLabel className="font-normal">
                    <div className="flex flex-col space-y-1">
                      <p className="text-sm font-medium leading-none">{user.fullName}</p>
                      <p className="text-xs leading-none text-muted-foreground">
                        {user.email}
                      </p>
                    </div>
                  </DropdownMenuLabel>
                  <DropdownMenuSeparator />
                  <DropdownMenuItem>
                    <User className="mr-2 h-4 w-4" />
                    <span>Profile</span>
                  </DropdownMenuItem>
                  <DropdownMenuItem>
                    <Settings className="mr-2 h-4 w-4" />
                    <span>Settings</span>
                  </DropdownMenuItem>
                  <DropdownMenuSeparator />
                  <DropdownMenuItem onClick={handleLogout}>
                    <LogOut className="mr-2 h-4 w-4" />
                    <span>Log out</span>
                  </DropdownMenuItem>
                </DropdownMenuContent>
              </DropdownMenu>
            ) : (
              <div className="flex items-center space-x-2">
                <Link to="/login">
                  <Button variant="ghost" size="sm">
                    Login
                  </Button>
                </Link>
                <Link to="/register">
                  <Button size="sm">
                    Register
                  </Button>
                </Link>
              </div>
            )}
          </div>
        </div>
      </div>
    </header>
  );
};

export default Header;
```

### **Create components/layout/Footer.tsx:**
```tsx
import React from 'react';

const Footer: React.FC = () => {
  return (
    <footer className="bg-gray-800 text-white py-8">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="grid grid-cols-1 md:grid-cols-4 gap-8">
          <div>
            <h3 className="text-lg font-semibold mb-4">Online Bookstore</h3>
            <p className="text-gray-300">
              Your one-stop destination for all your reading needs.
            </p>
          </div>
          
          <div>
            <h4 className="text-md font-semibold mb-4">Quick Links</h4>
            <ul className="space-y-2">
              <li><a href="/books" className="text-gray-300 hover:text-white">All Books</a></li>
              <li><a href="/categories" className="text-gray-300 hover:text-white">Categories</a></li>
              <li><a href="/authors" className="text-gray-300 hover:text-white">Authors</a></li>
            </ul>
          </div>
          
          <div>
            <h4 className="text-md font-semibold mb-4">Customer Service</h4>
            <ul className="space-y-2">
              <li><a href="/contact" className="text-gray-300 hover:text-white">Contact Us</a></li>
              <li><a href="/shipping" className="text-gray-300 hover:text-white">Shipping Info</a></li>
              <li><a href="/returns" className="text-gray-300 hover:text-white">Returns</a></li>
            </ul>
          </div>
          
          <div>
            <h4 className="text-md font-semibold mb-4">Follow Us</h4>
            <div className="flex space-x-4">
              <a href="#" className="text-gray-300 hover:text-white">Facebook</a>
              <a href="#" className="text-gray-300 hover:text-white">Twitter</a>
              <a href="#" className="text-gray-300 hover:text-white">Instagram</a>
            </div>
          </div>
        </div>
        
        <div className="border-t border-gray-700 mt-8 pt-8 text-center">
          <p className="text-gray-300">
            © 2024 Online Bookstore. All rights reserved.
          </p>
        </div>
      </div>
    </footer>
  );
};

export default Footer;
```

---

## 🎯 Step 5: Create Context for State Management

### **Create context/AuthContext.tsx:**
```tsx
import React, { createContext, useContext, useState, useEffect, ReactNode } from 'react';

interface User {
  id: string;
  email: string;
  fullName: string;
}

interface AuthContextType {
  user: User | null;
  login: (email: string, password: string) => Promise<void>;
  register: (email: string, password: string, fullName: string) => Promise<void>;
  logout: () => void;
  loading: boolean;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};

interface AuthProviderProps {
  children: ReactNode;
}

export const AuthProvider: React.FC<AuthProviderProps> = ({ children }) => {
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    // Check if user is logged in on app start
    const token = localStorage.getItem('token');
    if (token) {
      // Validate token and get user info
      validateToken(token);
    } else {
      setLoading(false);
    }
  }, []);

  const validateToken = async (token: string) => {
    try {
      const response = await fetch('https://localhost:7273/api/auth/me', {
        headers: {
          'Authorization': `Bearer ${token}`
        }
      });
      
      if (response.ok) {
        const userData = await response.json();
        setUser(userData);
      } else {
        localStorage.removeItem('token');
      }
    } catch (error) {
      localStorage.removeItem('token');
    } finally {
      setLoading(false);
    }
  };

  const login = async (email: string, password: string) => {
    try {
      const response = await fetch('https://localhost:7273/api/auth/login', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({ email, password })
      });

      if (response.ok) {
        const data = await response.json();
        localStorage.setItem('token', data.token);
        setUser(data.user);
      } else {
        throw new Error('Login failed');
      }
    } catch (error) {
      throw error;
    }
  };

  const register = async (email: string, password: string, fullName: string) => {
    try {
      const response = await fetch('https://localhost:7273/api/auth/register', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({ email, password, fullName })
      });

      if (response.ok) {
        const data = await response.json();
        localStorage.setItem('token', data.token);
        setUser(data.user);
      } else {
        throw new Error('Registration failed');
      }
    } catch (error) {
      throw error;
    }
  };

  const logout = () => {
    localStorage.removeItem('token');
    setUser(null);
  };

  const value = {
    user,
    login,
    register,
    logout,
    loading
  };

  return (
    <AuthContext.Provider value={value}>
      {children}
    </AuthContext.Provider>
  );
};
```

### **Create context/CartContext.tsx:**
```tsx
import React, { createContext, useContext, useState, useEffect, ReactNode } from 'react';

interface CartItem {
  id: number;
  bookId: number;
  title: string;
  price: number;
  quantity: number;
  coverImageUrl?: string;
}

interface CartContextType {
  cartItems: CartItem[];
  addToCart: (book: any) => void;
  removeFromCart: (bookId: number) => void;
  updateQuantity: (bookId: number, quantity: number) => void;
  clearCart: () => void;
  getTotalPrice: () => number;
  getTotalItems: () => number;
}

const CartContext = createContext<CartContextType | undefined>(undefined);

export const useCart = () => {
  const context = useContext(CartContext);
  if (context === undefined) {
    throw new Error('useCart must be used within a CartProvider');
  }
  return context;
};

interface CartProviderProps {
  children: ReactNode;
}

export const CartProvider: React.FC<CartProviderProps> = ({ children }) => {
  const [cartItems, setCartItems] = useState<CartItem[]>([]);

  useEffect(() => {
    // Load cart from localStorage
    const savedCart = localStorage.getItem('cart');
    if (savedCart) {
      setCartItems(JSON.parse(savedCart));
    }
  }, []);

  useEffect(() => {
    // Save cart to localStorage
    localStorage.setItem('cart', JSON.stringify(cartItems));
  }, [cartItems]);

  const addToCart = (book: any) => {
    setCartItems(prevItems => {
      const existingItem = prevItems.find(item => item.bookId === book.id);
      
      if (existingItem) {
        return prevItems.map(item =>
          item.bookId === book.id
            ? { ...item, quantity: item.quantity + 1 }
            : item
        );
      } else {
        return [...prevItems, {
          id: Date.now(),
          bookId: book.id,
          title: book.title,
          price: book.price,
          quantity: 1,
          coverImageUrl: book.coverImageUrl
        }];
      }
    });
  };

  const removeFromCart = (bookId: number) => {
    setCartItems(prevItems => prevItems.filter(item => item.bookId !== bookId));
  };

  const updateQuantity = (bookId: number, quantity: number) => {
    if (quantity <= 0) {
      removeFromCart(bookId);
    } else {
      setCartItems(prevItems =>
        prevItems.map(item =>
          item.bookId === bookId ? { ...item, quantity } : item
        )
      );
    }
  };

  const clearCart = () => {
    setCartItems([]);
  };

  const getTotalPrice = () => {
    return cartItems.reduce((total, item) => total + (item.price * item.quantity), 0);
  };

  const getTotalItems = () => {
    return cartItems.reduce((total, item) => total + item.quantity, 0);
  };

  const value = {
    cartItems,
    addToCart,
    removeFromCart,
    updateQuantity,
    clearCart,
    getTotalPrice,
    getTotalItems
  };

  return (
    <CartContext.Provider value={value}>
      {children}
    </CartContext.Provider>
  );
};
```

---

## 🎮 Step 6: Create API Service

### **Create services/api.ts:**
```tsx
const API_BASE_URL = 'https://localhost:7273/api';

class ApiService {
  private async request<T>(
    endpoint: string,
    options: RequestInit = {}
  ): Promise<T> {
    const token = localStorage.getItem('token');
    
    const config: RequestInit = {
      headers: {
        'Content-Type': 'application/json',
        ...(token && { Authorization: `Bearer ${token}` }),
        ...options.headers,
      },
      ...options,
    };

    const response = await fetch(`${API_BASE_URL}${endpoint}`, config);

    if (!response.ok) {
      throw new Error(`API request failed: ${response.statusText}`);
    }

    return response.json();
  }

  // Books
  async getBooks(): Promise<any[]> {
    return this.request('/books');
  }

  async getBook(id: number): Promise<any> {
    return this.request(`/books/${id}`);
  }

  async searchBooks(query: string): Promise<any[]> {
    return this.request(`/books/search?query=${encodeURIComponent(query)}`);
  }

  // Authors
  async getAuthors(): Promise<any[]> {
    return this.request('/authors');
  }

  async getAuthor(id: number): Promise<any> {
    return this.request(`/authors/${id}`);
  }

  // Categories
  async getCategories(): Promise<any[]> {
    return this.request('/categories');
  }

  async getCategory(id: number): Promise<any> {
    return this.request(`/categories/${id}`);
  }

  // Auth
  async login(email: string, password: string): Promise<any> {
    return this.request('/auth/login', {
      method: 'POST',
      body: JSON.stringify({ email, password }),
    });
  }

  async register(email: string, password: string, fullName: string): Promise<any> {
    return this.request('/auth/register', {
      method: 'POST',
      body: JSON.stringify({ email, password, fullName }),
    });
  }

  async getCurrentUser(): Promise<any> {
    return this.request('/auth/me');
  }
}

export const apiService = new ApiService();
```

---

## 🎮 Step 7: Create Custom Hooks

### **Create hooks/useAuth.ts:**
```tsx
import { useAuth as useAuthContext } from '../context/AuthContext';

export const useAuth = useAuthContext;
```

### **Create hooks/useCart.ts:**
```tsx
import { useCart as useCartContext } from '../context/CartContext';

export const useCart = useCartContext;
```

### **Create hooks/useApi.ts:**
```tsx
import { useState, useCallback } from 'react';
import { apiService } from '../services/api';

export const useApi = () => {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const execute = useCallback(async <T>(
    apiCall: () => Promise<T>
  ): Promise<T | null> => {
    setLoading(true);
    setError(null);

    try {
      const result = await apiCall();
      return result;
    } catch (err) {
      setError(err instanceof Error ? err.message : 'An error occurred');
      return null;
    } finally {
      setLoading(false);
    }
  }, []);

  return {
    loading,
    error,
    execute,
  };
};
```

---

## 🎨 Step 8: Add Styling with Tailwind CSS

### **Install Tailwind CSS:**
```bash
npm install -D tailwindcss postcss autoprefixer
npx tailwindcss init -p
```

### **Configure tailwind.config.js:**
```javascript
/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./src/**/*.{js,jsx,ts,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        primary: {
          50: '#eff6ff',
          500: '#3b82f6',
          600: '#2563eb',
          700: '#1d4ed8',
        }
      }
    },
  },
  plugins: [],
}
```

### **Update src/index.css:**
```css
@tailwind base;
@tailwind components;
@tailwind utilities;

body {
  margin: 0;
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', 'Roboto', 'Oxygen',
    'Ubuntu', 'Cantarell', 'Fira Sans', 'Droid Sans', 'Helvetica Neue',
    sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
}

.main-content {
  min-height: calc(100vh - 64px - 200px); /* Adjust based on header/footer height */
}
```

---

## 🧪 Step 9: Test Your Setup

1. **Start the development server:**
   ```bash
   npm start
   ```

2. **Open your browser:**
   - Navigate to `http://localhost:3000`
   - You should see the basic layout with header and footer

3. **Test the routing:**
   - Click on navigation links
   - Verify that routes work correctly

---

## 🏆 Best Practices

### **Component Design:**
- Use **functional components** with hooks
- Keep components **small and focused**
- Use **TypeScript** for type safety
- Implement **proper prop interfaces**

### **State Management:**
- Use **Context API** for global state
- Use **local state** for component-specific data
- Implement **proper error handling**
- Use **loading states** for better UX

### **API Integration:**
- Create **dedicated service classes**
- Use **custom hooks** for API calls
- Implement **proper error handling**
- Use **loading and error states**

### **Performance:**
- Use **React.memo** for expensive components
- Implement **proper key props** for lists
- Use **lazy loading** for large components
- Optimize **re-renders** with proper dependencies

---

## ✅ What You've Accomplished

- ✅ Set up React component structure
- ✅ Implemented React Router for navigation
- ✅ Created reusable UI components
- ✅ Set up Context API for state management
- ✅ Created API service for backend communication
- ✅ Implemented custom hooks
- ✅ Added Tailwind CSS for styling
- ✅ Created layout components (Header, Footer)

---

## 🚀 Next Steps

Your React frontend foundation is now ready! In the next section, we'll create the actual page components and connect them to your API endpoints.

**You've successfully set up the React frontend structure. Great job!**

---

**Next up:**
- [Section 8: Managing Author Data](./08-MANAGING-AUTHOR-DATA.md) 