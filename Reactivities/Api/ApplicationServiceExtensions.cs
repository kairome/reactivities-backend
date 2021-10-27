using System.Text;
using System.Threading.Tasks;
using Application.Activities;
using Application.Auth;
using FluentValidation.AspNetCore;
using Infrastructure.Security;
using Infrastructure.Storage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Api.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {

            services
                .AddControllers(opts =>
                {
                    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                    opts.Filters.Add(new AuthorizeFilter(policy));
                })
                .AddJsonOptions(opt => { opt.JsonSerializerOptions.PropertyNamingPolicy = null; })
                .AddFluentValidation(config =>
                {
                    config.RegisterValidatorsFromAssemblyContaining<WriteActivitiesService>();
                });

            services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy", policy =>
                {
                    policy
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
                        .WithOrigins("http://localhost:3000");
                });
            });

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWTSecretKey"]));
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = key,
                        ValidateIssuer = false,
                        ValidateAudience = false,
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var authCookie = context.Request.Cookies[AuthConfigs.AuthCookieName];
                            if (!string.IsNullOrEmpty(authCookie))
                            {
                                context.Token = authCookie;
                            }

                            return Task.CompletedTask;
                        }
                    };
                })
                ;

            services.AddAuthorization(opt =>
            {
                opt.AddPolicy("IsActivityAuthor", policy =>
                {
                    policy.Requirements.Add(new ActivityAuthorRequirement());
                });
            });
            
            services.AddTransient<IAuthorizationHandler, ActivityAuthorRequirementHandler>();

            services.Configure<CloudinarySettings>(config.GetSection("Cloudinary"));
            
            return services;
        }
    }
}