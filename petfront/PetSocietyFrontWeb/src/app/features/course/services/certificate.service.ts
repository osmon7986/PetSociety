import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class CertificateService {
  private baseUrl = 'https://localhost:7138';
  constructor(private httpClient: HttpClient) { }

  getCertificatePdf(courseDetailId: number) {
    return this.httpClient.get(`${this.baseUrl}/Certificate/${courseDetailId}/pdf`, { responseType: 'blob' })
  }
}
