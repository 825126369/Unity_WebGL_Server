using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Net.Http.Headers;

namespace Unity_WebGL_Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateSlimBuilder(args);
            builder.WebHost.UseUrls("http://localhost:5059", "http://localhost:5000");
            var app = builder.Build();
            app.UseStaticFiles();
            app.Run();
        }
    }
}
