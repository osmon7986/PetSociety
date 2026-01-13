export interface ActivityApply {
  title: string;
  organizerName: string;
  description: string;
  location: string;
  startTime: string | Date; // 傳送給 API 時通常使用 ISO 字串
  endTime: string | Date;
  maxCapacity: number;
  registrationStartTime: string | Date;
  registrationEndTime: string | Date;
  latitude: number;
  longitude: number;
}
