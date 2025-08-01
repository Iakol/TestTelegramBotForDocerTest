using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace Dashboard
{
    public static class ResoursePathHelper
    {
        private static IHttpContextAccessor _httpContextAccessor;


        public static void Configure(IHttpContextAccessor contextAccessor) 
        {
            _httpContextAccessor = contextAccessor;
        }

        public static string GetHost()
        {
            return _httpContextAccessor.HttpContext.Request.Host.ToString();
        }



        public static string GetRightPath(string resoursePath) {

            Console.WriteLine(GetHost() + _httpContextAccessor.HttpContext.Request.Path.ToString());
            if (GetHost().Contains("dashboard"))
            {
                string path = resoursePath.Trim('~');
                return $"/dashboard/{path}";
            }
            else
            {//dashboard
                string path = resoursePath.Trim('~');
                Console.WriteLine($"http://{GetHost()}/{path}");
                return $"http://{GetHost()}/{path}"; 
            }
        }
    }
}
