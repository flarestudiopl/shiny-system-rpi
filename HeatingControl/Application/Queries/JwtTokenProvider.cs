using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Commons.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Storage.StorageDatabase.User;

namespace HeatingControl.Application.Queries
{
    public interface IJwtTokenProvider
    {
        string Provide(JwtTokenProviderInput input);
    }

    public class JwtTokenProviderInput
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }

    public class JwtTokenProvider : IJwtTokenProvider
    {
        private const int TokenLifetimeMinutes = 60;

        private readonly IActiveUserByLoginProvider _activeUserByLoginProvider;
        private readonly IConfiguration _configuration;

        public JwtTokenProvider(IActiveUserByLoginProvider activeUserByLoginProvider,
                                IConfiguration configuration)
        {
            _activeUserByLoginProvider = activeUserByLoginProvider;
            _configuration = configuration;
        }

        public string Provide(JwtTokenProviderInput input)
        {
            var user = _activeUserByLoginProvider.Provide(input.Login);

            if (user == null || user.PasswordHash != input.Password.CalculateHash())
            {
                return null;
            }

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var issuer = _configuration["Jwt:Issuer"];

            var token = new JwtSecurityToken(issuer,
                                             issuer,
                                             expires: DateTime.Now.AddMinutes(TokenLifetimeMinutes),
                                             signingCredentials: signingCredentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
