namespace ServiceApi.Servise
{
    public class VibeSevice(HttpClient _httpClient)
    {
        public async Task SendVibeRequest(int dayPart) 
        {
            using HttpResponseMessage responce = await _httpClient.PostAsync($"http://TelegramBot:80/api/ApiComunication/RetriveCommand", JsonContent.Create(dayPart));
        }
    }
}
