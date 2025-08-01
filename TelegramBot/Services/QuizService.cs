using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Enums;
using TelegramBot.Model;

namespace TelegramBot.Services
{
    public class QuizService([FromServices] ITelegramBotClient bot,DictonaryBuffer vibeBuffer, HttpClient _httpClient)
    {
        public static string QuizType = "";

        public InlineKeyboardMarkup GetKeyBoard(string QuizTime,int questionCount) 
        {
            return new InlineKeyboardMarkup(new[]
            {
                new[]{  InlineKeyboardButton.WithCallbackData("1",$"{QuizTime}_Question{ questionCount }_1"),
                        InlineKeyboardButton.WithCallbackData("2",$"{QuizTime}_Question{ questionCount }_2"),
                        InlineKeyboardButton.WithCallbackData("3",$"{QuizTime}_Question{ questionCount }_3"),
                        InlineKeyboardButton.WithCallbackData("4",$"{QuizTime}_Question{ questionCount }_4"),
                        InlineKeyboardButton.WithCallbackData("5",$"{QuizTime}_Question{ questionCount }_5"),
                }
            });
        }
        public async Task StartQuiz(ApiComandEnum flag) 
        {
            await BreakQuiz();
            QuizType = Enum.GetName<ApiComandEnum>(flag).ToString();
            var responce = await _httpClient.GetAsync("http://vibeservice/serviceapi/api/ApiBot/GetRegisteredUsers");
            List<long> Users = await responce.Content.ReadFromJsonAsync<List<long>>(); // ask abot Register users
            if (Users.Count != 0)
            {
                await Task.WhenAll(Users.Select(u => NextQuestion($"{QuizType}_StartQuiz", u)));
            }
            else 
            {
                Console.WriteLine("User list is null");
            }
        }

        public async Task StartQuizNowRegisteredUser(long id)
        {
            if (QuizType.Equals("")) 
            {
                int time = DateTime.Now.Hour;
                if (time >= 8 && time < 14)
                {
                    QuizType = "MorningQuiz";
                }
                else if (time >= 14 && time < 20)
                {
                    QuizType = "MidDayQuiz";

                }
                else
                {
                    QuizType = "EvningQuiz";
                }
            }
            await NextQuestion( $"{QuizType}_StartQuiz", id);
        }

        public async Task FinishTaskForNowUnregisteredUser(long UserId) 
        {
            vibeBuffer.getDictonary.Remove(UserId, out _);
            await GetAndRemoveQuizMessageForUser(UserId, DeleteMessageInBot);
        }

        public async Task NextQuestion(string PrevQuestinTag, long UserId)
        {

            if (PrevQuestinTag.StartsWith("MorningQuiz"))
            {
                await MorningQuiz(PrevQuestinTag, UserId);
            }
            else if (PrevQuestinTag.StartsWith("MidDayQuiz"))
            {
                await MidDayQuiz(PrevQuestinTag, UserId);
            }
            else if (PrevQuestinTag.StartsWith("EvningQuiz"))
            {
                await EvningQuiz(PrevQuestinTag, UserId);
            }
            else 
            {
                await BreakQuiz();
            }

        }

        public async Task BreakQuiz() 
        {
            await Task.WhenAll(vibeBuffer.getDictonary.Select(k => FinishQuiz(k.Key)));
        }
        public async Task FinishQuiz(long UserId) 
        {
            await GetAndRemoveQuizMessageForUser(UserId, DeleteMessageInBot);
            await GetAndSendVibeModelForUser(UserId, SendVibeToApi);

        }

        public async Task MorningQuiz(string PrevQuestinTag,long UserId)
        {
            string split = PrevQuestinTag.Split("_")[1];
            Message mes;
            switch (split) {
                case "StartQuiz":
                    mes = await bot.SendMessage(UserId, " Починаємо Ранкове Опитування \n Питання 1", replyMarkup: GetKeyBoard("MorningQuiz", 1));
                    await vibeBuffer.SetQuizMessagesForUserToDelete(UserId, mes.Id);
                    break;
                case "Question1":
                    mes = await bot.SendMessage(UserId, "Питання 2", replyMarkup: GetKeyBoard("MorningQuiz", 2));
                    await vibeBuffer.SetQuizMessagesForUserToDelete(UserId, mes.Id);
                    break;
                case "Question2":
                    mes = await bot.SendMessage(UserId, "Питання 3", replyMarkup: GetKeyBoard("MorningQuiz", 3));
                    await vibeBuffer.SetQuizMessagesForUserToDelete(UserId, mes.Id);
                    break;
                case "Question3":
                    mes = await bot.SendMessage(UserId, "Питання 4", replyMarkup: GetKeyBoard("MorningQuiz", 4));
                    await vibeBuffer.SetQuizMessagesForUserToDelete(UserId, mes.Id);
                    break;
                case "Question4":
                    mes = await bot.SendMessage(UserId, "Питання 5", replyMarkup: GetKeyBoard("MorningQuiz", 5));
                    await vibeBuffer.SetQuizMessagesForUserToDelete(UserId, mes.Id);
                    break;
                case "Question5":
                    await bot.SendMessage(UserId, $"Ранкове Опитування {DateTime.Now.ToString()} Закінчено");
                    await FinishQuiz(UserId);
                    break;
            }
        }

