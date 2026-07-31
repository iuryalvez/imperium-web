using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace backend.Domain.General.Extensions
{
    public static class EnumExtensions
    {
        // Extracts the string from [Display(Name = "...")] or falls back to enum variable name
        public static string GetDisplayName(this Enum enumValue)
        {
            var enumType = enumValue.GetType();
            var memberInfo = enumType.GetMember(enumValue.ToString()).FirstOrDefault();

            if (memberInfo == null) return enumValue.ToString();

            // Attempts to retrieve the DisplayAttribute metadata attached to the enum member
            var displayAttribute = memberInfo.GetCustomAttribute<DisplayAttribute>();

            return displayAttribute?.Name ?? enumValue.ToString();
        }
    }
}