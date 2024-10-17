using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using tooms.dtos.message;
using tooms.dtos.user;
using tooms.models;

namespace tooms.dtos.conversation
{
    public class ConversationDto
    {
        public int Id { get; set; }
        public List<UserConversationDto> Users { get; set; } = new List<UserConversationDto>();
        public List<MessageConversationDto> Messages { get; set; } = new List<MessageConversationDto>();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}