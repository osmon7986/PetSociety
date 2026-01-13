import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Product } from '../models/product.model';

// 新增：定義後端回傳的新格式 (包裹)
export interface ProductResponse {
  totalCount: number;
  items: Product[];
}

@Injectable({
  providedIn: 'root'
})
export class MallService {

  private baseUrl = 'https://localhost:7138/api'
  constructor(private http: HttpClient) { }

  // 修改：接收 page 和 pageSize，並回傳 ProductResponse
  getProducts(page: number = 1,
    pageSize: number = 15,
    category: string = '全部商品',
    minPrice?: number,
    maxPrice?: number,
    keyword?: string): Observable<ProductResponse> {

    // 透過網址傳參數，例如：/api/products?page=1&pageSize=15&category=保健護理
    // 使用 encodeURIComponent 是為了防止中文亂碼
    let url = `${this.baseUrl}/products?page=${page}&pageSize=${pageSize}&category=${encodeURIComponent(category)}`;

    // 如果有設定價格，就串接上去
    if (minPrice != null) url += `&minPrice=${minPrice}`;
    if (maxPrice != null) url += `&maxPrice=${maxPrice}`;

    // 如果有設定關鍵字，也串接上去 (記得用 encodeURIComponent 防止中文亂碼)
    if (keyword) url += `&keyword=${encodeURIComponent(keyword)}`;

    return this.http.get<ProductResponse>(url);
  }
  // 取得補救付款的資料
  getPaymentInfo(orderId: number): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/orders/payment-info/${orderId}`);
  }
  // 取消訂單 (會呼叫後端同時恢復庫存)
  cancelOrder(orderId: number): Observable<any> {
    return this.http.put(`${this.baseUrl}/orders/${orderId}/cancel`, {});
  }
  // 軟刪除訂單
  deleteOrder(orderId: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/orders/${orderId}`);
  }
}


