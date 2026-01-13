// src/app/features/mall/product.model.ts

export interface Product {
  productId: number;
  productName: string;
  price: number;
  categoryName: string;
  // Base64 字串的欄位
  thumbnail: string | null;
}
