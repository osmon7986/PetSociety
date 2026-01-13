import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import Swal from 'sweetalert2';
import { NotificationDto } from '../../Interfaces/notification.dto';
import { NotificationService } from '../../services/notification.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-notifications',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './notifications.component.html',
  styleUrls: ['./notifications.component.css']
})
export class NotificationsComponent implements OnInit {

  notifications: NotificationDto[] = [];
  currentFilter: 'all' | 'unread' = 'all';

  constructor(private notificationService: NotificationService, private router: Router) { }

  ngOnInit(): void {
    this.loadData();
  }

  loadData() {
    this.notificationService.getNotifications().subscribe(data => {
      this.notifications = data.sort((a, b) =>
        new Date(b.createDate).getTime() - new Date(a.createDate).getTime()
      );
    });
  }

  onRead(item: NotificationDto) {
    if (!item.readTime) {
      this.notificationService.markAsRead(item.notificationId).subscribe(() => {
        item.readTime = new Date().toISOString();
      });
    }
    if (item.targetUrl) {
      this.router.navigateByUrl(item.targetUrl);
    }
  }

  onDelete(id: number, event: Event) {
    event.stopPropagation();
    Swal.fire({
      title: '刪除通知？',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: '刪除',
      cancelButtonText: '取消',
      confirmButtonColor: '#D8C3A5',
      cancelButtonColor: '#d33'
    }).then((res) => {
      if (res.isConfirmed) {
        this.notificationService.deleteNotification(id).subscribe(() => {
          this.loadData();
          const Toast = Swal.mixin({
            toast: true, position: 'top-end', showConfirmButton: false, timer: 1500
          });
          Toast.fire({ icon: 'success', title: '已刪除' });
        });
      }
    });
  }

  onMarkAllRead() {
    this.notificationService.markAllAsRead().subscribe(() => {
      this.notifications.forEach(n => n.readTime = n.readTime || new Date().toISOString());
    });
  }

  setFilter(filter: 'all' | 'unread') {
    this.currentFilter = filter;
  }

  get filteredNotifications() {
    if (this.currentFilter === 'unread') {
      return this.notifications.filter(n => !n.readTime);
    }
    return this.notifications;
  }

  getCategoryIcon(category: string | undefined): string {
    switch (category) {
      case '系統': return 'bi-gear-fill';
      case '訂單': return 'bi-box-seam-fill';
      case '活動': return 'bi-calendar-heart-fill';
      case '課程': return 'bi-mortarboard-fill';
      case '論壇': return 'bi-chat-quote-fill';
      default: return 'bi-bell-fill';
    }
  }

  getCategoryClass(category: string | undefined): string {
    switch (category) {
      case '系統': return 'type-system';
      case '訂單': return 'type-order';
      case '活動': return 'type-activity';
      case '課程': return 'type-course';
      case '論壇': return 'type-forum';
      default: return 'type-default';
    }
  }
}
