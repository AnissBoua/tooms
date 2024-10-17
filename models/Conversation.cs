using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tooms.models
{
    public class Conversation
    {
        public int Id { get; set; }
        public List<UserConversation> UserConversations { get; set; } = new List<UserConversation>();
        public List<Message> Messages { get; set; } = new List<Message>();
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}