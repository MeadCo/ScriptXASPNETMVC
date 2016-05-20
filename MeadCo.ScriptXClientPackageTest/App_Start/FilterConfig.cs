using System.Web;
using System.Web.Mvc;

namespace MeadCo.ScriptXClientPackage
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
