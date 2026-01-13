import { CourseIndex } from './course-index';
export interface CoursePaged {
  items: CourseIndex[],
  page: number,
  pageSize: number,
  totalCount: number,
  totalPages: number,
  lastUpdated: string,
}
