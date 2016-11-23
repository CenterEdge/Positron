using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Positron.Server.Hosting.FileProvider
{
    public class ResourceFileMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ResourceFileProvider _fileProvider;

        public ResourceFileMiddleware(RequestDelegate next, ResourceFileProvider fileProvider)
        {
            _next = next;
            _fileProvider = fileProvider;
        }

        public async Task Invoke(HttpContext context)
        {
            var path = context.Request.Path.Value;

            if (path.EndsWith(".cshtml", StringComparison.OrdinalIgnoreCase) ||
                path.EndsWith(".vbhtml", StringComparison.OrdinalIgnoreCase))
            {
                // Don't allow access views for security

                await _next(context);
                return;
            }

            var fileInfo = _fileProvider.GetFileInfo(path);
            if (fileInfo.Exists && !fileInfo.IsDirectory)
            {
                var resourceFileInfo = fileInfo as ResourceFileInfo;
                context.Response.ContentType = resourceFileInfo?.MimeType ?? "application/octet-stream";

                context.Response.ContentLength = fileInfo.Length;

                using (var stream = fileInfo.CreateReadStream())
                {
                    await stream.CopyToAsync(context.Response.Body);
                }
            }
            else
            {
                await _next(context);
            }
        }
    }
}
