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
    }
}
