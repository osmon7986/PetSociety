import { Component, OnInit, NgZone, ViewChild, ElementRef, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { MemberService } from '../../services/member.service';
import { LoginDto, RegisterDto } from '../../Interfaces/member.dto';
import { AuthService } from '../../../../core/auth/auth.service';
import { ToastService } from '../../services/toast.service';
import { CartService } from '../../../mall/services/cart.service';

declare var google: any;

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit, AfterViewInit {

  loginData: LoginDto = {
    email: '',
    password: ''
  };

  registerData: RegisterDto = {
    name: '',
    email: '',
    password: '',
    confirmPassword: ''
  };

  captchaCode: string = '';
  userCaptchaInput: string = '';
  @ViewChild('captchaCanvas') captchaCanvas!: ElementRef<HTMLCanvasElement>;

  constructor(
    private authService: AuthService,
    private memberService: MemberService,
    private router: Router,
    private route: ActivatedRoute,
    private toast: ToastService,
    private cartService: CartService,
    private ngZone: NgZone
  ) { }

  ngOnInit(): void {
    // 強制清除前端登入狀態
    this.authService.logout();

    // 初始化 Google 登入
    this.initGoogleAuth();

    // 處理註冊切換的邏輯
    this.route.queryParams.subscribe(params => {
      if (params['tab'] === 'register') {
        setTimeout(() => {
          const registerBtn = document.getElementById('nav-register-tab');
          if (registerBtn) registerBtn.click();
        }, 100);
      }
    });
  }
  // 1. 快速填入測試會員
  fillTestUser() {
    this.loginData.email = 'norah@gmail.com'; // 測試帳號
    this.loginData.password = 'norah202526';        // 測試密碼
    this.userCaptchaInput = this.captchaCode;  // 自動填入正確的驗證碼
  }
  // 2. 快速填入測試會員
  fillDemoUser() {
    this.loginData.email = 'petsociety.usertest@gmail.com'; // 測試帳號
    this.loginData.password = 'test123456!';        // 測試密碼
    this.userCaptchaInput = this.captchaCode;  // 自動填入正確的驗證碼
  }
  // 3. 快速填入管理員
  fillAdminUser() {
    this.loginData.email = 'p2025@gmail.com'; // 管理員帳號
    this.loginData.password = '123456';       // 管理員密碼
    this.userCaptchaInput = this.captchaCode;   // 自動填入正確驗證碼
  }
  // 3. 快速填入註冊會員
  fillDemoRegisterUser() {
    this.registerData.name = 'Petsociety';
    this.registerData.email = 'petsociety.usertest@gmail.com';
    this.registerData.password = 'test123456!';
    this.registerData.confirmPassword = 'test123456!';
  }
  ngAfterViewInit(): void {
    this.generateCaptcha();
  }

  // 產生圖形驗證碼邏輯
  generateCaptcha() {
    if (!this.captchaCanvas) return;

    const canvas = this.captchaCanvas.nativeElement;
    const ctx = canvas.getContext('2d');
    if (!ctx) return;

    ctx.clearRect(0, 0, canvas.width, canvas.height);
    ctx.fillStyle = '#fcf9f5';
    ctx.fillRect(0, 0, canvas.width, canvas.height);

    // 生成隨機文字 (4碼，去除易混淆字元)
    const chars = 'ABCDEFGHJKLMNPQRSTUVWXYZ23456789';
    let code = '';
    for (let i = 0; i < 4; i++) {
      code += chars.charAt(Math.floor(Math.random() * chars.length));
    }
    this.captchaCode = code;

    // 畫干擾線 (隨機顏色)
    for (let i = 0; i < 6; i++) {
      ctx.strokeStyle = this.getRandomColor(180, 230);
      ctx.beginPath();
      ctx.moveTo(Math.random() * canvas.width, Math.random() * canvas.height);
      ctx.lineTo(Math.random() * canvas.width, Math.random() * canvas.height);
      ctx.stroke();
    }

    // 畫文字 (加入旋轉與位移)
    ctx.font = 'bold 26px Arial';
    ctx.textBaseline = 'middle';

    for (let i = 0; i < code.length; i++) {
      ctx.fillStyle = this.getRandomColor(50, 120);
      ctx.save();
      const x = 20 + i * 25;
      const y = 30 + (Math.random() - 0.5) * 8;
      const angle = (Math.random() - 0.5) * 0.4;

      ctx.translate(x, y);
      ctx.rotate(angle);
      ctx.fillText(code[i], 0, 0);
      ctx.restore();
    }
  }

  getRandomColor(min: number, max: number) {
    const r = Math.floor(Math.random() * (max - min) + min);
    const g = Math.floor(Math.random() * (max - min) + min);
    const b = Math.floor(Math.random() * (max - min) + min);
    return `rgb(${r},${g},${b})`;
  }

  initGoogleAuth() {
    if (typeof google !== 'undefined') {
      google.accounts.id.initialize({
        client_id: '972290926876-vr878fu52s36ln8pkuu0glp0c4toholf.apps.googleusercontent.com',
        callback: (response: any) => this.handleGoogleCredential(response)
      });

      const btnContainer = document.getElementById('google-btn');
      if (btnContainer) {
        google.accounts.id.renderButton(
          btnContainer,
          { theme: 'outline', size: 'large', width: 250, text: 'signin_with' }
        );
      }
    } else {
      setTimeout(() => this.initGoogleAuth(), 500);
    }
  }

  handleGoogleCredential(response: any) {
    this.ngZone.run(() => {
      const googleToken = response.credential;

      this.authService.googleLogin(googleToken).subscribe({
        next: (res: any) => {
          this.toast.success('登入成功！');
          this.cartService.getCartCount();
          if (res.role === 1) {
            // 管理員邏輯...
          } else {
            this.router.navigate(['/member/profile'])
              .then(success => console.log("5. 跳轉結果:", success ? "成功" : "失敗"))
              .catch(err => console.error("5. 跳轉發生錯誤:", err));
          }
        },
        error: (err) => {
          console.error("後端回傳錯誤:", err);
          this.toast.error('登入失敗');
        }
      });
    });
  }

  onLogin() {
    // 檢查驗證碼 (轉大寫比對，不分大小寫)
    if (!this.userCaptchaInput || this.userCaptchaInput.toUpperCase() !== this.captchaCode) {
      this.toast.warning('驗證碼錯誤，請重新輸入');
      this.generateCaptcha();
      this.userCaptchaInput = '';
      return; //
    }

    if (!this.loginData.email || !this.loginData.password) {
      this.toast.warning('請輸入帳號與密碼');
      return;
    }

    const payload = {
      username: this.loginData.email,
      password: this.loginData.password
    };

    this.authService.login(payload).subscribe({
      next: (res: any) => {
        this.toast.success('登入成功！');
        this.cartService.getCartCount();

        if (res.role === 1) {
          this.toast.info('管理員身分確認，前往後台中...');
          this.authService.getAdminSsoToken().subscribe({
            next: (tokenRes: any) => {
              const mvcUrl = `https://localhost:7032/Home/SsoLogin?token=${tokenRes.token}`;
              window.location.href = mvcUrl;
            },
            error: (err) => {
              console.error('SSO Token 取得失敗', err);
              this.toast.error('無法進入後台，請重新登入');
            }
          });
        } else {
          this.router.navigate(['/member/profile']);
        }
      },
      error: (err) => {
        console.error(err);
        const msg = err.error?.message || '登入失敗';
        this.toast.error(msg);
        this.generateCaptcha();
        this.userCaptchaInput = '';
      }
    });
  }

  onRegister() {
    if (this.registerData.password !== this.registerData.confirmPassword) {
      this.toast.warning('兩次輸入的密碼不一致！');
      return;
    }

    this.memberService.register(this.registerData).subscribe({
      next: (res: any) => {
        this.toast.success('註冊成功！請至信箱收取驗證碼');
        this.router.navigate(['/member/verify-email'], {
          queryParams: { email: this.registerData.email }
        });
      },
      error: (err) => {
        console.error(err);
        const msg = err.error?.message || err.error || '註冊失敗，請稍後再試';
        this.toast.error(msg);
      }
    });
  }
}
