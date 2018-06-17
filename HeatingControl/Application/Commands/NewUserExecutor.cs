using Commons;
using Commons.Extensions;
using Storage.StorageDatabase.User;

namespace HeatingControl.Application.Commands
{
    public interface INewUserExecutor
    {
        void Execute(NewUserExecutorInput input, int createdByUserId);
    }

    public class NewUserExecutorInput
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }

    public class NewUserExecutor : INewUserExecutor
    {
        private readonly IActiveUserByLoginProvider _activeUserByLoginProvider;
        private readonly IUserSaver _userSaver;

        public NewUserExecutor(IActiveUserByLoginProvider activeUserByLoginProvider,
                               IUserSaver userSaver)
        {
            _activeUserByLoginProvider = activeUserByLoginProvider;
            _userSaver = userSaver;
        }

        public void Execute(NewUserExecutorInput input, int createdByUserId)
        {
            if (_activeUserByLoginProvider.Provide(input.Login) != null)
            {
                Logger.Warning("User with login '{0}' already exists.", new object[] { input.Login });

                return;
            }

            if (input.Login.IsNullOrEmpty() || input.Password.IsNullOrEmpty())
            {
                Logger.Warning("User login and password shall not be empty.");

                return;
            }

            _userSaver.Save(input.Login, input.Password.CalculateHash(), createdByUserId);
        }
    }
}
