using System;
using Microsoft.AspNetCore.Http.Features;

namespace Positron.Server.Hosting.Internal
{
    internal class PositronServerContext
    {
        public Func<IHttpRequestFeature, RequestFrame> FrameFactory { get; set; }
    }
}
