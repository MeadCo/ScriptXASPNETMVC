using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;

namespace MeadCo.ScriptXClient.Library
{

    // credit : https://gist.github.com/jarrettmeyer/798667

    /// <summary>
    /// Convert object to dictionary of properties
    /// For use with html attributes replaces underscore characters (_) with hyphens (-).
    /// </summary>
    internal static class ObjectToDictionaryHelper
    {
        public static IDictionary<string, object> ToDictionary(this object source)
        {
            return source.ToDictionary<object>();
        }

        public static IDictionary<string, T> ToDictionary<T>(this object source)
        {
            if (source == null)
                return null;

            var dictionary = new Dictionary<string, T>();
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(source))
                AddPropertyToDictionary<T>(property, source, dictionary);

            return dictionary;
        }

        private static void AddPropertyToDictionary<T>(PropertyDescriptor property, object source, Dictionary<string, T> dictionary)
        {
            object value = property.GetValue(source);
            if (IsOfType<T>(value))
                dictionary.Add(property.Name.Replace('_', '-'), (T)value);
        }

        private static bool IsOfType<T>(object value)
        {
            return value is T;
        }

    }

}
