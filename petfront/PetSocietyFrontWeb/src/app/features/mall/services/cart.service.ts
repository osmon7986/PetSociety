import { Product } from './../models/product.model';
import { Injectable } from '@angular/core';
import { HttpClient, HttpHandler, HttpHeaders } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CartService {

  private apiUrl = 'https://localhost:7138/api/Cart';

  // ★ 建立計數器 (預設是 0)
  private cartCountSubject = new BehaviorSubject<number>(0);
  cartCount$ = this.cartCountSubject.asObservable(); // 公開頻道

  constructor(private http: HttpClient) { }

  // ★ 加入購物車 (需要 token)
  addToCart(productId: number, quantity: number = 1): Observable<any> {

    // 1. 從口袋拿出 Token
    const token = localStorage.getItem('jwtToken');

    // 2. 製作識別證 (Header)
    // ★ 修正：Bearer 後面一定要加一個空格！
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    // 3. 準備包裹 (要傳的資料)
    const body = {
      productId: productId, // 這裡的名稱要對應後端 DTO 的屬性
      quantity: quantity
    }

    // 4. 發送請求 (記得帶 headers)
    return this.http.post(`${this.apiUrl}/add`, body, { headers });
  }
  // ★ 去後端抓最新的數量
  getCartCount(): void {
    const token = localStorage.getItem('jwtToken');
    if (!token) return;

    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    this.http.get<any>(`${this.apiUrl}/count`, { headers }).subscribe({
      next: (res) => {
        // 拿到最新的數量 (res.count)，更新廣播電台
        console.log('已從後端同步購物車數量:', res.count);
        this.cartCountSubject.next(res.count);
      },
      error: (err) => console.error('無法取得購物車數量', err)
    });
  }

  getCartItems(): Observable<any[]> {
    const token = localStorage.getItem('jwtToken');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.get<any[]>(`${this.apiUrl}/items`, { headers });
  }

  // ★ 讓別人可以來更新數量
  incrementCartCount(quantity: number = 1): void {
    // 取得目前的值，加上新的數量
    const currentValue = this.cartCountSubject.value;
    const newValue = currentValue + quantity;
    this.cartCountSubject.next(newValue); // 發送廣播！
  }

  // ★ 移除購物車商品
  removeCartItem(productId: number): Observable<any> {
    const token = localStorage.getItem('jwtToken');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    // 對應後端的 DELETE api/Cart/items/{productId}
    return this.http.delete(`${this.apiUrl}/items/${productId}`, { headers });
  }

  // ★ 更新購物車商品數量
  updateCartItem(productId: number, quantity: number): Observable<any> {
    const token = localStorage.getItem('jwtToken');
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`, 'Content-Type': 'application/json'// 傳的是純數字 JSON
    });

    return this.http.patch(`${this.apiUrl}/items/${productId}`, quantity, { headers });
  }

  clearCartState() {
    this.cartCountSubject.next(0);
  }
  // 新增這個方法：讓外部可以直接更新購物車數字
  updateCartCount(count: number) {
    if (count < 0) {
      console.error("數量不能是負的！");
      return; // 擋下來！
    }
    this.cartCountSubject.next(count);
  }
}
