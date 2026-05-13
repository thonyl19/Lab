using System.Collections.Generic;
using System.Linq;
using md = YourApp.Entities;
using YourApp.Db;

namespace YourApp.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public md.User GetUser(int id)
        {
            return _context.Users.Find(id);
        }

        public List<md.User> GetAll()
        {
            return _context.Users.ToList();
        }
    }
}
