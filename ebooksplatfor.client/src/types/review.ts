export interface Review {
  id: number;
  bookId: number;
  bookTitle: string;
  userName: string;
  rating: number;
  comment?: string;
  createdAt: string;
}

export interface CreateReview {
  bookId: number;
  rating: number;
  comment?: string;
}

export interface UpdateReview {
  rating: number;
  comment?: string;
}






