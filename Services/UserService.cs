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

        public async Task<User?> GetByToken(string token)
        {
            if (token == null) return null;
            if (token == "HelloMotoToken1")
            {
                return await context.Users.FindAsync(1);
            } else if (token == "HelloMotoToken2")
            {
                return await context.Users.FindAsync(2);
            } else if (token == "HelloMotoToken3")
            {
                return await context.Users.FindAsync(3);
            }
            return null;
        }
    }
}
