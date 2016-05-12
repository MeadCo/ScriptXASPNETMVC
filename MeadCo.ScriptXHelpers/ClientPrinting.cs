﻿using System;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Optimization;
using System.Web.UI;
using MeadCo.ScriptX;
using MeadCo.ScriptXClient.Library;
using Microsoft.Ajax.Utilities;

namespace MeadCo.ScriptXClient
{
    /// <summary>
    /// Render the ScriptX objects to the page with required script to install and
    /// initialise the object with the required print parameters.
    /// </summary>
    public class ClientPrinting
    {
        /// <summary>
        /// Possible values for MeadCo:// template URLs
        /// </summary>
        public enum ScriptXHtmlPrintProcessors
        {
            Default = 1, // chooses the best template for the version of IE.
            Classic = 3, // the old IE 8 template, can resolve some issues with the newest template with IE11
        }

        /// <summary>
        /// should the install be validated 
        /// </summary>
        public enum ValidationAction
        {
            None = 0, // no
            // inpage=1,
            Redirect = 2 // yes and redirect to the install helper
        }

        /// <summary>
        /// Model required by the exemplar/default install helper partial view/control
        /// </summary>
        public class InstallUiModel
        {
            public string LoadingElementId { get; set; }
            public string SuccessElementId { get; set; }
            public string OnSuccess { get; set; }
        }

        public ClientPrinting()
        {
            IMeadCoBinaryBitsProvider installer = ConfigurationProvider.CodebaseProvider;

            InstallHelperUrl = installer.InstallHelper;
            HtmlPrintProcessor = ScriptXHtmlPrintProcessors.Default;
            ClientValidate = ValidationAction.None;
        }

        public string InstallHelperUrl { get; set; }
        public ScriptXHtmlPrintProcessors HtmlPrintProcessor { get; set; }
        public ValidationAction ClientValidate { get; set; }
        public string Id
        {
            get
            {
                return "factory";
            }
            set
            {
                throw new NotSupportedException("The ID for the ScriptX Client Printing cannot be specified, it must be 'factory'");
            }
        }

        private const string SXclsid = "1663ed61-23eb-11d2-b92f-008048fdd814";
        private const string SMclsid = "5445BE81-B796-11D2-B931-002018654E2E";

        private PrintSettings _printSettings = null;

        public PrintSettings PrintSettings
        {
            get { return _printSettings ?? (_printSettings = new PrintSettings()); }
        }

