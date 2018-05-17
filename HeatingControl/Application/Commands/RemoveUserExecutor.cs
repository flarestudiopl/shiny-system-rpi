using Storage.StorageDatabase.User;

namespace HeatingControl.Application.Commands
{
    public interface IRemoveUserExecutor
    {
        void Execute(int userId, int removedByUserId);
    }

    public class RemoveUserExecutor : IRemoveUserExecutor
    {
        private readonly IUserDeactivator _userDeactivator;

        public RemoveUserExecutor(IUserDeactivator userDeactivator)
        {
            _userDeactivator = userDeactivator;
        }

        public void Execute(int userId, int removedByUserId)
        {
            _userDeactivator.Deactivate(userId, removedByUserId);
        }
    }
}
