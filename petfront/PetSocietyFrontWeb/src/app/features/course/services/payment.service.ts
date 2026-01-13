import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class PaymentService {
  private httpClient = inject(HttpClient);
  private baseUrl = 'https://localhost:7138/CoursePayment';
  constructor() { }

  getEcpayParameters(courseDetailId: number, returnUrl: string) {
    return this.httpClient.post(`${this.baseUrl}/checkout`, {
      courseDetailId: courseDetailId,
      clientBackUrl: returnUrl
    })
  }
}
