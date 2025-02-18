using System.ComponentModel;

namespace MyFirestoreApi.Enums
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var field = value.GetType().GetField(value.ToString());
            if (field == null)
                return value.ToString(); // or handle the case where the field is null

            var attribute = (DescriptionAttribute?)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));

            return attribute?.Description ?? value.ToString();
        }
    }
}