using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UserAPI.Data.DAL;
using UserAPI.Data.Entities;
using UserAPI.Services;

namespace UserAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            //dotnet ef migrations add CategoryChangetoRequired --context StoreDbContext --output-dir Data/Migrations

            services.AddControllers();
            services.AddDbContext<UserDbContext>(opt =>
            {
                opt.UseSqlServer(Configuration.GetConnectionString("Default"));
            });
            services.AddIdentity<AppUser, IdentityRole>(opt =>
            {
                opt.Password.RequireNonAlphanumeric = false;
            })
                  .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<UserDbContext>();

           
            services.AddScoped<IJwtService, JwtService>();

            services.AddAuthentication(opt =>
            {
                opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(cfg =>
            {
                cfg.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidIssuer = Configuration.GetSection("JWT:issuer").Value,
                    ValidAudience = Configuration.GetSection("JWT:audience").Value,
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(Configuration.GetSection("JWT:secret").Value))
                };
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "User API",
                    Version = "v1",
                    Description = "An API to perform User operations",
                    TermsOfService = new Uri("https://example.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "Tebriz",
                        Email = "tbrzmmdvs@gmail.com",
                        Url = new Uri("https://gmail.com"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Tebriz",
                        Url = new Uri("https://gmail.com"),
                    }
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                    new OpenApiSecurityScheme
                     {
                      Reference = new OpenApiReference
                     {
                       Type = ReferenceType.SecurityScheme,
                      Id = "Bearer"
                    }
                     },
                     new string[] { }
                 }
                 });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
                c.AddFluentValidationRulesScoped();
            });


        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseSwagger();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseSwaggerUI(x =>
            {
                x.SwaggerEndpoint("/swagger/v1/swagger.json", "User API V1");

            });
        }
    }
}
