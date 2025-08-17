// API Configuration
export const API_CONFIG = {
  // Base URL for API calls - can be overridden by environment variable
  BASE_URL: 'https://onlinebookstore-backend-f4ejgsdudbghhkfz.australiaeast-01.azurewebsites.net/api',
  
  // Timeout for API requests (in milliseconds)
  TIMEOUT: 10000,
  
  // Retry configuration
  RETRY_ATTEMPTS: 3,
  RETRY_DELAY: 1000,
  
  // Headers
  DEFAULT_HEADERS: {
    'Content-Type': 'application/json',
  },
  
  // Endpoints
  ENDPOINTS: {
    BOOKS: '/books',
    AUTHORS: '/authors',
    CATEGORIES: '/categories',
    PUBLISHERS: '/publishers',
    REVIEWS: '/reviews',
    SHOPPING_CART: '/shoppingcart',
    ORDERS: '/orders',
    AUTH: '/auth',
    USERS: '/users',
  },
} as const;

// Environment-specific configurations
export const getApiConfig = () => {
  return {
    ...API_CONFIG,
    BASE_URL: API_CONFIG.BASE_URL,
  };
};
