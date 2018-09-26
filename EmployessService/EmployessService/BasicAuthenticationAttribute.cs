using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace EmployessService
{
    /// <summary>
    /// Implementing basic user:password authentication
    /// </summary>
    public class BasicAuthenticationAttribute : AuthorizationFilterAttribute
    {
        private static readonly ILog s_log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            //in case that the client haven't provided credentials
            if (actionContext.Request.Headers.Authorization == null)
            {
                HttpResponseMessage httpMsg = new HttpResponseMessage();
                httpMsg.Content = new StringContent("Client haven't provided credentials!");
                s_log.Warn(httpMsg.Content);
                httpMsg.StatusCode = System.Net.HttpStatusCode.Unauthorized;
                actionContext.Response = httpMsg;
            }
            //credentials provided 
            else
            {
                string autenticatioToken = actionContext.Request.Headers.Authorization.Parameter;
                string decodedAutenticatioToken =  System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(autenticatioToken));
                string[] credentials = decodedAutenticatioToken.Split(':');

                //User password are mach
                if (EmployeeScurity.AuthenticateLogin(credentials[0], credentials[1]))
                {
                    Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity(credentials[0]), null);
                }
                else
                {
                    HttpResponseMessage httpMsg = new HttpResponseMessage();
                    httpMsg.Content = new StringContent("Provided credentials do not match!");
                    s_log.Warn(httpMsg.Content);
                    httpMsg.StatusCode = System.Net.HttpStatusCode.Unauthorized;
                    actionContext.Response = httpMsg;
                }
            }
        }
    }
}