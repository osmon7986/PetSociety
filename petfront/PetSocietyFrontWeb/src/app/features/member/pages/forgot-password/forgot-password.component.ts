import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { MemberService } from '../../services/member.service';
import { ToastService } from '../../services/toast.service';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  template: `
    <div class="container mt-5" style="max-width: 500px;">
      <div class="card shadow-sm border-0">
        <div class="card-body p-4">
          <h3 class="text-center mb-4">忘記密碼</h3>

          @if (step === 1) {
            <form (ngSubmit)="onSendCode()">
              <div class="mb-3">
                <label class="form-label">請輸入註冊的 Email</label>
                <input type="email" class="form-control" [(ngModel)]="email" name="email"
                       required placeholder="name@example.com"
                       autocapitalize="off" autocorrect="off" spellcheck="false" style="text-transform: none !important;">
              </div>
              <div class="d-flex justify-content-end mb-3">
                <button type="button" class="btn btn-outline-secondary btn-sm" (click)="fillDemo()">
                  <i class="bi bi-magic me-1"></i>Demo
                </button>
              </div>
              <button type="submit" class="btn btn-primary w-100" [disabled]="!email">發送驗證碼</button>
            </form>
          }

          @if (step === 2) {
            <form (ngSubmit)="onResetPassword()">
              <div class="mb-3">
                <label class="form-label">驗證碼</label>
                <input type="text" class="form-control" [(ngModel)]="resetData.token" name="token" required>
              </div>
              <div class="mb-3">
                <label class="form-label">新密碼</label>
                <input type="password" class="form-control" [(ngModel)]="resetData.newPassword" name="newPassword" required minlength="6">
              </div>
              <div class="mb-3">
                <label class="form-label">確認新密碼</label>
                <input type="password" class="form-control" [(ngModel)]="resetData.confirmPassword" name="confirmPassword" required>
              </div>
              <div class="d-flex justify-content-end mb-3">
                <button type="button" class="btn btn-outline-secondary btn-sm" (click)="fillDemo()">
                  <i class="bi bi-key-fill me-1"></i>Demo
                </button>
              </div>
              <button type="submit" class="btn btn-primary w-100">重設密碼</button>
            </form>
          }

          <div class="text-center mt-3">
            <a routerLink="/member/login" class="text-decoration-none">返回登入</a>
          </div>
        </div>
      </div>
    </div>
  `
})
export class ForgotPasswordComponent {
  step: number = 1;
  email: string = '';
  simulationCode: string = '';

  resetData = {
    token: '',
    newPassword: '',
    confirmPassword: ''
  };

  constructor(
    private memberService: MemberService,
    private router: Router,
    private toast: ToastService
  ) { }
  fillDemo() {
    if (this.step === 1) {
      this.email = 'petsociety.usertest@gmail.com';
    } else if (this.step === 2) {
      this.resetData.newPassword = 'test123456!';
      this.resetData.confirmPassword = 'test123456!';
    }
  }
  onSendCode() {
    const cleanEmail = this.email.trim().toLowerCase();

    // 呼叫後端寄信
    this.memberService.forgotPassword(cleanEmail).subscribe({
      next: (res: any) => {
        // ✅ 成功寄信後，只顯示提示，並切換到 Step 2
        this.toast.success('驗證碼已發送至您的信箱！');
        this.step = 2; // 切換到輸入驗證碼的畫面
      },
      error: (err) => {
        console.error(err);
        // 顯示後端回傳的錯誤訊息 (例如: 寄信失敗、找不到 Email)
        // 注意：err.error 可能是 string 也可能是 object，視後端回傳格式而定
        const msg = typeof err.error === 'string' ? err.error : (err.error?.message || '發送失敗');
        this.toast.error(msg);
      }
    });
  }

  onResetPassword() {
    if (this.resetData.newPassword !== this.resetData.confirmPassword) {
      this.toast.warning('新密碼與確認密碼不一致');
      return;
    }

    const payload = {
      email: this.email.trim().toLowerCase(),
      token: this.resetData.token,
      newPassword: this.resetData.newPassword,
      confirmPassword: this.resetData.confirmPassword
    };

    this.memberService.resetPassword(payload).subscribe({
      next: (res) => {
        this.toast.success('密碼重設成功！請使用新密碼登入');
        this.router.navigate(['/member/login']);
      },
      error: (err) => {
        this.toast.error(err.error || '重設失敗，驗證碼可能已過期');
      }
    });
  }
}
