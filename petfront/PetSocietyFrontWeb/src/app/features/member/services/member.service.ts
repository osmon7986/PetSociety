import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, Subject } from 'rxjs';
import { LoginDto, MemberDto, RegisterDto, ChangePasswordDto } from '../Interfaces/member.dto';
import { FavoriteItemDto } from '../Interfaces/favorite.dto';

export interface TokenResponse {
  token: string;
  expireIn: number;
  tokenType: string;
}

@Injectable({ providedIn: 'root' })
export class MemberService {
  private baseUrl = 'https://localhost:7138/api/members';
  private authUrl = 'https://localhost:7138/api/auth';
  private avatarApiUrl = 'https://localhost:7138/api/avatar';
  private favoriteApiUrl = 'https://localhost:7138/api/favorites';
  private refreshProfile$ = new Subject<void>();

  constructor(private http: HttpClient) { }

  get onRefreshProfile() {
    return this.refreshProfile$.asObservable();
  }

  notifyProfileUpdated() {
    this.refreshProfile$.next();
  }

  // 1. 註冊
  register(data: RegisterDto): Observable<TokenResponse> {
    return this.http.post<TokenResponse>(`${this.authUrl}/register`, data);
  }

  // 2. 上傳頭像
  uploadAvatar(file: File): Observable<any> {
    const formData = new FormData();
    formData.append('file', file);

    return this.http.post(`${this.avatarApiUrl}`, formData);
  }

  // 3. 取得頭像
  getAvatar(): Observable<Blob> {
    return this.http.get(`${this.avatarApiUrl}`, {
      responseType: 'blob'
    });
  }

  // 4. 取得個人資料
  getProfile(): Observable<MemberDto> {
    return this.http.get<MemberDto>(`${this.baseUrl}/profile`);
  }

  // 5. 修改個人資料
  // 對應後端: [HttpPut] api/members/{id}
  updateProfile(memberId: number, data: any): Observable<any> {
    return this.http.put(`${this.baseUrl}/${memberId}`, data);
  }

  // 6. 修改密碼
  changePassword(data: ChangePasswordDto): Observable<any> {
    return this.http.put(`${this.baseUrl}/password`, data);
  }

  // 7. 忘記密碼
  forgotPassword(email: string): Observable<any> {
    return this.http.post(`${this.authUrl}/forgot-password`, { email });
  }

  // 8. 重設密碼
  resetPassword(data: any): Observable<any> {
    return this.http.post(`${this.authUrl}/reset-password`, data);
  }

  // 9. 驗證帳號
  verifyAccount(data: any): Observable<any> {
    return this.http.post(`${this.authUrl}/verify`, data);
  }

  // 10. 取得收藏列表
  getFavorites(type: number): Observable<FavoriteItemDto[]> {
    return this.http.get<FavoriteItemDto[]>(`${this.favoriteApiUrl}?targetType=${type}`);
  }

  // 11. 新增收藏
  addFavorite(targetId: number, typeId: number, title: string, price: number, imageUrl: string): Observable<any> {
    const body = {
      TargetId: targetId,
      TargetType: typeId,
      Title: title,
      Price: price,
      ImageUrl: imageUrl
    };
    return this.http.post(`${this.favoriteApiUrl}`, body);
  }

  // 12. 移除收藏
  removeFavorite(favoriteId: number): Observable<any> {
    return this.http.delete(`${this.favoriteApiUrl}/${favoriteId}`);
  }

  // 13. 根據「商品ID」移除收藏 (只知道 ProductId，不知道 FavoriteId)
  removeFavoriteByTarget(targetId: number, typeId: number): Observable<any> {
    return this.http.delete(`${this.favoriteApiUrl}/delete-by-target`, {
      params: {
        targetId: targetId.toString(),
        targetType: typeId.toString()
      }
    });
  }

}
