using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace OkToBoardServices
{
    public class RequireHttpsAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext == null)
            {
                throw new ArgumentNullException("actionContext");
            }

            if (ConfigurationManager.AppSettings["IsProduction"] == "true")
            {
                Logger.log.Debug("Production mode");
                if (actionContext.Request.RequestUri.Scheme != Uri.UriSchemeHttps)
                {
                    Logger.log.Debug("Uri.UriSchemeHttps: " + Uri.UriSchemeHttps);
                    Logger.log.Debug("actionContext.Request.RequestUri.Scheme: " + actionContext.Request.RequestUri.Scheme);
                    HandleNonHttpsRequest(actionContext);
                }
                else
                {
                    base.OnAuthorization(actionContext);
                }
            }
        }

        protected virtual void HandleNonHttpsRequest(HttpActionContext actionContext)
        {
            actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden);
            actionContext.Response.ReasonPhrase = "SSL Required";
        }
    }
}