export interface NotificationDto {
  notificationId: number;
  memberId: number;
  typeId?: number;        // 類型編號
  categoryName?: string;  // 種類名稱
  title: string;          // 標題
  message: string;
  createDate: string;
  readTime?: string | null;
  targetUrl?: string;
}
