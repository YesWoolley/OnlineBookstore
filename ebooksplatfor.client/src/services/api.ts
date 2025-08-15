// API Base Configuration
import { getApiConfig } from '../config/api';

const API_BASE_URL = getApiConfig().BASE_URL;



// API Service Class
class ApiService {
  private baseUrl: string;

  constructor(baseUrl: string) {
    this.baseUrl = baseUrl;
  }

  // Generic GET request
  private async get<T>(endpoint: string): Promise<T> {
    try {
      const token = localStorage.getItem('token');
      const headers: Record<string, string> = {
        'Content-Type': 'application/json',
      };
      
      if (token) {
        headers['Authorization'] = `Bearer ${token}`;
      }

      const response = await fetch(`${this.baseUrl}${endpoint}`, {
        method: 'GET',
        headers,
      });

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      return await response.json();
    } catch (error) {
      console.error(`API GET Error (${endpoint}):`, error);
      throw error;
    }
  }

  // Generic POST request
  private async post<T>(endpoint: string, data: any): Promise<T> {
    try {
      const token = localStorage.getItem('token');
      const headers: Record<string, string> = {
        'Content-Type': 'application/json',
      };
      
      if (token) {
        headers['Authorization'] = `Bearer ${token}`;
      }

      const response = await fetch(`${this.baseUrl}${endpoint}`, {
        method: 'POST',
        headers,
        body: JSON.stringify(data),
      });

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      return await response.json();
    } catch (error) {
      console.error(`API POST Error (${endpoint}):`, error);
      throw error;
    }
  }

  // Generic PUT request
  private async put<T>(endpoint: string, data: any): Promise<T> {
    try {
      const token = localStorage.getItem('token');
      const headers: Record<string, string> = {
        'Content-Type': 'application/json',
      };
      
      if (token) {
        headers['Authorization'] = `Bearer ${token}`;
      }

      const response = await fetch(`${this.baseUrl}${endpoint}`, {
        method: 'PUT',
        headers,
        body: JSON.stringify(data),
      });

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      return await response.json();
    } catch (error) {
      console.error(`API PUT Error (${endpoint}):`, error);
      throw error;
    }
  }

  // Generic DELETE request
  private async delete<T>(endpoint: string): Promise<T> {
    try {
      const token = localStorage.getItem('token');
      const headers: Record<string, string> = {
        'Content-Type': 'application/json',
      };
      
      if (token) {
        headers['Authorization'] = `Bearer ${token}`;
      }

      const response = await fetch(`${this.baseUrl}${endpoint}`, {
        method: 'DELETE',
        headers,
      });

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      return await response.json();
    } catch (error) {
      console.error(`API DELETE Error (${endpoint}):`, error);
      throw error;
    }
  }

  // Book-related API methods
  async getAllBooks() {
    return this.get<any[]>('/books');
  }

  async getBookById(id: number) {
    return this.get<any>(`/books/${id}`);
  }

  async getBookDetail(id: number) {
    return this.get<any>(`/books/${id}/detail`);
  }

  async searchBooks(query: string) {
    return this.get<any[]>(`/books/search?q=${encodeURIComponent(query)}`);
  }

  async getBooksByCategory(categoryId: number) {
    return this.get<any[]>(`/books/category/${categoryId}`);
  }

  // Author-related API methods
  async getAllAuthors() {
    return this.get<any[]>('/authors');
  }

  async getAuthorById(id: number) {
    return this.get<any>(`/authors/${id}`);
  }

  // Category-related API methods
  async getAllCategories() {
    return this.get<any[]>('/categories');
  }

  async getCategoryById(id: number) {
    return this.get<any>(`/categories/${id}`);
  }

  // Publisher-related API methods
  async getAllPublishers() {
    return this.get<any[]>('/publishers');
  }

  async getPublisherById(id: number) {
    return this.get<any>(`/publishers/${id}`);
  }

  // Review-related API methods
  async getBookReviews(bookId: number) {
    return this.get<any[]>(`/reviews/book/${bookId}`);
  }

  async createReview(review: any) {
    return this.post<any>('/reviews', review);
  }



  async addToCart(cartItem: any) {
    return this.post<any>('/shoppingcart', cartItem);
  }

  async updateCartItem(id: number, cartItem: any) {
    return this.put<any>(`/shoppingcart/${id}`, cartItem);
  }

  async removeFromCart(id: number) {
    return this.delete<any>(`/shoppingcart/${id}`);
  }

  // Order API methods
  async createOrder(order: any) {
    return this.post<any>('/orders', order);
  }

  async getUserOrders() {
    return this.get<any[]>(`/orders/user`);
  }

  async getCurrentUserOrders() {
    return this.get<any[]>('/orders/user');
  }

  // Review API methods
  async getUserReviews() {
    return this.get<any[]>('/reviews/user/current');
  }

  async getCurrentUserReviews() {
    return this.get<any[]>('/reviews/user/current');
  }

  // Authentication API methods
  async signIn(credentials: any) {
    return this.post<any>('/auth/signin', credentials);
  }

  async signUp(userData: any) {
    return this.post<any>('/auth/signup', userData);
  }

  async getCurrentUser() {
    return this.get<any>('/auth/me');
  }
}

// Create and export the API service instance
export const apiService = new ApiService(API_BASE_URL);

// Debug logging
console.log('API Service initialized with URL:', API_BASE_URL);
console.log('API Service instance:', apiService);

// Export the service class for testing purposes
export { ApiService };