        /// <summary>
        /// Returns object tag(s) declaring license manager and scriptx factory on the page.
        /// </summary>
        /// <param name="installHelper">hyperlink to the install helper page</param>
        /// <param name="clientValidationAction">Action to take if ScriptX is not installed</param>
        /// <param name="htmlPrintProcessor">Print template to use</param>
        /// <param name="printSettings">Settings to be applied before any print.</param>
        /// <param name="clientId">id to assign to the scriptx factory object</param>
        /// <returns></returns>
        public static HtmlString GetHtml(string installHelper="",
            ValidationAction clientValidationAction=ValidationAction.None,
            ScriptXHtmlPrintProcessors htmlPrintProcessor=ScriptXHtmlPrintProcessors.Default,
            PrintSettings printSettings=null,
            string clientId="factory")
        {
            // Dependency: Microsoft.aspnet.web.optimization
            // IHtmlString x = System.Web.Optimization.Scripts.Render("/");
            // BundleTable.Bundles.GetBundleFor()
            Bundle b = BundleTable.Bundles.GetBundleFor("~/bundles/scriptx");
            if (b == null)
            {
                BundleTable.Bundles.Add(new ScriptBundle("~/bundles/scriptx").Include("~/Scripts/meadco-scriptx-{version}.js"));
            }

            LicenseConfiguration lic = Configuration.License;
            IMeadCoBinaryBitsProvider installer = ConfigurationProvider.CodebaseProvider;

            StringWriter sOut = new StringWriter();
            HtmlTextWriter output = new HtmlTextWriter(sOut);

            bool isLicensed = lic.IsLicensed;

            // if validate is redirect then for this page just output the #Version so we get version checked but dont attempt
            // to install if it is the wrong version or not yet installed - the page redirected to will do that.
            string codebase = (clientValidationAction == ValidationAction.Redirect ? "" : installer.CodeBase.ToString());

            output.AddStyleAttribute("display", "none");
            output.RenderBeginTag(HtmlTextWriterTag.Div);

            // if licensed then we need to output security manager with the codebase and license..
            if (isLicensed)
            {
                output.AddAttribute("classid", "clsid:" + SMclsid);

                if (!string.IsNullOrEmpty(codebase))
                    output.AddAttribute("codebase", codebase);

                output.AddAttribute(HtmlTextWriterAttribute.Id, "SecMgr");

                output.RenderBeginTag(HtmlTextWriterTag.Object);

                output.AddAttribute(HtmlTextWriterAttribute.Name, "GUID");
                output.AddAttribute(HtmlTextWriterAttribute.Value, lic.Guid.ToString());
                output.RenderBeginTag(HtmlTextWriterTag.Param);
                output.RenderEndTag();

                output.AddAttribute(HtmlTextWriterAttribute.Name, "Revision");
                output.AddAttribute(HtmlTextWriterAttribute.Value, lic.Revision.ToString());
                output.RenderBeginTag(HtmlTextWriterTag.Param);
                output.RenderEndTag();

                output.AddAttribute(HtmlTextWriterAttribute.Name, "Path");
                output.AddAttribute(HtmlTextWriterAttribute.Value, Url.ResolveUrl(lic.FileName));
                output.RenderBeginTag(HtmlTextWriterTag.Param);
                output.RenderEndTag();

                output.AddAttribute(HtmlTextWriterAttribute.Name, "PerUser");
                output.AddAttribute(HtmlTextWriterAttribute.Value, lic.PerUser.ToString());
                output.RenderBeginTag(HtmlTextWriterTag.Param);
                output.RenderEndTag();

                output.RenderEndTag();
            }

            // always output factory
            if (!isLicensed && !string.IsNullOrEmpty(codebase))
                output.AddAttribute("codebase", codebase);

            output.AddAttribute("classid", "clsid:" + SXclsid);
            output.AddAttribute(HtmlTextWriterAttribute.Id, clientId);

            output.RenderBeginTag(HtmlTextWriterTag.Object);

            if (htmlPrintProcessor != ScriptXHtmlPrintProcessors.Default)
            {
                output.AddAttribute(HtmlTextWriterAttribute.Name, "Template");
                output.AddAttribute(HtmlTextWriterAttribute.Value, "MeadCo://" + htmlPrintProcessor.ToString());
                output.RenderBeginTag(HtmlTextWriterTag.Param);
                output.RenderEndTag();
            }

            output.RenderEndTag();

            output.RenderEndTag(); // div

            StringBuilder markup = new StringBuilder(System.Web.Optimization.Scripts.Render("~/bundles/scriptx").ToString());

            if (clientValidationAction == ValidationAction.Redirect)
            {
                markup.AppendScript(ScriptSnippets.BuildInstallOkCode(clientId, string.IsNullOrEmpty(installHelper) ? installer.InstallHelper : installHelper));
            }

            if (printSettings != null)
            {
                markup.AppendScript(printSettings.BuildPrintSettingsCode());
            }

            markup.Append(sOut);
            return new HtmlString(markup.ToString());
        }

        /// <summary>
        /// Returns object tag(s) declaring license manager and scriptx factory on the page.
        /// </summary>
        /// <returns></returns>
        public HtmlString GetHtml()
        {
            return GetHtml(InstallHelperUrl, ClientValidate, HtmlPrintProcessor,_printSettings,Id);
        }
    }
}