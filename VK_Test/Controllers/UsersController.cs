using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VK_Test.Context;
using VK_Test.Models;

namespace VK_Test.Controllers
{
    [Route("Users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserContext _context;

        public UsersController(UserContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUserItems()
        {
          if (_context.UserItems == null)
          {
              return NotFound();
          }
            return await _context.UserItems.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(long id)
        {
          if (_context.UserItems == null)
          {
              return NotFound();
          }
            var user = await _context.UserItems.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            var timer = DateTime.Now;

          if (_context.UserItems == null)
          {
              return Problem("Entity set 'UserContext.UserItems'  is null.");
          }

            if (!_context.Validation(user.login, user.password))
            {
                //ждем 5 секунд создания из условия
                while ((DateTime.Now - timer).TotalSeconds < 5)
                    continue;
                return BadRequest("Ошибка создания пользователя!\nЛогин и пароль не могут быть пустыми или пользователь с таким логином уже существует!");
            }

            if (_context.AddUser(user))
            {
                await _context.SaveChangesAsync();

                //ждем 5 секунд создания из условия
                while ((DateTime.Now - timer).TotalSeconds < 5)
                    continue;

                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
            }
            //ждем 5 секунд создания из условия
            while ((DateTime.Now - timer).TotalSeconds < 5)
                continue;

            return BadRequest("Admin может быть только один!");
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(long id)
        {
            if (_context.UserItems == null)
            {
                return NotFound();
            }
            var user = await _context.UserItems.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.DeleteUser(id);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return (_context.UserItems?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
