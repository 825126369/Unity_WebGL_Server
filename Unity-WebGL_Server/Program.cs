using Microsoft.Net.Http.Headers;
using System.Text.Json.Serialization;

namespace Unity_WebGL_Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateSlimBuilder(args);
            var app = builder.Build();
            
            string webRoot = Path.GetFullPath("wwwroot");
            if (!Directory.Exists(webRoot))
            {
                throw new DirectoryNotFoundException($"Web root not found: {webRoot}");
            }

            // �Զ��� Gzip ��̬�ļ��м����רΪ Unity WebGL ��ƣ�
            app.Use(async (context, next) =>
            {
                var requestPath = context.Request.Path.Value;
                Console.WriteLine("requestPath: " + requestPath);
                if (string.IsNullOrEmpty(requestPath) || requestPath == "/")
                {
                    requestPath = "APP/index.html";
                }

                // ��ȫ·������
                var fullPath = Path.GetFullPath(Path.Combine(webRoot, requestPath.TrimStart('/')));
                if (!fullPath.StartsWith(webRoot, StringComparison.OrdinalIgnoreCase))
                {
                    context.Response.StatusCode = 403;
                    return;
                }

                // ��� 1��������� .js �� .wasm���Ž�����ͨ����������
                bool isCompressible = requestPath.EndsWith(".js") || requestPath.EndsWith(".wasm") || requestPath.EndsWith(".data");
                string gzipPath = fullPath + ".gz";

                // ��� 2��������Ѿ��� .js.gz��ĳЩ loader ��Ϊ��
                if (requestPath.EndsWith(".js.gz") || requestPath.EndsWith(".wasm.gz") || requestPath.EndsWith(".data.gz"))
                {
                    // ȥ�� .gz ��ȡԭʼ·������������ Content-Type
                    var originalPath = requestPath.Substring(0, requestPath.Length - 3);
                    var originalFullPath = Path.GetFullPath(Path.Combine(webRoot, originalPath.TrimStart('/')));
                    if (!originalFullPath.StartsWith(webRoot, StringComparison.OrdinalIgnoreCase))
                    {
                        context.Response.StatusCode = 403;
                        return;
                    }

                    if (!File.Exists(fullPath))
                    {
                        context.Response.StatusCode = 404;
                        return;
                    }

                    var contentType = GetContentType(originalPath);
                    context.Response.ContentType = contentType;
                    context.Response.Headers[HeaderNames.ContentEncoding] = "gzip";
                    context.Response.Headers[HeaderNames.Vary] = "Accept-Encoding";
                    await context.Response.SendFileAsync(fullPath);
                    return;
                }

                // ��� 1������ .js�������� .js.gz �� ���� .gz ����
                if (isCompressible && File.Exists(gzipPath))
                {
                    var contentType = GetContentType(requestPath);
                    context.Response.ContentType = contentType;
                    context.Response.Headers[HeaderNames.ContentEncoding] = "gzip";
                    context.Response.Headers[HeaderNames.Vary] = "Accept-Encoding";
                    await context.Response.SendFileAsync(gzipPath);
                    return;
                }

                // ���򷵻�ԭʼ�ļ����� index.html, .data �� .gz �ȣ�
                if (File.Exists(fullPath))
                {
                    context.Response.ContentType = GetContentType(requestPath);
                    await context.Response.SendFileAsync(fullPath);
                    return;
                }

                context.Response.StatusCode = 404;
                await next();
            });

            app.Run();
        }

        static string GetContentType(string path)
        {
            return Path.GetExtension(path).ToLowerInvariant() switch
            {
                ".html" => "text/html",
                ".js" => "application/javascript",
                ".wasm" => "application/wasm",
                ".data" or ".bundle" => "application/octet-stream",
                ".css" => "text/css",
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".json" => "application/json",
                _ => "application/octet-stream"
            };
        }
    }

    public record Todo(int Id, string? Title, DateOnly? DueBy = null, bool IsComplete = false);

    [JsonSerializable(typeof(Todo[]))]
    internal partial class AppJsonSerializerContext : JsonSerializerContext
    {

    }
}
