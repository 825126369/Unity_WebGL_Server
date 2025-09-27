using Microsoft.Net.Http.Headers;
using System.Net;
using System.Text.Json.Serialization;

namespace Unity_WebGL_Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateSlimBuilder(args);

            builder.Services.ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
            });

            var app = builder.Build();

            var sampleTodos = new Todo[] {
                new(1, "Walk the dog"),
                new(2, "Do the dishes", DateOnly.FromDateTime(DateTime.Now)),
                new(3, "Do the laundry", DateOnly.FromDateTime(DateTime.Now.AddDays(1))),
                new(4, "Clean the bathroom"),
                new(5, "Clean the car", DateOnly.FromDateTime(DateTime.Now.AddDays(2)))
            };

            var todosApi = app.MapGroup("/todos");
            //todosApi.MapGet("/", () => sampleTodos);
            todosApi.MapGet("/{id}", (int id) =>
                sampleTodos.FirstOrDefault(a => a.Id == id) is { } todo
                    ? Results.Ok(todo)
                    : Results.NotFound());


            string webRoot = Path.GetFullPath("wwwroot");
            if (!Directory.Exists(webRoot))
            {
                throw new DirectoryNotFoundException($"Web root not found: {webRoot}");
            }
            app.UseStaticFiles();

            // �Զ��� Gzip ��̬�ļ��м��
            app.Use(async (context, next) =>
            {
                var path = context.Request.Path.Value;
                if (string.IsNullOrEmpty(path) || path == "/")
                    path = "/index.html";

                var filePath = Path.Combine(webRoot, path.TrimStart('/'));

                // ��ȫ����ֹ·������
                if (!Path.GetFullPath(filePath).StartsWith(webRoot))
                {
                    context.Response.StatusCode = 403;
                    return;
                }

                // ��� .gz �ļ��Ƿ���� �� �ͻ���֧�� gzip
                bool clientAcceptsGzip = context.Request.Headers[HeaderNames.AcceptEncoding].ToString().Contains("gzip");
                string gzipPath = filePath + ".gz";

                if (clientAcceptsGzip && File.Exists(gzipPath))
                {
                    // ����ԭʼ�ļ���չ������ Content-Type
                    string contentType = Path.GetExtension(filePath) switch
                    {
                        ".js" => "application/javascript",
                        ".wasm" => "application/wasm",
                        ".data" or ".bundle" => "application/octet-stream",
                        ".html" => "text/html",
                        ".css" => "text/css",
                        _ => "application/octet-stream"
                    };

                    context.Response.ContentType = contentType;
                    context.Response.Headers[HeaderNames.ContentEncoding] = "gzip";
                    context.Response.Headers[HeaderNames.Vary] = "Accept-Encoding";

                    await context.Response.SendFileAsync(gzipPath);
                    return;
                }

                // ���򽻸���̬�ļ��м��
                await next();
            });

            app.Run();
        }
    }

    public record Todo(int Id, string? Title, DateOnly? DueBy = null, bool IsComplete = false);

    [JsonSerializable(typeof(Todo[]))]
    internal partial class AppJsonSerializerContext : JsonSerializerContext
    {

    }
}
