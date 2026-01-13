using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace PetSocietyWeb.Areas.Class.Extensions
{
    public static class EnumExtensions
    {
        // 處理Enum顯示DisplayName
        public static string GetDisplayName(this Enum enumValue)
        {
            var member = enumValue.GetType().GetMember(enumValue.ToString());
            var attribute = member[0].GetCustomAttribute<DisplayAttribute>();

            return attribute?.Name ?? enumValue.ToString();
        }
    }
}
