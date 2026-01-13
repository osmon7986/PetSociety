import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-course-payment-success',
  imports: [RouterLink],
  templateUrl: './course-payment-success.component.html',
  styleUrl: './course-payment-success.component.css'
})
export class CoursePaymentSuccessComponent implements OnInit {

  private route = inject(ActivatedRoute);
  private router = inject(Router);
  returnUrl: string | null = null;

  ngOnInit(): void {
    // 從網址列抓取 returnUrl
    this.returnUrl = this.route.snapshot.queryParamMap.get('returnUrl');
    console.log(this.returnUrl);
  }

  goToCourse() {
    if (this.returnUrl) {
      this.router.navigateByUrl(this.returnUrl); //使用完整字串
    }
    else {
      this.router.navigate(['/'])
    }
  }

}
