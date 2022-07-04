using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SwaggerWcf
{
    public static class Extensions
    {
        public static Attribute GetCustomAttribute(this MemberInfo memberInfo, Type type)
        {
            return (Attribute)memberInfo.GetCustomAttributes(type, true).FirstOrDefault();
        }
        public static TAttribute GetCustomAttribute<TAttribute>(this MemberInfo memberInfo)
        {
            return (TAttribute)memberInfo.GetCustomAttributes(typeof(TAttribute), true).FirstOrDefault();
        }
        public static TAttribute GetCustomAttribute<TAttribute>(this ParameterInfo memberInfo)
        {
            return (TAttribute)memberInfo.GetCustomAttributes(typeof(TAttribute), true).FirstOrDefault();
        }

        public static TAttribute[] GetCustomAttributes<TAttribute>(this MemberInfo memberInfo)
        {
            return memberInfo.GetCustomAttributes(typeof(TAttribute), true).Cast<TAttribute>().ToArray();
        }
        public static TAttribute[] GetCustomAttributes<TAttribute>(this ParameterInfo memberInfo)
        {
            return memberInfo.GetCustomAttributes(typeof(TAttribute), true).Cast<TAttribute>().ToArray();
        }

        public static object GetValue(this PropertyInfo propertyInfo, object obj)
        {
            return propertyInfo.GetValue(obj, null);
        }

        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrEmpty(str?.Trim());
        }

        public static string[] GetEnumNames(this Type type)
        {
            return Enum.GetNames(type);
        }
    }

    public class Tuple<T1, T2>
    {
        public T1 Item1 { get; set; }
        public T2 Item2 { get; set; }

        public Tuple(T1 Item1, T2 Item2)
        {
            this.Item1 = Item1;
            this.Item2 = Item2;
        }
    }

}
