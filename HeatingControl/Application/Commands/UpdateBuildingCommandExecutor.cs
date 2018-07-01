using Commons.Extensions;
using Commons.Localization;
using Storage.BuildingModel;

namespace HeatingControl.Application.Commands
{
    public class UpdateBuildingCommand
    {
        public string Name { get; set; }
    }

    public class UpdateBuildingCommandExecutor : ICommandExecutor<UpdateBuildingCommand>
    {
        private readonly IBuildingModelSaver _buildingModelSaver;

        public UpdateBuildingCommandExecutor(IBuildingModelSaver buildingModelSaver)
        {
            _buildingModelSaver = buildingModelSaver;
        }

        public CommandResult Execute(UpdateBuildingCommand command, CommandContext context)
        {
            if (command.Name.IsNullOrEmpty())
            {
                return CommandResult.WithValidationError(Localization.ValidationMessage.NameCantBeEmpty);
            }

            context.ControllerState.Model.Name = command.Name;

            _buildingModelSaver.Save(context.ControllerState.Model);

            return CommandResult.Empty;
        }
    }
}
