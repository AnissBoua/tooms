using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using tooms.models;

namespace tooms.dtos.conversation
{
    public class ConversationCreateDto
    {
        public List<int> Users { get; set; } = new List<int>();
    }
}