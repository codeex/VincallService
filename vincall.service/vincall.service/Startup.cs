using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Vincall.Application;
using Vincall.Application.AutoMapper;
using Vincall.Infrastructure;
using Vincall.Service.Cache;
using Vincall.Service.Models;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using Vincall.Service.WebApiServices;
using Vincall.Service.Services;
using Vincall.Service.Applications;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;

namespace Vincall.service
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
            services.AddControllersWithViews();
            services.AddAutoMapper(typeof(EntityProfile));
            services.AddMemoryCache();
            services.AddScoped<ITwilioSettingCacheService, TwilioSettingCacheService>();
            var connectionString = Configuration.GetConnectionString("Vincall");
            var migrationAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            services.AddEntityFrameworkSqlServer();
            services.AddDbContextPool<VincallDBContext>((sp, builder) =>
            {
                builder.UseSqlServer(connectionString, b => {
                    b.MigrationsAssembly(migrationAssembly);
                });
                builder.UseInternalServiceProvider(sp);
            });
          
            services.AddHttpApi<IComm100OauthClient>();           
            services.AddHttpApi<IComm100ApiClient>();           
            services.AddSingleton<IMigrateCallLogsService, MigrateCallLogsService>();
            services.AddSingleton<ICheckAgentChangeStateService, CheckAgentChangeStateService>();
            services.AddScoped(typeof(ICrudServices<>), typeof(CrudServices<>));
            services.AddScoped<ICrudServices, CrudServices>();
            services.AddScoped<GlobalSettingService>();
            services.AddScoped(s => (DbContext)s.GetRequiredService<VincallDBContext>());
            services.AddScoped<HostProvider>();

            services.AddSwaggerGen(options => {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Vincall Service Api",
                    Version = "v1"
                });

            });
            var oauthUri = new Uri(Configuration["OauthUri"]);            

            services.AddCors(options =>
               options.AddPolicy("cors", p =>
               {
                   p.SetIsOriginAllowed((host) => true);
                   p.AllowAnyHeader();
                   p.AllowAnyMethod();
                   p.AllowCredentials();
                   p.SetPreflightMaxAge(TimeSpan.FromSeconds(24 * 60 * 60));
               }));


            services.AddAuthorization(options => {
                options.AddPolicy("Api", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", "api");
                });
             });


            var x509Cert =new System.Security.Cryptography.X509Certificates.X509Certificate2("vincall.pfx");
            services.AddDataProtection()
                .PersistKeysToDbContext<VincallDBContext>()
                .ProtectKeysWithCertificate(x509Cert)
                .SetApplicationName("vincall");
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.Cookie.Name = CookieHelper.Name;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.Cookie.SameSite = SameSiteMode.None;
                    options.Cookie.HttpOnly = true;
                    options.Cookie.Domain = CookieHelper.GetDomainName(oauthUri.Host);
                    options.ExpireTimeSpan = TimeSpan.FromDays(2);
                    options.Events.OnRedirectToLogin = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return Task.CompletedTask;
                    };
                });  
        }

        
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var migrateService = serviceScope.ServiceProvider.GetService<IMigrateCallLogsService>();
                var checkAgentAndUpdateService = serviceScope.ServiceProvider.GetService<ICheckAgentChangeStateService>();
                // migrateService.MigrateCallLogs();
            }
            InitializeDatabase(app);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.Use(async (ctx, next) =>
                {
                    if(ctx.Request.Path == "/Home/Error")
                    {
                        var ex = ctx.Features.Get<IExceptionHandlerFeature>().Error;
                        ctx.Response.ContentType = "application/json";
                        await ctx.Response.WriteAsync(JsonConvert.SerializeObject(ex));
                    }
                    await next();
                });
                app.UseHsts();
            }
            app.UseCors("cors");
            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("v1/swagger.josn", "v1");
            });
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private void InitializeDatabase(IApplicationBuilder app)
        {
            if (Configuration["InitDB"] == "true")
            {
                Console.WriteLine("Start initialize database...");
                using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
                {
                    var context = serviceScope.ServiceProvider.GetRequiredService<VincallDBContext>();
                    context.Database.Migrate();
                    if (!context.Users.Any())
                    {
                        foreach (var user in Users.All)
                        {
                            context.Users.Add(user);
                        }
                        context.SaveChanges();
                    }
                    if (!context.Agents.Any())
                    {
                        foreach (var agent in Agents.All)
                        {
                            context.Agents.Add(agent);
                        }
                        context.SaveChanges();
                    }
                    if (!context.CallLists.Any())
                    {
                        foreach (var callList in CallLists.All)
                        {
                            context.CallLists.Add(callList);
                        }
                        context.SaveChanges();
                    }
                    if (!context.Settings.Any())
                    {
                        foreach (var setting in Settings.All)
                        {
                            context.Settings.Add(setting);
                        }
                        context.SaveChanges();
                    }
                    if (!context.TwilioSettings.Any())
                    {
                        foreach (var twilioSetting in TwilioSettings.All)
                        {
                            context.TwilioSettings.Add(twilioSetting);
                        }
                        context.SaveChanges();
                    }
                    if (!context.GlobalSetting.Any())
                    {
                        foreach (var globalSetting in GlobalSettings.All)
                        {
                            context.GlobalSetting.Add(globalSetting);
                        }
                        context.SaveChanges();
                    }

                }
                Console.WriteLine("End initialize database!!!");
            }
        }
    }
}
