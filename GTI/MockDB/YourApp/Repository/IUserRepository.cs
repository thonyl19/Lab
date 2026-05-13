using System.Collections.Generic;
using md = YourApp.Entities;

namespace YourApp.Repository
{
    public interface IUserRepository
    {
        md.User GetUser(int id);
        List<md.User> GetAll();
    }
}
