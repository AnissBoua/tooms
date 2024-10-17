using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tooms.dtos.user;
using tooms.models;

namespace tooms.dtos.contact
{
    public class ContactDto
    {
        public int Id { get; set; }
        public UserConversationDto User { get; set; } = new UserConversationDto();    
        public UserConversationDto Friend { get; set; } = new UserConversationDto();
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}