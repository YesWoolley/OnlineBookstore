import { useQueryClient } from '@tanstack/react-query';

export const useInvalidateUserStats = () => {
  const queryClient = useQueryClient();

  const invalidateUserStats = (userId: string) => {
    // Immediately invalidate user stats to force a fresh fetch
    queryClient.invalidateQueries({ queryKey: ['userStats', userId] });
    
    // Also invalidate related queries that might affect stats
    queryClient.invalidateQueries({ queryKey: ['userOrders', userId] });
    queryClient.invalidateQueries({ queryKey: ['userReviews', userId] });
    
    console.log('User stats invalidated for user:', userId);
  };

  return { invalidateUserStats };
};
