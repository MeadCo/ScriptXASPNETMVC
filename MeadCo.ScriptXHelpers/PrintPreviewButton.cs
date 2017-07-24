using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using MeadCo.ScriptXClient.Helpers;

namespace MeadCo.ScriptXClient
{
    public class PrintPreviewButton
    {
        public static HtmlString GetHtml(string text = "Preview ...", string frame = null, object htmlAttributes = null)
        {
            return GetHtml(text, frame,htmlAttributes.ToDictionary());
        }

        public static HtmlString GetHtml(string text, string frame, IDictionary<string, object> htmlAttributes=null)
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
                html.Append(" onclick=\"try { MeadCo_ScriptX_Settings(); } catch(e) {} MeadCo.ScriptX.PreviewPage(); return false;\"");
            }
            else
            {
                html.Append(" onclick=\"try { MeadCo_ScriptX_Settings(); } catch(e) {} MeadCo.ScriptX.PreviewFrame('" + frame + "'); return false;\"");
            }

            html.Append(string.Format(CultureInfo.InvariantCulture, ">{0}</button>", text));

            return new HtmlString(html.ToString());
        }


    }
}
