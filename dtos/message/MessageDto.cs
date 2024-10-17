using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tooms.models;

namespace tooms.dtos.message
{
    public class MessageDto
    {
        public int Id { get; set; }
        public User User { get; set; } = new User();
        public Conversation Conversation { get; set; } = new Conversation();
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}