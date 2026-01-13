import { Router, RouterLink } from '@angular/router';
import { Component, inject, OnDestroy, OnInit, Pipe, signal } from '@angular/core';
import { CourseService } from '../../services/course.service';
import { CourseDetail } from '../../interfaces/course-detail';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { delay, of, single, Subscription, switchMap } from 'rxjs';
import { BackButtonComponent } from "../../../../shared/back-button/back-button.component";
import { AuthService } from '../../../../core/auth/auth.service';
import { ToastService } from '../../../member/services/toast.service';
import { PaymentService } from '../../services/payment.service';

@Component({
  selector: 'app-course-detail',
  imports: [RouterModule, BackButtonComponent,],
  templateUrl: './course-detail.component.html',
  styleUrl: './course-detail.component.css'
})
export class CourseDetailComponent implements OnInit {

  courseDetail: CourseDetail = {};
  courseDetailId = 0;
  backButtonLabel = '返回課程列表'

  hasPurchased = signal(false);
  hasSubscribed = signal(false);
  isLoggedIn = signal(false);

  ecpayParams: any = null; // 後端回傳的綠界參數
  ecpayUrl = 'https://payment-stage.ecpay.com.tw/Cashier/AioCheckOut/V5' // 要呼叫的綠界測試網址


  // Inject
  private paymentService = inject(PaymentService)

  // DI CourseService, ActivatedRoute, Router
  constructor(
    private courseService: CourseService,
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private authservice: AuthService,
    private toast: ToastService,
  ) { }

  ngOnInit(): void {
    this.courseService.goToTop();
    this.courseDetailId = +this.activatedRoute.snapshot.paramMap.get('courseDetailId')!;

    this.courseService.getCourseDetail(this.courseDetailId).subscribe((data) => {
      if (data) {
        this.courseDetail = data;
      } else {
        this.router.navigate(['academy'])
      }
    })
    this.checkMemberStatus();
  }

  /**Navigate to course main page */
  goBack() {
    this.router.navigate(['academy']);
  }

  /**Check member's logged in status and purchased status */
  checkMemberStatus() {
    // 檢查會員登入狀態
    if (this.authservice.isLoggedIn()) {
      this.isLoggedIn.set(true);

      // 檢查已登入的會員是否已購買過此課程
      this.courseService.checkOwnerShip(this.courseDetailId).subscribe
        ((result) => {
          if (result) {
            this.hasPurchased.set(true);
          }
        });
      // 檢查已登入的會員是否已訂閱此課程
      this.courseService.checkSubscription(this.courseDetailId).subscribe((result) => {
        if (result) {
          this.hasSubscribed.set(true);
        }
      })
    }
  }

  goToPlayback() {
    this.router.navigate(['academy', 'my-course', this.courseDetailId, 'playback'])
  }

  /**Subscribe course and navigate to course playback page */
  subscribeCourse() {
    if (!this.isLoggedIn) {
      this.toast.warning('請先登入會員');
    }
    else {
      // 訂閱API
      this.courseService.subscribeCourse(this.courseDetailId).subscribe(() => {
        // 確認訂閱成功
        // 前往課程播放畫面
        this.toast.success('訂閱成功，已加入我的課程清單');
        this.router.navigate(['/academy', 'my-course', this.courseDetailId, 'playback'])
      })
    }
  }

  // 付款
  checkout() {
    if (!this.authservice.isLoggedIn()) {
      this.toast.warning('請先登入會員');
    }
    else {
      const currentUrl = this.router.url; // 拿當前網址，沒有...4200...
      this.paymentService.getEcpayParameters(this.courseDetailId, currentUrl)
        .subscribe((params) => {
          console.log(params);
          this.ecpayParams = params; // 回傳的參數

          setTimeout(() => {
            const form = document.getElementById('ecpay-form') as HTMLFormElement;
            if (form) {
              form.submit(); // 呼叫原生的 submit() 才可以帶著資料跳轉到綠界網址
            }
          }, 100); // 等待0.1秒，把表單資料渲染完畢再送出表單
        })
    }
  }
  get objectKeys() { // Getter 取值器
    return this.ecpayParams ? Object.keys(this.ecpayParams) : []; // Object.keys 抓欄位名稱
  }

}
