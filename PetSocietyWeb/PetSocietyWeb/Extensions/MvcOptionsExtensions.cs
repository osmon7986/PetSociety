using Microsoft.AspNetCore.Mvc;

namespace PetSocietyWeb
{
    public static class MvcOptionsExtensions
    {
        // 這是一個擴充方法，專門用來設定 MVC 的選項
        public static void AddCustomMvcOptions(this IServiceCollection services)
        {
            services.AddControllersWithViews(options =>
            {
                // 1. 修正 "The field ... must be a number." (後端轉型錯誤)
                options.ModelBindingMessageProvider.SetValueMustBeANumberAccessor(
                    field => $"欄位 {field} 必須是數字。");

                // 2. 修正其他無效數值格式
                options.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor(
                    (value, field) => $"數值 '{value}' 無效。");

                // 3. 修正未知的無效錯誤
                options.ModelBindingMessageProvider.SetUnknownValueIsInvalidAccessor(
                    field => $"數值無效。");
            });
        }
    }
}