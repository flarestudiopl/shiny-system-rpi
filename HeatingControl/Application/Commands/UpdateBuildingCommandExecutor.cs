using Commons.Extensions;
using Commons.Localization;
using Domain;
using HeatingControl.Application.DataAccess;

namespace HeatingControl.Application.Commands
{
    public class UpdateBuildingCommand
    {
        public string Name { get; set; }
    }

    public class UpdateBuildingCommandExecutor : ICommandExecutor<UpdateBuildingCommand>
    {
        private readonly IRepository<Building> _buildingRepository;

        public UpdateBuildingCommandExecutor(IRepository<Building> buildingRepository)
        {
            _buildingRepository = buildingRepository;
        }

        public CommandResult Execute(UpdateBuildingCommand command, CommandContext context)
        {
            if (command.Name.IsNullOrEmpty())
            {
                return CommandResult.WithValidationError(Localization.ValidationMessage.NameCantBeEmpty);
            }

            context.ControllerState.Model.Name = command.Name;

            _buildingRepository.Update(context.ControllerState.Model, null);

            return CommandResult.Empty;
        }
    }
}
