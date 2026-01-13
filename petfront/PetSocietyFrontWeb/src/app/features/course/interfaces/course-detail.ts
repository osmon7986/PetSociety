import { CourseChapter } from "./course-chapter";

// Angular 組件載入時，資料還沒準備好，因此要給?
export interface CourseDetail {
  imageUrl?: string,
  courseId?: number,
  title?: string,
  description?: string,
  categoryId?: number,
  type?: boolean,               // 實體 true 線上 false
  totalDuration?: number,
  courseDetailId?: number,
  categoryName?: string,
  areaName?: string,
  instructorName?: string,
  price?: number,
  chapters?: CourseChapter[],
}
