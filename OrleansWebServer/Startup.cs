
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrleansWebServer.Backend.Extensions;
using OrleansWebServer.Grains;
using OrleansWebServer.Grains.Models;

namespace OrleansWebServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen();
            services.AddMvc();
            
            // Adds request processing for every type of your requests
            // All you need - is to define your grain type, request type and response type 
            services.AddOrleansWebServerForRequest<IntegralX2Request, IntegralX2Response, IX2IntegrationGrain>(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) 
                app.UseDeveloperExceptionPage();

            app.UseRouting();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Orleans WebServer API V1"));

            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
