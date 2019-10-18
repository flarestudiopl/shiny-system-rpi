using Commons.Extensions;
using Commons.Localization;
using HeatingControl.Application.DataAccess.Building;

namespace HeatingControl.Application.Commands
{
    public class UpdateBuildingCommand
    {
        public string Name { get; set; }
    }

    public class UpdateBuildingCommandExecutor : ICommandExecutor<UpdateBuildingCommand>
    {
        private readonly IBuildingNameUpdater _buildingNameUpdater;

        public UpdateBuildingCommandExecutor(IBuildingNameUpdater buildingNameUpdater)
        {
            _buildingNameUpdater = buildingNameUpdater;
        }

        public CommandResult Execute(UpdateBuildingCommand command, CommandContext context)
        {
            if (command.Name.IsNullOrEmpty())
            {
                return CommandResult.WithValidationError(Localization.ValidationMessage.NameCantBeEmpty);
            }

            _buildingNameUpdater.Update(command.Name, context.ControllerState.Model);

            return CommandResult.Empty;
        }
    }
}
