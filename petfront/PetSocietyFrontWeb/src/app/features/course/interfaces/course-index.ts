export interface CourseIndex {
  imageUrl: string,
  courseId: number,
  title: string,
  description: string,
  categoryId: number,
  type: boolean,               // 實體 true 線上 false
  totalDuration: number,
  courseDetailId: number,
  areaName: string,
  instructorName: string,
  price: number,
  subsCount: number,
  isFavorite?: boolean;
}

