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

            Logger.log.Debug(String.Format("Go to service."));

            var header_authorizer = actionContext.Request.Headers.Authorization;
            string token = "";
            if (header_authorizer == null)
            {
                Logger.log.Debug(String.Format("There is no token."));
                HandleNonAuthenRequest(actionContext, "Unauthorized: There is no token");
            }
            else
            {
                token = header_authorizer.ToString();
                if (!TokenManager.ValidateToken(token))
                {
                    Logger.log.Debug(String.Format("Token ({0}) is not valid.", token));
                    HandleNonAuthenRequest(actionContext, "Unauthorized: Token is not valid");
                }
                else
                {
                    if (ConfigurationManager.AppSettings["IsProduction"] == "true")
                    {
                        Logger.log.Debug("Production mode");
                        if (actionContext.Request.RequestUri.Scheme != Uri.UriSchemeHttps)
                        {
                            Logger.log.Debug("Uri.UriSchemeHttps: " + Uri.UriSchemeHttps);
                            HandleNonHttpsRequest(actionContext);
                        }
                        else
                        {
                            base.OnAuthorization(actionContext);
                        }
                    }
                }
            }
        }

        protected virtual void HandleNonHttpsRequest(HttpActionContext actionContext)
        {
            actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden);
            actionContext.Response.ReasonPhrase = "SSL Required";
        }

        protected virtual void HandleNonAuthenRequest(HttpActionContext actionContext, string message)
        {
            actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
            actionContext.Response.ReasonPhrase = message;
        }
    }
}