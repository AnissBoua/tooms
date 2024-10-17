using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tooms.models;

namespace tooms.dtos.user
{
public class UserConversationDto
{
    public int Id { get; set; }
    public string Nickname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    // Avoid including sensitive data like passwords in DTOs
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
}