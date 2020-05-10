using System.Collections.Generic;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.AspNetCore;
using GlobalExceptionHandler.WebApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Photex.Core.Contracts.Settings;
using Photex.Core.Interfaces;
using Photex.Core.Services;
using Photex.Database;
using Photex.Database.Entities;
using Phtotex.Api.Middlewares;

namespace Phtotex.Api
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
            services.Configure<ImageContainerSettings>(Configuration.GetSection(nameof(ImageContainerSettings)));
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<ICatalogueService, CatalogueService>();

            services.AddDbContextPool<PhotexDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("Database")));

            services.AddScoped<JwtAuthorizationMiddleware>();

            services.AddIdentity<ApplicationUser, IdentityRole<long>>(options =>
            {
                options.Password.RequiredLength = 8;
                options.User.RequireUniqueEmail = true;
            })
             .AddEntityFrameworkStores<PhotexDbContext>()
             .AddDefaultTokenProviders();

            services
                .AddAuthentication(options =>
                {
                    options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                 .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                 {
                     options.TokenValidationParameters = new TokenValidationParameters
                     {
                         ValidateIssuer = true,
                         ValidateAudience = true,
                         ValidateLifetime = true,
                         ValidateIssuerSigningKey = true,
                         ValidIssuer = "IssuerPhotex",
                         ValidAudience = "IssuerPhotex",
                         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("KeyPhotexKeyPhotexKeyPhotexKeyPhotexKeyPhotexKeyPhotex"))
                     };
                 });

            services.AddAuthorization(options =>
            {
                var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(
                    JwtBearerDefaults.AuthenticationScheme);
                defaultAuthorizationPolicyBuilder = defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
                options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
            });

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddFluentValidation(config => config.RegisterValidatorsFromAssemblyContaining<Startup>())
                .AddJsonOptions(setup =>
                {
                    setup.AllowInputFormatterExceptionMessages = true;
                    setup.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                    setup.SerializerSettings.Formatting = Formatting.Indented;
                    setup.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    setup.SerializerSettings.Converters.Add(new StringEnumConverter());
                    setup.SerializerSettings.ContractResolver = new DefaultContractResolver();
                });

#if !DEBUG
            services.AddApplicationInsightsTelemetry(Configuration["APPINSIGHTS_INSTRUMENTATIONKEY"]);
#endif
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Photex API definition", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Bearer",
                    BearerFormat = "JWT",
                    Scheme = "bearer",
                    Description = "Specify the authorization token.",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http
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
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Photex API definition");
                c.RoutePrefix = string.Empty;
            });

            app.UseCors(builder => builder
             .WithOrigins("https://photex-company.github.io", "http://localhost:4200")
             .AllowAnyMethod()
             .AllowAnyHeader()
             .AllowCredentials());

            app.UseHttpsRedirection();
            app.UseMiddleware<JwtAuthorizationMiddleware>();
            app.UseAuthentication();

            app.UseGlobalExceptionHandler(opt =>
            {
                opt.ContentType = MediaTypeNames.Application.Json;
                opt.OnError((ex, context) =>
                {
                    var logger = context.RequestServices.GetRequiredService<ILogger<StartupBase>>();
                    logger.LogError(0, ex, "An error occured during processing the request.");
                    return Task.CompletedTask;
                });
                opt.ResponseBody(ex =>
                    JsonConvert.SerializeObject(new { error = "An error occured during processing the request.", exceptionType = ex.GetType().Name, exceptionMessage = ex.Message }));
            });


            app.UseMvc();
        }
    }
}
