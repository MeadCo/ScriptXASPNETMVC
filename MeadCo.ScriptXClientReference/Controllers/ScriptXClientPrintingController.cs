using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MeadCo.ScriptXClientReference.Controllers
{
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
        // Note that the View files are linked to those in the package project.
        //

        public ActionResult Install()
        {
            return View();
        }
    }
}