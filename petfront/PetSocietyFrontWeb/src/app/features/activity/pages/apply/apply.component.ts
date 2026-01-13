import { AuthService } from './../../../../core/auth/auth.service';
import { Component, signal } from '@angular/core';
import { ActivityApplyformComponent } from "../../components/activity-applyform/activity-applyform.component";
import { BreadcrumbItem } from '../../../../shared/interfaces/breadcrumb-items';
import { BreadcrumbBannerComponent } from '../../../../shared/breadcrumb-banner/breadcrumb-banner.component';
import { ToastService } from '../../../member/services/toast.service';


@Component({
  selector: 'app-apply',
  imports: [ActivityApplyformComponent, BreadcrumbBannerComponent],
  templateUrl: './apply.component.html',
  styleUrl: './apply.component.css'
})
export class ApplyComponent {

  isLogged = signal(false);


  constructor(
    private authservice: AuthService,
    private toast: ToastService,
  ) { }

  breadcrumbData: BreadcrumbItem[] = [
    { label: '活動', link: '/activity/intro' }
  ]

  checkMemberStatus() {
    //檢查會員登入
    if (this.authservice.isLoggedIn()) {
      this.isLogged.set(true);
    }
  }


  ngOnInit(): void {
    this.checkMemberStatus();
    if (!this.isLogged()) {
      this.toast.warning('請先登入會員以完成申請活動');
    }

  }

}
