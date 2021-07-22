export interface OrderDetail {
  id: number;
  orderId: number;
  productId: number;
  quantity: number | null;
  price: number | null;
}
