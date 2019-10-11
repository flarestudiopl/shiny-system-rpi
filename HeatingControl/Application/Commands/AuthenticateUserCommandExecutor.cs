using System;
using System.IdentityModel.Tokens.Jwt;
using Commons.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Collections.Generic;
using Commons.Localization;
using HeatingControl.Application.DataAccess.User;
using System.Linq;
using Domain;

namespace HeatingControl.Application.Commands
{
    public class AuthenticateUserCommand
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string Pin { get; set; }
        public string IpAddress { get; set; }
    }

    public class AuthenticateUserCommandResult
    {
        public string Token { get; set; }
        public ICollection<Permission> Permissions { get; set; }
    }

    public class AuthenticateUserCommandExecutor : ICommandExecutor<AuthenticateUserCommand>
    {
        private const int TokenLifetimeMinutes = 15;

        public static SecurityKey JwtSigningKey = new SymmetricSecurityKey(Guid.NewGuid().ToByteArray());

        private readonly IActiveUserProvider _activeUserProvider;
        private readonly IUserLastLogonUpdater _userUpdater;
        private readonly IConfiguration _configuration;

        public AuthenticateUserCommandExecutor(IActiveUserProvider activeUserProvider,
                                               IUserLastLogonUpdater userUpdater,
                                               IConfiguration configuration)
        {
            _activeUserProvider = activeUserProvider;
            _userUpdater = userUpdater;
            _configuration = configuration;
        }

        public CommandResult Execute(AuthenticateUserCommand command, CommandContext context)
        {
            if (command.Pin.IsNullOrEmpty() && command.Password.IsNullOrEmpty())
            {
                return CommandResult.WithValidationError(Localization.ValidationMessage.PinOrPasswordRequired);
            }

            var user = _activeUserProvider.Provide(x => x.Login == command.Login);

            if (user == null ||
                (!command.Pin.IsNullOrEmpty() && user.QuickLoginPinHash != command.Pin.CalculateHash()) ||
                (!command.Password.IsNullOrEmpty() && user.PasswordHash != command.Password.CalculateHash()))
            {
                return CommandResult.WithValidationError(Localization.ValidationMessage.UnknownUserOrWrongPassword);
            }

            _userUpdater.Update(new UserLastLogonUpdaterInput
            {
                UserId = user.UserId,
                LastLogonDate = DateTime.UtcNow,
                LastSeenIpAddress = command.IpAddress
            });

            var signingCredentials = new SigningCredentials(JwtSigningKey, SecurityAlgorithms.HmacSha256);
            var issuer = _configuration["Jwt:Issuer"];

            var claims = new List<Claim> { new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()) };
            claims.AddRange(user.UserPermissions.Select(x => new Claim(ClaimTypes.Role, x.Permission.ToString())));

            var token = new JwtSecurityToken(issuer,
                                             issuer,
                                             claims,
                                             expires: DateTime.UtcNow.AddMinutes(TokenLifetimeMinutes),
                                             signingCredentials: signingCredentials);

            var response = new AuthenticateUserCommandResult
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Permissions = user.UserPermissions.Select(x => x.Permission).ToArray()
            };

            return CommandResult.WithResponse(response);
        }
    }
}
