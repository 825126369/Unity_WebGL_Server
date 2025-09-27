using Microsoft.Net.Http.Headers;

namespace Unity_WebGL_Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateSlimBuilder(args);
            var app = builder.Build();
            
            string appDirPath = Path.GetFullPath("wwwroot/APP/");
            if (!Directory.Exists(appDirPath))
            {
                throw new DirectoryNotFoundException($"Web root not found: {appDirPath}");
            }

            // �Զ��� Gzip ��̬�ļ��м����רΪ Unity WebGL ��ƣ�
            app.Use(async (context, next) =>
            {
                var requestPath = context.Request.Path.Value;
                Console.WriteLine("requestPath: 000 " + requestPath);
                if (string.IsNullOrEmpty(requestPath) || requestPath == "/")
                {
                    requestPath = "/index.html";
                    context.Request.Path = requestPath;
                }
                Console.WriteLine("requestPath: 111 " + requestPath);

                // ��ȫ·������
                var fullPath = Path.GetFullPath(Path.Combine(appDirPath, requestPath.TrimStart('/')));
                if (!fullPath.StartsWith(appDirPath, StringComparison.OrdinalIgnoreCase))
                {
                    context.Response.StatusCode = 403;
                    return;
                }
                Console.WriteLine("requestPath: 222 " + fullPath);

                // ��� 1��������� .js �� .wasm���Ž�����ͨ����������
                bool isCompressible = requestPath.EndsWith(".js") || requestPath.EndsWith(".wasm") || requestPath.EndsWith(".data");
                string gzipPath = fullPath + ".gz";

                // ��� 2��������Ѿ��� .js.gz��ĳЩ loader ��Ϊ��
                if (requestPath.EndsWith(".js.gz") || requestPath.EndsWith(".wasm.gz") || requestPath.EndsWith(".data.gz"))
                {
                    // ȥ�� .gz ��ȡԭʼ·������������ Content-Type
                    var originalPath = requestPath.Substring(0, requestPath.Length - 3);
                    var originalFullPath = Path.GetFullPath(Path.Combine(appDirPath, originalPath.TrimStart('/')));
                    if (!originalFullPath.StartsWith(appDirPath, StringComparison.OrdinalIgnoreCase))
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

                    Console.WriteLine("AAA");
                }
                // ��� 1������ .js�������� .js.gz �� ���� .gz ����
                else if (isCompressible && File.Exists(gzipPath))
                {
                    var contentType = GetContentType(requestPath);
                    context.Response.ContentType = contentType;
                    context.Response.Headers[HeaderNames.ContentEncoding] = "gzip";
                    context.Response.Headers[HeaderNames.Vary] = "Accept-Encoding";
                    await context.Response.SendFileAsync(gzipPath);
                    Console.WriteLine("BBB");
                }
                // ���򷵻�ԭʼ�ļ����� index.html, .data �� .gz �ȣ�
                else if (File.Exists(fullPath))
                {
                    context.Response.ContentType = GetContentType(requestPath);
                    await context.Response.SendFileAsync(fullPath);
                    Console.WriteLine("CCC");
                }
                else
                {
                    context.Response.StatusCode = 404;
                    await next();
                }
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
}
