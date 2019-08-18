namespace HeatingControl.Application.DataAccess.User
{
    public interface IActiveUserByLoginProvider
    {
        Domain.StorageDatabase.User Provide(string login);
    }

    public class ActiveUserByLoginProvider : IActiveUserByLoginProvider
    {
        private readonly IRepository<Domain.StorageDatabase.User> _userRepository;

        public ActiveUserByLoginProvider(IRepository<Domain.StorageDatabase.User> userRepository)
        {
            _userRepository = userRepository;
        }

        public Domain.StorageDatabase.User Provide(string login)
        {
            return _userRepository.ReadSingleOrDefault(x => x.IsActive &&
                                                            x.Login == login);
        }
    }
}
