import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { ChapterQuiz, QuizResult, QuizSubmission } from '../interfaces/quiz-model';

@Injectable({
  providedIn: 'root'
})
export class QuizService {

  private baseUrl = 'https://localhost:7138';
  private httpClient = inject(HttpClient);
  constructor() { }

  /**Get quiz by chapterId */
  getChapterQuiz(chapterId: number) {
    return this.httpClient.get<ChapterQuiz>(`${this.baseUrl}/Courses/Quiz/${chapterId}`)
  }

  submitQuiz(submission: QuizSubmission) {
    return this.httpClient.post<QuizResult>(`${this.baseUrl}/Courses/quizResult`, submission)
  }
}
