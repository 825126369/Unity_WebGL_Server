using Microsoft.Net.Http.Headers;
using System.Reflection.Emit;

namespace Unity_WebGL_Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateSlimBuilder(args);
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

            // �Զ��� Gzip ��̬�ļ��м����רΪ Unity WebGL ��ƣ�
            app.Use(async (context, next) =>
            {
                string requestPath = context.Request.Path.Value;
                string requestRootDir = wwwroot;
                
                if (string.IsNullOrEmpty(requestPath) || requestPath == "/")
                {
                    requestPath = "/index.html";
                    requestRootDir = appDirPath;
                }

                // ��ȫ·������
                string fullPath = string.Empty;
                fullPath = Path.GetFullPath(Path.Combine(requestRootDir, requestPath.TrimStart('/')));
                if (fullPath.StartsWith(appDirPath))
                {

                }
                else if(fullPath.StartsWith(HotUpdateResDirPath))
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
                    Console.WriteLine($"requestPath:  {fullPath} ������");
                }

                // ��� 1��������� .js �� .wasm���Ž�����ͨ����������
                bool isCompressible = requestPath.EndsWith(".js") || requestPath.EndsWith(".wasm") || requestPath.EndsWith(".data");
                string gzipPath = fullPath + ".gz";

                // ��� 2��������Ѿ��� .js.gz��ĳЩ loader ��Ϊ��
                if (requestPath.EndsWith(".js.gz") || requestPath.EndsWith(".wasm.gz") || requestPath.EndsWith(".data.gz"))
                {
                    // ȥ�� .gz ��ȡԭʼ·������������ Content-Type
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
                // ��� 1������ .js�������� .js.gz �� ���� .gz ����
                else if (isCompressible && File.Exists(gzipPath))
                {
                    var contentType = GetContentType(requestPath);
                    context.Response.ContentType = contentType;
                    context.Response.Headers[HeaderNames.ContentEncoding] = "gzip";
                    context.Response.Headers[HeaderNames.Vary] = "Accept-Encoding";
                    await context.Response.SendFileAsync(gzipPath);
                }
                // ���򷵻�ԭʼ�ļ����� index.html, .data �� .gz �ȣ�
                else if (File.Exists(fullPath))
                {
                    context.Response.ContentType = GetContentType(requestPath);
                    await context.Response.SendFileAsync(fullPath);
                }
            Next:
                Console.WriteLine("��һ������");
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
