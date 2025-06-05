using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceApi.Model;

namespace ServiceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController(AppDbContext _db) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<VibeModel>>> GetVibeDataOfUser(int UserID) 
        {
            return Ok(_db.Vibes.Where(v => v.userID == UserID).OrderByDescending(v => v.timeOfTest).ToList());
        }
    }
}
