using System;
using System.IdentityModel.Tokens.Jwt;
using Commons.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Collections.Generic;
using Commons.Localization;
using HeatingControl.Application.DataAccess.User;

namespace HeatingControl.Application.Commands
{
    public class AuthenticateUserByPinCommand
    {
        public string Login { get; set; }
        public string Pin { get; set; }
        public string IpAddress { get; set; }
    }

    public class AuthenticateUserByPinCommandExecutor : ICommandExecutor<AuthenticateUserByPinCommand>
    {
        private const int TokenLifetimeMinutes = 15;
        
        private readonly IActiveUserByLoginProvider _activeUserByLoginProvider;
        private readonly IUserLastLogonUpdater _userUpdater;
        private readonly IConfiguration _configuration;

        public AuthenticateUserByPinCommandExecutor(IActiveUserByLoginProvider activeUserByLoginProvider,
                                                    IUserLastLogonUpdater userUpdater,
                                                    IConfiguration configuration)
        {
            _activeUserByLoginProvider = activeUserByLoginProvider;
            _userUpdater = userUpdater;
            _configuration = configuration;
        }

        public CommandResult Execute(AuthenticateUserByPinCommand command, CommandContext context)
        {
            var user = _activeUserByLoginProvider.Provide(command.Login);

            if (user == null || command.Pin.IsNullOrEmpty() || user.QuickLoginPinHash != command.Pin.CalculateHash())
            {
                return CommandResult.WithValidationError(Localization.ValidationMessage.UnknownUserOrWrongPassword);
            }

            _userUpdater.Update(new UserLastLogonUpdaterInput
                                {
                                    UserId = user.UserId,
                                    LastLogonDate = DateTime.UtcNow,
                                    LastSeenIpAddress = command.IpAddress
                                });

            var signingCredentials = new SigningCredentials(AuthenticateUserCommandExecutor.JwtSigningKey, SecurityAlgorithms.HmacSha256);
            var issuer = _configuration["Jwt:Issuer"];

            var token = new JwtSecurityToken(issuer,
                                             issuer,
                                             new List<Claim> { new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()) },
                                             expires: DateTime.UtcNow.AddMinutes(TokenLifetimeMinutes),
                                             signingCredentials: signingCredentials);

            return CommandResult.WithResponse(new JwtSecurityTokenHandler().WriteToken(token));
        }
    }
}
