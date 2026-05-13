using YourApp.Repository;

namespace YourApp.Service
{
    public class UserService
    {
        private readonly IUserRepository _repository;

        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }

        public string GetUserName(int id)
        {
            var user = _repository.GetUser(id);
            return user?.Name;
        }
    }
}
