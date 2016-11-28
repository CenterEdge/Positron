
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Positron.Server.Hosting;

namespace Positron.Application
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddPositronServer()
                // ReSharper disable once PossibleNullReferenceException
                .AddApplicationParts(p => Path.GetFileName(p).StartsWith("Positron"));
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStatusCodePages();
            app.UsePositron();
        }
    }
}
