using System.Collections.Concurrent;
using Telegram.Bot.Types;
using TelegramBot.Model;

namespace TelegramBot.Services
{
    public class DictonaryBuffer
    {
        private readonly ConcurrentDictionary<long, VibeCount> VibeBuffer = new ConcurrentDictionary<long, VibeCount>();
        private readonly ConcurrentDictionary<long, List<int>> VibeBufferMessages = new ConcurrentDictionary<long, List<int>>();

        public ConcurrentDictionary<long, VibeCount> getDictonary => VibeBuffer;
        public ConcurrentDictionary<long, List<int>> getMessageDictonary => VibeBufferMessages;


        public async Task SetVibeForUser(long UserID, int quizNum, int Mark)
        {
            VibeCount vibe;
            if (VibeBuffer.TryGetValue(UserID, out vibe))
            {
                vibe.Quizes = quizNum;
                vibe.VibeLevel = vibe.VibeLevel + Mark;

            }
            else
            {
                VibeBuffer[UserID] = new VibeCount
                {
                    Quizes = quizNum,
                    VibeLevel = Mark
                };

            }
        }

        public async Task SetQuizMessagesForUserToDelete(long UserID, int MessageId)
        {
            List<int> Messages;
            if (VibeBufferMessages.TryGetValue(UserID, out Messages))
            {
                Messages.Add(MessageId);
            }
            else
            {
                VibeBufferMessages[UserID] = new List<int>();
                VibeBufferMessages[UserID].Add(MessageId);
            }
        }

        public bool RemoveQuizMessageForUser(long UserId, out List<int> Messages)
        {
            return VibeBufferMessages.TryRemove(UserId, out Messages);
        }

        

        public bool GetAndRemoveVibeForUser(long UserID, out VibeCount vibe)
        {
            return VibeBuffer.TryRemove(UserID, out vibe);

        }

        




    }
}
