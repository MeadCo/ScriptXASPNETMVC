using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;

namespace MeadCo.ScriptXClient.Library
{
    internal class Helpers
    {
        /// <summary>
        /// Replaces underscore characters (_) with hyphens (-) in the specified HTML attributes.
        /// </summary>
        /// 
        /// <returns>
        /// The HTML attributes with underscore characters replaced by hyphens.
        /// </returns>
        /// <param name="htmlAttributes">The HTML attributes.</param>
        internal static RouteValueDictionary AnonymousObjectToHtmlAttributes(object htmlAttributes)
        {
            RouteValueDictionary routeValueDictionary = new RouteValueDictionary();
            if (htmlAttributes != null)
            {
                foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(htmlAttributes))
                    routeValueDictionary.Add(propertyDescriptor.Name.Replace('_', '-'), propertyDescriptor.GetValue(htmlAttributes));
            }
            return routeValueDictionary;
        }
    }
}
