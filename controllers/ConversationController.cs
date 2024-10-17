using Microsoft.AspNetCore.Mvc;
using tooms.data;
using tooms.models;
using tooms.dtos.conversation;
using Microsoft.EntityFrameworkCore;
using tooms.dtos.message;
using tooms.dtos.user;
using tooms.mappers;

namespace tooms.controllers
{
    [Route("api/conversation")]
    [ApiController]
    public class ConversationController : ControllerBase
    {
        private readonly ApplicationDBContext context;
        public ConversationController(ApplicationDBContext DBcontext)
        {
            context = DBcontext;
        }

        // TODO: Return only the conversations where the user is present
        [HttpGet]
        public async Task<IActionResult> GetAll() {
            var conversations = await context.Conversations.ToListAsync();
            return Ok(conversations);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne(int id)
        {
            var conversation = await context.Conversations.FindAsync(id);
            if (conversation == null) return NotFound();

            return Ok(conversation.ToConversationDto(context));
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ConversationCreateDto conversationDto) {
            if (conversationDto == null || !conversationDto.Users.Any()) return BadRequest("Users field is required.");

            var conversation = new Conversation {
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            // Add the conversation to the context
            await context.Conversations.AddAsync(conversation);
            // Save changes
            await context.SaveChangesAsync();

            // Add users to the conversation
            foreach (var userId in conversationDto.Users)
            {
                var user = await context.Users.FindAsync(userId);
                if (user == null) continue;

                var userConversation = new UserConversation{
                    User = user, // Set the user entity directly
                    Conversation = conversation // Set the conversation reference
                };

                // Add to the UserConversations collection
                context.UserConversations.Add(userConversation);
            }

            // Save changes
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOne), new { id = conversation.Id }, conversation.ToConversationDto(context));
        }

        [HttpPost]
        [Route("{id}/users")]
        public async Task<IActionResult> AddUser(int id, [FromBody] ConversationCreateDto conversationDto) {
            var conversation = await context.Conversations.FindAsync(id);
            if (conversation == null) return NotFound();

            if (conversationDto == null || !conversationDto.Users.Any()) return BadRequest("Users field is required.");

            // Fetch user entities based on IDs
            var users = context.Users
                .Where(u => conversationDto.Users.Contains(u.Id))
                .ToList();

            if (!users.Any()) return BadRequest("No valid users found.");

            foreach (var user in users)
            {
                var userConversation = new UserConversation
                {
                    User = user,
                    Conversation = conversation
                };

                context.UserConversations.Add(userConversation);
            }

            conversation.UpdatedAt = DateTime.Now;
            await context.SaveChangesAsync();

            return Ok(conversation.ToConversationDto(context));
        }

        // TODO: Check if the user it's the admin of the conversation
        [HttpPut]
        [Route("{id}/user")]
        public async Task<IActionResult> RemoveUser(int id, [FromBody] ConversationCreateDto conversationDto) {
            var conversation = await context.Conversations.FindAsync(id);
            if (conversation == null) return NotFound();

            if (conversationDto == null || !conversationDto.Users.Any()) return BadRequest("Users field is required.");

            // Fetch user entities based on IDs
            var users = context.Users
                .Where(u => conversationDto.Users.Contains(u.Id))
                .ToList();

            if (!users.Any()) return BadRequest("No valid users found.");

            foreach (var user in users) {
                var userConversation = new UserConversation {
                    User = user,
                    Conversation = conversation
                };

                context.UserConversations.Remove(userConversation);
            }

            conversation.UpdatedAt = DateTime.Now;
            await context.SaveChangesAsync();

            return Ok(conversation.ToConversationDto(context));
        }

        // TODO: Check if the user it's the admin of the conversation
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(int id) {
            var conversation = await context.Conversations.FindAsync(id);
            if (conversation == null) return NotFound();

            context.Conversations.Remove(conversation);
            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}