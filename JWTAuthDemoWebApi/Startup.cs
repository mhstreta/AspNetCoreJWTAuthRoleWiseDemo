using JWTAuthDemoWebApi.Models;
using JWTAuthDemoWebApi.Repository;
using JWTAuthDemoWebApi.Util;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTAuthDemoWebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var jwtConfiguration = Configuration.GetSection("JWT");
            services.Configure<JWTConfiguration>(jwtConfiguration);

            var jwtConfigurationModel = jwtConfiguration.Get<JWTConfiguration>();

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                var Key = Encoding.UTF8.GetBytes(jwtConfigurationModel.SigningKey);
                o.SaveToken = true;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtConfigurationModel.Issuer,
                    ValidAudience = jwtConfigurationModel.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Key)
                };
            });

            //Role access policies
            services.AddAuthorization(options =>
            {
                options.AddPolicy("FullAccess", policy =>
                      policy.RequireRole(RoleEnum.Admin.ToString(), RoleEnum.Employee.ToString(), RoleEnum.Student.ToString()));

                options.AddPolicy("AdminOnlyAccess", policy =>
                      policy.RequireRole(RoleEnum.Admin.ToString()));

                options.AddPolicy("EmployeeAccess", policy =>
                      policy.RequireRole(RoleEnum.Admin.ToString(), RoleEnum.Employee.ToString()));

                options.AddPolicy("StudentAccess", policy =>
                      policy.RequireRole(RoleEnum.Admin.ToString(), RoleEnum.Student.ToString()));
            });

            services.AddSingleton<IUserRepository, UserRepository>();

            services.AddControllers();
            services.AddTransient<ProblemDetailsFactory, YourBussProblemDetailsFactory>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "JWTAuthDemoWebApi", Version = "v1" });

                // To Enable authorization using Swagger (JWT)  
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "JWTAuthDemoWebApi v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication(); // This need to be added	
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
