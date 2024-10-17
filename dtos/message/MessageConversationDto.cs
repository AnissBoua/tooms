using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tooms.dtos.user;
using tooms.models;

namespace tooms.dtos.message
{
    public class MessageConversationDto
    {
        public int Id { get; set; }
        public UserConversationDto User { get; set; } = new UserConversationDto();
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}