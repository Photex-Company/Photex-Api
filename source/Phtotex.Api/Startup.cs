using System;
using System.Net.Mime;
using System.Threading.Tasks;
using FluentValidation.AspNetCore;
using GlobalExceptionHandler.WebApi;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Photex.Core.Contracts;
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

            /*
            services.AddDbContextPool<PhotexDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("database")));
            */

            services.AddDbContextPool<PhotexDbContext>(options =>
            {
                options.UseInMemoryDatabase(Guid.NewGuid().ToString());
                options.ConfigureWarnings(builder =>
                    builder.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            });

            services.AddScoped<CookieAuthorizationMiddleware>();

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
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                 .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                 {
                     options.Cookie.Name = "PhotexAuhtentication";
                     options.Cookie.Expiration = TimeSpan.FromDays(1);
                 });

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
                options.SlidingExpiration = true;
            });

            services.AddAuthorization(options =>
            {
                var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(
                    CookieAuthenticationDefaults.AuthenticationScheme);
                defaultAuthorizationPolicyBuilder = defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
                options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
            });

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                 .AddFluentValidation(config => config.RegisterValidatorsFromAssemblyContaining<Startup>())
                   .AddJsonOptions(setup =>
                   {
                       setup.AllowInputFormatterExceptionMessages = true;
                       setup.SerializerSettings.DateFormatString = "yyyy-MM-dd";
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
             .AllowAnyOrigin()
             .AllowAnyMethod()
             .AllowAnyHeader()
             .AllowCredentials());

            app.UseHttpsRedirection();
            app.UseCookiePolicy();
            app.UseMiddleware<CookieAuthorizationMiddleware>();
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
