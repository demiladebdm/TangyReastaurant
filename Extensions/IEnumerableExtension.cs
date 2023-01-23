using Microsoft.AspNetCore.Mvc.Rendering;

namespace Tangy.Extensions
{
    public static class IEnumerableExtension
    {
        public static IEnumerable<SelectListItem> ToSelectListItem<T>(this IEnumerable<T> items, int selectedValue)
        {
            var query = from item in items
                        select new SelectListItem
                        {
                            Text = item.GetPropertyValue("Name"),
                            Value = item.GetPropertyValue("Id"),
                            Selected = item.GetPropertyValue("Id").Equals(selectedValue.ToString())
                        };
            return query.ToList();
        }
    }
}