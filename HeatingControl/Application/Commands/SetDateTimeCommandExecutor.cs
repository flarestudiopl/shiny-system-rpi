using System;
using Commons.Extensions;
using HardwareAccess.PlatformIntegration;
using Microsoft.Extensions.Configuration;

namespace HeatingControl.Application.Commands
{
    public class SetDateTimeCommand
    {
        public DateTime NewDateTimeUtc { get; set; }
    }

    public class SetDateTimeCommandExecutor : ICommandExecutor<SetDateTimeCommand>
    {
        private readonly IConfiguration _configuration;
        private readonly IProcessRunner _processRunner;

        private const string SetDateTimeCommandConfigPath = "PlatformIntegration:SetDateTimeCommand";
        private const string SetDateTimeArgumentsConfigPath = "PlatformIntegration:SetDateTimeArguments";

        public SetDateTimeCommandExecutor(IConfiguration configuration,
                                          IProcessRunner processRunner)
        {
            _configuration = configuration;
            _processRunner = processRunner;
        }

        public CommandResult Execute(SetDateTimeCommand command, CommandContext context)
        {
            var localTime = command.NewDateTimeUtc.ToLocalTime();

            _processRunner.Run(_configuration[SetDateTimeCommandConfigPath].FormatWith(localTime),
                               _configuration[SetDateTimeArgumentsConfigPath].FormatWith(localTime))
                          .Wait(2000);

            return CommandResult.Empty;
        }
    }
}
