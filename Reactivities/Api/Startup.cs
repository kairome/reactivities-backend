using System.Threading.Tasks;
using Api.Extensions;
using Api.Middleware;
using AspNetCore.ServiceRegistration.Dynamic;
using Infrastructure.SignalR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Api
{
    public class Startup
    {
        private readonly IConfiguration _config;
        
        public Startup(IConfiguration config)
        {
            _config = config;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationServices(_config);

            services.AddSignalR().AddJsonProtocol(options =>
            {
                options.PayloadSerializerOptions.PropertyNamingPolicy = null;
            });
            services.AddServicesWithAttributeOfType<ScopedServiceAttribute>();

            services.AddSingleton<IMongoClient, MongoClient>((s) => new MongoClient(_config.GetConnectionString("MongoUri")));
            services.AddSwaggerGen();
            services.AddStartupTask<DbMigrator>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseRouting();

            app.UseDefaultFiles();
            app.UseStaticFiles();
            
            app.UseCors("CorsPolicy");
            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Reactivities API"); });

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ChatHub>("/api/signalr/chat");
                endpoints.MapHub<NotificationHub>("/api/signalr/notifications");
                endpoints.MapFallbackToController("Index", "Fallback");
            });
        }
    }
}