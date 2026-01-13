import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class OrderService {

  private apiUrl = 'https://localhost:7138/api/Orders';

  // 👇 HttpClient 是要在這裡注入的！
  constructor(private http: HttpClient) { }

  /**
   * 建立訂單
   * @param orderData 包含商品明細和收件人資訊的物件
   */
  createOrder(orderData: any): Observable<any> {
    // 這行就是負責打電話給後端的 CreateOrder API
    return this.http.post<any>(this.apiUrl, orderData);
  }
  getMyOrders(): Observable<any[]> {
    const token = localStorage.getItem('jwtToken');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.get<any[]>(`${this.apiUrl}/my-orders`, { headers });
  }
}
