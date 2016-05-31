using Arebis.Extensions;
using System;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace Arebis.Web.Mvc
{
    /// <summary>
    /// Allows cross-origin HTTP requests.
    /// When an 'Origin' request header is found that matches the given originPattern, a matching
    /// 'Access-Control-Allow-Origin' response header is added.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class AllowCORSAttribute : ActionFilterAttribute
    {
        private static string[] cachedDefaultOriginPatterns = null;
        private static Regex[] cachedDefaultOriginPatternRegexs = null;

        /// <summary>
        /// Marks the controller or action to allow cross-origin HTTP requests. By default, all CORS requests are
        /// accepted, but the appSetting "DefaultAllowCORSOrigins" can contain a comma-separated list of origin
        /// patterns to restrict to.
        /// I.e: &lt;add key="DefaultAllowCORSOrigins" value="*.example.com, *.arebis.be"/&gt;
        /// </summary>
        public AllowCORSAttribute()
        {
            if (cachedDefaultOriginPatterns == null)
            {
                lock(this.GetType())
                {
                    if (cachedDefaultOriginPatterns == null)
                    {
                        var origins = (ConfigurationManager.AppSettings["DefaultAllowCORSOrigins"] ?? "*").Split(',').Select(s => s.Trim()).ToList();
                        cachedDefaultOriginPatterns = new string[origins.Count];
                        cachedDefaultOriginPatternRegexs = new Regex[origins.Count];
                        for (int i = 0; i < origins.Count; i++)
                        {
                            cachedDefaultOriginPatterns[i] = origins[i];
                            cachedDefaultOriginPatternRegexs[i] = OriginPatternToRegex(origins[i]);
                        }
                    }
                }
            }

            this.OriginPatterns = cachedDefaultOriginPatterns;
            this.OriginPatternRegexs = cachedDefaultOriginPatternRegexs;
        }

        /// <summary>
        /// Marks the controller or action to allow cross-origin HTTP requests from the given origin.
        /// </summary>
        /// <param name="originPattern">
        /// The pattern the origin should match. Must contain protocol and hostname.
        /// Use * and ? as wildcards.
        /// I.e. "https://www.example.com" or "https://*.example.com" or (allowing http as well) "*.example.com".
        /// </param>
        /// <remarks>
        /// To restrict to https trafic, best also use the [RequireHttps] attribute.
        /// </remarks>
        public AllowCORSAttribute(string originPattern)
        {
            this.OriginPatterns = new string[] { originPattern };
            this.OriginPatternRegexs = new Regex[] { OriginPatternToRegex(originPattern) };
        }

        /// <summary>
        /// Turns an origin pattern into a regular expression.
        /// </summary>
        private Regex OriginPatternToRegex(string originPattern)
        {
            return originPattern.GetLikeRegex(RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        public string[] OriginPatterns { get; private set; }

        public Regex[] OriginPatternRegexs { get; private set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var httpContext = filterContext.HttpContext;

            // Retrieve origin:
            var origin = httpContext.Request.Headers["Origin"];

            // No need to validate if not a cross-origin request:
            if (origin == null) return;

            // Once the header is set, don't overwrite:
            if (httpContext.Response.Headers.AllKeys.Contains("Access-Control-Allow-Origin")) return;

            // If passed origin matches one of the patterns, add Access-Control-Allow-Origin header:
            foreach (var prex in OriginPatternRegexs)
            {
                if (prex.IsMatch(origin))
                {
                    httpContext.Response.Headers.Add("Access-Control-Allow-Origin", origin);
                    break;
                }
            }
        }        
    }
}
