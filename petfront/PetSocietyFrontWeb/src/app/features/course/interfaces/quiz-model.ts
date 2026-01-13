// 顯示章節測驗
export interface ChapterQuiz {
  quizId: number;
  title: string;
  questions: QuizQuestion[];
}
export interface QuizQuestion {
  questionId: number;
  questionText: string;
  options: QuizOption[];
}
export interface QuizOption {
  optionId: number;
  optionText: string;
}

// 回傳章節測驗內容
export interface UserAnswer {
  questionId: number;
  selectedOptionId: number;
}
export interface QuizSubmission {
  quizId: number;
  courseDetailId: number;
  answers: UserAnswer[];
}

// 顯示章節測驗結果
export interface QuizResult {
  score: number;
  correctCount: number;
  totalQuestions: number;
  feedbacks: QuizFeedback[];
}
export interface QuizFeedback {
  questionId: number;
  correctOptionId: number;
  isCorrect: boolean;
  selectedOptionId: number;
}
