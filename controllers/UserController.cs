using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using tooms.data;
using tooms.dtos.user;
using tooms.mappers;

namespace tooms.controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDBContext context;
        public UserController(ApplicationDBContext DBcontext)
        {
            context = DBcontext;
        }

        [HttpGet]
        public IActionResult GetAll() {
            var users = context.Users.ToList();  // Synchronous method
            var usersDto = users.Select(user => user.ToUserDto());

            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne(int id) {
            var user = await context.Users.FindAsync(id);
            if (user == null) return NotFound();

            return Ok(user.ToUserDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserCreateDto userDto) {
            // Check if the user already exists
            var userExists = context.Users.Any(user => user.Email == userDto.Email);
            if (userExists) return BadRequest("User already exists");
            
            var user = userDto.ToUser();
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOne), new { id = user.Id }, user.ToUserDto());
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UserUpdateDto userDto) {
            var user = await context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.Nickname = userDto.Nickname;
            user.Email = userDto.Email;
            user.UpdatedAt = DateTime.Now;

            await context.SaveChangesAsync();

            return Ok(user.ToUserDto());
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(int id) {
            var user = await context.Users.FindAsync(id);
            if (user == null) return NotFound();

            context.Users.Remove(user);
            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}