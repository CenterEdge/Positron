using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Positron.Server.Hosting.FileProvider
{
    /// <summary>
    /// Middleware that returns assembly resources as static file content.
    /// </summary>
    public class ResourceFileMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ResourceFileProvider _fileProvider;

        /// <summary>
        /// Creates a new <see cref="ResourceFileMiddleware"/>.
        /// </summary>
        /// <param name="next">Next delegate to call if this middleware doesn't handle the request.</param>
        /// <param name="fileProvider"><see cref="ResourceFileProvider"/> used to provide file resources.</param>
        public ResourceFileMiddleware(RequestDelegate next, ResourceFileProvider fileProvider)
        {
            _next = next;
            _fileProvider = fileProvider;
        }

        /// <summary>
        /// Handle a request
        /// </summary>
        /// <param name="context"><see cref="HttpContext"/> for  the request</param>
        /// <returns><see cref="Task"/> which will be complete when the request is handled.</returns>
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
