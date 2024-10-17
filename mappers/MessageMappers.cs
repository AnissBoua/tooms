using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tooms.dtos.message;
using tooms.models;

namespace tooms.mappers
{
    public static class MessageMappers
    {
        public static MessageDto ToMessageDto(this Message message) {
            return new MessageDto {
                Id = message.Id,
                User = message.User,
                Conversation = message.Conversation,
                CreatedAt = message.CreatedAt,
                UpdatedAt = message.UpdatedAt
            };
        }

        public static Message ToMessage(this MessageCreateDto request) {
            return new Message {
                Content = request.Content,
            };
        }

        public static MessageConversationDto ToMessageConversationDto(this Message message) {
            return new MessageConversationDto {
                Id = message.Id,
                User = message.User.ToUserConversationDto(),
                Content = message.Content,
                CreatedAt = message.CreatedAt,
                UpdatedAt = message.UpdatedAt
            };
        }
    }
}