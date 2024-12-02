export interface Product {
  id: number;
  name: string;
  categoryName: string;
  vendor: string;
  description: string;
  price: number;
  imageUrl: string;
  quantity: number;
  status: boolean;
}
export interface Category {
  id: number;
  name: string;
  products?: Product[];
}
