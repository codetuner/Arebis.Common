using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Configuration;
using System.Globalization;
using System.Threading;
using System.Web.Routing;
using System.Web;

namespace Arebis.Web.Mvc
{
    /// <summary>
    /// A Controller factory that applies localization by setting threads Current[UI]Culture.
    /// Localization is done based on browser settings and user selection, and stored
    /// in the Session variable. Supported languages must be defined in the AppSetting
    /// "SupportedCultures".
    /// </summary>
    public class LocalizingControllerFactory : IControllerFactory
    {
        private const string LanguageKeyName = "__language";
        private const string LanguageParameterName = "ui_language";

        private static volatile List<CultureInfo> _supportedCultures;
        private static volatile object _cookieValidityDuration;

        private bool _createPersistentCookie;
        private IControllerFactory _innerFactory;

        public LocalizingControllerFactory(IControllerFactory innerFactory)
            : this(false, innerFactory)
        { }

        public LocalizingControllerFactory(bool createPersistentCookie, IControllerFactory innerFactory)
        {
            this._innerFactory = innerFactory;
            this._createPersistentCookie = createPersistentCookie;
        }

        #region IControllerFactory Members

        public IController CreateController(RequestContext requestContext, string controllerName)
        {
            // Handle localization:
            HandleLocalization(requestContext);

            // Forward to inner factory:
            return _innerFactory.CreateController(requestContext, controllerName);
        }

        public void ReleaseController(IController controller)
        {
            // Forward to inner factory:
            _innerFactory.ReleaseController(controller);
        }

        public System.Web.SessionState.SessionStateBehavior GetControllerSessionBehavior(RequestContext requestContext, string controllerName)
        {
            return System.Web.SessionState.SessionStateBehavior.Default;
        }

        #endregion

        #region Localization implementation

        protected internal virtual void HandleLocalization(RequestContext requestContext)
        {
            // Retrieve default language:
            string language = GetLanguage(requestContext);

            // Overwrite language from request if requested:
            if (requestContext.HttpContext.Request[LanguageParameterName] != null)
                language = requestContext.HttpContext.Request[LanguageParameterName];

            // Identify culture:
            CultureInfo selectedCulture = IdentifyCulture(language);

            // Set culture:
            SetCulture(requestContext, selectedCulture);

            // Write/update language cookie:
            if (_createPersistentCookie)
                SetLanguageCookie(requestContext, selectedCulture);
        }

        protected internal virtual string GetLanguage(RequestContext requestContext)
        {
            // Get language from session variable:
            if (!String.IsNullOrEmpty((string)requestContext.HttpContext.Session[LanguageKeyName]))
                return (string)requestContext.HttpContext.Session[LanguageKeyName];

            // Else, get first supported language from browser:
            if (requestContext.HttpContext.Request.UserLanguages != null)
            {
                foreach (var userLang in requestContext.HttpContext.Request.UserLanguages)
                {
                    if ((!String.IsNullOrEmpty(userLang)) && (userLang.Length >= 2))
                    {
                        String twoLetterLang = userLang.Substring(0, 2);
                        if (GetCultureForLanguage(twoLetterLang, null) != null)
                            return twoLetterLang;
                    }
                }
            }

            // Else, get language from cookie (if any):
            HttpCookie langCookie = GetLanguageCookie(requestContext);
            if (langCookie != null)
                return langCookie.Value;

            // Else, no language identified:
            return null;
        }

        protected internal virtual void SetLanguage(RequestContext requestContext, string language)
        {
            requestContext.HttpContext.Session[LanguageKeyName] = language;
        }

        protected internal virtual CultureInfo IdentifyCulture(string language)
        {
            // Get culture for language:
            return GetCultureForLanguage(language, SupportedCultures[0]);
        }

        protected internal virtual HttpCookie GetLanguageCookie(RequestContext requestContext)
        {
            return requestContext.HttpContext.Request.Cookies[LanguageKeyName];
        }

        protected internal virtual void SetLanguageCookie(RequestContext requestContext, CultureInfo culture)
        {
            string language = culture.TwoLetterISOLanguageName;
            HttpCookie langCookie = new HttpCookie(LanguageKeyName, language);
            langCookie.Value = language;
            langCookie.Expires = DateTime.Now.Add(this.CookieValidityDuration);
            requestContext.HttpContext.Response.Cookies.Add(langCookie);
        }

        protected internal virtual void SetCulture(RequestContext requestContext, CultureInfo selectedCulture)
        {
            Thread.CurrentThread.CurrentCulture = selectedCulture;
            Thread.CurrentThread.CurrentUICulture = selectedCulture;
            requestContext.HttpContext.Session[LanguageKeyName] = selectedCulture.TwoLetterISOLanguageName;
        }

        public virtual TimeSpan CookieValidityDuration
        {
            get
            {
                if (_cookieValidityDuration == null)
                {
                    _cookieValidityDuration
                        = TimeSpan.Parse(
                            ConfigurationManager.AppSettings["LanguageCookieValidityDuration"]
                            ?? "30"
                        );
                }
                return (TimeSpan)_cookieValidityDuration;
            }
        }

        public virtual List<CultureInfo> SupportedCultures
        {
            get
            {
                if (_supportedCultures == null)
                {
                    List<CultureInfo> cultures = new List<CultureInfo>();
                    foreach (var item in (ConfigurationManager.AppSettings["SupportedCultures"] ?? "en-US").Split(',', ';'))
                        cultures.Add(new CultureInfo(item));
                    _supportedCultures = cultures;
                }

                return _supportedCultures;
            }
        }

        public CultureInfo GetCultureForLanguage(string twoLetterISOLanguageName, CultureInfo defaultCulture)
        {
            foreach (var culture in SupportedCultures)
            {
                if (culture.TwoLetterISOLanguageName.Equals(twoLetterISOLanguageName, StringComparison.InvariantCultureIgnoreCase))
                    return culture;
            }

            return defaultCulture;
        }

        #endregion
    }
}
