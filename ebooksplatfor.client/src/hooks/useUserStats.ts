import { useQuery, useQueryClient } from '@tanstack/react-query';
import { apiService } from '../services/api';

interface UserStats {
  orderCount: number;
  reviewCount: number;
  booksPurchased: number;
}

export const useUserStats = (userId: string) => {
  const queryClient = useQueryClient();
  
  const { data: stats, isLoading, error, refetch } = useQuery({
    queryKey: ['userStats', userId],
    queryFn: async (): Promise<UserStats> => {
      try {
        // Fetch real user data from database
        const [orders, reviews] = await Promise.all([
          apiService.getCurrentUserOrders(),
          apiService.getCurrentUserReviews()
        ]);
        
        // Calculate real counts from database data
        const orderCount = orders.length;
        const reviewCount = reviews.length;
        
        // Calculate books purchased from actual order items
        const booksPurchased = orders.reduce((total, order) => {
          if (order.orderItems && Array.isArray(order.orderItems)) {
            return total + order.orderItems.length;
          }
          return total;
        }, 0);
        
        console.log('Real user stats from database:', {
          orderCount,
          reviewCount,
          booksPurchased,
          orders: orders.length,
          reviews: reviews.length
        });
        
        return {
          orderCount,
          reviewCount,
          booksPurchased
        };
      } catch (error) {
        console.error('Error fetching real user stats from database:', error);
        return {
          orderCount: 0,
          reviewCount: 0,
          booksPurchased: 0
        };
      }
    },
    enabled: !!userId,
    staleTime: 0, // Always consider data stale - fetch fresh data every time
    gcTime: 1 * 60 * 1000, // Keep in cache for 1 minute
    refetchOnWindowFocus: true, // Refetch when user returns to tab
    refetchOnMount: true, // Refetch when component mounts
  });

  // Function to immediately invalidate and refetch stats
  const invalidateAndRefetch = () => {
    queryClient.invalidateQueries({ queryKey: ['userStats', userId] });
    refetch();
  };

  return {
    stats: stats || { orderCount: 0, reviewCount: 0, booksPurchased: 0 },
    isLoading,
    error,
    refetch,
    invalidateAndRefetch
  };
};
