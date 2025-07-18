using OnlineBookstore.DTOs;

namespace OnlineBookstore.Services
{
    public interface IPublisherService
    {
        Task<IEnumerable<PublisherDto>> GetAllPublishersAsync();
        Task<PublisherDto?> GetPublisherByIdAsync(int id);
        Task<PublisherDto> CreatePublisherAsync(CreatePublisherDto createPublisherDto);
        Task<PublisherDto> UpdatePublisherAsync(int id, UpdatePublisherDto updatePublisherDto);
        Task<bool> DeletePublisherAsync(int id);
        Task<IEnumerable<PublisherDto>> SearchPublishersAsync(string searchTerm);
    }
}
