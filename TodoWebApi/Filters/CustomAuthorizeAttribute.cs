using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace TodoWebApi.Filters
{
    public class CustomAuthorizeAttribute: AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            var response = new
            {
                status = 401,
                errorMsg = "Access denied. Please login to continue."
            };

            actionContext.Response =
                actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, response);
        }
    }
}