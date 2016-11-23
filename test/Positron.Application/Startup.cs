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
                .AddControllersFromEntryPoint();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UsePositron();
        }
    }
}
