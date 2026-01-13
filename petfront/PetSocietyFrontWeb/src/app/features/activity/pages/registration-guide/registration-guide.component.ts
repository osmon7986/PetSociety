import { Component } from '@angular/core';
import { ActivityBlockComponent } from '../../components/activity-block/activity-block.component';
import { ActivityInfoComponent } from "../../components/activity-info/activity-info.component";
import { BreadcrumbItem } from '../../../../shared/interfaces/breadcrumb-items';
import { BreadcrumbBannerComponent } from '../../../../shared/breadcrumb-banner/breadcrumb-banner.component';
import { BackButtonComponent } from "../../../../shared/back-button/back-button.component";
import { Router } from '@angular/router';
import { ActivityCommentComponent } from "../../components/activity-comment/activity-comment.component";
import { ActivityNewCommentComponent } from "../../components/activity-new-comment/activity-new-comment.component";
import { ActivityMapComponent } from "../../components/activity-map/activity-map.component";

@Component({
  selector: 'app-registration-guide',
  imports: [ActivityBlockComponent, ActivityInfoComponent, BreadcrumbBannerComponent, BackButtonComponent, ActivityCommentComponent, ActivityNewCommentComponent, ActivityMapComponent],
  templateUrl: './registration-guide.component.html',
  styleUrl: './registration-guide.component.css'
})
export class RegistrationGuideComponent {
  breadcrumbData: BreadcrumbItem[] = [
    { label: '活動', link: '/activity/intro' }
  ]

  backButtonLabel = '返回活動列表'

  constructor(private router: Router) { }

  goBack() {
    this.router.navigate(['activity/intro']);
  }

  goRegistration() {
    this.router.navigate(['activity/registration']);
  }


}
