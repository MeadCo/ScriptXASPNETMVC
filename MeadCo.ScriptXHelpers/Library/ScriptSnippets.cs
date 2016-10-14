using System;
using System.Text;
using MeadCo.ScriptX;

namespace MeadCo.ScriptXClient.Library
{
    internal static class ScriptSnippets
    {
        public static StringBuilder AppendScript(this StringBuilder sb, StringBuilder script)
        {
            sb.AppendLine("<script type=\"text/javascript\">");
            sb.Append(script);
            sb.AppendLine("</script>");
            return sb;
        }

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
            sb.AppendLine("if ( f==null || typeof(f)==\"undefined\" || f.object == null ) {");
            sb.AppendLine("window.location = \"" + sRedirectUri + "?scope=" + scope + "\";");
            sb.AppendLine("}");
            sb.AppendLine("}");
            sb.AppendLine("if ( window.addEventListener ) { window.addEventListener('load',MeadCo_ScriptX_CheckInstalled,false); } else { window.attachEvent(\"onload\",MeadCo_ScriptX_CheckInstalled); }");
            return sb;
        }

        public static StringBuilder BuildPrintSettingsCode(this PrintSettings ps)
        {
            StringBuilder sb = new StringBuilder("function MeadCo_ScriptX_Settings() { if ( MeadCo.ScriptX.Init() ) { ");

            sb.AppendLine("try {");

            if (!string.IsNullOrWhiteSpace(ps.Printer))
            {
                sb.AppendLine("MeadCo.ScriptX.Printing.printer = \"" + ps.Printer + "\";");
            }

            if (ps.PageSetup != null && ps.PageSetup.Units != PrintSettings.MarginUnits.Default)
            {
                sb.AppendLine("MeadCo.ScriptX.Printing.SetMarginMeasure(" + (ps.PageSetup.Units == PrintSettings.MarginUnits.Inches ? 2 : 1) + ");");
            }

            if ( ps.Header != null )
            {
                sb.AppendLine("MeadCo.ScriptX.Printing.header = \"" + ps.Header + "\";");
            }

            if ( ps.Footer != null )
            {
                sb.AppendLine("MeadCo.ScriptX.Printing.footer = \"" + ps.Footer + "\";");
            }

            if (!string.IsNullOrWhiteSpace(ps.HeaderfooterFont))
            {
                sb.AppendLine("MeadCo.ScriptX.Printing.headerFooterFont = \"" + ps.HeaderfooterFont + "\";");
            }

            if (ps.PageSetup != null)
            {
                // page setup
                if (ps.PageSetup.Orientation != PrintSettings.Orientation.Default)
                {
                    sb.AppendLine("if ( MeadCo.ScriptX.HasOrientation() ) { MeadCo.ScriptX.Printing.Orientation = \"" + ps.PageSetup.Orientation.ToString() + "\"; } else { MeadCo.ScriptX.Printing.portait = " + (ps.PageSetup.Orientation == PrintSettings.Orientation.Portrait).ToString().ToLower() + "; } ");
                }

                if (!string.IsNullOrWhiteSpace(ps.PageSetup.PaperSize))
                {
                    sb.AppendLine("MeadCo.ScriptX.Printing.paperSize = \"" + ps.PageSetup.PaperSize + "\";");
                }

                if (!string.IsNullOrWhiteSpace(ps.PageSetup.PaperSource))
                {
                    sb.AppendLine("if ( MeadCo.ScriptX.IsVersion('7.1.0.0') ) { MeadCo.ScriptX.Printing.paperSource2 = \"" + ps.PageSetup.PaperSource + "\";} else { MeadCo.ScriptX.Printing.paperSource = \"" + ps.PageSetup.PaperSource + "\";} ");
                }

                if (ps.PageSetup.Margins != null)
                {
                    if (!string.IsNullOrWhiteSpace(ps.PageSetup.Margins.Left))
                    {
                        sb.AppendLine("MeadCo.ScriptX.Printing.leftMargin = \"" + ps.PageSetup.Margins.Left + "\";");
                    }

                    if (!string.IsNullOrWhiteSpace(ps.PageSetup.Margins.Right))
                    {
                        sb.AppendLine("MeadCo.ScriptX.Printing.rightMargin = \"" + ps.PageSetup.Margins.Right + "\";");
                    }

                    if (!string.IsNullOrWhiteSpace(ps.PageSetup.Margins.Top))
                    {
                        sb.AppendLine("MeadCo.ScriptX.Printing.topMargin = \"" + ps.PageSetup.Margins.Top + "\";");
                    }

                    if (!string.IsNullOrWhiteSpace(ps.PageSetup.Margins.Bottom))
                    {
                        sb.AppendLine("MeadCo.ScriptX.Printing.bottomMargin = \"" + ps.PageSetup.Margins.Bottom + "\";");
                    }
                }

            }

            sb.AppendLine("} catch (e) { alert(\"Warning - print setup failed: \\n\\n\" + e.message); }");
            sb.AppendLine(" } else { console.log(\"Warning : ScriptX failed to initialise in MeadCo_ScriptX_Settings(). Has install failed?\"); } ");
            sb.AppendLine(" }");
            return sb;
        }

    }
}
