using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tooms.data;
using tooms.dtos.contact;
using tooms.mappers;
using tooms.models;

namespace tooms.controllers
{
    [Route("api/contact")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly ApplicationDBContext context;
        public ContactController(ApplicationDBContext DBcontext)
        {
            context = DBcontext;
        }
        
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddContact([FromBody] ContactAddDto contactDto) {
            // Find the user by email
            var friend = context.Users.FirstOrDefault(user => user.Email == contactDto.Email);
            if (friend == null) return NotFound("User not found");

            var userIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdentifier == null) return Unauthorized("User ID not found");
            Console.WriteLine(userIdentifier);

            var user = context.Users.Where(user => user.Identifier == userIdentifier).FirstOrDefault();
            if (user == null) return NotFound("User not found");

            // Add the user to the contacts
            var contact = new Contact {
                User = user, // TODO: Get the user id from the token
                Friend = friend,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            await context.Contacts.AddAsync(contact);
            await context.SaveChangesAsync();

            return Ok(contact.ToContactDto(context));
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteContact(int id) {
            var contact = await context.Contacts.FindAsync(id);
            if (contact == null) return NotFound();

            context.Contacts.Remove(contact);
            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}