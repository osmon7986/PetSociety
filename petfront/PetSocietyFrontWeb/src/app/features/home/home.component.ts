import { Component, AfterViewInit } from '@angular/core';

declare var Swiper: any;

@Component({
  selector: 'app-home',
  imports: [],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})

export class HomeComponent implements AfterViewInit { // 生命週期掛鉤 (Swiper初始化)
  ngAfterViewInit(): void {
    new Swiper('.main-swiper', {
      loop: true,
      // 圓點
      pagination: {
        el: '.swiper-pagination',
        clickable: true
      },
      // 輪播
      autoplay: {
        delay: 3000,
        disableOnInteraction: false
      },
      // 輪播速度
      speed: 1500
    });
  }
}
