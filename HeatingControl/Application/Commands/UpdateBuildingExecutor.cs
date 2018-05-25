using Commons.Extensions;
using Domain.BuildingModel;
using Storage.BuildingModel;

namespace HeatingControl.Application.Commands
{
    public interface IUpdateBuildingExecutor
    {
        void Execute(UpdateBuildingExecutorInput input, Building model);
    }

    public class UpdateBuildingExecutorInput
    {
        public string Name { get; set; }
    }

    public class UpdateBuildingExecutor : IUpdateBuildingExecutor
    {
        private readonly IBuildingModelSaver _buildingModelSaver;

        public UpdateBuildingExecutor(IBuildingModelSaver buildingModelSaver)
        {
            _buildingModelSaver = buildingModelSaver;
        }

        public void Execute(UpdateBuildingExecutorInput input, Building model)
        {
            if (input.Name.IsNullOrEmpty())
            {
                return;
            }

            model.Name = input.Name;

            _buildingModelSaver.Save(model);
        }
    }
}
