
import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';

interface LoginDTO {
  username: string;
  password: string;
}

interface TokenDTO {
  token: string;
  expireIn: number;
  tokenType: string;
  role?: number;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private authApiUrl = 'https://localhost:7138/api/auth';
  private memberApiUrl = 'https://localhost:7138/api/members';

  // 用 Signal 管理登入狀態
  private _isLogin = signal(!!localStorage.getItem('jwtToken'));
  readonly isLogin = this._isLogin.asReadonly(); // 封裝登入狀態，避免外部組件改動狀態

  constructor(private http: HttpClient) { }

  login(loginDto: LoginDTO) {
    return this.http.post<TokenDTO>(`${this.authApiUrl}/login`, loginDto).pipe(
      tap(response => {
        this.setToken(response.token, response.expireIn);
        this._isLogin.set(true); // 登入後 signal設定true
      })
    );
  }
  googleLogin(idToken: string) {
    return this.http.post<TokenDTO>(`${this.authApiUrl}/google-login`, { idToken }).pipe(
      tap(response => {
        const expire = response.expireIn || 3600;
        this.setToken(response.token, expire);
      })
    );
  }

  // 3. 取得管理員 SSO Token
  getAdminSsoToken(): Observable<any> {
    return this.http.get(`${this.authApiUrl}/sso-token`);
  }

  getMemberInfo(): Observable<any> {
    return this.http.get<any>(`${this.memberApiUrl}/profile`);
  }


  private setToken(token: string, expireIn: number) {
    localStorage.setItem('jwtToken', token);
    const expireAt = Date.now() + expireIn * 1000;
    localStorage.setItem('jwtTokenExpire', expireAt.toString());
  }

  getToken(): string | null {
    return localStorage.getItem('jwtToken');
  }

  getTokenExpire(): number | null {
    const exp = localStorage.getItem('jwtTokenExpire');
    return exp ? Number(exp) : null;
  }

  isLoggedIn(): boolean {
    const expire = this.getTokenExpire()
    // 沒有token or tokenExpire 回傳 false
    if (!this.getToken() || !expire) {
      console.log('localStorage沒有token');
      return false;

    }
    // 現在時間大於 token 過期時間回傳 false
    if (Date.now() > Number(expire)) {
      console.log('localStorage token過期');
      return false;

    }
    return true;
  }

  logout(): void {
    localStorage.removeItem('jwtToken');
    localStorage.removeItem('jwtTokenExpire');
    localStorage.removeItem('user');
    this._isLogin.set(false);
  }
}
