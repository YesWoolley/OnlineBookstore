import { useQuery } from '@tanstack/react-query';
import { apiService } from '../services/api';

// Hook for fetching all books
export const useBooks = () => {
  // Debug logging
  console.log('useBooks hook called, apiService:', apiService);
  
  return useQuery({
    queryKey: ['books'],
    queryFn: () => {
      console.log('Fetching books from API...');
      return apiService.getAllBooks();
    },
    staleTime: 5 * 60 * 1000, // 5 minutes
    gcTime: 10 * 60 * 1000, // 10 minutes
  });
};

// Hook for fetching a single book by ID
export const useBook = (id: number) => {
  return useQuery({
    queryKey: ['book', id],
    queryFn: () => apiService.getBookById(id),
    enabled: !!id,
    staleTime: 5 * 60 * 1000, // 5 minutes
    gcTime: 10 * 60 * 1000, // 10 minutes
  });
};

// Hook for searching books
export const useBookSearch = (query: string) => {
  return useQuery({
    queryKey: ['books', 'search', query],
    queryFn: () => apiService.searchBooks(query),
    enabled: !!query.trim(),
    staleTime: 2 * 60 * 1000, // 2 minutes
    gcTime: 5 * 60 * 1000, // 5 minutes
  });
};

// Hook for fetching books by category
export const useBooksByCategory = (categoryId: number) => {
  return useQuery({
    queryKey: ['books', 'category', categoryId],
    queryFn: () => apiService.getBooksByCategory(categoryId),
    enabled: !!categoryId,
    staleTime: 5 * 60 * 1000, // 5 minutes
    gcTime: 10 * 60 * 1000, // 10 minutes
  });
};
