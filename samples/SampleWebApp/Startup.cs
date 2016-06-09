using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Reflection;

namespace SampleWebApp
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();
            services.AddMvc();
            services.AddVersionedAssets();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory, IHostingEnvironment env)
        {
            loggerFactory.AddConsole(LogLevel.Debug);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // respond to version asset requests (defaults to /static/[hash]) 
            app.UseVersionedAssets()
                //.WithUrlPrefix("//mycdn.azureedge.net")
                .WithAssemblyHashGlobalVersion(typeof(Startup).GetTypeInfo().Assembly);


            app.UseMvcWithDefaultRoute();
        }
    }
}
