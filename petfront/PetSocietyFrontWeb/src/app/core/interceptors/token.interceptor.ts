import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse } from '@angular/common/http';
import { Observable, catchError, throwError } from 'rxjs';
import { AuthService } from '../auth/auth.service';
import { Router } from '@angular/router';

@Injectable()                            // Http 攔截器
export class AuthInterceptor implements HttpInterceptor {

  constructor(private authService: AuthService, private router: Router) { }

  // 任何Http請求被發出，都會先經過這個方法
  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    // 確認 token 是否存在 localStorage
    const token = this.authService.getToken();
    let handlerequest = request;

    // 只有在 Token 存在時才附加 Authorization 標頭
    if (token) {
      handlerequest = request.clone({
        headers: request.headers.set('Authorization', `Bearer ${token}`)
      });
    }
    // 無論是否有token，都進入pipe處理錯誤
    return next.handle(handlerequest).pipe(
      catchError((error: HttpErrorResponse) => {
        // 處理 Unauthorized
        if (error.status === 401) {
          // 清空 localStorage 並自動跳轉到登入畫面
          localStorage.removeItem('jwtToken');
          localStorage.removeItem('jwtTokenExpire');
          this.router.navigate(['/member'])
        }
        return throwError(() => error);
      }));
  }
}

// let message = '系統發生錯誤，請稍後再試';

//   // 有後端 message → 用後端的
//   if (error.error?.message) {
//     message = error.error.message;
//   }

//   // 或用 status 決定
//   else {
//     switch (error.status) {
//       case 400:
//         message = '請求資料有誤';
//         break;
//       case 401:
//         message = '請先登入';
//         break;
//       case 403:
//         message = '權限不足';
//         break;
//       case 409:
//         message = '目前狀態無法執行此操作';
//         break;
//       case 500:
//         message = '系統發生錯誤，請稍後再試';
//         break;
//     }
