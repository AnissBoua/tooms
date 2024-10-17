using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace tooms.models
{
    public class Contact
    {
        // Primary Key
        public int Id { get; set; }
        public int UserId { get; set; }  // Make sure this property exists
        public int FriendId { get; set; }  // Make sure this property exists
        [NotMapped]
        public User User { get; set; } = new User();   
        [NotMapped]
        public User Friend { get; set; } = new User();
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}