        public async Task MidDayQuiz(string PrevQuestinTag, long UserId)
        {
            string split = PrevQuestinTag.Split("_")[1];
            Message mes;
            switch (split)
            {
                case "StartQuiz":
                    mes = await bot.SendMessage(UserId, "Починаємо Обіднє Опитування \n Питання 1", replyMarkup: GetKeyBoard("MorningQuiz", 1));
                    await vibeBuffer.SetQuizMessagesForUserToDelete(UserId, mes.Id);
                    break;
                case "Question1":
                    mes = await bot.SendMessage(UserId, "Питання 2", replyMarkup: GetKeyBoard("MorningQuiz", 2));
                    await vibeBuffer.SetQuizMessagesForUserToDelete(UserId, mes.Id);
                    break;
                case "Question2":
                    mes = await bot.SendMessage(UserId, "Питання 3", replyMarkup: GetKeyBoard("MorningQuiz", 3));
                    await vibeBuffer.SetQuizMessagesForUserToDelete(UserId, mes.Id);
                    break;
                case "Question3":
                    mes = await bot.SendMessage(UserId, "Питання 4", replyMarkup: GetKeyBoard("MorningQuiz", 4));
                    await vibeBuffer.SetQuizMessagesForUserToDelete(UserId, mes.Id);
                    break;
                case "Question4":
                    mes = await bot.SendMessage(UserId, "Питання 5", replyMarkup: GetKeyBoard("MorningQuiz", 5));
                    await vibeBuffer.SetQuizMessagesForUserToDelete(UserId, mes.Id);
                    break;
                case "Question5":
                    await bot.SendMessage(UserId, $"Обіднє Опитування {DateTime.Now.ToString()} Закінчено");
                    await FinishQuiz(UserId);
                    break;
            }
        }

        public async Task EvningQuiz(string PrevQuestinTag, long UserId)
        {
            string split = PrevQuestinTag.Split("_")[1];
            Message mes;
            switch (split)
            {
                case "StartQuiz":
                    mes = await bot.SendMessage(UserId, "Починаємо Вечірне Опитування \n Питання 1", replyMarkup: GetKeyBoard("MorningQuiz", 1));
                    await vibeBuffer.SetQuizMessagesForUserToDelete(UserId, mes.Id);

                    break;
                case "Question1":
                    mes = await bot.SendMessage(UserId, "Питання 2", replyMarkup: GetKeyBoard("MorningQuiz", 2));
                    await vibeBuffer.SetQuizMessagesForUserToDelete(UserId, mes.Id);
                    break;
                case "Question2":
                    mes = await bot.SendMessage(UserId, "Питання 3", replyMarkup: GetKeyBoard("MorningQuiz", 3));
                    await vibeBuffer.SetQuizMessagesForUserToDelete(UserId, mes.Id);
                    break;
                case "Question3":
                    mes = await bot.SendMessage(UserId, "Питання 4", replyMarkup: GetKeyBoard("MorningQuiz", 4));
                    await vibeBuffer.SetQuizMessagesForUserToDelete(UserId, mes.Id);
                    break;
                case "Question4":
                    mes = await bot.SendMessage(UserId, "Питання 5", replyMarkup: GetKeyBoard("MorningQuiz", 5));
                    await vibeBuffer.SetQuizMessagesForUserToDelete(UserId, mes.Id);
                    break;
                case "Question5":
                    await bot.SendMessage(UserId, "Вечірнє Опитування Закінчено");
                    await FinishQuiz(UserId);
                    break;
            }
        }

        public async Task GetAndRemoveQuizMessageForUser(long UserID, Func<int, long, Task> deleteTasks)
        {
            List<int> Messages;
            List<Task> taskList = new List<Task>();

            if (vibeBuffer.RemoveQuizMessageForUser(UserID, out Messages))
            {
                await Task.WhenAll( Messages.Select(id => deleteTasks(id, UserID)));

            }
        }

        public async Task GetAndSendVibeModelForUser(long UserID, Func<VibeCount,long, Task> SendTask)
        {
            VibeCount Vibe;
            List<Task> taskList = new List<Task>();

            if (vibeBuffer.GetAndRemoveVibeForUser(UserID, out Vibe))
            {
                await SendTask(Vibe, UserID);
            }
        }

        public async Task SendVibeToApi(VibeCount vibe, long UserID) 
        {
            Console.WriteLine("SendVibeToBD");
            JsonContent Vibe = JsonContent.Create(new {  vibeLevel = (float)(vibe.VibeLevel/vibe.Quizes), userID = UserID}); // get Question     
            for(int attempt = 0; attempt < 3; attempt++) 
            {
                var responce = await _httpClient.PostAsync("http://vibeservice/serviceapi/api/ApiBot/RetriveVibeDataFromUser", Vibe);
                if (responce.IsSuccessStatusCode)
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Can`t Send Vibe");
                    Console.WriteLine(responce.StatusCode.ToString());

                }
            }

        }

        public async Task DeleteMessageInBot(int messageId, long UserID)
        {
            await bot.DeleteMessage(UserID, messageId);
        }

    }
}
