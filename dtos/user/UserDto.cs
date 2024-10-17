using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tooms.dtos.user
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Nickname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}