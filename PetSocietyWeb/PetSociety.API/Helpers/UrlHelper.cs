namespace PetSociety.API.Helpers
{
    public static class UrlHelper
    {
        /// <summary>
        /// Combines the base image URL with the specified relative path to create a full image URL.
        /// </summary>
        /// <param name="imgUrl">The relative path of the image to append to the base image URL. This value should not be null or empty.</param>
        /// <returns>The full URL string representing the absolute path to the image.</returns>
        public static string ImgFullUrl(string imgUrl, string param)
        {
            // 圖片存放路徑 MVC 專案
            string baseImgUrl = $"https://localhost:7032/img/{param}/";

            if (string.IsNullOrWhiteSpace(imgUrl))
            {
                return string.Empty;
            }

            return $"{baseImgUrl}{imgUrl}";
        }

        public static string VideoFullUrl(string videoUrl)
        {
            string baseVideoUrl = "https://www.youtube.com/embed/";
            if (string.IsNullOrEmpty(videoUrl))
            {
                return string.Empty;
            }
            return $"{baseVideoUrl}{videoUrl}";
        }
    }
}
