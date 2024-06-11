using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using PetShop.Application.Interfaces;
using PetShop.Application.Interfaces.Repositories;
using PetShop.Application.Wrappers;
using PetShop.Domain.Entities;
using PetShop.Domain.Settings;
using PetShop.Infrastructure.Data.Repositories;
using PetShop.Infrastructure.Identity.Contexts;
using PetShop.Infrastructure.Identity.Seeds;
using PetShop.Infrastructure.Identity.Services;
using PetShop.Infrastructure.Shared.Services;
using System.Text;

namespace PetShop.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Connect database
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseNpgsql(connectionString);
        });

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddIdentity<User, Role>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
        services.AddScoped<ApplicationDbContextInitializer>();

        // Configurations
        services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
        services.Configure<JWTSettings>(configuration.GetSection("JWTSettings"));

        // Register JWTSettings as a singleton
        services.AddSingleton(sp => sp.GetRequiredService<IOptions<JWTSettings>>().Value);
        services.AddAuthentication()
                .AddBearerToken(IdentityConstants.BearerScheme).AddJwtBearer(o =>
                {
                    o.RequireHttpsMetadata = false;
                    o.SaveToken = false;
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,
                        ValidIssuer = configuration["JWTSettings:Issuer"],
                        ValidAudience = configuration["JWTSettings:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWTSettings:Key"]))
                    };
                    o.Events = new JwtBearerEvents()
                    {
                        OnAuthenticationFailed = c =>
                        {
                            c.NoResult();
                            c.Response.StatusCode = 500;
                            c.Response.ContentType = "text/plain";
                            return c.Response.WriteAsync(c.Exception.ToString());
                        },
                        OnChallenge = context =>
                        {
                            context.HandleResponse();
                            context.Response.StatusCode = 401;
                            context.Response.ContentType = "application/json";
                            var result = JsonConvert.SerializeObject(new Response<string>("You are not Authorized"));
                            return context.Response.WriteAsync(result);
                        },
                        OnForbidden = context =>
                        {
                            context.Response.StatusCode = 403;
                            context.Response.ContentType = "application/json";
                            var result = JsonConvert.SerializeObject(new Response<string>("You are not authorized to access this resource"));
                            return context.Response.WriteAsync(result);
                        },
                    };
                }); ;
        services.AddAuthorizationBuilder();

        #region Services
        services.AddTransient<IEmailService, EmailService>();
        services.AddTransient<IAuthService, AuthService>();
        #endregion

        #region Repositories
        services.AddTransient(typeof(IGenericRepositoryAsync<>), typeof(GenericRepositoryAsync<>));
        services.AddTransient<IServiceRepositoryAsync, ServiceRepositoryAsync>();
        #endregion
        return services;
    }
}
