using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PetSocietyWeb.Areas.Class.Extensions
{
    public static class SelectListExtensions
    {
        public static IEnumerable<SelectListItem> InsertEmpty(this List<SelectListItem> list)
        {
            // 新增空白下拉選項
            list.Insert(0, new SelectListItem
            {
                Value = string.Empty,
                Text = "-- 無 --"
            });
            return list;
        }
    }
}
