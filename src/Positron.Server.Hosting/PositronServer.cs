using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;

namespace Positron.Server.Hosting
{
    internal class PositronServer : IServer, IInternalHttpRequestFeature
    {
        private readonly ILoggerFactory _loggerFactory;
        private PositronServerContext _context;

        public IFeatureCollection Features { get; }

        public PositronServer(ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            _loggerFactory = loggerFactory;

            Features = new FeatureCollection();
            Features.Set<IInternalHttpRequestFeature>(this);
        }

        public void Start<TContext>(IHttpApplication<TContext> application)
        {
            var logger = _loggerFactory.CreateLogger<RequestFrame<TContext>>();

            _context = new PositronServerContext()
            {
                FrameFactory = requestContext => new RequestFrame<TContext>(application, logger, requestContext)
            };
        }

        public async Task<IHttpResponseFeature> ProcessRequestAsync(IHttpRequestFeature request)
        {
            if (_context == null)
            {
                throw new InvalidOperationException("Server Not Started");
            }

            var frame = _context.FrameFactory(request);
            await frame.ProcessRequestAsync();

            return frame.Get<IHttpResponseFeature>();
        }

        public void Dispose()
        {
        }
    }
}
