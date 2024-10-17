using System.Data.Entity;
using Microsoft.AspNetCore.Mvc;
using tooms.data;
using tooms.dtos.message;
using tooms.mappers;


namespace tooms.controllers
{
    [Route("api/message")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly ApplicationDBContext context;
        public MessageController(ApplicationDBContext DBcontext)
        {
            context = DBcontext;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MessageCreateDto messageDto) {
            var message = messageDto.ToMessage();
            await context.Messages.AddAsync(message);
            await context.SaveChangesAsync();

            return Ok(message.ToMessageDto());
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] MessageUpdateDto messageDto) {
            var message = await context.Messages.FindAsync(id);
            if (message == null) return NotFound();

            message.Content = messageDto.Content;
            message.UpdatedAt = DateTime.Now;

            await context.SaveChangesAsync();

            return Ok(message.ToMessageDto());
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(int id) {
            var message = await context.Messages.FindAsync(id);
            if (message == null) return NotFound();

            context.Messages.Remove(message);
            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}