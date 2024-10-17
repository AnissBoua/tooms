using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tooms.data;
using tooms.dtos.contact;
using tooms.models;

namespace tooms.mappers
{
    public static class ContactMapper
    {
        public static ContactDto ToContactDto(this Contact conversation, ApplicationDBContext context) {
            var user = context.Users
                .Where(u => u.Id == conversation.UserId)
                .Select(u => u.ToUserConversationDto())
                .FirstOrDefault();

            var friend = context.Users
                .Where(u => u.Id == conversation.FriendId)
                .Select(u => u.ToUserConversationDto())
                .FirstOrDefault();

            return new ContactDto {
                Id = conversation.Id,
                User = user,
                Friend = friend,
                CreatedAt = conversation.CreatedAt,
                UpdatedAt = conversation.UpdatedAt
            };
        }
    }
}