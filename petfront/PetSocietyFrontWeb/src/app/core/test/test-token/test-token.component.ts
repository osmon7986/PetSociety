import { Component } from '@angular/core';
import { AuthService } from '../../auth/auth.service';
import { catchError, of } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-test-token',
  imports: [FormsModule, CommonModule],
  templateUrl: './test-token.component.html',
  styleUrl: './test-token.component.css'
})
export class TestTokenComponent {
  // 預設測試值，方便快速測試
  username = 'ava@gmail.com';
  password = '123456';

  statusMessage: string = '';
  memberInfo: any = null;

  constructor(private authService: AuthService) { }

  /**
   * 登入處理
   */
  handleLogin() {
    this.statusMessage = '登入中...';
    this.memberInfo = null;

    this.authService.login({ username: this.username, password: this.password }).pipe(
      // 捕獲並處理錯誤
      catchError(err => {
        this.statusMessage = `登入失敗: ${err.status === 401 ? '帳號或密碼錯誤' : '伺服器錯誤'}`;
        return of(null); // 返回一個空的 Observable
      })
    ).subscribe(response => { // 訂閱才觸發API呼叫，response拿到TokenDTO
      if (response) {
        this.statusMessage = `登入成功! JWT 已儲存。`;
        // 登入成功後立即測試受保護端點
        this.handleGetInfo();
      }
    });
  }

  /**
   * 測試受保護端點處理
   */
  handleGetInfo(): void {
    if (!this.authService.isLoggedIn()) {
      this.statusMessage = '請先登入才能測試受保護端點。';
      return;
    }

    this.memberInfo = { Message: '取得資訊中...' };

    this.authService.getMemberInfo().pipe(
      // 捕獲並處理錯誤 (例如 401/403)
      catchError(err => {
        if (err.status === 401 || err.status === 403) {
          this.statusMessage = '取得資訊失敗: JWT 無效或已過期，請重新登入。';
          this.authService.logout(); // 清除無效 Token
        } else {
          this.statusMessage = `取得資訊失敗: 伺服器錯誤 (${err.status})`;
        }
        this.memberInfo = null;
        return of(null);
      })
    ).subscribe(info => {
      if (info) {
        this.statusMessage = '成功取得會員資訊 (Token 驗證通過)';
        this.memberInfo = info;
      }
    });
  }

  /**
   * 登出處理
   */
  handleLogout(): void {
    this.authService.logout();
    this.statusMessage = '您已登出。';
    this.memberInfo = null;
  }

  // 輔助方法
  isLoggedIn(): boolean {
    return this.authService.isLoggedIn();
  }
}

