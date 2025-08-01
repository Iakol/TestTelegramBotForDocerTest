using System.Text.Json;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using static System.Net.WebRequestMethods;

namespace TelegramBot.Services
{
    public class UpdateHandler(ITelegramBotClient bot, ILogger<UpdateHandler> logger, HttpClient _httpClient, QuizService _quizService, DictonaryBuffer vibeBuffer) : IUpdateHandler
    {
        public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
        {
            Console.WriteLine(exception.Message);
        }



        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Console.WriteLine("I hande");
            await (update switch
            {
                { Message: { } message } => OnMessage(message),
                { CallbackQuery: { } callbackQuery } => OnCallbackQuery(callbackQuery),
                
            });
        }

        public async Task OnMessage(Message message) {
            if (message.Text is not { } messageText)
                return;
            var text = (messageText.Split(' ')[0]);
            Message sentMessage = await (messageText.Split(' ')[0] switch
            {
                "/start" => OnStart(message),                
                _ => BasicReply(message)
            });

        }

        public async Task<Message> OnStart(Message message) 
        {
            return await BasicReply(message);
        }

        public async Task<Message> BasicReply(Message message)
        {
            JsonContent content = JsonContent.Create(message.From.Id);
            HttpResponseMessage responce = await _httpClient.GetAsync($"http://vibeservice/serviceapi/api/ApiBot/GetUser/{message.From.Id}");

            JsonElement jsonresponce = await responce.Content.ReadFromJsonAsync<JsonElement>();

            string Text;
            bool registerFlag;
            InlineKeyboardMarkup button;

            if (jsonresponce.TryGetProperty("id", out _))
            {
                Text = $"Привіт {message.From.FirstName} дякую що користуєшся ботом," +
                    $"Вибирай як хочеш взаємодіяти з ботом через меню";
                registerFlag = true;
                button = new InlineKeyboardButton[][]
                {
                    [("Продивитися Статисуку настроїв","GetMyVibes")],
                    [("Відписатися",$"UnRegisterUser_{message.From.Id}")]
                };
            }
            else 
            {
                Text = $"Привіт {message.Chat.FirstName} цей бот збирає дані про ваший настрій," +
                    $"щоб використати бот потрібно зареєструватися";
                registerFlag = false;
                button = new InlineKeyboardMarkup(new[]
                {
                    new[]{
                        InlineKeyboardButton.WithCallbackData("Зареєструватися",$"RegisterUser_{message.From.Id}_{message.From.FirstName}_{message.From.LastName}_{message.From.Username}")
                    }
                });
            }

            return await bot.SendMessage(message.From.Id, Text, replyMarkup: button);
        }

        public async Task OnCallbackQuery(CallbackQuery callbackQuery)
        {
            await bot.AnswerCallbackQuery(callbackQuery.Id);


            if (callbackQuery.Data.IndexOf("Quiz") != -1)
            {
                string[] UserReply = callbackQuery.Data.Split("_");
                int QuestionNum = int.Parse(UserReply[1].Replace("Question", ""));
                await vibeBuffer.SetVibeForUser(callbackQuery.From.Id, QuestionNum, int.Parse(UserReply[2]));

                await _quizService.NextQuestion(callbackQuery.Data, callbackQuery.From.Id);
            }
            else if (callbackQuery.Data.StartsWith("RegisterUser"))
            {
                await bot.DeleteMessage(callbackQuery.Message.Chat, callbackQuery.Message.Id);
                string[] UserReply = callbackQuery.Data.Split("_");
                var User = new { Id = long.Parse(UserReply[1]), FirstName = UserReply[2], LastName = UserReply[3], Username = UserReply[4] };
                Console.WriteLine(JsonSerializer.Serialize(User));
                JsonContent UserContent = JsonContent.Create(User);
                HttpResponseMessage response = new HttpResponseMessage();
                for (int attempt = 0; attempt <= 3; attempt++)
                {
                    response = await _httpClient.PostAsync("http://vibeservice/serviceapi/api/ApiBot/RegisterUser", UserContent);
                    if (response.IsSuccessStatusCode)
                    {
                        break;

                    }
                    else
                    {
                        await Task.Delay(attempt * 100);
                    }

                }

                if (response.IsSuccessStatusCode)
                {
                    await bot.SendMessage(callbackQuery.From.Id, "Вас зареєстровано для опитувань");
                    await _quizService.StartQuizNowRegisteredUser(callbackQuery.From.Id);
                }
                else
                {
                    await bot.SendMessage(callbackQuery.From.Id, "Вас не зареєстровано для опитувань, повторіть спробу пізніше");
                    Console.WriteLine("Bad Request");

                }

            }
            else if (callbackQuery.Data.StartsWith("GetMyVibes")) 
            {
                await bot.DeleteMessage(callbackQuery.Message.Chat, callbackQuery.Message.Id);
                HttpResponseMessage responce = await _httpClient.GetAsync($"http://vibeservice/serviceapi/api/ApiBot/GetUserStats/{callbackQuery.From.Id}" );
                string url = await responce.Content.ReadAsStringAsync();
                url = "http://" + Environment.GetEnvironmentVariable("PublicUrl") + "/dashboard/" + url;
                Console.WriteLine("Url is");
                Console.WriteLine(url);

                await bot.SendMessage(callbackQuery.From.Id, "Ваша статистика",replyMarkup:new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl("Ваша статистика",url)));

            }
            else if (callbackQuery.Data.StartsWith("UnRegisterUser"))
            {
                await bot.DeleteMessage(callbackQuery.Message.Chat, callbackQuery.Message.Id);
                JsonContent UserContent = JsonContent.Create(callbackQuery.From.Id);
                await _httpClient.PostAsync("http://vibeservice/serviceapi/api/ApiBot/UnRegisterUser", UserContent);
                await bot.SendMessage(callbackQuery.From.Id, "Вас відписано від опитувань", replyMarkup: new ReplyKeyboardRemove());
                await _quizService.FinishTaskForNowUnregisteredUser(callbackQuery.From.Id);

            }
            else
            {

            }

        }

    }

        
}
