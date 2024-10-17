using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tooms.dtos.message
{
    public class MessageCreateDto
    {
        public int UserId { get; set; } = 0;
        public string Content { get; set; } = string.Empty;
    }
}