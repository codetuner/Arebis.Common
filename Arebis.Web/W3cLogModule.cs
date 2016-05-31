using Arebis.IO;
using System;
using System.Web;

namespace Arebis.Web
{
    public class W3cLogModule : IHttpModule
    {
        public static Exception FirstFailure;
        public static Exception LastFailure;

        public virtual void Init(HttpApplication application)
        {
            // Attach log event handler:
            application.BeginRequest += OnBeginRequest;
            application.EndRequest += OnEndRequest;
        }

        public void Dispose()
        { }

        protected virtual void OnBeginRequest(object sender, EventArgs e)
        {
            // Gather information to use OnEndRequest.

            try
            {
                var application = (HttpApplication)sender;
                var context = application.Context;
                var response = context.Response;

                context.Items["_W3cLog_Rt"] = Current.DateTime.UtcNow;

                context.Items["_W3cLog_Os"] = response.Filter = new MeteringStream(response.Filter);
            }
            catch (Exception ex)
            {
                // Keep the exception for diagnosing logging failures:
                if (FirstFailure == null) FirstFailure = ex;
                LastFailure = ex;
            }
        }

        protected virtual void OnEndRequest(object sender, EventArgs e)
        {
            // Log the request.

            try
            {
                W3cLogSystem.Log(((HttpApplication)sender).Context);
            }
            catch (Exception ex)
            {
                // Keep the exception for diagnosing logging failures:
                if (FirstFailure == null) FirstFailure = ex;
                LastFailure = ex;
            }
        }
    }
}
