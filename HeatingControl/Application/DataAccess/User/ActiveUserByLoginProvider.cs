namespace HeatingControl.Application.DataAccess.User
{
    public interface IActiveUserByLoginProvider
    {
        Domain.User Provide(string login);
    }

    public class ActiveUserByLoginProvider : IActiveUserByLoginProvider
    {
        private readonly IRepository<Domain.User> _userRepository;

        public ActiveUserByLoginProvider(IRepository<Domain.User> userRepository)
        {
            _userRepository = userRepository;
        }

        public Domain.User Provide(string login)
        {
            return _userRepository.ReadSingleOrDefault(x => x.IsActive &&
                                                            x.Login == login);
        }
    }
}
