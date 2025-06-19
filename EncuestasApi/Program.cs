using EncuestasApi.Hubs;
using EncuestasApi.Models.Entities;
using EncuestasApi.Models.Validations;
using EncuestasApi.Repositories;
using EncuestasApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace EncuenstasAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {


            var builder = WebApplication.CreateBuilder(args);


            var cs = builder.Configuration.GetConnectionString("EncuentasCS");
            builder.Services.AddDbContext<EncuestasContext>(x => x.UseMySql(cs, ServerVersion.AutoDetect(cs)));

            builder.Services.AddScoped(typeof(Repository<>), typeof(Repository<>));
            builder.Services.AddScoped<UsuariosValidator>();
            builder.Services.AddScoped<EncuestaValidator>();
            builder.Services.AddTransient<JWTService>();
            builder.Services.AddSignalR();

            builder.Services.AddControllers();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("PermitirTodo", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

           

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
                    ValidateLifetime = true
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        // Solo para conexiones al hub
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/encuestas"))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };
            });




            var app = builder.Build();

            //app.UseCors("todos");
            app.UseCors("PermitirTodo");
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapGet("/", () => "Solo para que se vea algo");
            app.MapControllers();
            app.UseStaticFiles();
            app.MapHub<HubEncuesta>("/hubs/encuestas");

            app.Run();
        }
    }
}