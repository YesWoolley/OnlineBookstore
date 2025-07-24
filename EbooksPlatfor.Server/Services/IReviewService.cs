using OnlineBookstore.DTOs;

namespace OnlineBookstore.Services
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewDto>> GetAllReviewsAsync();
        Task<IEnumerable<ReviewDto>> GetReviewsByBookAsync(int bookId);
        Task<IEnumerable<ReviewDto>> GetReviewsByUserAsync(string userId);
        Task<ReviewDto?> GetReviewByIdAsync(int id);
        Task<ReviewDto> CreateReviewAsync(string userId, CreateReviewDto createReviewDto);
        Task<ReviewDto> UpdateReviewAsync(int id, string userId, UpdateReviewDto updateReviewDto);
        Task<bool> DeleteReviewAsync(int id, string userId);
        Task<double> GetAverageRatingAsync(int bookId);
        Task<int> GetReviewCountAsync(int bookId);
        Task<bool> HasUserReviewedBookAsync(string userId, int bookId);
    }
}