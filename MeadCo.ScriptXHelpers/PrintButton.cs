using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;
using System.Web.UI;
using MeadCo.ScriptXClient.Library;

namespace MeadCo.ScriptXClient
{
    public class PrintButton
    {
        public static HtmlString GetHtml(string text = "Print", bool prompt = true, string frame = null, object htmlAttributes=null)
        {
            return GetHtml(text, prompt, frame,htmlAttributes.ToDictionary());
        }

        public static HtmlString GetHtml(string text, bool prompt, string frame, IDictionary<string,object> htmlAttributes)
        {
            StringBuilder html = new StringBuilder("<button");

            if (htmlAttributes != null)
            {
                foreach (var pair in htmlAttributes)
                {
                    html.Append(string.Format(CultureInfo.InvariantCulture, " {0}=\"{1}\"", pair.Key, pair.Value));
                }
            }

            if (string.IsNullOrWhiteSpace(frame))
            {
                html.Append(" onclick=\"MeadCo_ScriptX_Settings(); MeadCo.ScriptX.PrintPage(" +
                            prompt.ToString().ToLower() + "); return false;\"");
            }
            else
            {
                html.Append(" onclick=\"MeadCo_ScriptX_Settings(); MeadCo.ScriptX.PrintFrame('" + frame + "'," +
                            prompt.ToString().ToLower() + "); return false;\"");
            }

            html.Append(string.Format(CultureInfo.InvariantCulture, ">{0}</button>", text));

            return new HtmlString(html.ToString());
        }

    }
}
