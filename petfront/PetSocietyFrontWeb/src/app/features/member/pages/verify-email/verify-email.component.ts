import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { MemberService } from '../../services/member.service';
import { ToastService } from '../../services/toast.service';

@Component({
  selector: 'app-verify-email',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './verify-email.component.html',
  styleUrls: ['./verify-email.component.css']
})
export class VerifyEmailComponent implements OnInit {

  email: string = '';
  code: string = '';
  isLoading: boolean = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private memberService: MemberService,
    private toast: ToastService
  ) { }

  ngOnInit(): void {
    // 自動抓取網址上的 Email 參數 (?email=xxx)
    this.route.queryParams.subscribe(params => {
      if (params['email']) {
        this.email = params['email'];
      }
    });
  }

  onSubmit() {
    if (!this.email || !this.code) {
      this.toast.warning('請填寫完整資訊');
      return;
    }

    this.isLoading = true;

    const payload = {
      email: this.email,
      code: this.code
    };

    this.memberService.verifyAccount(payload).subscribe({
      next: (res) => {
        this.isLoading = false;
        this.toast.success('驗證成功！歡迎加入，請登入');
        this.router.navigate(['/member/login']); // 成功後去登入
      },
      error: (err) => {
        this.isLoading = false;
        this.toast.error(err.error?.message || '驗證失敗，請檢查驗證碼');
      }
    });
  }
}
