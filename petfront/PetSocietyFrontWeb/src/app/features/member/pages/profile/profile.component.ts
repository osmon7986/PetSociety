import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';

import { MemberService } from '../../services/member.service';
import { ToastService } from '../../services/toast.service';
import { AuthService } from '../../../../core/auth/auth.service';
import { MemberDto, ChangePasswordDto } from '../../Interfaces/member.dto';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit, OnDestroy {

  member: MemberDto | null = null;
  isLoading: boolean = true;
  avatarUrl: string = 'images/member/default-avatar.png';
  selectedFile: File | null = null;

  isChangePasswordVisible: boolean = false;
  passwordData: ChangePasswordDto = {
    oldPassword: '',
    newPassword: '',
    confirmNewPassword: ''
  };

  isEditModalVisible: boolean = false;
  editData: any = {
    name: '',
    phone: '',
    introduction: '',
    address: ''
  };

  constructor(
    private memberService: MemberService,
    private authService: AuthService,
    private router: Router,
    private toast: ToastService
  ) { }

  ngOnInit(): void {
    if (this.authService.isLoggedIn()) {
      this.fetchMemberProfile();
      this.loadAvatarImage();
    } else {
      this.toast.warning('請先登入');
      this.router.navigate(['/member/login']);
    }
  }

  // 2. 當元件被銷毀時 (例如跳轉到別頁)，釋放圖片記憶體
  ngOnDestroy(): void {
    if (this.avatarUrl && this.avatarUrl.startsWith('blob:')) {
      URL.revokeObjectURL(this.avatarUrl);
    }
  }

  // --- 呼叫後端 API 取得會員資料 ---
  fetchMemberProfile() {
    this.isLoading = true;
    this.memberService.getProfile().subscribe({
      next: (res) => {
        this.member = res;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('❌ 會員資料讀取失敗', err);
        this.isLoading = false;
        if (err.status === 401) {
          this.authService.logout();
          this.router.navigate(['/member/login']);
        }
      }
    });
  }

  loadAvatarImage() {
    this.memberService.getAvatar().subscribe({
      next: (blob) => {
        if (this.avatarUrl && this.avatarUrl.startsWith('blob:')) {
          URL.revokeObjectURL(this.avatarUrl);
        }
        this.avatarUrl = URL.createObjectURL(blob);
      },
      error: (err) => {
        this.avatarUrl = 'images/member/default-avatar.png';
      }
    });
  }

  handleImageError(event: any) {
    if (event.target.src !== 'assets/images/default-avatar.png') {
      event.target.src = 'images/member/default-avatar.png';
    }
    event.target.onerror = null;
  }

  onFileSelected(event: any) {
    if (event.target.files && event.target.files.length > 0) {
      const file = event.target.files[0];

      if (file.size > 2 * 1024 * 1024) {
        this.toast.warning('圖片大小不能超過 2MB');
        return;
      }

      this.selectedFile = file;
    }
  }

  onUploadAvatar() {
    if (!this.selectedFile) return;

    this.memberService.uploadAvatar(this.selectedFile).subscribe({
      next: (res) => {
        this.toast.success('頭像更新成功！');
        this.loadAvatarImage();
        this.memberService.notifyProfileUpdated();
        this.selectedFile = null;
      },
      error: (err) => {
        console.error(err);
        this.toast.error('上傳失敗，請稍後再試');
      }
    });
  }

  openEditModal() {
    if (!this.member) return;

    this.editData = {
      name: this.member.name,
      phone: this.member.phone,
    };

    this.isEditModalVisible = true;
  }

  closeEditModal() {
    this.isEditModalVisible = false;
  }

  onSubmitEdit() {
    if (!this.member) return;

    this.memberService.updateProfile(this.member.memberId, this.editData).subscribe({
      next: (res) => {
        this.toast.success('個人資料更新成功！');
        this.isEditModalVisible = false;
        this.fetchMemberProfile();
      },
      error: (err) => {
        console.error(err);
        this.toast.error('更新失敗：' + (err.error?.message || '系統錯誤'));
      }
    });
  }

  toggleChangePassword() {
    this.isChangePasswordVisible = !this.isChangePasswordVisible;
    if (!this.isChangePasswordVisible) {
      this.passwordData = { oldPassword: '', newPassword: '', confirmNewPassword: '' };
    }
  }

  onSubmitPassword() {
    if (this.passwordData.newPassword !== this.passwordData.confirmNewPassword) {
      this.toast.warning('兩次輸入的密碼不一致');
      return;
    }

    this.memberService.changePassword(this.passwordData).subscribe({
      next: (res) => {
        this.toast.success('密碼修改成功！請重新登入');
        this.authService.logout();
        this.router.navigate(['/member/login']);
      },
      error: (err) => {
        console.error(err);
        const msg = err.error?.message || '修改失敗，請檢查舊密碼';
        this.toast.error(msg);
      }
    });
  }
}
