using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Arebis.Extensions;
using Arebis.Caching;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Web;
using System.Net;

namespace Arebis.Rest
{
    /// <summary>
    /// A base class to implement REST service clients.
    /// Supports none, basic, API key or OAuth2 (client credentials) authentication schemes. OAuth2 access tokens are cached across RestClient instances.
    /// Provides access to logging.
    /// </summary>
    public abstract class RestClient : IDisposable
    {
        #region Construction & disposal

        /// <summary>
        /// Constructs a new RestClient with given service parameters.
        /// </summary>
        /// <param name="serviceParameters">Service parameters defining service URL and authentication information.</param>
        /// <remarks>
        /// Service parameters should include:<br/>
        /// - "url" : the URL of the base service<br/>
        /// - "authenticationMode" : the authentication mode<br/>
        /// If authentication mode is "none", no further parameters are required.<br/>
        /// If authentication mode is "basic", following parameters are required:<br/>
        /// - "username" : username or client ID (is missing, uses the current identity name)<br/>
        /// - "password" : password or passphrase<br/>
        /// Both are then combined and base64 encoded to form a "Basic" authentication header.
        /// If authenticaiton mode is "oauth2/clientcredentials", following parameters are required:<br/>
        /// - "username" : username or client ID (is missing, uses the current identity name)<br/>
        /// - "password" : password or passphrase<br/>
        /// - "tokenUrl" : (relative or absolute) url to the token endpoint (if missing, defaults to "oauth2/token")<br/>
        /// Using basic authentication, an access token is then requested and used.<br/>
        /// The access token is cached for as long as it is valid and reused for all subsequent instances of the RestService instances
        /// to the same url with the same username and password. The cache can be tuned as described in the documentation of
        /// Arebis.Common, Arebis.Caching.LruCache. The cache name is "RestServicesOAuth2AccessTokensCache".<br/>
        /// Following additional parameters are allowed and are applied independent of the choosen security scheme:<br/>
        /// - "param" : string to add to the parameter string, already URL encoded, i.e: "name=john+smith&amp;id=12"<br/>
        /// - "pathparam" : its value will be used to replace the "pathparam" substring in the URL<br/>
        /// In addition, custom headers can be added by defining settings with as key: "header:&lt;headerkey$gt;" and as value the value of the header.<br/>
        /// For instance: "header:X-My-Authentication-Header" with value "secret".
        /// </remarks>
        protected RestClient(IDictionary<string, string> serviceParameters)
        {
            if (serviceParameters == null) throw new ArgumentNullException("serviceParameters");
            this.HttpClient = new HttpClient();
            var url = serviceParameters.GetValueOrDefault("url");
            this.baseServiceUrl = url;
            if (String.IsNullOrWhiteSpace(this.baseServiceUrl)) throw new ArgumentException("The serviceParameters dictionary does not contain a valid \"uri\" value.");
            var authenticationMode = serviceParameters.GetValueOrDefault("authenticationMode");
            if (authenticationMode == "basic")
            {
                // Add header "Authorization: Basic base64(username:password)":
                var username = serviceParameters.GetValueOrDefault("username", System.Threading.Thread.CurrentPrincipal.Identity.Name);
                var password = serviceParameters.GetValueOrDefault("password", "");
                var basicValue = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));
                this.HttpClient.DefaultRequestHeaders.Add("Authorization", basicValue);
            }
            else if (authenticationMode == "oauth2/clientcredentials")
            {
                // Retrieve cached token (if any):
                var username = serviceParameters.GetValueOrDefault("username", System.Threading.Thread.CurrentPrincipal.Identity.Name);
                var password = serviceParameters.GetValueOrDefault("password", "");
                var tokenKey = url + "/" + username + "/" + password;
                lock (Oauth2AccessTokensCache)
                {
                    this.Oauth2Token = Oauth2AccessTokensCache.GetNoFetch(tokenKey);
                    if (this.Oauth2Token == null)
                    {
                        Oauth2AccessTokensCache.Set(tokenKey, this.Oauth2Token = new OAuth2AccessTokenInfo()
                        {
                            // Build value for header "Authorization: Basic base64(username:password)":
                            BasicAuthorizationHeader = Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password)),
                            TokenType = "Basic",
                            Token = null,
                            TokenUri = serviceParameters.GetValueOrDefault("tokenUrl", "oauth2/token"),
                            TokenExpirationTime = DateTimeOffset.MinValue,
                            GrantType = "client_credentials"
                        });
                        this.HttpClient.DefaultRequestHeaders.Add("Authorization", "Bearer");
                    }
                    else
                    {
                        this.HttpClient.DefaultRequestHeaders.Add("Authorization", this.Oauth2Token.TokenType + " " + this.Oauth2Token.Token);
                    }
                }
            }
            this.parameter = serviceParameters.GetValueOrDefault("param");
            this.pathParameter = serviceParameters.GetValueOrDefault("pathparam", "pathparam");
            foreach (var pair in serviceParameters)
            {
                if (pair.Key.StartsWith("header:"))
                {
                    this.HttpClient.DefaultRequestHeaders.Add(pair.Key.Substring(7).Trim(), pair.Value);
                }
            }
        }

        /// <summary>
        /// Disposes the RestClient.
        /// </summary>
        public virtual void Dispose()
        {
            this.HttpClient.Dispose();
        }

        protected HttpClient HttpClient { get; private set; }
        protected OAuth2AccessTokenInfo Oauth2Token { get; private set; }

        private readonly string baseServiceUrl;
        private readonly string pathParameter;
        private readonly string parameter;

        #endregion

        #region API delegation

        protected virtual async Task ApiSendAsync(HttpMethod method, string url, object args = null, object content = null, NameValueCollection requestHeaders = null, NameValueCollection responseHeaders = null)
        {
            await ApiSendRawAsync(method, url, args, content, requestHeaders, responseHeaders);
        }

        protected async Task<TResult> ApiSendAsync<TResult>(HttpMethod method, string url, object args = null, object content = null, NameValueCollection requestHeaders = null, NameValueCollection responseHeaders = null)
        {
            var str = await ApiSendRawAsync(method, url, args, content, requestHeaders, responseHeaders);
            return JsonConvert.DeserializeObject<TResult>(str);
        }

        protected virtual async Task<string> ApiSendRawAsync(HttpMethod method, string url, object args = null, object content = null, NameValueCollection requestHeaders = null, NameValueCollection responseHeaders = null)
        {
            if (Oauth2Token != null && DateTimeOffset.Now >= Oauth2Token.TokenExpirationTime)
                RenewOAuthToken();

            var sw = Stopwatch.StartNew();
            try
            {
                // Build URL:
                url = new Uri(new Uri(baseServiceUrl.Replace("pathparam", this.pathParameter)), url).ToString();
                if (args != null)
                {
                    var argsDict = args.ToDictionary();
                    url = url + "?";
                    url = url + String.Join("&", argsDict.Select(p => HttpUtility.UrlEncode(p.Key) + "=" + HttpUtility.UrlEncode(p.Value.ToStringOr(""))));
                    if (this.parameter != null)
                    {
                        if (url.EndsWith("?"))
                            url = url + this.parameter;
                        else
                            url = url + "&" + this.parameter;
                    }
                }
                else
                {
                    if (this.parameter != null)
                    {
                        url = url + "?" + this.parameter;
                    }
                }

                // Build message:
                var msg = new HttpRequestMessage(method, url);
                if (requestHeaders != null)
                {
                    foreach (string key in requestHeaders.Keys)
                    {
                        foreach (var value in requestHeaders.GetValues(key))
                        {
                            msg.Headers.Add(key, value);
                        }
                    }
                }
                if (content != null)
                {
                    var contentAsString = JsonConvert.SerializeObject(content);
                    if (ConversationLoggingEnabled)
                    {
                        this.LogWriteLn("// Request:");
                        this.LogWriteLn("// " + method + " " + url);
                        this.LogWriteLn(contentAsString);
                    }
                    msg.Content = new StringContent(contentAsString, Encoding.UTF8, "application/json");
                }
                else if (ConversationLoggingEnabled)
                {
                    this.LogWriteLn("// Request:");
                    this.LogWriteLn("// " + method + " " + url);
                }

                // Invoke operation and handle result:
                var resp = await this.HttpClient.SendAsync(msg, HttpCompletionOption.ResponseContentRead)/*.ConfigureAwait(false)*/;
                if (resp.IsSuccessStatusCode)
                {
                    if (responseHeaders != null)
                    {
                        foreach (var resph in resp.Headers)
                        {
                            responseHeaders[resph.Key] = String.Join("\r\n", resph.Value);
                        }
                    }

                    var rstr = resp.Content.ReadAsStringAsync().Result;
                    if (!String.IsNullOrWhiteSpace(rstr))
                    {
                        if (ConversationLoggingEnabled)
                        {
                            this.LogWriteLn("// Response:");
                            this.LogWriteLn(rstr);
                        }
                        return rstr;
                    }
                    else
                    {
                        if (ConversationLoggingEnabled)
                        {
                            this.LogWriteLn("Response : #null#");
                        }
                        return null;
                    }
                }
                else
                {
                    Exception exception;
                    var rstr = await resp.Content.ReadAsStringAsync();
                    if (ConversationLoggingEnabled)
                    {
                        this.LogWriteLn("// Response:");
                        this.LogWriteLn(rstr);
                        this.LogWriteLn(String.Format("// Response error code: {0}", resp.StatusCode));
                    }
                    if (!String.IsNullOrWhiteSpace(rstr))
                    {
                        var errobj = JObject.Parse(rstr);
                        exception = this.ConstructExceptionFromErrorObject(resp.StatusCode, errobj) ?? new HttpRequestException("Unknown exception.");
                        exception.WithData("HttpResponseContent", rstr);
                        foreach (JProperty prop in errobj.Properties())
                        {
                            exception.WithData(prop.Name, prop.Value.ToStringOr("#null#"));
                        }
                        if (ConversationLoggingEnabled)
                        {
                            exception.WithData("ConversationLog", this.ConversationLog);
                        }
                    }
                    else
                    {
                        exception = new HttpRequestException("Unknown exception.");
                    }

                    throw exception;
                }
            }
            finally
            {
                Debug.WriteLine("[{0}] {1:#,##0.000}sec : " + url, this.GetType().Name, sw.Elapsed.TotalSeconds);
            }
        }

        protected virtual void RenewOAuthToken()
        {

            var sw = Stopwatch.StartNew();
            this.Oauth2Token.TokenLock.AcquireWriterLock(15000);
            try
            {
                if (DateTimeOffset.Now >= this.Oauth2Token.TokenExpirationTime)
                {
                    this.Oauth2Token.TokenExpirationTime = DateTimeOffset.Now.AddMinutes(10);
                    this.Oauth2Token.TokenType = "Basic";
                    this.Oauth2Token.Token = this.Oauth2Token.BasicAuthorizationHeader;
                    this.HttpClient.DefaultRequestHeaders.Remove("Authorization");
                    this.HttpClient.DefaultRequestHeaders.Add("Authorization", this.Oauth2Token.TokenType + " " + this.Oauth2Token.Token);
                    this.HttpClient.DefaultRequestHeaders.Remove("Accept");
                    this.HttpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                    var url = new Uri(new Uri(this.baseServiceUrl.Replace("pathparam", this.pathParameter)), this.Oauth2Token.TokenUri);
                    var resp = this.HttpClient.PostAsync(url, new StringContent("grant_type=" + this.Oauth2Token.GrantType, Encoding.UTF8, "application/x-www-form-urlencoded")).Result;
                    if (resp.IsSuccessStatusCode)
                    {
                        var rstr = resp.Content.ReadAsStringAsync().Result;
                        this.Oauth2Token.LastRefreshResponse = rstr;
                        dynamic robj = JsonConvert.DeserializeObject(rstr);
                        this.Oauth2Token.TokenType = (string)robj.token_type;
                        this.Oauth2Token.Token = (string)robj.access_token;
                        this.Oauth2Token.TokenExpirationTime = DateTimeOffset.Now.AddSeconds(-10 + (int)robj.expires_in);
                        this.HttpClient.DefaultRequestHeaders.Remove("Authorization");
                        this.HttpClient.DefaultRequestHeaders.Add("Authorization", this.Oauth2Token.TokenType + " " + this.Oauth2Token.Token);
                    }
                }
            }
            finally
            {
                this.Oauth2Token.TokenLock.ReleaseWriterLock();
                Debug.WriteLine("[{0}] {1:#,##0.000}sec : Getting/Renewing OAuth Token", this.GetType().Name, sw.Elapsed.TotalSeconds);
            }
        }

        /// <summary>
        /// Return a suitable exception based on the given error object.
        /// If null is returned, a default exception will be created.
        /// </summary>
        protected abstract Exception ConstructExceptionFromErrorObject(HttpStatusCode statusCode, dynamic errobj);

        #endregion

        #region Logging

        private StringBuilder log = new StringBuilder();

        public bool ConversationLoggingEnabled { get; set; }

        /// <summary>
        /// Log of the conversation with the service.
        /// </summary>
        public string ConversationLog
        {
            get
            {
                return this.log.ToString();
            }
        }

        protected void LogWrite(string str)
        {
            if (!ConversationLoggingEnabled) return;

            if (this.log.Length == 0)
            {
                log.AppendFormat("// {1:yyyy-MM-dd\\THH:mm:ss} (UTC) : {0} created\r\n", this.GetType().Name, Current.DateTime.UtcNow);
            }

            log.Append(str);
        }

        protected void LogWriteLn(string str)
        {
            if (!ConversationLoggingEnabled) return;

            this.LogWrite(str);
            this.LogWrite("\r\n");
        }

        public void LogWriteComment(string comment)
        {
            if (!ConversationLoggingEnabled) return;
            if (String.IsNullOrWhiteSpace(comment)) return;

            this.LogWrite("// " + comment.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "\r\n// ") + "\r\n");

        }

        #endregion

        #region OAuth2 Access Tokens caching

        public class OAuth2AccessTokenInfo
        {
            public OAuth2AccessTokenInfo()
            {
                this.TokenLock = new ReaderWriterLock();
            }

            public string BasicAuthorizationHeader { get; set; }

            public string TokenType { get; set; }

            public string Token { get; set; }

            public string TokenUri { get; set; }

            public DateTimeOffset TokenExpirationTime { get; set; }

            public ReaderWriterLock TokenLock { get; set; }

            public string GrantType { get; set; }

            public string LastRefreshResponse { get; set; }
        }

        private static LruCache<string, OAuth2AccessTokenInfo> Oauth2AccessTokensCache = new LruCache<string, OAuth2AccessTokenInfo>("RestServicesOAuth2AccessTokensCache");

        #endregion
    }
}
