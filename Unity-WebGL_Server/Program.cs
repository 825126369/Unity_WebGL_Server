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

            // 自定义 Gzip 静态文件中间件（专为 Unity WebGL 设计）
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

                // 安全路径处理
                var fullPath = Path.GetFullPath(Path.Combine(appDirPath, requestPath.TrimStart('/')));
                if (!fullPath.StartsWith(appDirPath, StringComparison.OrdinalIgnoreCase))
                {
                    context.Response.StatusCode = 403;
                    return;
                }
                Console.WriteLine("requestPath: 222 " + fullPath);

                // 情况 1：请求的是 .js 或 .wasm（团结引擎通常这样请求）
                bool isCompressible = requestPath.EndsWith(".js") || requestPath.EndsWith(".wasm") || requestPath.EndsWith(".data");
                string gzipPath = fullPath + ".gz";

                // 情况 2：请求的已经是 .js.gz（某些 loader 行为）
                if (requestPath.EndsWith(".js.gz") || requestPath.EndsWith(".wasm.gz") || requestPath.EndsWith(".data.gz"))
                {
                    // 去掉 .gz 获取原始路径，用于设置 Content-Type
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
                // 情况 1：请求 .js，但存在 .js.gz → 返回 .gz 内容
                else if (isCompressible && File.Exists(gzipPath))
                {
                    var contentType = GetContentType(requestPath);
                    context.Response.ContentType = contentType;
                    context.Response.Headers[HeaderNames.ContentEncoding] = "gzip";
                    context.Response.Headers[HeaderNames.Vary] = "Accept-Encoding";
                    await context.Response.SendFileAsync(gzipPath);
                    Console.WriteLine("BBB");
                }
                // 否则返回原始文件（如 index.html, .data 无 .gz 等）
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
