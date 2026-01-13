import { Component, Input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { BreadcrumbItem } from '../interfaces/breadcrumb-items';

@Component({
  selector: 'app-breadcrumb-banner',
  imports: [RouterLink],
  templateUrl: './breadcrumb-banner.component.html',
  styleUrl: './breadcrumb-banner.component.css'
})
export class BreadcrumbBannerComponent {
  @Input() bannerTitle: string = '標題';
  @Input() bannerSubtitle: string = ''; // 副標題
  @Input() breadcrumbItem: BreadcrumbItem[] = []; // 陣列，可以新增更身路徑
  @Input() currentTitle: string = '目前頁面';
}
