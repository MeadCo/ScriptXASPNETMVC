using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeadCo.ScriptXClient.Extensions
{
    internal static class StringBuilderExtensions
    {
        public static StringBuilder AppendScript(this StringBuilder sb, StringBuilder script)
        {
            sb.AppendLine("<script type=\"text/javascript\">");
            sb.Append(script);
            sb.AppendLine("</script>");
            return sb;
        }
    }
}
