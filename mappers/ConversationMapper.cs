using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tooms.data;
using tooms.dtos.conversation;
using tooms.models;

namespace tooms.mappers
{
    public static class ConversationMapper
    {
        public static ConversationDto ToConversationDto(this Conversation conversation, ApplicationDBContext context) {
            var users = context.UserConversations
                .Where(uc => uc.ConversationId == conversation.Id)
                .Select(uc => uc.User.ToUserConversationDto())
                .ToList();

            Console.WriteLine("LOGGING USERS");
            Console.WriteLine(users);

            var messages = context.Messages
                .Where(m => m.ConversationId == conversation.Id)
                .Select(m => m.ToMessageConversationDto())
                .ToList();

            return new ConversationDto {
                Id = conversation.Id,
                Users = users,
                Messages = messages,
                CreatedAt = conversation.CreatedAt,
                UpdatedAt = conversation.UpdatedAt
            };
        }
    }
}