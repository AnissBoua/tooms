using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace tooms.models
{
    public class UserConversation
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = new User();
        public int ConversationId { get; set; }
        public Conversation Conversation { get; set; } = new Conversation();
    }
}