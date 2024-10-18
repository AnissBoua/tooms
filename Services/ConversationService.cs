using tooms.data;
using tooms.models;

namespace tooms.Services
{
    public class ConversationService
    {
        private readonly ApplicationDBContext context;
        public ConversationService(ApplicationDBContext DBcontext)
        {
            context = DBcontext;
        }

        public async Task<Conversation?> GetOne(int id)
        {
            var conversation = await context.Conversations.FindAsync(id);
            return conversation;
        }
    }
}
