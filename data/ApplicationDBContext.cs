using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using tooms.models;

namespace tooms.data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Contact>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);  // Set UserId to NULL if the user is deleted

            modelBuilder.Entity<Contact>()
                .HasOne(c => c.Friend)
                .WithMany()
                .HasForeignKey(c => c.FriendId)
                .OnDelete(DeleteBehavior.Cascade);  // Set FriendId to NULL if the friend is deleted
        }




        public DbSet<User> Users { get; set; } 
        public DbSet<Contact> Contacts { get; set; } 
        public DbSet<UserConversation> UserConversations { get; set; } 
        public DbSet<Conversation> Conversations { get; set; } 
        public DbSet<Message> Messages { get; set; } 
        
    }
}