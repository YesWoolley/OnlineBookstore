export interface Book {
  id: number;
  title: string;
  description?: string;
  price: number;
  stockQuantity: number;
  coverImageUrl?: string;
  authorName: string;
  publisherName: string;
  categoryName: string;
  reviewCount: number;
  averageRating: number;
}

export interface CartItem {
  id: string;
  book: Book;
  quantity: number;
  price: number;
}
