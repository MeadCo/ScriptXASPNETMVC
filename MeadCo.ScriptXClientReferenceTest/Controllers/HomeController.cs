using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MeadCo.ScriptXClientReference.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult InstallInPage()
        {
            return View();
        }

        public ActionResult InstallWithRedirect()
        {
            return View();
        }

        public ActionResult TestNoFooter()
        {
            return View();
        }

        public ActionResult TestBlankFooter()
        {
            return View();
        }

        public ActionResult TestButtonsWithNoSettings()
        {
            return View();
        }
    }
}