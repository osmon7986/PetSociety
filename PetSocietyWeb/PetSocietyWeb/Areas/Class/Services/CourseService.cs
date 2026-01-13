using GenerativeAI;
using GenerativeAI.Clients;
using GenerativeAI.Types;
using NuGet.Packaging;
using PetSocietyWeb.Areas.Class.Repositories;
using PetSocietyWeb.Areas.Class.ViewModels;
using PetSocietyWeb.Models.Generated;
using YoutubeExplode;
using YoutubeExplode.Videos.ClosedCaptions;


namespace PetSocietyWeb.Areas.Class.Services
{
    public class CourseService
    {
        private readonly IWebHostEnvironment _env;
        private readonly CourseRepository _repo;
        public CourseService(IWebHostEnvironment env, CourseRepository repo)
        {
            _repo = repo;
            _env = env;
        }

        public async Task<string> UploadCourseImageAsync(IFormFile file)
        {
            if(file == null || file.Length == 0)
            {
                return null;
            }

            // wwwroot/courses 資料夾
            string folder = Path.Combine(_env.WebRootPath, "img", "courses");

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            // 取原始檔名
            string originalName = Path.GetFileNameWithoutExtension(file.FileName);
            // 取原始副檔名
            string ext = Path.GetExtension(file.FileName);
            // 取目前日期時間
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            // 拼新檔名
            string fileName = $"{originalName}_{timestamp}{ext}";
            // 產生硬碟要寫入的路徑
            string path = Path.Combine(folder, fileName);
            // 寫入檔案
            using(var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            // 回傳檔名
            return fileName;

        }

        public async Task CreateCourseAsync(CourseFormViewModel model)
        {
            string fileName = null;

            if (model.Details.ImageFile != null)
            {
                // 圖片寫到 wwwroot & 回傳圖片檔名
                fileName = await UploadCourseImageAsync(model.Details.ImageFile);
            }

            // Mapping course
            var course = new Course
            {
                Title = model.Course.Title,
                Description = model.Course.Description,
                CategoryId = model.Course.CategoryId,
                Type = model.Course.Type,
            };
            // Mapping detail
            var detail = new CourseDetail
            {
                ImageUrl = fileName,
                Price = model.Details.Price,
                AreaId = model.Details.AreaId,
                InstructorId = model.Details.InstructorId,
                StartDate = model.Details.StartDate,
                EndDate = model.Details.EndDate,
                Status = (byte)model.Details.Status,
            };

            // Mapping chapters & videos
            // Select會迭代每一個章節資料並建立新的 CourseChapter 物件
            var chapters = model.Chapters
                .Select((ch, index) => new CourseChapter // LINQ 的 Select 會提供 index
                {
                    ChapterName = ch.ChapterName,
                    Summary = ch.Summary,
                    SortOrder = index,
                    ChapterVideos = new List<ChapterVideo>
                    {
                        new ChapterVideo {
                            VideoUrl = ch.Video.VideoUrl,
                        }
                    }

                }
            ).ToList();

            // 綁定之後 EF 會自動先新增Course > 拿到Course Id > 再新增 Detail > Chapters > Videos
            course.CourseDetails.Add(detail);
            detail.CourseChapters = chapters;

            // 寫入DB
            await _repo.CreateCourseAsync(course);
        }

        public async Task<bool> DeleteCourseAsync(int courseId)
        {
            var course = await _repo.GetCourseAsync(courseId);

            if (course == null)
            {
                return false;
            }

            // 刪圖片
            foreach (var detail in course.CourseDetails)
            {
                if (!string.IsNullOrEmpty(detail.ImageUrl))
                {
                    string path = Path.Combine(_env.WebRootPath, "img", "courses", detail.ImageUrl);
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                }
            }

            // 刪影片
            foreach (var detail in course.CourseDetails)
            {
                _repo.RemoveVideos(detail.CourseChapters);
            }
            // 刪章節
            foreach(var detail in course.CourseDetails)
            {
                _repo.RemoveChapters(detail.CourseChapters);
            }
            // 刪 detail
            _repo.RemoveDetails(course.CourseDetails);
            // 刪 course
            _repo.RemoveCourse(course);
            // 儲存 DB
            await _repo.SaveAsync();

            return true;
        }

        /// <summary>
        /// 用 Youtube 影片 ID 取得字幕
        /// </summary>
        /// <param name="videoId"></param>
        /// <returns></returns>
        public async Task<string> GetVideoCaptionAsync(string videoId)
        {
            try
            {
                // using YoutubeExplode
                YoutubeClient youtube = new YoutubeClient();

                // 取得字幕軌列表(所有語言)
                ClosedCaptionManifest track = await youtube.Videos.ClosedCaptions.GetManifestAsync(videoId);

                var trackInfo = track.Tracks.FirstOrDefault(
                                    t => t.Language.Code.StartsWith("zh") || t.Language.Name.Contains("Chinese"));

                // 如果仍然找不到，就使用第一個可用的字幕軌
                if (trackInfo == null)
                    trackInfo = track.Tracks.FirstOrDefault();

                // 取得字幕內容
                ClosedCaptionTrack captionTrack = await youtube.Videos.ClosedCaptions.GetAsync(trackInfo);
                // 取單一字幕物件
                var captions = captionTrack.Captions;
                var captionText = String.Join(" ", captions.Select(c => c.Text));
            
                return captionText;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return $"取得字幕失敗{ex.Message}";
            }
           
        }

        public async Task<string> GenerateSummaryAsync(string caption)
        {
            string apiKey = "your_key";
            
            // 透過 Generative AI SDK 建立 Gemini 2.5 Flash 模型的客戶端
            GenerativeModel client = new GenerativeModel(apiKey, "gemini-2.5-flash");
            // Prompt
            var prompt = $"請將以下影片字幕內容濃縮成重點摘要（繁體中文），" +
                     $"語氣用吸引人閱讀的課程摘要。" +
                     $"請直接輸出摘要內容，不要添加任何開頭或引導語句，也不要包含創作者名稱。" +
                     $"摘要的總字數應在 200 到 300 個中文字之間。" +
                     $"且以單行、最小化格式呈現，不要包含任何換行符號（\n）或任何 Markdown 標記（如**、##、* 等）的純文本格式。" +
                     $"\n\n{caption}";
            GenerateContentResponse response = await client.GenerateContentAsync(prompt);
            string rawResponse = response.Text();
            string trimResponse = rawResponse.Trim(); 
            return trimResponse;
        }
    }
}
