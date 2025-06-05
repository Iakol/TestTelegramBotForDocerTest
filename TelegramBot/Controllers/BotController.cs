using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using Telegram.Bot;
using Microsoft.Extensions.Options;
using TelegramBot.Services;

namespace TelegramBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BotController(IOptions<BotConfiguration> Config) : ControllerBase
    {
        [HttpGet("setWebhook")]
        public async Task<string> SetWebHook([FromServices] ITelegramBotClient bot, CancellationToken ct)
        {
            Console.WriteLine("I Set webhook");
            string webhookUrl = Environment.GetEnvironmentVariable("PublicUrl") + "/api/Bot";
            await bot.SetWebhook(webhookUrl, allowedUpdates: [], cancellationToken: ct);
            return $"Webhook set to {webhookUrl}";
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Update update, [FromServices] ITelegramBotClient bot, [FromServices] UpdateHandler handleUpdateService, CancellationToken ct)
        {
            Console.WriteLine("Post is exicute");

            try
            {
                await handleUpdateService.HandleUpdateAsync(bot, update, ct);
            }
            catch (Exception exception)
            {
                await handleUpdateService.HandleErrorAsync(bot, exception, Telegram.Bot.Polling.HandleErrorSource.HandleUpdateError, ct);
            }
            return Ok();
        }
    }
}
