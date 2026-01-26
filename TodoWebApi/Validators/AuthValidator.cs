using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Web;
using TodoWebApi.Models.Auth;
using System.Net.Mail;

namespace TodoWebApi.Validators
{
    public static class AuthValidator
    {
        public static string LoginValidate(LoginRequestModel model)
        {
            if (model == null)
                return "Username and password required";

            if (string.IsNullOrWhiteSpace(model.username))
                return "Username is required";

            if (string.IsNullOrWhiteSpace(model.password))
                return "Password is required";

            if (model.password.Length < 6)
                return "Password should be greater than or equal to 6.";

            return null;
        }

        public static string RegisterValidate(RegisterRequestModel registerRequestModel)
        {
            if (string.IsNullOrWhiteSpace(registerRequestModel.Email) ||
                string.IsNullOrWhiteSpace(registerRequestModel.Password))
            {
                return "Email and Password are required";
            }

            // Email format validation
            try
            {
                var mailAddress = new MailAddress(registerRequestModel.Email);
            }
            catch
            {
                return "Invalid email format";
            }

            return null;
        }
    }
}