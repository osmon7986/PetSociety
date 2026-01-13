import { BreadcrumbItem } from './../../../../shared/interfaces/breadcrumb-items';
import { Component, OnInit, signal } from '@angular/core';
import { BreadcrumbBannerComponent } from "../../../../shared/breadcrumb-banner/breadcrumb-banner.component";
import { CourseService } from '../../services/course.service';
import { MyCourses } from '../../interfaces/my-courses';
import { Router, RouterLink } from '@angular/router';
import { CertificateService } from '../../services/certificate.service';

@Component({
  selector: 'app-my-course',
  imports: [BreadcrumbBannerComponent, RouterLink],
  templateUrl: './my-course.component.html',
  styleUrl: './my-course.component.css'
})
export class MyCourseComponent implements OnInit {
  breadcrumbData: BreadcrumbItem[] = [
    { label: '學院', link: '/academy' }
  ]
  downloadingId = signal<number | null>(null); // 紀錄目前下載中的課程ID
  imageLoaded = signal(false);
  myCourses: MyCourses[] = [];

  constructor(
    private courseService: CourseService,
    private router: Router,
    private certificateService: CertificateService) { }

  ngOnInit(): void {
    this.courseService.getMyCourses().subscribe((data) => {
      this.myCourses = data;

    })
  }

  resume(courseDetailId: number) {
    this.router.navigate(['/academy', 'my-course', courseDetailId, 'playback'])
  }

  // 圖片載入設定signal 為true
  onLoad() {
    this.imageLoaded.set(true);
  }

  downloadPdf(courseDetailId: number) {
    console.log(courseDetailId);

    this.downloadingId.set(courseDetailId); // 開始下載，更新signal

    this.certificateService.getCertificatePdf(courseDetailId)
      .subscribe(blob => {
        const url = window.URL.createObjectURL(blob); // 產生臨時的URL特殊字串
        this.downloadingId.set(null); // 下載完成，恢復signal
        window.open(url, '_blank'); // 呼叫瀏覽器訪問blob URL並開啟新分頁


      })
  }
}
