using System;
using Microsoft.AspNetCore.Http.Features;

namespace Positron.Server
{
    class PositronServerContext
    {
        public Func<IHttpRequestFeature, RequestFrame> FrameFactory { get; set; }
    }
}
