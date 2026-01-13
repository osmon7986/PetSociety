import { Injectable } from "@angular/core";
import { CanActivate, Router } from "@angular/router";
import { AuthService } from "../auth/auth.service";

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(private authService: AuthService, private router: Router) { }
  // 使用者嘗試導航到受保護的路由時，會被框架自動呼叫
  canActivate(): boolean {
    if (!this.authService.isLoggedIn()) {
      this.router.navigate(['member/login']);
      return false;
    }
    return true;
  }
}
