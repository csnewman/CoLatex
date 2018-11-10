using System;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace CoLatex.Database
{
    public class UserRepository
    {
        private DatabaseContext _databaseContext;

        public UserRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public Task<UserDbModel> GetUserByUsername(string username)
        {
            return _databaseContext.Users.Find(user => string.Equals(user.Username, username)).FirstOrDefaultAsync();
        }

        public Task<UserDbModel> GetUserByEmail(string email)
        {
            return _databaseContext.Users.Find(user => string.Equals(user.Email, email)).FirstOrDefaultAsync();
        }
    }
}