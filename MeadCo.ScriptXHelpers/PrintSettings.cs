using System.ComponentModel;
using System.Web.UI;

namespace MeadCo.ScriptXClient
{
    public class PrintSettings
    {
        public enum MarginUnits
        {
            Default,
            Inches,
            Mm
        }

        public enum Orientation
        {
            Default,
            Landscape,
            Portrait
        }

        public class PrintMargins
        {
            [Category("Behavior"), DefaultValue("")]
            public string Left { get; set; }

            [Category("Behavior"), DefaultValue("")]
            public string Top { get; set; }

            [Category("Behavior"), DefaultValue("")]
            public string Bottom { get; set; }

            [Category("Behavior"), DefaultValue("")]
            public string Right { get; set; }
        }

        public class PaperSetup
        {
            [Category("Behavior"), DefaultValue(typeof(Orientation), "portrait")]
            public Orientation Orientation
            {
                get;
                set;
            }

            [Category("Behavior"), DefaultValue("")]
            [Description("The name of the papersize to select. Requires a license")]
            public string PaperSize { get; set; }

            [Category("Behavior"), DefaultValue("")]
            [Description("The paper tray to use. Requires a license")]
            public string PaperSource { get; set; }

            [Category("Behavior"), DefaultValue(typeof(MarginUnits), "Default")]
            public MarginUnits Units { get; set; }

            [Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty)]
            public PrintMargins Margins
            {
                get;
                set;
            }
        }

        [Category("Behavior"), DefaultValue("")]
        public string Printer
        {
            get;
            set;
        }

        [Category("Behavior"), DefaultValue(null)]
        public string Header
        {
            get;
            set;
        }


        [Category("Behavior"), DefaultValue(null)]
        public string Footer
        {
            get;
            set;
        }

        [Category("Behavior"), DefaultValue("")]
        public string HeaderfooterFont
        {
            get;
            set;
        }

        [Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty)]
        public PaperSetup PageSetup
        {
            get;
            set;
        }
    }
}
