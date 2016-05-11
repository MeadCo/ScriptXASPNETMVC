using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeadCo.ScriptX;

namespace MeadCo.ScriptXClient
{
    /// <summary>
    /// Enable using alternative bits providers (code downloads).
    /// 
    /// The default provider uses data obtained from web.config which
    /// will specify where the cab is located.
    /// 
    /// MeadCo use this with the samples system to use a provider that accesses
    /// the codestore.
    /// </summary>
    public static class ConfigurationProvider
    {
        private static IMeadCoBinaryBitsProvider _provider = null;
        public static IMeadCoBinaryBitsProvider CodebaseProvider
        {
            get { return _provider ?? (_provider = Configuration.ClientInstaller); }
            set { _provider = value; }
        } 
    }
}
