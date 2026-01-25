using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace TodoWebApi.Controllers
{
    public class HelloController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            var response = new
            {
                message = "Hello World",
                status = "success"
            };

            return Ok(response);
        }
    }
}
