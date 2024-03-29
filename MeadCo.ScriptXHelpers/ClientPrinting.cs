﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Optimization;
using System.Web.UI;
using MeadCo.ScriptX;
using MeadCo.ScriptXClient.Extensions;
using MeadCo.ScriptXClient.Helpers;

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
            Redirect = 2, // yes and redirect to the install helper
            NoneAsync = 10, // no and the scriptx.services libraries use async initialisation
            RedirectAsync = 12 // yes and async initialisation is attempted to determine if implementation is available
        }

        /// <summary>
        /// Model required by the exemplar/default install helper partial view/control
        /// </summary>
        public class InstallUiModel
        {
            public InstallUiModel(InstallScope scope)
            {
                ManualInstallers = ConfigProviders.CodebaseFinder.Find(UserAgent).ToDictionary(i => i.Scope, i => i.ManualInstallerDownloadUrl);

                // determine if there is a bits provider for the alternative scope
                Scope = scope;
                AlternativeScope = scope == InstallScope.Machine ? InstallScope.User : InstallScope.Machine;
                if (ConfigProviders.CodebaseFinder.FindSingle(AlternativeScope, UserAgent) == default(IBitsProvider) )
                    AlternativeScope = scope;

            }

            /// <summary>
            /// the id of the element displayed while the page is loading
            /// </summary>
            public string LoadingElementId { get; set; }
            /// <summary>
            /// the id of the element to display when installation succeeded
            /// </summary>
            public string SuccessElementId { get; set; }

            /// <summary>
            /// name of the javascript method to call when install succeeds
            /// </summary>
            public string OnSuccess { get; set; }

            /// <summary>
            /// The installation scope to use on this install attempt
            /// </summary>
            public InstallScope Scope { get; private set; }

            /// <summary>
            /// The available manual installers for the client processor architecture
            /// Dictionary of scope by download url.
            /// </summary>
            public Dictionary<InstallScope,string> ManualInstallers { get; private set; }

            public InstallScope AlternativeScope { get; private set; }

        }

        public ClientPrinting()
        {
            InstallHelperUrl = Configuration.ClientInstaller.InstallHelperUrl;
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
        /// <param name="installHelper">hyperlink to the install helper page (for IE)</param>
        /// <param name="clientValidationAction">Action to take if ScriptX is not installed (for IE)</param>
        /// <param name="htmlPrintProcessor">Print template to use</param>
        /// <param name="printSettings">Settings to be applied before any print.</param>
        /// <param name="clientId">id to assign to the scriptx factory object</param>
        /// <returns></returns>
        public static HtmlString GetHtml(string installHelper = "",
            ValidationAction clientValidationAction = ValidationAction.None,
            ScriptXHtmlPrintProcessors htmlPrintProcessor = ScriptXHtmlPrintProcessors.Default,
            PrintSettings printSettings = null,
            string clientId = "factory")
        {
            // determine the (default) install scope to use
            // defined as the first/best available in web.config for the client agent ...
            string userAgent = UserAgent;
            IBitsProvider provider = ConfigProviders.CodebaseFinder.Find(userAgent).FirstOrDefault();
            InstallScope scope = InstallScope.User;

            // no add-on provider for this agent then is ScriptX.Print available
            if (provider == default(IBitsProvider))
            {
                // Note: We may have no add-on available, scriptx.print available but agent is IE 10 or less (yes, this is unlikely)
                // so have to check scriptx.print is usable for the agent as well as generally available.
                if ( ConfigProviders.PrintServiceProvider.Availability == ServiceConnector.None || !ConfigProviders.PrintServiceProvider.UseForAgent(userAgent) )
                    return new HtmlString("");

                // by definition the 'scope' for scriptx.print is user
            }
            else
            {
                scope = provider.Scope;
            }

            return GetHtmlForScope(scope,installHelper, clientValidationAction, htmlPrintProcessor, printSettings, clientId);    
        }

        /// <summary>
        /// Returns object tag(s) declaring license manager and scriptx factory on the page.
        /// </summary>
        /// <param name="installHelper">hyperlink to the install helper page</param>
        /// <param name="clientValidationAction">Action to take if ScriptX is not installed</param>
        /// <param name="htmlPrintProcessor">Print template to use</param>
        /// <param name="printSettings">Settings to be applied before any print.</param>
        /// <param name="clientId">id to assign to the scriptx factory object</param>
        /// <param name="scope">Use a per machine or per user installer</param>
        /// <returns></returns>
        public static HtmlString GetHtmlForScope(MeadCo.ScriptX.InstallScope scope,string installHelper = "",
            ValidationAction clientValidationAction = ValidationAction.None,
            ScriptXHtmlPrintProcessors htmlPrintProcessor = ScriptXHtmlPrintProcessors.Default,
            PrintSettings printSettings = null,
            string clientId = "factory")
        {
            const string wrapperScriptsBundleName = "~/bundles/wrapscriptx";
            const string dotPrintScriptsBundleName = "~/bundles/scriptxdotprint";
            const string promiseBundle = "~/bundles/scriptxpromise";

            // Dependency: Microsoft.aspnet.web.optimization
            // IHtmlString x = System.Web.Optimization.Scripts.Render("/");
            // BundleTable.Bundles.GetBundleFor()
            Bundle b = BundleTable.Bundles.GetBundleFor(wrapperScriptsBundleName);
            if (b == null)
            {
                BundleTable.Bundles.Add(new ScriptBundle(wrapperScriptsBundleName).Include("~/Scripts/meadco-scriptx-{version}.js"));
            }

            // and a bundle for ScriptX.Print which includes the factory anti-polyfill which will be used by the wrapper
            b = BundleTable.Bundles.GetBundleFor(dotPrintScriptsBundleName);
            if (b == null)
            {
                BundleTable.Bundles.Add(new ScriptBundle(dotPrintScriptsBundleName).Include("~/Scripts/MeadCo.ScriptX/meadco-core-{version}.js")
                .Include("~/Scripts/MeadCo.ScriptX/meadco-scriptxprint-{version}.js")
                .Include("~/Scripts/MeadCo.ScriptX/meadco-scriptxprinthtml-{version}.js")
                .Include("~/Scripts/MeadCo.ScriptX/meadco-scriptxprintpdf-{version}.js")
                .Include("~/Scripts/MeadCo.ScriptX/meadco-scriptxprintlicensing-{version}.js")
                .Include("~/Scripts/MeadCo.ScriptX/meadco-scriptxfactory-{version}.js")
                .Include("~/Scripts/MeadCo.ScriptX/meadco-secmgr-{version}.js"));
            }

            // and a bundle for a promise polyfill which will be required by IE 11
            b = BundleTable.Bundles.GetBundleFor(promiseBundle);
            if (b == null)
            {
                BundleTable.Bundles.Add(new ScriptBundle(promiseBundle).Include("~/Scripts/MeadCo.ScriptX/promise.js"));
            }

            StringWriter sOut = new StringWriter();
            HtmlTextWriter output = new HtmlTextWriter(sOut);

            StringBuilder markup = new StringBuilder(System.Web.Optimization.Scripts.Render(wrapperScriptsBundleName).ToString());

            bool bUseAddOn = !ConfigProviders.PrintServiceProvider.UseForAgent(UserAgent);

            if (bUseAddOn)
            {
                ILicenseProvider lic = ConfigProviders.LicenseProvider;
                bool isLicensed = lic.IsLicensed;

                // if validate is redirect then for this page just output the #Version so we get version checked but dont attempt
                // to install if it is the wrong version or not yet installed - the page redirected to will do that.

                // find a bits provider for the client processor for the request install scope
                // if we cant find one then return empty string  - we have nothing that can be installed.
                IBitsFinder codeBitsFinder = ConfigProviders.CodebaseFinder;

                IBitsProvider bitsProvider = codeBitsFinder.FindSingle(scope, UserAgent);
                if (bitsProvider == default(IBitsProvider))
                {
                    return new HtmlString("");
                }

                // by definition IE and to use some wrapper functions requires promise implementation.
                markup.AppendLine(System.Web.Optimization.Scripts.Render(promiseBundle).ToString());

                string codebase = clientValidationAction == ValidationAction.Redirect || clientValidationAction == ValidationAction.RedirectAsync
                    ? $"#Version={bitsProvider.CodebaseVersion}"
                    : bitsProvider.CodeBase;

                output.AddStyleAttribute("display", "none");
                output.RenderBeginTag(HtmlTextWriterTag.Div);

                // if licensed then we need to output security manager with the codebase and license..
                if (isLicensed)
                {
                    output.AddAttribute("classid", "clsid:" + SMclsid);

                    if (!string.IsNullOrEmpty(codebase))
                        output.AddAttribute("codebase", codebase, false);

                    output.AddAttribute(HtmlTextWriterAttribute.Id, "secmgr");

                    output.RenderBeginTag(HtmlTextWriterTag.Object);

                    output.WriteBeginTag("param");
                    output.WriteAttribute("name", "GUID");
                    output.WriteAttribute("value", lic.Guid.ToString("B"));
                    output.Write(HtmlTextWriter.SelfClosingTagEnd);
                    output.WriteLine();

                    output.WriteBeginTag("param");
                    output.WriteAttribute("name", "Revision");
                    output.WriteAttribute("value", lic.Revision.ToString());
                    output.Write(HtmlTextWriter.SelfClosingTagEnd);
                    output.WriteLine();

                    output.WriteBeginTag("param");
                    output.WriteAttribute("name", "Path");
                    output.WriteAttribute("value", Url.ResolveUrl(lic.FileName), false);
                    output.Write(HtmlTextWriter.SelfClosingTagEnd);
                    output.WriteLine();

                    output.WriteBeginTag("param");
                    output.WriteAttribute("name", "PerUser");
                    output.WriteAttribute("value", lic.PerUser.ToString());
                    output.Write(HtmlTextWriter.SelfClosingTagEnd);

                    output.RenderEndTag();
                    output.WriteLine();
                }

                // always output factory
                if (!isLicensed)
                {
                    if (!string.IsNullOrEmpty(codebase))
                        output.AddAttribute("codebase", codebase, false);
                }
                else
                {
                    // licensed, assume codebased somehow, ensure we request the required version of factory
                    // in case a lower version is installed
                    codebase = bitsProvider.CodebaseVersion;
                    if (!string.IsNullOrEmpty(codebase))
                        output.AddAttribute("codebase", $"#Version={bitsProvider.CodebaseVersion}");
                }

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

                if (clientValidationAction == ValidationAction.Redirect || clientValidationAction == ValidationAction.RedirectAsync)
                {
                    string helper = installHelper;

                    if (string.IsNullOrEmpty(helper))
                    {
                        helper = Configuration.ClientInstaller.InstallHelperUrl;
                        if (string.IsNullOrEmpty(helper))
                        {
                            // not overridden anywhere so use the default
                            // installed with this package.
                            helper = Url.ResolveUrl("~/ScriptXClientPrinting/Install");
                        }
                    }

                    markup.AppendScript(ScriptSnippets.BuildInstallOkCode(clientId, helper, scope));
                }
            }
            else
            {
                if ( ConfigProviders.PrintServiceProvider.Availability == ServiceConnector.None)
                {
                    return new HtmlString("");
                }

                // not IE add-on, using ScriptX.Print ...
                markup.AppendLine(System.Web.Optimization.Scripts.Render(dotPrintScriptsBundleName).ToString());
                if (MeadCo.ScriptX.Helpers.AgentParser.IsInternetExplorer(UserAgent))
                {
                    markup.AppendLine(System.Web.Optimization.Scripts.Render(promiseBundle).ToString());
                }

                // if Cloud, then just need to connect the subscription
                // If Workstation, then requesting that the license is installed ..
                if (ConfigProviders.PrintServiceProvider.Availability == ServiceConnector.Cloud)
                {
                    markup.AppendScript(
                        ScriptSnippets.BuildDotPrintLicenseDetail(
                            ConfigProviders.PrintServiceProvider.LicenseService.ToString(),
                            ConfigProviders.PrintServiceProvider.Guid.ToString("B")));

                    markup.AppendScript(
                        ScriptSnippets.BuildDotPrintInitialisation(clientValidationAction == ValidationAction.NoneAsync || clientValidationAction == ValidationAction.RedirectAsync,
                            ConfigProviders.PrintServiceProvider.PrintHtmlService.ToString(),
                            ConfigProviders.PrintServiceProvider.Guid.ToString("B")));
                }
                else
                {
                    if (clientValidationAction == ValidationAction.None ||
                        clientValidationAction == ValidationAction.Redirect)
                    {
                        markup.AppendScript(
                            ScriptSnippets.BuildDotPrintInstallLicense(
                                false,
                                ConfigProviders.PrintServiceProvider.LicenseService.ToString(),
                                ConfigProviders.PrintServiceProvider.PrintHtmlService.ToString(),
                                ConfigProviders.PrintServiceProvider.Guid.ToString("B"),
                                ConfigProviders.PrintServiceProvider.FileName,
                                ConfigProviders.PrintServiceProvider.Revision));

                        markup.AppendScript(
                            ScriptSnippets.BuildDotPrintInitialisation(false,
                                ConfigProviders.PrintServiceProvider.PrintHtmlService.ToString(),
                                ConfigProviders.PrintServiceProvider.Guid.ToString("B")));
                    }
                    else
                    {
                        markup.AppendScript(
                            ScriptSnippets.BuildDotPrintInstallLicense(
                                true,
                                ConfigProviders.PrintServiceProvider.LicenseService.ToString(),
                                ConfigProviders.PrintServiceProvider.PrintHtmlService.ToString(),
                                ConfigProviders.PrintServiceProvider.Guid.ToString("B"),
                                ConfigProviders.PrintServiceProvider.FileName,
                                ConfigProviders.PrintServiceProvider.Revision));
                    }
                }


            }

            if (printSettings != null)
            {
                bool bCheckLicense;

                // check if fWPC (=> cloud on Windows)
                // or Addon and License required.
                if (bUseAddOn)
                {
                    bCheckLicense = ConfigProviders.LicenseProvider.IsLicensed;
                }
                else
                {
                    bCheckLicense = ConfigProviders.PrintServiceProvider.Availability == ServiceConnector.Windows;
                }
                markup.AppendScript(printSettings.BuildPrintSettingsCode(bCheckLicense));
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

        /// <summary>
        /// Returns the url for attempt to install via codebase in the scope 
        /// </summary>
        /// <param name="scope">scope : machine or user</param>
        /// <returns></returns>
        public static string GetTryAlternativeInstallerRef(InstallScope scope)
        {
            return Configuration.ClientInstaller.InstallHelperUrl + "?scope=" + scope;
        }

        private static string UserAgent
        {
            get
            {
                return System.Web.HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"];
            }
        }
    }
}
