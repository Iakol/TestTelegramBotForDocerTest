using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceApi.Model;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ServiceApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ApiBotController(AppDbContext _db,HttpClient _httpClient) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult> RetriveVibeDataFromUser([FromBody] JsonElement vibeJson)
        {
            Console.WriteLine(JsonSerializer.Serialize(vibeJson));
            VibeModel vibe = new VibeModel
            {
                vibeLevel = (float)vibeJson.GetProperty("vibeLevel").GetDouble(),
                userID = vibeJson.GetProperty("userID").GetInt64(),
            };
            _db.Vibes.Add(vibe);
            await _db.SaveChangesAsync();
            return Ok();
        }
        [HttpPost]
        public async Task<ActionResult> RegisterUser([FromBody] JsonElement userJson)
        {

            UserModel user = new UserModel
            {
                Id = userJson.GetProperty("id").GetInt64(),
                FirstName = userJson.GetProperty("firstName").ToString(),
                LastName = userJson.GetProperty("lastName").ToString(),
                Username = userJson.GetProperty("username").ToString(),
            };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            UserModel registeredUser = _db.Users.FirstOrDefault(u => u.Id == user.Id);
            if (registeredUser == null)
            {
                return BadRequest("User NotRegistered");
            }
            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult> UnRegisterUser([FromBody] long userId)
        {
            _db.Users.Remove(_db.Users.FirstOrDefault(u => u.Id == userId));
            await _db.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<JsonElement>> GetUser([FromRoute] long userId)
        {
            UserModel user = _db.Users.FirstOrDefault(u => u.Id == userId);
            if (user is not null)
            {
                Console.WriteLine(user);
                return Ok(user);
            }
            Console.WriteLine("user not Found");
            return Ok(new { Error = "User not Found" });
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

        [HttpGet("{UserId}")]
        public async Task<ActionResult<string>> GetUserStats(long UserId)
        {
            return Ok(await _httpClient.GetStringAsync($"http://vibeservice/dashboard/GetDashBoardUrl/{UserId}"));


        }
    }

        
    }
