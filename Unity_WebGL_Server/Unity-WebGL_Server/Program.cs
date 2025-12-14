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

            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".unityweb"] = "application/octet-stream";
            provider.Mappings[".data"] = "application/octet-stream";
            provider.Mappings[".wasm"] = "application/wasm";
            provider.Mappings[".symbols.json"] = "application/json";

            app.UseStaticFiles(new StaticFileOptions
            {
                ContentTypeProvider = provider,
                OnPrepareResponse = ctx =>
                {
                    var path = ctx.File.Name;
                    if (path.EndsWith(".br"))
                    {
                        ctx.Context.Response.Headers[HeaderNames.ContentEncoding] = "br";
                    }
                    else if (path.EndsWith(".gz"))
                    {
                        ctx.Context.Response.Headers[HeaderNames.ContentEncoding] = "gzip";
                    }
                }
            });

            app.Run();
        }
    }
}
