using apiTicket.Repository;
using apiTicket.Repository.DB;
using apiTicket.Repository.Interfaces;
using apiTicket.Services;
using apiTicket.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Authentication;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace apiTicket
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseAuthentication();
            app.Use(async (ctx, next) =>
            {
                await next();
                if (ctx.Response.StatusCode == 204)
                {
                    ctx.Response.ContentLength = 0;
                }
            });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowFromAll");
            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                //c.RoutePrefix = "swagger";
                c.SwaggerEndpoint("../swagger/v1/swagger.json", "ApiTicket");
            });
        }

        public IConfiguration Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowFromAll",
                    builder => builder
                    .WithMethods("GET", "POST", "PUT", "DELETE")
                    .AllowAnyOrigin()
                    .AllowAnyHeader());
            });

            services.AddAuthorization(options =>
            options.DefaultPolicy = 
            new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme).RequireAuthenticatedUser().Build()
            );
            //estos valores los obtenemos de nuestro appsettings
            var issuer = Configuration["Jwt:Issuer"];
            var audience = Configuration["Jwt:Audience"];
            var signinKey = Configuration["Jwt:SecretKey"];
            //autenticacion
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.Audience = audience;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer, 
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(signinKey))
                };
            }
            );

            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddScoped<IConnectionBase, ConnectionBase>();
            services.AddScoped<ITicketService, TicketService>();
            services.AddScoped<ITicketRepository, TicketRepository>();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "API Ticket", Version = "v1" });
            }
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    }
}