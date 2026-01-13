import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CardComponent } from "../../../../shared/card/card.component";
import { CourseIndex } from '../../interfaces/course-index';
import { RouterLink } from "@angular/router";
import { DecimalPipe } from '@angular/common';

@Component({
  selector: 'app-course-card',
  imports: [CardComponent, RouterLink, DecimalPipe],
  templateUrl: './course-card.component.html',
  styleUrl: './course-card.component.css'
})
export class CourseCardComponent {

  // 讓外層父 Component 傳資料進子層
  @Input() course!: CourseIndex;

  // 定義一個「通知父元件」的事件發送器
  @Output() toggleFav = new EventEmitter<any>();

  // 按鈕點擊時觸發的方法
  onFavoriteClick(event: Event) {
    event.preventDefault();
    event.stopPropagation();
    this.toggleFav.emit(this.course); // 把這門課程資料傳給父元件
  }
}
