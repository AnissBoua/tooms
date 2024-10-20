using tooms.data;
using Microsoft.EntityFrameworkCore;
using tooms.models;

namespace tooms.Services
{
    public class UserService
    {
        private readonly ApplicationDBContext context;
        public UserService(ApplicationDBContext DBcontext)
        {
            context = DBcontext;
        }

        public async Task<User?> GetOne(int id)
        {
            var user = await context.Users.FindAsync(id);
            return user;
        }

        public async Task<User?> GetByToken(string identifier) {
            if (identifier == null) return null;

            var user = await context.Users.Where(user => user.Identifier == identifier).FirstOrDefaultAsync();
            return user;
        }
    }
}
