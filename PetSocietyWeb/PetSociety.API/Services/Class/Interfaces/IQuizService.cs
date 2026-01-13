using PetSociety.API.DTOs.Class;

namespace PetSociety.API.Services.Class.Interfaces
{
    public interface IQuizService
    {
        /// <summary>
        /// Get a member's course progress
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="courseDetailId"></param>
        /// <returns></returns>
        Task<double> GetCourseProgress(int memberId, int courseDetailId);
        /// <summary>
        /// Retrieves the progress percentages for a batch of courses for the specified member.
        /// </summary>
        /// <param name="memberId">The unique identifier of the member whose course progress is to be retrieved. Must be a valid member ID.</param>
        /// <param name="courseDetailId">A list of course detail identifiers for which to retrieve progress. Cannot be null or empty.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a dictionary mapping each course
        /// detail ID to the member's progress percentage for that course. The progress value is a double between 0.0
        /// and 100.0. If a course detail ID is not found or the member has no progress, it may be omitted from the
        /// dictionary.</returns>
        Task<Dictionary<int, double>> GetCourseProgressBatch(int memberId, List<int> courseDetailId);
        /// <summary>
        /// Retrieves the quiz associated with the specified chapter.
        /// </summary>
        /// <param name="chapterId">The unique identifier of the chapter for which to retrieve the quiz. Must be a positive integer.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="ChapterQuizDTO"/>
        /// representing the quiz for the specified chapter, or <see langword="null"/> if no quiz exists for the
        /// chapter.</returns>
        Task<ChapterQuizDTO?> GetChapterQuiz(int chapterId);
        /// <summary>
        /// Grades a quiz submission asynchronously and returns the result.
        /// </summary>
        /// <param name="submission">The quiz submission to be graded. Must not be <c>null</c>.</param>
        /// <returns>A task that represents the asynchronous grading operation. The task result contains a <see
        /// cref="QuizResultDTO"/> with the grading outcome, including scores and feedback.</returns>
        Task<QuizResultDTO> GradeQuizAsync(QuizSubmissionDTO submission);
        Task CreateQuizRecordAsync(int memberId, QuizSubmissionDTO submission, QuizResultDTO result);
        Task<bool> IsCompleted(int memberId, int CourseDetailId);
    }
}
