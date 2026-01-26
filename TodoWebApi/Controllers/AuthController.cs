using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using System.Web.WebPages;
using TodoWebApi.Data;
using TodoWebApi.Filters;
using TodoWebApi.Helpers;
using TodoWebApi.Models.Auth;
using TodoWebApi.Models.Auth.LoginWithOtp;
using TodoWebApi.Services.Auth;
using TodoWebApi.Services.Notification;
using TodoWebApi.Validators;

namespace TodoWebApi.Controllers
{
    [RoutePrefix("api/auth")]
    public class AuthController : ApiController
    {
        [Route("login")]
        [HttpPost]
        public HttpResponseMessage Login([FromBody] LoginRequestModel loginRequestModel)
        {
            LoginResponseModel response = new LoginResponseModel();

            var error = AuthValidator.LoginValidate(loginRequestModel);

            if (error != null)
            {
                response.status = 400;
                response.errorMsg = error;
                return Request.CreateResponse(HttpStatusCode.BadRequest, response);
            }
            else
            {
                string hashedPassword = PasswordHelper.HashPassword(loginRequestModel.password);
                using (SqlConnection con = DbHelper.GetConnection())
                using (SqlCommand cmd = new SqlCommand("Spr_User_Login", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Username", loginRequestModel.username);
                    cmd.Parameters.AddWithValue("@Password", hashedPassword);

                    con.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            response.status = 500;
                            response.errorMsg = "Unexpected error";
                            return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
                        }

                        int status = Convert.ToInt32(reader["Status"]);

                        if (status != 200)
                        {
                            response.status = status;
                            response.errorMsg = reader["Message"].ToString();
                            return Request.CreateResponse((HttpStatusCode)status, response);
                        }

                        response.status = 200;
                        response.message = reader["Message"].ToString();
                        int userId = Convert.ToInt32(reader["UserId"]);

                        response.data = new User
                        {
                            UserId = Convert.ToInt32(reader["UserId"]),
                            Username = Convert.ToString(reader["Username"]),
                            AccessToken = TokenService.GenerateAccessToken(loginRequestModel.username),
                            MobileNumber = Convert.ToString(reader["MobileNumber"]),
                            FullName = Convert.ToString(reader["FullName"]),
                            Email = Convert.ToString(reader["Email"]),
                        };
                        reader.Close();

                        string refreshToken = TokenService.GenerateRefreshToken();

                        using (SqlCommand refreshCmd = new SqlCommand("Spr_Save_Update_Refresh_Token", con))
                        {
                            refreshCmd.CommandType = CommandType.StoredProcedure;
                            refreshCmd.Parameters.AddWithValue("@UserId", userId);
                            refreshCmd.Parameters.AddWithValue("@RefreshToken", refreshToken);
                            refreshCmd.Parameters.AddWithValue("@ExpiryDate", DateTime.UtcNow.AddDays(7));
                            refreshCmd.ExecuteNonQuery();
                        }
                        response.data.RefreshToken = refreshToken;
                    }
                }
                return Request.CreateResponse(HttpStatusCode.OK, response);

            }
        }

