import { AuthService } from './../../../../core/auth/auth.service';
import { ActivityCalenderComponent } from '../../components/activity-calender/activity-calender.component';
import { Component, inject, Inject, OnInit, signal } from '@angular/core';
import { BreadcrumbBannerComponent } from "../../../../shared/breadcrumb-banner/breadcrumb-banner.component";
import { BreadcrumbItem } from '../../../../shared/interfaces/breadcrumb-items';
import { ToastService } from '../../../member/services/toast.service';


@Component({
  selector: 'app-calender',
  imports: [ActivityCalenderComponent, BreadcrumbBannerComponent],
  templateUrl: './calender.component.html',
  styleUrl: './calender.component.css'
})
export class CalenderComponent implements OnInit {
  private authservice = inject(AuthService);
  private toast = inject(ToastService);

  isLogged = signal(false);

  breadcrumbData: BreadcrumbItem[] = [
    { label: '活動', link: '/activity/intro' }
  ]
  TitleLabel: string = '';
  ngOnInit(): void {
    this.checkMemberStatus();
    if (this.isLogged()) {
      this.TitleLabel = '我的活動日歷';
      this.toast.info('歡迎回來！這是您的個人活動日歷。');

    }
    else {
      this.TitleLabel = '活動日歷';
      this.toast.info('歡迎來到活動日歷！請登入以查看您參加的個人活動。');
    }
  }
  checkMemberStatus() {
    //檢查會員登入
    if (this.authservice.isLoggedIn()) {
      this.isLogged.set(true);
    }
  }

}
