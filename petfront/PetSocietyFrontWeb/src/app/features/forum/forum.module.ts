import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common'; // 重要：提供 *ngFor, *ngIf

// 引入元件
import { ForumComponent } from './forum.component';


@NgModule({
  declarations: [
  ],
  imports: [
    CommonModule,   //  引入通用模組
    ForumComponent  //  引入獨立元件
  ],
  exports: [
    ForumComponent //  匯出元件，讓外部 (AppModule) 可以使用它
  ]
})
export class ForumModule { }
