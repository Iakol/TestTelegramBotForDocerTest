using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceApi.Model;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ServiceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiBotController(AppDbContext _db) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult> RetriveVibeDataFromUser([FromBody] VibeModel vibe)
        {
            _db.Vibes.Add(vibe);
            await _db.SaveChangesAsync();
            return Ok();
        }
        [HttpPost]
        public async Task<ActionResult> RegisterUser([FromBody] UserModel user)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult> UnRegisterUser([FromBody] long userId)
        {
            _db.Users.Remove(_db.Users.FirstOrDefault(u => u.Id == userId));
            await _db.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<JsonElement>> GetUser([FromRoute] long userId)
        {
            UserModel user = _db.Users.FirstOrDefault(u => u.Id == userId);
            if (user is not null) 
            {
                Console.WriteLine(user);
                return Ok(user);
            }
            Console.WriteLine("user not Found");
            return Ok( new { Error = "User not Found"});
        }


        [HttpPost]
        public async Task<ActionResult> UpdateUser([FromBody] UserModel user)
        {
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<List<long>>> GetRegisteredUsers()
        {
           
            return Ok(_db.Users.Select(u => u.Id).ToList());
        }
    }

        
    }
