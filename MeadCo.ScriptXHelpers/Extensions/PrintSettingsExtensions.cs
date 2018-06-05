using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeadCo.ScriptXClient.Extensions
{
    internal static class PrintSettingsExtensions
    {
        public static StringBuilder BuildPrintSettingsCode(this PrintSettings ps,bool bLicensed)
        {
            StringBuilder sb = new StringBuilder();

            if (bLicensed)
            {
                sb.Append("function MeadCo_ScriptX_Settings() { if ( MeadCo.Licensing.IsLicensed() ) { if ( MeadCo.ScriptX.Init() ) { ");
            }
            else
            {
                sb.Append("function MeadCo_ScriptX_Settings() { if ( MeadCo.ScriptX.Init() ) { ");
            }

            sb.AppendLine("try {");

            if (!string.IsNullOrWhiteSpace(ps.Printer))
            {
                sb.AppendLine("MeadCo.ScriptX.Printing.printer = \"" + ps.Printer + "\";");
            }

            if (ps.PageSetup != null && ps.PageSetup.Units != PrintSettings.MarginUnits.Default)
            {
                sb.AppendLine("MeadCo.ScriptX.Printing.SetMarginMeasure(" + (ps.PageSetup.Units == PrintSettings.MarginUnits.Inches ? 2 : 1) + ");");
            }

            if (ps.Header != null)
            {
                sb.AppendLine("MeadCo.ScriptX.Printing.header = \"" + ps.Header + "\";");
            }

            if (ps.Footer != null)
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
            if (bLicensed)
            {
                sb.AppendLine("} else { MeadCo.Licensing.ReportError(); }");
            }
            sb.AppendLine(" }");
            return sb;
        }

    }
}
