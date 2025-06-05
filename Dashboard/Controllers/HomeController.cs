using Dashboard.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;

namespace Dashboard.Controllers
{
    public class HomeController(HttpClient _httpClient) : Controller
    {

        public async Task<IActionResult> Index(long id)
        {
            return View(id);
        }

        [HttpPost]
        public async Task<IActionResult> GetVibeDateForUser(long id)
        {
            var response = await _httpClient.GetAsync($"http://serviceapi:80/Dashboard/GetVibeDataOfUser/${id}");
            return Content(await response.Content.ReadAsStringAsync(), "application/json");
        }
    }
}
