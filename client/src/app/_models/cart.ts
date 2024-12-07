export interface Cart{
  id : number;
  clientName : string;
  vendorName : string;
  totalPrice: number;
  dateCreated: string;
  cartItems: CartItem[];
}
export interface CartItem{
  productId : number;
  productName : string;
  cartId : number;
  quantity : number;
  quantityInStock : number;
  price : number;
}