        [HttpPost]
        [Route("register")]
        public HttpResponseMessage Register(RegisterRequestModel registerRequestModel)
        {
            RegisterResponseModel response = new RegisterResponseModel();

            try
            {
                var error = AuthValidator.RegisterValidate(registerRequestModel);
                if (error != null)
                {
                    response.status = 400;
                    response.errorMsg = error;
                    return Request.CreateResponse(HttpStatusCode.BadRequest, response);
                }
                else
                {
                    // 2️⃣ Hash password
                    string hashedPassword = PasswordHelper.HashPassword(registerRequestModel.Password);
                    using (SqlConnection con = DbHelper.GetConnection())
                    {
                        using (SqlCommand cmd = new SqlCommand("Spr_User_Registration", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@Username", registerRequestModel.Username);
                            cmd.Parameters.AddWithValue("@Email", registerRequestModel.Email);
                            cmd.Parameters.AddWithValue("@Password", hashedPassword);
                            cmd.Parameters.AddWithValue("@FullName", registerRequestModel.FullName);
                            cmd.Parameters.AddWithValue("@MobileNumber", registerRequestModel.MobileNumber);
                            cmd.Parameters.AddWithValue("@CreatedBy", 1);

                            con.Open();

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (!reader.Read())
                                {
                                    response.status = 500;
                                    response.errorMsg = "Unexpected error";
                                    return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
                                }

                                int status = Convert.ToInt32(reader["Status"]);

                                if (status != 200)
                                {
                                    response.status = status;
                                    response.errorMsg = reader["Message"].ToString();
                                    return Request.CreateResponse((HttpStatusCode)status, response);
                                }

                                response.status = 200;
                                response.message = "Thank you for registering successfully";
                                response.loginMessage = "Please log in using your registered email and password.";

                            }
                        }
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
            }
            catch (Exception)
            {
                response.status = 500;
                response.message = "Internal server error";
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [HttpPost]
        [Route("refresh_token")]
        public HttpResponseMessage RefreshToken([FromBody] RefreshTokenRequestModel model)
        {
            RefreshTokenResponseModel response = new RefreshTokenResponseModel();

            if (string.IsNullOrEmpty(model.RefreshToken))
            {
                response.Status = 400;
                response.ErrorMsg = "Refresh token is required";
                return Request.CreateResponse(HttpStatusCode.BadRequest, response);
            }

            try
            {
                using (SqlConnection con = DbHelper.GetConnection())
                using (SqlCommand cmd = new SqlCommand("Spr_Validate_RefreshToken", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@RefreshToken", model.RefreshToken);

                    con.Open();

                    int userId = 0;
                    string username = "";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            response.Status = 401;
                            response.ErrorMsg = "Invalid or expired refresh token";
                            return Request.CreateResponse(HttpStatusCode.Unauthorized, response);
                        }

                        // Read user info
                        userId = Convert.ToInt32(reader["UserId"]);
                        username = Convert.ToString(reader["Username"]);
                    }

                    string newAccessToken = TokenService.GenerateAccessToken(username);
                    string newRefreshToken = TokenService.GenerateRefreshToken();

                    using (SqlCommand updateCmd = new SqlCommand("Spr_Save_Update_Refresh_Token", con))
                    {
                        updateCmd.CommandType = CommandType.StoredProcedure;
                        updateCmd.Parameters.AddWithValue("@UserId", userId);
                        updateCmd.Parameters.AddWithValue("@RefreshToken", newRefreshToken);
                        updateCmd.Parameters.AddWithValue("@ExpiryDate", DateTime.UtcNow.AddDays(7));
                        updateCmd.ExecuteNonQuery();
                    }

                    response.Status = 200;
                    response.Message = "Token refreshed successfully";
                    response.AccessToken = newAccessToken;
                    response.RefreshToken = newRefreshToken;
                }
            }
            catch (Exception ex)
            {
                response.Status = 500;
                response.Message = "Internal server error";
                response.ErrorMsg = ex.Message;
            }

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpPost]
        [Route("login_with_otp")]
        public HttpResponseMessage SendOtp([FromBody] SendOtpRequestModel sendOtpRequestModel)
        {
            SendOtpResponseModel sendOtpResponseModel = new SendOtpResponseModel();
            if (string.IsNullOrEmpty(sendOtpRequestModel.Email) && string.IsNullOrEmpty(sendOtpRequestModel.Mobilenumber))
            {
                sendOtpResponseModel.Status = 400;
                sendOtpResponseModel.ErrorMsg = "Email/Mobile number is required";
                return Request.CreateResponse(HttpStatusCode.BadRequest, sendOtpResponseModel);
            }

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = new SqlCommand("Spr_Generate_User_OTP", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                if (!string.IsNullOrEmpty(sendOtpRequestModel.Email))
                {
                    cmd.Parameters.AddWithValue("@Email", sendOtpRequestModel.Email);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@MobileNumber", sendOtpRequestModel.Mobilenumber);
                }

                con.Open();

                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    if (!sdr.Read())
                    {
                        sendOtpResponseModel.Status = 404;
                        sendOtpResponseModel.ErrorMsg = "User not found";
                        return Request.CreateResponse(HttpStatusCode.NotFound, sendOtpResponseModel);
                    }

                    string otp = sdr["OTP"].ToString();
                    string email = sdr["Email"].ToString().ToLower();
                    string mobile = sdr["MobileNumber"].ToString();

                    if (!string.IsNullOrEmpty(email))
                    {
                        EmailService.SendOtp(email, otp);
                    }
                    else if (!string.IsNullOrEmpty(mobile))
                    {
                        SmsService.SendOtp(mobile, otp);
                    }

                }
            }

            sendOtpResponseModel.Status = 200;
            sendOtpResponseModel.Message = "Otp sent successfully";
            return Request.CreateResponse(HttpStatusCode.OK, sendOtpResponseModel);
        }

        [HttpPost]
        [Route("verify-otp")]
        public HttpResponseMessage VerifyOtp([FromBody] VerifyOtpRequestModel model)
        {
            var response = new LoginResponseModel();

            if ((string.IsNullOrEmpty(model.Email) && string.IsNullOrEmpty(model.MobileNumber)) && string.IsNullOrEmpty(model.OTP))
            {
                response.status = 400;
                response.errorMsg = "Email/Mobile and OTP are required";
                return Request.CreateResponse(HttpStatusCode.BadRequest, response);
            }

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = new SqlCommand("Spr_Verify_User_OTP", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                if (!string.IsNullOrEmpty(model.Email))
                {
                    cmd.Parameters.AddWithValue("@Email", model.Email);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@MobileNumber", model.MobileNumber);
                }
                cmd.Parameters.AddWithValue("@OTP", model.OTP);

                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        response.status = 401;
                        response.errorMsg = "Invalid or expired OTP";
                        return Request.CreateResponse(HttpStatusCode.Unauthorized, response);
                    }

                    int userId = Convert.ToInt32(reader["UserId"]);
                    string username = reader["Username"].ToString();

                    string accessToken = TokenService.GenerateAccessToken(username);
                    string refreshToken = TokenService.GenerateRefreshToken();

                    TokenService.SaveRefreshToken(userId, refreshToken);

                    response.status = 200;
                    response.message = "Login successfully";
                    response.data = new User
                    {
                        UserId = userId,
                        Username = username,
                        MobileNumber = Convert.ToString(reader["MobileNumber"]),
                        FullName = Convert.ToString(reader["FullName"]),
                        Email = Convert.ToString(reader["Email"]),
                        AccessToken = accessToken,
                        RefreshToken = refreshToken
                    };
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

    }
}
