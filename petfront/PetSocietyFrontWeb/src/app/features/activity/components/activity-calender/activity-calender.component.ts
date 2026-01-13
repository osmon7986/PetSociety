import { AuthService } from './../../../../core/auth/auth.service';
import { ToastService } from './../../../member/services/toast.service';
import { ActivityCalenderService } from '../../services/activity-calender.service';
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FullCalendarModule } from '@fullcalendar/angular';
import { CalendarOptions } from '@fullcalendar/core'; // useful for typechecking
import dayGridPlugin from '@fullcalendar/daygrid';
import { ActivityCalender } from '../../interface/activityCalender';

@Component({
  selector: 'app-activity-calender',
  standalone: true,
  imports: [CommonModule, FullCalendarModule],
  templateUrl: './activity-calender.component.html',
  styleUrls: ['./activity-calender.component.css']
})
export class ActivityCalenderComponent {
  constructor(
    private ActivityCalenderService: ActivityCalenderService,
    private ToastService: ToastService,
    private AuthService: AuthService,
  ) { }

  events: {
    title: string,
    date: Date,
    isRegistered: boolean
  }[] = []

  calendarOptions: CalendarOptions = {
    initialView: 'dayGridMonth',
    plugins: [dayGridPlugin],
    events: [] // 這裡保持空陣列，等待異步數據載入
  };

  ngOnInit(): void {
    this.ActivityCalenderService.getCalenderApi().subscribe({
      next: (Data: ActivityCalender[]) => {
        this.events = Data.map(item => ({
          title: item.title,
          // 假設 item.starttime 是字串，必須轉換為 Date 物件
          date: new Date(item.startTime),
          isRegistered: item.isRegistered,
          color: item.isRegistered ? '#28a745' : '#3788d8',
          textColor: '#ffffff'
        }
        ));
        console.log(this.events);
        //更新calenderoption
        this.updateCalendarOptions();
      }, error: (err) => {
        console.error('API呼叫失敗', err);
      }
    })


  }

  // 新增方法：專門用來更新 calendarOptions
  updateCalendarOptions(): void {

    //   // 1. 轉換為 FullCalendar 所需的 events 格式 (保持 date 屬性)
    const fullCalenderEvents = this.events.map(data => ({
      title: data.title,
      date: data.date, // 這裡 data.date 必須是 Date 物件或 ISO 字串
      classNames: data.isRegistered ? ['my-registered-event'] : []
    }));

    //   // 2. 創建一個新的 calendarOptions 物件，強制 FullCalendar 重新渲染
    this.calendarOptions = {
      ...this.calendarOptions,
      events: fullCalenderEvents
    }
  }
}
