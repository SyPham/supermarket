export interface Cart {
  teamId: number;
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
  teamId: number;
}
export interface DeleteCartRequest {
  accountId: number;
  productId: number;
}
