using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using TelegramBot.Enums;
using TelegramBot.Services;

namespace TelegramBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiComunication([FromServices] ITelegramBotClient bot, [FromServices] QuizService _quizService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> RetriveCommand(ApiComandEnum command) 
        {
            await _quizService.StartQuiz(command);
            return Ok(command);
        }

        
    }
}
