export interface Cart {
  accountId: number;
  productId: number;
  quantity: number | null;
  createdTime: string;
}
export interface UpdateQuantityRequest {
  accountId: number;
  productId: number;
  quantity: number | null;
}
export interface AddToCartRequest {
  accountId: number;
  productId: number;
  quantity: number | null;
}
export interface DeleteCartRequest {
  accountId: number;
  productId: number;
}
