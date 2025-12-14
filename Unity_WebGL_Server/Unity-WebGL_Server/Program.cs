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

            string defaultRequestDir = "APP/";
            string appDirPath = Path.GetFullPath("wwwroot/APP/");
            string HotUpdateResDirPath = Path.GetFullPath("wwwroot/HotUpdateRes/");
            string wwwroot = Path.GetFullPath("wwwroot/");

            if (!Directory.Exists(appDirPath))
            {
                throw new DirectoryNotFoundException($"requestRootDir not found: {appDirPath}");
            }

            if (!Directory.Exists(HotUpdateResDirPath))
            {
                throw new DirectoryNotFoundException($"HotUpdateResPath found: {HotUpdateResDirPath}");
            }

            // 1. 让框架知道 .br / .gz 是什么 MIME
            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".br"] = "application/octet-stream"; // 先给个体面身份
            provider.Mappings[".gz"] = "application/octet-stream";

            // 2.注册静态文件中间件，并给.br 加响应头
            //app.UseStaticFiles(new StaticFileOptions
            //{
            //    ContentTypeProvider = provider,
            //    OnPrepareResponse = ctx =>
            //    {
            //        var req = ctx.Context.Request;
            //        var resp = ctx.Context.Response;

            //        // 只处理 .br 文件
            //        if (ctx.File.Name.EndsWith(".br"))
            //        {
            //            resp.Headers[HeaderNames.ContentEncoding] = "br";

            //            // 去掉 Content-Type 里的 charset，避免某些浏览器误判
            //            if (resp.ContentType is not null)
            //            {
            //                resp.ContentType = resp.ContentType.Split(';')[0];
            //            }
            //        }
            //        // 如果还有 .gz 文件，也顺手处理
            //        else if (ctx.File.Name.EndsWith(".gz"))
            //        {
            //            resp.Headers[HeaderNames.ContentEncoding] = "gzip";
            //        }
            //    }
            //});

            //自定义 Gzip 静态文件中间件（专为 Unity WebGL 设计）
            app.Use(async (context, next) =>
            {
                string requestPath = context.Request.Path.Value;
                string requestRootDir = wwwroot;

                if (string.IsNullOrEmpty(requestPath) || requestPath == "/")
                {
                    requestPath = "/index.html";
                    requestRootDir = appDirPath;
                }

                // 安全路径处理
                string fullPath = string.Empty;
                fullPath = Path.GetFullPath(Path.Combine(requestRootDir, requestPath.TrimStart('/')));
                if (fullPath.StartsWith(appDirPath))
                {

                }
                else if (fullPath.StartsWith(HotUpdateResDirPath))
                {

                }
                else
                {
                    requestRootDir = appDirPath;
                    fullPath = Path.GetFullPath(Path.Combine(requestRootDir, requestPath.TrimStart('/')));
                }

                if (File.Exists(fullPath))
                {
                    Console.WriteLine($"requestPath:  {fullPath}");
                }
                else
                {
                    Console.WriteLine($"requestPath:  {fullPath} 不存在");
                }

                // 情况 1：请求的是 .js 或 .wasm（团结引擎通常这样请求）
                bool isCompressible = requestPath.EndsWith(".js") || requestPath.EndsWith(".wasm") || requestPath.EndsWith(".data");
                string gzipPath = fullPath + ".gz";

                // 情况 2：请求的已经是 .js.gz（某些 loader 行为）
                if (requestPath.EndsWith(".js.gz") || requestPath.EndsWith(".wasm.gz") || requestPath.EndsWith(".data.gz"))
                {
                    // 去掉 .gz 获取原始路径，用于设置 Content-Type
                    var originalPath = requestPath.Substring(0, requestPath.Length - 3);
                    var originalFullPath = Path.GetFullPath(Path.Combine(requestRootDir, originalPath.TrimStart('/')));
                    if (!originalFullPath.StartsWith(requestRootDir, StringComparison.OrdinalIgnoreCase))
                    {
                        context.Response.StatusCode = 403;
                        goto Next;
                    }

                    if (!File.Exists(fullPath))
                    {
                        context.Response.StatusCode = 404;
                        goto Next;
                    }

                    var contentType = GetContentType(originalPath);
                    context.Response.ContentType = contentType;
                    context.Response.Headers[HeaderNames.ContentEncoding] = "gzip";
                    context.Response.Headers[HeaderNames.Vary] = "Accept-Encoding";
                    await context.Response.SendFileAsync(fullPath);
                }
                // 情况 1：请求 .js，但存在 .js.gz → 返回 .gz 内容
                else if (isCompressible && File.Exists(gzipPath))
                {
                    var contentType = GetContentType(requestPath);
                    context.Response.ContentType = contentType;
                    context.Response.Headers[HeaderNames.ContentEncoding] = "gzip";
                    context.Response.Headers[HeaderNames.Vary] = "Accept-Encoding";
                    await context.Response.SendFileAsync(gzipPath);
                }
                // 否则返回原始文件（如 index.html, .data 无 .gz 等）
                else if (File.Exists(fullPath))
                {
                    context.Response.ContentType = GetContentType(requestPath);
                    await context.Response.SendFileAsync(fullPath);
                }
            Next:
                Console.WriteLine("下一个请求");
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
}
