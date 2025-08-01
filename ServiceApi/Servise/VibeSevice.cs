namespace ServiceApi.Servise
{
    public class VibeSevice(HttpClient _httpClient)
    {
        public async Task SendVibeRequest(int dayPart) 
        {
            try
            {
                using HttpResponseMessage responce = await _httpClient.PostAsync($"http://vibeservice/telegram/api/ApiComunication/RetriveCommand", JsonContent.Create(dayPart));
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
