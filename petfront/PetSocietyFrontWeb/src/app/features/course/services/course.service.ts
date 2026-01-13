import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CourseIndex } from '../interfaces/course-index';
import { CourseDetail } from '../interfaces/course-detail';
import { MyCourses } from '../interfaces/my-courses';
import { CoursePlayback } from '../interfaces/course-playback';
import { ChapterRecord } from '../interfaces/chapter-record';
import { CoursePaged } from '../interfaces/course-paged';
import { CourseQuery } from '../interfaces/course-query';
import { CourseCategory } from '../interfaces/course-category';

@Injectable({
  providedIn: 'root'
})
export class CourseService {

  private baseUrl = 'https://localhost:7138';

  // HttpClient
  constructor(private httpClient: HttpClient) { }

  goToTop() {
    window.scrollTo({
      top: 0,
      left: 0,
      behavior: 'smooth',
    });
  }
  getCourses() {
    return this.httpClient.get<CourseIndex[]>(`${this.baseUrl}/Courses`)
  }

  getPagedCourse(query: CourseQuery) {
    let params = new HttpParams();
    // 掃描 query 可列舉的屬性，裝到新的字串陣列
    Object.keys(query).forEach(key => {
      const value = (query as any)[key]; // 中括號表示法，取出query屬性的值
      if (value !== null && value !== undefined) {
        params = params.append(key, value.toString());
      }
    })
    return this.httpClient.get<CoursePaged>(`${this.baseUrl}/Courses/pagedCourse`, { params })
  }

  getHotCourse() {
    return this.httpClient.get<CourseIndex[]>(`${this.baseUrl}/Courses/hotCourse`)
  }

  getCourseCategory() {
    return this.httpClient.get<CourseCategory[]>(`${this.baseUrl}/Courses/category`)
  }
  getCourseDetail(courseDetailId: number) {
    return this.httpClient.get<CourseDetail>(`${this.baseUrl}/Courses/${courseDetailId}`)
  }

  /**Get member's subscribed courses */
  getMyCourses() {
    return this.httpClient.get<MyCourses[]>(`${this.baseUrl}/Courses/myCourse`)
  }

  /**Subscribe course by courseDetailId*/
  subscribeCourse(courseDetailId: number) {
    return this.httpClient.post<void>(`${this.baseUrl}/Courses/subscribe`, { courseDetailId })
  }

  /**Get course playback */
  getCoursePlayback(courseDetailId: number) {
    return this.httpClient.get<CoursePlayback>(`${this.baseUrl}/Courses/playback/${courseDetailId}`)
  }

  /**Update member's chapter record */
  updateChapterRec(record: ChapterRecord) {
    return this.httpClient.put<void>(`${this.baseUrl}/Courses/playback/chapter`, record, { observe: 'response' })
    // subscribe 的 next 處理函式將會收到一個完整的 HttpResponse 物件
  }

  /**Check if member owned this course */
  checkOwnerShip(courseDetailId: number) {
    return this.httpClient.get<boolean>(`${this.baseUrl}/CoursePayment/checkOwnerShip/${courseDetailId}`);
  }

  /**Check if member has subscribed this course */
  checkSubscription(courseDetailId: number) {
    return this.httpClient.get<boolean>(`${this.baseUrl}/Courses/checkSubscription/${courseDetailId}`);
  }
}
