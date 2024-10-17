using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tooms.models
{
    public class Message
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = new User();
        
        public int ConversationId { get; set; }
        public Conversation Conversation { get; set; } = new Conversation();
        
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}