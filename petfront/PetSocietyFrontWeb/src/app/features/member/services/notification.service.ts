import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { NotificationDto } from '../Interfaces/notification.dto';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {

  private mockData: NotificationDto[] = [
    {
      notificationId: 1,
      memberId: 20,
      categoryName: '系統',
      title: '歡迎加入我們！',
      message: '感謝您註冊成為會員，快去逛逛網站吧！',
      createDate: '2025-10-25 10:00',
      readTime: '2025-10-25 10:05',
      targetUrl: './',
    },
    {
      notificationId: 2,
      memberId: 20,
      categoryName: '訂單',
      title: '訂單 #2023102601 已出貨',
      message: '您的商品已經交由物流配送，預計 2-3 天內送達。',
      createDate: '2025-10-26 14:30',
      readTime: null,
      targetUrl: '/mall/cart',
    },
    {
      notificationId: 3,
      memberId: 20,
      categoryName: '課程',
      title: '課程優惠提醒',
      message: '您收藏的「新手犬隻訓練」目前有 8 折優惠，活動只到本週五！',
      createDate: '2025-10-27 09:00',
      readTime: null,
      targetUrl: '/academy/my-course',
    },
    {
      notificationId: 4,
      memberId: 20,
      categoryName: '活動',
      title: '寵物野餐日報名開始！',
      message: '這週末在市民廣場舉辦寵物野餐，趕快帶著毛小孩來參加吧。',
      createDate: '2026-01-02 10:00',
      readTime: null,
      targetUrl: '/activity/intro',
    },
    {
      notificationId: 5,
      memberId: 20,
      categoryName: '論壇',
      title: '有人回覆了您的文章',
      message: '您發表的「請問幼犬飼料推薦」有一則新的回覆，快去看看吧！',
      createDate: '2026-01-03 15:20',
      readTime: null,
      targetUrl: '/forum',
    },
  ];

  constructor() { }

  // 1. 取得所有通知
  getNotifications(): Observable<NotificationDto[]> {
    return of(this.mockData);
  }

  // 2. 標示為已讀
  markAsRead(id: number): Observable<boolean> {
    const target = this.mockData.find(n => n.notificationId === id);
    if (target) {
      target.readTime = new Date().toISOString();
    }
    return of(true);
  }

  // 3. 刪除通知
  deleteNotification(id: number): Observable<boolean> {
    this.mockData = this.mockData.filter(n => n.notificationId !== id);
    return of(true);
  }

  // 4.全部已讀
  markAllAsRead(): Observable<boolean> {
    const now = new Date().toISOString();
    this.mockData.forEach(n => {
      if (!n.readTime) n.readTime = now;
    });
    return of(true);
  }
}
