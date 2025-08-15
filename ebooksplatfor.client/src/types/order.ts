export interface OrderItem {
  id: string;
  bookId: number;
  bookTitle: string;
  quantity: number;
  unitPrice: number;
}

export interface Order {
  id: string;
  userId: string;
  orderDate: string;
  status: 'Pending' | 'Processing' | 'Completed' | 'Cancelled';
  totalAmount: number;
  shippingAddress: string;
  orderItems: OrderItem[];
}






