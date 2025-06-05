using System.Text.Json;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

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
                //{ EditedMessage: { } message } => OnMessage(message),
                { CallbackQuery: { } callbackQuery } => OnCallbackQuery(callbackQuery),
                //{ InlineQuery: { } inlineQuery } => OnInlineQuery(inlineQuery),
                //{ ChosenInlineResult: { } chosenInlineResult } => OnChosenInlineResult(chosenInlineResult),
                //{ Poll: { } poll } => OnPoll(poll),
                //{ PollAnswer: { } pollAnswer } => OnPollAnswer(pollAnswer),
                //ChannelPost:
                // EditedChannelPost:
                // ShippingQuery:
                // PreCheckoutQuery:
                //_ => UnknownUpdateHandlerAsync(update)
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
            HttpResponseMessage responce = await _httpClient.GetAsync($"http://serviceapi:80/api/ApiBot/GetUser/{message.From.Id}");

            JsonElement jsonresponce = await responce.Content.ReadFromJsonAsync<JsonElement>();

            string Text;
            bool registerFlag;
            InlineKeyboardMarkup button;

            if (jsonresponce.TryGetProperty("UserId", out _))
            {
                Text = $"Привіт {message.From.FirstName} дякую що користуєшся ботом," +
                    $"якщо ти хочеш відписатися від повідомлень , нажми кнопку нижче";
                registerFlag = true;
                button = new InlineKeyboardMarkup(new[]
                {
                    new[]{
                        InlineKeyboardButton.WithCallbackData("Відписатися",$"UnRegisterUser_{message.From.Id}")
                    }
                });
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
            await bot.DeleteMessage(callbackQuery.Message.Chat, callbackQuery.Message.Id);


            if (callbackQuery.Data.IndexOf("Quiz") != -1)
            {
                string[] UserReply = callbackQuery.Data.Split("_");
                int QuestionNum = int.Parse( UserReply[1].Replace("Question",""));
                await vibeBuffer.SetVibeForUser(callbackQuery.From.Id, QuestionNum, int.Parse(UserReply[2]));

                await _quizService.NextQuestion(callbackQuery.Data, callbackQuery.From.Id);
            }
            else if (callbackQuery.Data.StartsWith("RegisterUser"))
            {
                string[] UserReply = callbackQuery.Data.Split("_");

                var User = new { Id = long.Parse(UserReply[1]), FirstName = UserReply[2], LastName = UserReply[3], Username = UserReply[4] };
                JsonContent UserContent = JsonContent.Create(User);
                await _httpClient.PostAsync("http://serviceapi:80/api/ApiBot/RegisterUser", UserContent);
                await bot.SendMessage(callbackQuery.From.Id, "Вас зареєстровано для опитувань");
                await _quizService.StartQuizNowRegisteredUser(callbackQuery.From.Id);

            }
            else if (callbackQuery.Data.StartsWith("UnRegisterUser"))
            {
                JsonContent UserContent = JsonContent.Create(callbackQuery.From.Id);
                await _httpClient.PostAsync("http://serviceapi:80/api/ApiBot/UnRegisterUser", UserContent);
                await bot.SendMessage(callbackQuery.From.Id, "Вас відписано від опитувань");
                await _quizService.FinishTaskForNowUnregisteredUser(callbackQuery.From.Id);

            }
            else 
            {
            
            }

        }

    }

        
}
