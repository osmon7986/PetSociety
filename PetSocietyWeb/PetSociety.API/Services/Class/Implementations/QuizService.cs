using Betalgo.Ranul.OpenAI.ObjectModels.ResponseModels;
using Humanizer;
using PetSociety.API.DTOs;
using PetSociety.API.DTOs.Class;
using PetSociety.API.Models;
using PetSociety.API.Repositories.Class.Interfaces;
using PetSociety.API.Services.Class.Interfaces;

namespace PetSociety.API.Services.Class.Implementations
{
    public class QuizService : IQuizService
    {
        private readonly IQuizRecordRepository _quizRecordRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IChapterQuizRepository _chapterQuizRepository;

        public QuizService(
            IQuizRecordRepository quizRecordRepository,
            ICourseRepository courseRepository,
            IChapterQuizRepository chapterQuizRepository
            )
        {
            _quizRecordRepository = quizRecordRepository;
            _courseRepository = courseRepository;
            _chapterQuizRepository = chapterQuizRepository;
        }

        public async Task CreateQuizRecordAsync(int memberId, QuizSubmissionDTO submission, QuizResultDTO result)
        {
            // 查詢是否已有此測驗紀錄
            var record = await _chapterQuizRepository.GetQuizRecordAsync(memberId, submission.QuizId);
            
            if (record == null)
            {
                // 沒有此測驗紀錄，建立新紀錄
                var newRecord = new ChapterQuizRecord
                {
                    MemberId = memberId,
                    QuizId = submission.QuizId,
                    CorrectCount = (byte?)result.CorrectCount,
                    Score = (byte?)result.Score,
                    SubmittedAt = DateTime.Now
                };
                await _chapterQuizRepository.AddRecordAsync(newRecord);
            }
            else if (result.Score > record.Score) // 已有此測驗紀錄
            {
                // 只有分數較高才更新
                record.CorrectCount = (byte?)result.CorrectCount;
                record.Score = (byte?)result.Score;
                record.SubmittedAt = DateTime.Now;

                await _chapterQuizRepository.UpdateRecordAsync(record);
            }
        }

        public async Task<ChapterQuizDTO?> GetChapterQuiz(int chapterId)
        {
            var data = await _chapterQuizRepository.GetQuizByChapterId(chapterId);

            if (data == null)
            {
                return null;
            }

            return new ChapterQuizDTO
            {
                QuizId = data.QuizId,
                Title = data.Title,
                Questions = data.QuizQuestions.Select(q => new QuizQuestionDTO
                {
                    QuestionId = q.QuestionId,
                    QuestionText = q.QuestionText,
                    Options = q.QuizOptions.Select(o => new QuizOptionDTO
                    {
                        OptionId = o.OptionId,
                        OptionText = o.OptionText,
                    }).ToList()
                }).ToList()
            };
        }

        public async Task<double> GetCourseProgress(int memberId, int courseDetailId)
        {
            // 查詢會員的課程測驗紀錄
            int quizRecordCount = await _quizRecordRepository.CountQuizRecord(memberId, courseDetailId);
            
            // 若沒有紀錄，回傳無紀錄
            if (quizRecordCount <= 0)
            {
                return 0;
            }

            // 查詢課程的章節總數
            int chapterCount = await _courseRepository.GetChapterCount(courseDetailId);

            // 計算測驗完成度
            double progress = (double)quizRecordCount / chapterCount * 100;

            return progress;
        }

        public async Task<Dictionary<int, double>> GetCourseProgressBatch(int memberId, List<int> courseDetailIds)
        {
            // 查詢會員完成的課程章節數
            var completedRaw = await _quizRecordRepository.GetCompletedCourseRaw(memberId, courseDetailIds);
            // 查詢課程總章節數
            var chapterCountRaw = await _courseRepository.GetChapterCountRaw(courseDetailIds);

            // 轉 Dictionary
            var completedMap = completedRaw.ToDictionary(
                x => x.CourseDetailId,
                x => x.CompletedCount);

            var totalMap = chapterCountRaw.ToDictionary(
                x => x.CourseDetailId,
                x => x.ChapterCount);

            var result = new Dictionary<int, double>();

            // 取 courseDetailId 做計算
            foreach (var courseDetailId in courseDetailIds)
            {
                var completed = completedMap.TryGetValue(courseDetailId, out var c) ? c : 0;
                var total = totalMap.TryGetValue(courseDetailId, out var t) ? t : 0;

                double progress = (total == 0) ? 0 : (double)completed / total * 100;

                result[courseDetailId] = progress;
            }

            return result;
        }

        public async Task<QuizResultDTO> GradeQuizAsync(QuizSubmissionDTO submission)
        {
            // 取得正確答案的字典
            var correctAnswers = await _chapterQuizRepository.GetCorrectAnswerAsync(submission.QuizId);

            int totalQuestions = correctAnswers.Count; // 拿總題數
            int correctCount = 0; // 計算正確答案數量
            List<QuizFeedbackDTO> feedbacks = new List<QuizFeedbackDTO>();

            foreach (var answer in submission.Answers) // 遍歷使用者的答案
            {
                bool isCorrect = false;
                int correctId = 0;
                if (correctAnswers.TryGetValue(answer.QuestionId, out correctId)) // 安全檢查有沒有找到題目
                {
                    isCorrect = (answer.SelectedOptionId == correctId);
                    if (isCorrect)
                    {
                        correctCount++; // 正確答案數量加一
                    }
                }
                // 建立回饋紀錄
                feedbacks.Add( new QuizFeedbackDTO(
                    answer.QuestionId,
                    correctId,
                    isCorrect,
                    answer.SelectedOptionId));
                
            }
            int score = totalQuestions > 0 ? // 計算分數
                (int)Math.Round((double)correctCount / totalQuestions * 100) : 0;

            return new QuizResultDTO(
                score,
                correctCount,
                totalQuestions,
                feedbacks);
        }

        public async Task<bool> IsCompleted(int memberId, int CourseDetailId)
        {
            // 查詢章節總數
            var chapterCount = await _courseRepository.GetChapterCount(CourseDetailId);

            // 查詢會員已完成的章節數量
            var quizRecordCount = await _chapterQuizRepository.GetQuizRecordCountAsync(memberId, CourseDetailId);

            if (chapterCount != quizRecordCount)
            {
                return false;
            }
            
            return true;
        }
    }
}
