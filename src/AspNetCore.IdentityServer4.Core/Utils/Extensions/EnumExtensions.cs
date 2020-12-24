using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace AspNetCore.IdentityServer4.Core.Utils.Extensions
{
    /// <summary>
    /// Enum Extensions
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Convert enum variable to int value
        /// </summary>
        /// <param name="self">Enum</param>
        /// <returns>int</returns>
        public static int ToIntValue(this Enum self)
        {
            return Convert.ToInt16(self);
        }


        /// <summary>
        /// Get description
        /// </summary>
        /// <returns>Description</returns>
        public static string GetDescription(this Enum self)
        {
            FieldInfo fi = self.GetType().GetField(self.ToString());
            DescriptionAttribute[] attributes = null;

            if (fi != null)
            {
                attributes =
                    (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attributes != null && attributes.Length > 0)
                    return attributes[0].Description;
            }

            return self.ToString();
        }

        /// <summary>
        /// Parse string to enum
        /// </summary>
        /// <typeparam name="T">Enum Type</typeparam>
        /// <param name="self">Enum</param>
        /// <param name="value">The string which will be parsed</param>
        /// <returns>T</returns>
        public static T Parse<T>(this Enum self, string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        /// <summary>
        /// Parse description string to enum
        /// </summary>
        /// <typeparam name="T">Enum Type</typeparam>
        /// <param name="self">Enum</param>
        /// <param name="value">The description as the one in enum value's DescriptionAttribute</param>
        /// <returns>T</returns>
        public static T ParseFromDescription<T>(this Enum self, string value)
        {
            var valuesAndDescriptions = new Dictionary<T, string>();

            // Gets the Type of the enum class
            Type enumType = typeof(T);

            //Get all values and iterate through them    
            var enumValues = enumType.GetEnumValues();

            foreach (T item in enumValues)
            {
                // with our Type object we can get the information about    
                // the members of it    
                MemberInfo memberInfo =
                    enumType.GetMember(item.ToString()).First();

                // we can then attempt to retrieve the    
                // description attribute from the member info    
                var descriptionAttribute =
                    memberInfo.GetCustomAttribute<DescriptionAttribute>();

                // if we find the attribute we can access its values    
                if (descriptionAttribute != null && descriptionAttribute.Description.Equals(value))
                {
                    return item;
                }
            }

            throw new Exception($"Cannot convert {value} to type {typeof(T).Name}");
        }
    }

}
