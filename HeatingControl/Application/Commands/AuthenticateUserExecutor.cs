using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Commons.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Storage.StorageDatabase.User;
using System.Security.Claims;
using System.Collections.Generic;

namespace HeatingControl.Application.Commands
{
    public interface IAuthenticateUserExecutor
    {
        string Execute(AuthenticateUserExecutorInput input);
    }

    public class AuthenticateUserExecutorInput
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string IpAddress { get; set; }
    }

    public class AuthenticateUserExecutor : IAuthenticateUserExecutor
    {
        private const int TokenLifetimeMinutes = 60;
        
        public static SecurityKey JwtSigningKey = new SymmetricSecurityKey(Guid.NewGuid().ToByteArray());

        private readonly IActiveUserByLoginProvider _activeUserByLoginProvider;
        private readonly IUserUpdater _userUpdater;
        private readonly IConfiguration _configuration;

        public AuthenticateUserExecutor(IActiveUserByLoginProvider activeUserByLoginProvider,
                                        IUserUpdater userUpdater,
                                        IConfiguration configuration)
        {
            _activeUserByLoginProvider = activeUserByLoginProvider;
            _userUpdater = userUpdater;
            _configuration = configuration;
        }

        public string Execute(AuthenticateUserExecutorInput input)
        {
            var user = _activeUserByLoginProvider.Provide(input.Login);

            if (user == null || user.PasswordHash != input.Password.CalculateHash())
            {
                return null;
            }

            _userUpdater.Update(new UserUpdaterInput
                                {
                                    UserId = user.UserId,
                                    LastLogonDate = DateTime.Now,
                                    LastSeenIpAddress = input.IpAddress
                                });

            var signingCredentials = new SigningCredentials(JwtSigningKey, SecurityAlgorithms.HmacSha256);
            var issuer = _configuration["Jwt:Issuer"];

            var token = new JwtSecurityToken(issuer,
                                             issuer,
                                             new List<Claim> { new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()) },
                                             expires: DateTime.Now.AddMinutes(TokenLifetimeMinutes),
                                             signingCredentials: signingCredentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
