using AutoMapper;
using EbooksPlatform.Models;
using OnlineBookstore.DTOs;
using OnlineBookstore.Models;

namespace OnlineBookstore.Mapppings
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            // Book mappings
            CreateMap<Book, BookDto>()
                .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author.Name))
                .ForMember(dest => dest.PublisherName, opt => opt.MapFrom(src => src.Publisher.Name))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.ReviewCount, opt => opt.MapFrom(src => src.Reviews != null ? src.Reviews.Count : 0))
                .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src =>
                    src.Reviews != null && src.Reviews.Any()
                        ? src.Reviews.Average(r => r.Rating)
                        : 0.0));

            CreateMap<Book, BookDetailDto>()
                .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author.Name))
                .ForMember(dest => dest.PublisherName, opt => opt.MapFrom(src => src.Publisher.Name))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.ReviewCount, opt => opt.MapFrom(src => src.Reviews != null ? src.Reviews.Count : 0))
                .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src =>
                    src.Reviews != null && src.Reviews.Any()
                        ? src.Reviews.Average(r => r.Rating)
                        : 0.0));

            CreateMap<CreateBookDto, Book>();
            CreateMap<UpdateBookDto, Book>();

            // Author mappings
            CreateMap<Author, AuthorDto>()
                .ForMember(dest => dest.BookCount, opt => opt.MapFrom(src => src.Books != null ? src.Books.Count : 0));

            CreateMap<CreateAuthorDto, Author>();
            CreateMap<UpdateAuthorDto, Author>();

            // Category mappings
            CreateMap<Category, CategoryDto>()
                .ForMember(dest => dest.BookCount, opt => opt.MapFrom(src => src.Books != null ? src.Books.Count : 0));

            CreateMap<CreateCategoryDto, Category>();
            CreateMap<UpdateCategoryDto, Category>();

            // Publisher mappings
            CreateMap<Publisher, PublisherDto>()
                .ForMember(dest => dest.BookCount, opt => opt.MapFrom(src => src.Books != null ? src.Books.Count : 0));

            CreateMap<CreatePublisherDto, Publisher>();
            CreateMap<UpdatePublisherDto, Publisher>();

            // Review mappings
            CreateMap<Review, ReviewDto>()
                .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book.Title))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FullName));

            CreateMap<CreateReviewDto, Review>();
            CreateMap<UpdateReviewDto, Review>();

            // Order mappings
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FullName));

            CreateMap<CreateOrderDto, Order>();
            CreateMap<UpdateOrderDto, Order>();

            // OrderItem mappings
            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book.Title))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.Quantity * src.UnitPrice));

            CreateMap<CreateOrderItemDto, OrderItem>();
            CreateMap<UpdateOrderItemDto, OrderItem>();

            // ShoppingCartItem mappings
            CreateMap<ShoppingCartItem, ShoppingCartItemDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book.Title))
                .ForMember(dest => dest.BookPrice, opt => opt.MapFrom(src => src.Book.Price))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.Quantity * src.Book.Price));

            CreateMap<CreateShoppingCartItemDto, ShoppingCartItem>();
            CreateMap<UpdateShoppingCartItemDto, ShoppingCartItem>();

            // User mappings
            CreateMap<ApplicationUser, UserDto>();
            CreateMap<RegisterDto, ApplicationUser>();
            CreateMap<UpdateProfileDto, ApplicationUser>();
        }
    }
}
