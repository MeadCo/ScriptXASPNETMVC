using System.Web.Mvc;

namespace $rootnamespace$.Controllers
{
    /// <summary>
    /// A controller implemenation to illustrate providing a helpful
    /// UI when installing ScriptX.
    /// </summary>
    public class ScriptXClientPrintingController : Controller
    {
        //
        // GET: /ScriptXClientPrinting/Install
        //
        // Causes the Add-on to be prompted for install and gives 
        // the user assistance in determining problems in the install.
        //
        // This is the default handler used by 
        //      @ClientPrinting.GetHtml(
        //          clientValidationAction:ClientPrinting.ValidationAction.Redirect);
        //
        // You can provide your own implementation.
        //

        public ActionResult Install()
        {
            return View();
        }

    }
}
