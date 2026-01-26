using Microsoft.IdentityModel.Tokens;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TodoWebApi.Data;

namespace TodoWebApi.Services.Auth
{

    public static class TokenService
    {
        private static string SecretKey = "THIS_IS_A_SUPER_SECRET_KEY_12345";
        private static string Issuer = "TodoWebApi";

        public static string GenerateAccessToken(string username)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(ClaimTypes.Name, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var token = new JwtSecurityToken(
                issuer: Issuer,
                audience: Issuer,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
        }

        public static void SaveRefreshToken(int userId, string refreshToken)
        {
            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = new SqlCommand("Spr_Save_Update_Refresh_Token", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@RefreshToken", refreshToken);
                cmd.Parameters.AddWithValue("@ExpiryDate", DateTime.UtcNow.AddDays(7));

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }


}