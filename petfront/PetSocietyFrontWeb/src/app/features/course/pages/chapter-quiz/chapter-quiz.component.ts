
import { QuizService } from './../../services/quiz.service';
import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router, } from '@angular/router';
import { BackButtonComponent } from '../../../../shared/back-button/back-button.component';
import { ChapterQuiz, QuizResult, QuizSubmission, UserAnswer } from '../../interfaces/quiz-model';
import { AlertService } from '../../services/alert.service';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { Dialog } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';
import { RatingModule } from 'primeng/rating';
import { FormsModule } from '@angular/forms';
import { CourseService } from '../../services/course.service';
import { NgClass } from '@angular/common';

@Component({
  selector: 'app-chapter-quiz',
  imports: [BackButtonComponent, ConfirmDialogModule, Dialog, ButtonModule, FormsModule, RatingModule, NgClass],
  templateUrl: './chapter-quiz.component.html',
  styleUrl: './chapter-quiz.component.css'
})
export class ChapterQuizComponent implements OnInit {
  private quizService = inject(QuizService); // 注入 QuizService 服務
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private alertService = inject(AlertService);
  private courseService = inject(CourseService);

  public readonly chapterQuiz = signal<ChapterQuiz | undefined>(undefined);
  selectedAnswers = signal<Record<number, number>>({}); // 初始化物件，儲存答題的key-value
  quizResult = signal<QuizResult | undefined>(undefined); // 初始化 QuizResult signal
  hasStartedQuiz = computed(() => Object.keys(this.selectedAnswers()).length > 0); // computed signal 答案改變時計算
  private chapterId: number = 0;
  private courseDetailId: number = 0;
  backButtonLabel = '返回課程'
  displayResult = false;

  ngOnInit(): void {
    this.courseService.goToTop();
    const chapterId = this.route.snapshot.paramMap.get('chapterId'); // 拿章節id
    const courseDetailId = this.route.snapshot.paramMap.get('courseDetailId'); // 拿課程id
    if (chapterId) {
      this.chapterId = Number(chapterId);
    }
    if (courseDetailId) {
      this.courseDetailId = Number(courseDetailId);
    }


    this.quizService.getChapterQuiz(this.chapterId).subscribe(
      (data) => {
        if (data) {
          console.log(data);
          this.chapterQuiz.set(data);
        }
      })
  }

  selectOption(questionId: number, optionId: number) {
    console.log(optionId);
    // update，signal的方法，修改資料
    // prev 更新前的資料
    this.selectedAnswers.update(prev => ({ // 建立空物件
      ...prev,                // 把舊物件資料複製到新物件
      [questionId]: optionId  // 新增 or 覆蓋，點選的題目&選項
    }))
  }

  // 按下送出測驗按鈕
  async submitQuiz() {
    // 拿這包測驗資料
    const quiz = this.chapterQuiz();
    if (!quiz) {
      return;
    }
    const total = quiz.questions.length; // 總題數
    const answered = Object.keys(this.selectedAnswers()).length; // 答題數
    // 檢查答題數
    if (answered < total) {
      await this.alertService.confirm({
        header: '還沒寫完喔！',
        message: `這份測驗共有 ${total} 題，你還有 ${total - answered} 題尚未作答`,
        icon: 'pi pi-exclamation-circle',
        acceptLabel: '回教室寫題目',
        rejectVisible: false, // 隱藏取消按鈕，強制他們回去寫
      });
      return;
    }
    const confirmed = await this.alertService.confirm({
      header: '確定要提交測驗嗎？',
      message: '提交後您的答案將送出進行評分，且無法再進行修改，請確認是否完成作答。',
      icon: 'pi pi-check-circle',
      acceptLabel: '確認提交',
      rejectLabel: '回頭檢查'
    })
    if (!confirmed) {
      return;
    }

    this.processQuizSubmission(quiz);
    this.courseService.goToTop();

  }

  // 自動選取答案
  autoQuiz() {
    if (this.chapterQuiz()) {
      const currentQuiz = this.chapterQuiz();
      const demoAns: UserAnswer[] = [];
      const optionIndex = [2, 1, 1] // 定義答案Index
      if (currentQuiz) {
        for (let i = 0; i < currentQuiz.questions.length; i++) {
          const qId = currentQuiz.questions[i].questionId; // 取目前Question ID
          console.log(qId);

          const answer: UserAnswer = {
            questionId: qId,
            selectedOptionId: currentQuiz.questions[i].options[optionIndex[i]].optionId
          }
          // 畫面同步選取答案
          this.selectedAnswers.update(allAnswers => ({
            ...allAnswers,
            [qId]: answer.selectedOptionId
          }));
          demoAns.push(answer);
        }

        const submission: QuizSubmission = {
          quizId: currentQuiz.quizId,
          courseDetailId: this.courseDetailId,
          answers: demoAns
        }
        console.log(submission);
        this.submit(submission);
      }
    }
  }

  // 讀會員的測驗內容，送到後端
  processQuizSubmission(quiz: ChapterQuiz) {
    const answers: UserAnswer[] = Object.entries(this.selectedAnswers()).map(
      // 箭頭函示 + 表達式
      // item[0] = qId, item[1] = oId
      ([qId, oId]) => ({
        questionId: Number(qId),
        selectedOptionId: oId
      })
    );
    // 測驗資料裝進DTO
    const payload: QuizSubmission = {
      quizId: quiz.quizId,
      courseDetailId: this.courseDetailId,
      answers: answers,
    }
    console.log(payload);
    // 送出測驗資料
    this.submit(payload);
  }

  // 發API取得測驗結果
  submit(payload: QuizSubmission) {
    this.quizService.submitQuiz(payload).subscribe((result) => {
      if (result) {
        console.log(result);
        // 存入測驗結果
        this.quizResult.set(result);
        this.displayResult = true; // 顯示測驗結果視窗
        console.log(this.displayResult);
      }
    });
  }

  resetQuiz() {
    this.selectedAnswers.set({}); // 清空答案
    this.quizResult.set(undefined); // 清空結果
    this.displayResult = false; // 關閉彈窗
    window.scrollTo(0, 0); // 回到頂部
  }

  isCorrectOption(qId: number, oId: number) {
    const result = this.quizResult()
    if (!result) {
      return;
    }
    const feedback = result.feedbacks.find(f => f.questionId === qId)
    return feedback?.correctOptionId === oId;
  }

  isWrongSelected(qId: number, oId: number) {
    const result = this.quizResult()
    if (!result) {
      return;
    }
    const feedback = result.feedbacks.find(f => f.questionId === qId)
    return !feedback?.isCorrect && feedback?.selectedOptionId === oId;
  }
  // 返回課程按鈕
  async goback() {
    if (this.hasStartedQuiz() && this.quizResult() === undefined) {
      const confirmed = await this.alertService.confirm({
        header: '確定要中斷測驗嗎？',
        message: '目前作答的內容尚未提交，離開後您的測驗紀錄將不會被儲存。',
        icon: 'pi pi-info-circle',
        acceptLabel: '確定離開',
        rejectLabel: '繼續作答'
      });
      if (confirmed) {
        this.router.navigate(['../../', 'playback'], { relativeTo: this.route }) // 返回兩層
      }
    }
    else {
      this.router.navigate(['../../', 'playback'], { relativeTo: this.route }) // 返回兩層
    }
  }

}
