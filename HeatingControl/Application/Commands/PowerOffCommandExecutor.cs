using Commons;
using Commons.Localization;
using HardwareAccess.PlatformIntegration;
using Microsoft.Extensions.Configuration;
using System;

namespace HeatingControl.Application.Commands
{
    public class PowerOffCommand
    {
    }

    public class PowerOffCommandExecutor : ICommandExecutor<PowerOffCommand>
    {
        private readonly IConfiguration _configuration;
        private readonly IProcessRunner _processRunner;

        private const string PowerOffCommandConfigPath = "PlatformIntegration:PowerOffCommand";
        private const string PowerOffArgumentConfigPath = "PlatformIntegration:PowerOffArguments";

        public PowerOffCommandExecutor(IConfiguration configuration,
                                       IProcessRunner processRunner)
        {
            _configuration = configuration;
            _processRunner = processRunner;
        }

        public CommandResult Execute(PowerOffCommand command, CommandContext context)
        {
            Logger.Info(Localization.NotificationMessage.PowerOffRequested);

            string processResult;

            try
            {
                processResult = _processRunner.Run(_configuration[PowerOffCommandConfigPath],
                                                   _configuration[PowerOffArgumentConfigPath])
                                              .Result;
            }
            catch (Exception exception)
            {
                while (exception.InnerException != null)
                {
                    exception = exception.InnerException;
                }

                processResult = exception.Message;
            }

            return CommandResult.WithResponse(processResult);
        }
    }
}
