using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeadCo.ScriptX;

namespace MeadCo.ScriptXClient
{
    /// <summary>
    /// Enable using alternative providers (code downloads and license).
    /// 
    /// The default providers use data obtained from web.config which
    /// will specify where the cab/license is located.
    /// 
    /// MeadCo use this with the samples system to use a provider that accesses
    /// the codestore and warehouse
    /// </summary>
    public static class ConfigProviders
    {
        private static IBitsFinder _bitsFinder = null;
        private static ILicenseProvider _licenseProvider = null;
        private static IPrintService _printService = null;

        public static IBitsFinder CodebaseFinder
        {
            get { return _bitsFinder ?? (_bitsFinder = Configuration.ClientInstaller); }
            set { _bitsFinder = value; }
        }

        public static ILicenseProvider LicenseProvider
        {
            get { return _licenseProvider ?? (_licenseProvider = Configuration.License); }
            set { _licenseProvider = value; }
        }

        public static IPrintService PrintServiceProvider
        {
            get { return _printService ?? (_printService = Configuration.PrintService);  }
            set { _printService = value; }
        }
    }
}
