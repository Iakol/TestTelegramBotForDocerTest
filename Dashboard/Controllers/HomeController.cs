using Dashboard.Models;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;

namespace Dashboard.Controllers
{
    public class HomeController(HttpClient _httpClient) : Controller
    {
        [Route("/{id}")]
        public async Task<IActionResult> Index(long id)
        {
            return View(id);
        }

        [HttpPost("/GetVibeDateForUser/{id}")]
        public async Task<IActionResult> GetVibeDateForUser(long id)
        {
            var response = await _httpClient.GetAsync($"http://vibeservice/serviceapi/Dashboard/GetVibeDataOfUser/${id}");
            return Content(await response.Content.ReadAsStringAsync(), "application/json");
        }

        [Route("/[action]/{id}")]
        public async Task<IActionResult> GetDashBoardUrl(long id) 
        {
            string url = Url.Action(nameof(Index), new { id });
            

            return Ok(url);
        }
    }
}
