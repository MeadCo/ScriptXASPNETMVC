using System;
using System.Text;
using MeadCo.ScriptX;

namespace MeadCo.ScriptXClient.Helpers
{
    internal static class ScriptSnippets
    {
        /// <summary>
        /// builds a function to check if scriptx is installed and if not redirect 
        /// the browser client to the given url
        /// </summary>
        /// <param name="sFactoryObjectId"></param>
        /// <param name="sRedirectUri"></param>
        /// <returns></returns>
        public static StringBuilder BuildInstallOkCode(String sFactoryObjectId, string sRedirectUri,InstallScope scope)
        {
            if (string.IsNullOrEmpty((sRedirectUri)))
            {
                throw new ArgumentException("A redirect url is required");    
            }

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("function MeadCo_ScriptX_CheckInstalled() {");
            sb.AppendLine("var f = document.getElementById(\"" + sFactoryObjectId + "\");");
            sb.AppendLine("if ( typeof f === \"undefined\" || f==null || f.object == null ) {");
            sb.AppendLine("window.location = \"" + sRedirectUri + "?scope=" + scope + "\";");
            sb.AppendLine("}");
            sb.AppendLine("}");
            sb.AppendLine("if ( window.addEventListener ) { window.addEventListener('load',MeadCo_ScriptX_CheckInstalled,false); } else { window.attachEvent(\"onload\",MeadCo_ScriptX_CheckInstalled); }");
            return sb;
        }

        public static StringBuilder BuildDotPrintInitialisation(string htmlServerEndPoint,string subscriptionId)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("function MeadCo_ScriptX_Connect() {");
            sb.Append("MeadCo.ScriptX.Print.HTML.connect('");
            sb.Append(htmlServerEndPoint);
            sb.Append("','");
            sb.Append(subscriptionId);
            sb.AppendLine("');");
            sb.AppendLine("}");
            sb.AppendLine("window.addEventListener('load',MeadCo_ScriptX_Connect,false);");
            return sb;
        }

    }
}
