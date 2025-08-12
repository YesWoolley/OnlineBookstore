import { type Book } from '../types/book';

export const sampleBooks: Book[] = [
  {
    id: 1,
    title: "The Great Gatsby",
    description: "A story of the fabulously wealthy Jay Gatsby and his love for the beautiful Daisy Buchanan.",
    price: 12.99,
    stockQuantity: 10,
    coverImageUrl: "https://via.placeholder.com/300x400/1e40af/ffffff?text=The+Great+Gatsby",
    authorName: "F. Scott Fitzgerald",
    publisherName: "Scribner",
    categoryName: "Fiction",
    reviewCount: 1250,
    averageRating: 4.5
  },
  {
    id: 2,
    title: "To Kill a Mockingbird",
    description: "The story of young Scout Finch and her father Atticus in a racially divided Alabama town.",
    price: 11.99,
    stockQuantity: 15,
    coverImageUrl: "https://via.placeholder.com/300x400/059669/ffffff?text=To+Kill+a+Mockingbird",
    authorName: "Harper Lee",
    publisherName: "Grand Central Publishing",
    categoryName: "Fiction",
    reviewCount: 2100,
    averageRating: 4.8
  },
  {
    id: 3,
    title: "1984",
    description: "A dystopian novel about totalitarianism and surveillance society.",
    price: 10.99,
    stockQuantity: 8,
    coverImageUrl: "https://via.placeholder.com/300x400/dc2626/ffffff?text=1984",
    authorName: "George Orwell",
    publisherName: "Signet",
    categoryName: "Science Fiction",
    reviewCount: 1800,
    averageRating: 4.6
  },
  {
    id: 4,
    title: "Pride and Prejudice",
    description: "A romantic novel of manners that follows the emotional development of Elizabeth Bennet.",
    price: 9.99,
    stockQuantity: 0,
    coverImageUrl: "https://via.placeholder.com/300x400/7c3aed/ffffff?text=Pride+and+Prejudice",
    authorName: "Jane Austen",
    publisherName: "Penguin Classics",
    categoryName: "Romance",
    reviewCount: 1650,
    averageRating: 4.7
  }
];