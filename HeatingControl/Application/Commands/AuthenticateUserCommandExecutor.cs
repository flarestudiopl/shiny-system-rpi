using System;
using System.IdentityModel.Tokens.Jwt;
using Commons.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Storage.StorageDatabase.User;
using System.Security.Claims;
using System.Collections.Generic;
using Commons.Localization;

namespace HeatingControl.Application.Commands
{
    public class AuthenticateUserCommand
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string IpAddress { get; set; }
    }

    public class AuthenticateUserCommandExecutor : ICommandExecutor<AuthenticateUserCommand>
    {
        private const int TokenLifetimeMinutes = 60;
        
        public static SecurityKey JwtSigningKey = new SymmetricSecurityKey(Guid.NewGuid().ToByteArray());

        private readonly IActiveUserByLoginProvider _activeUserByLoginProvider;
        private readonly IUserUpdater _userUpdater;
        private readonly IConfiguration _configuration;

        public AuthenticateUserCommandExecutor(IActiveUserByLoginProvider activeUserByLoginProvider,
                                               IUserUpdater userUpdater,
                                               IConfiguration configuration)
        {
            _activeUserByLoginProvider = activeUserByLoginProvider;
            _userUpdater = userUpdater;
            _configuration = configuration;
        }

        public CommandResult Execute(AuthenticateUserCommand command, CommandContext context)
        {
            var user = _activeUserByLoginProvider.Provide(command.Login);

            if (user == null || user.PasswordHash != command.Password.CalculateHash())
            {
                return CommandResult.WithValidationError(Localization.ValidationMessage.UnknownUserOrWrongPassword);
            }

            _userUpdater.Update(new UserUpdaterInput
                                {
                                    UserId = user.UserId,
                                    LastLogonDate = DateTime.Now,
                                    LastSeenIpAddress = command.IpAddress
                                });

            var signingCredentials = new SigningCredentials(JwtSigningKey, SecurityAlgorithms.HmacSha256);
            var issuer = _configuration["Jwt:Issuer"];

            var token = new JwtSecurityToken(issuer,
                                             issuer,
                                             new List<Claim> { new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()) },
                                             expires: DateTime.Now.AddMinutes(TokenLifetimeMinutes),
                                             signingCredentials: signingCredentials);

            return CommandResult.WithResponse(new JwtSecurityTokenHandler().WriteToken(token));
        }
    }
}
