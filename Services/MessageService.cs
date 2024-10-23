using System;
using System.Threading.Tasks;
using tooms.data;
using tooms.dtos.message;
using tooms.models;

namespace tooms.Services
{
    public class MessageService
    {
        private readonly ApplicationDBContext context;

        public MessageService(ApplicationDBContext context)
        {
            this.context = context;
        }

        public async Task<Message> CreateMessageAsync(MessageCreateDto messageDto)
        {
            var message = new Message
            {
                UserId = messageDto.UserId,
                ConversationId = messageDto.ConversationId,
                Content = messageDto.Content,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await context.Messages.AddAsync(message);
            await context.SaveChangesAsync();

            return message;
        }
    }
}