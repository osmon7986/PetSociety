import { ActivityIntroCard } from './../../interface/activityIntroCard';
import { ActivityTable } from './../../interface/activityTable';
import { ActivityService } from '../../services/activity.service';
import { Component, OnInit } from '@angular/core';
import { ActivityTableComponent } from '../../components/activity-table/activity-table.component';
import { BreadcrumbItem } from '../../../../shared/interfaces/breadcrumb-items';
import { BreadcrumbBannerComponent } from '../../../../shared/breadcrumb-banner/breadcrumb-banner.component';
import { ActivityIntrocardComponent } from "../../components/activity-introcard/activity-introcard.component";

@Component({
  selector: 'app-intro',
  imports: [ActivityTableComponent, BreadcrumbBannerComponent, ActivityIntrocardComponent,],
  templateUrl: './intro.component.html',
  styleUrl: './intro.component.css'
})
export class IntroComponent implements OnInit {
  breadcrumbData: BreadcrumbItem[] = [
    { label: '活動', link: '' }
    //todo因為是主頁，修成不能點擊
  ]

  activityAdData: ActivityIntroCard[] = [];

  activityTable: ActivityTable[] = [];

  selectedCategory = '所有活動';

  constructor(private ActivityService: ActivityService) { }

  ngOnInit(): void {
    this.ActivityService.adData$.subscribe(data => {
      this.activityAdData = data;
    });
    this.ActivityService.tableData$.subscribe(data => {
      this.activityTable = data;
    });
  }
  selectCategory(category: string): void {
    this.selectedCategory = category;
    this.ActivityService.setCategory(category);
    // 在這裡可以加入篩選 activityTable 的邏輯
    console.log('Selected category:', category);
  }

}
