using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;

namespace Positron.Server
{
    class RequestFrame<TContext> : RequestFrame
    {
        // ReSharper disable once StaticMemberInGenericType
        private static readonly EventId ProcessRequestError = new EventId(1000, "Positron.Server.ProcessRequestError");

        private readonly IHttpApplication<TContext> _application;
        private readonly ILogger<RequestFrame<TContext>> _logger;

        public RequestFrame(IHttpApplication<TContext> application, ILogger<RequestFrame<TContext>> logger,
            IHttpRequestFeature requestContext)
            : base(requestContext)
        {
            if (application == null)
            {
                throw new ArgumentNullException(nameof(application));
            }
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            _application = application;
            _logger = logger;
        }

        public override async Task ProcessRequestAsync()
        {
            try
            {
                InitializeHeaders();
                InitializeStreams();

                var context = _application.CreateContext(this);

                await _application.ProcessRequestAsync(context);

                Response.Body.Seek(0, SeekOrigin.Begin);
            }
            catch (Exception e)
            {
                _logger.LogError(ProcessRequestError, e, "Error Processing Positron Request");

                throw;
            }
        }
    }
}
