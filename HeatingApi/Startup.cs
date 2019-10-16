using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Autofac;
using HeatingApi.DependencyResolution;
using HeatingControl.Application.Commands;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.AspNetCore.Authorization;
using HardwareAccess.PlatformIntegration;
using Commons.Extensions;

namespace HeatingApi
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
            services.Configure<RequestLocalizationOptions>(
                                                        opts =>
                                                        {
                                                            var supportedCultures = new List<CultureInfo>
                                                                                    {
                                                                                        new CultureInfo("en"),
                                                                                        new CultureInfo("pl")
                                                                                    };

                                                            opts.DefaultRequestCulture = new RequestCulture("en");

                                                            opts.SupportedCultures = supportedCultures;
                                                            opts.SupportedUICultures = supportedCultures;
                                                        });

            services.AddMvc()
                    .AddJsonOptions(x => x.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind);

            services.AddSwaggerGen(c =>
                                   {
                                       c.SwaggerDoc("v1", new Info { Title = "HeatingAPI", Version = "v1" });

                                       var xmlFile = $"{Assembly.GetEntryAssembly().GetName().Name}.xml";
                                       var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                                       c.IncludeXmlComments(xmlPath);

                                       c.AddSecurityDefinition("Bearer",
                                                               new ApiKeyScheme
                                                               {
                                                                   In = "header",
                                                                   Name = "Authorization",
                                                                   Type = "apiKey"
                                                               });
                                   });

            services.AddSingleton(Configuration);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                                  {
                                      options.TokenValidationParameters = new TokenValidationParameters
                                      {
                                          ValidateIssuer = true,
                                          ValidateAudience = true,
                                          RequireExpirationTime = true,
                                          ValidateLifetime = true,
                                          ValidateIssuerSigningKey = true,
                                          ValidIssuer = Configuration["Jwt:Issuer"],
                                          ValidAudience = Configuration["Jwt:Issuer"],
                                          IssuerSigningKey = AuthenticateUserCommandExecutor.JwtSigningKey
                                      };
                                  });

            services.AddCors(options =>
                             {
                                 options.AddPolicy("CorsPolicy", builder => builder.AllowAnyOrigin()
                                                                                   .AllowAnyMethod()
                                                                                   .AllowAnyHeader()
                                                                                   .AllowCredentials()
                                                                                   .Build());
                             });

            services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            Modules.Register(builder);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IApplicationLifetime applicationLifetime, IHostingEnvironment env)
        {
            applicationLifetime.ApplicationStarted.Register(() => OnApplicationStarted(app));

            app.UseCors("CorsPolicy");

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "HeatingAPI v1"); });

            app.UseExceptionHandler(ExceptionHandler);
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseAuthentication();

            var options = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(options.Value);

            app.UseMvc(routes => { routes.MapRoute("Spa", "{*url}", new { controller = "Home", action = "Spa" }); });
        }

        private void OnApplicationStarted(IApplicationBuilder app)
        {
            var command = Configuration["PlatformIntegration:RunWhenApiReadyCommand"];
            var arguments = Configuration["PlatformIntegration:RunWhenApiReadyArguments"];

            if (!command.IsNullOrEmpty())
            {
                var processRunner = app.ApplicationServices.GetService<IProcessRunner>();
                processRunner.Run(command, arguments);
            }
        }

        private static void ExceptionHandler(IApplicationBuilder options)
        {
            options.Run(
                        async context =>
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                            var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

                            if (exception == null)
                            {
                                return;
                            }

                            context.Response.ContentType = "application/json";

                            using (var writer = new StreamWriter(context.Response.Body))
                            {
                                new JsonSerializer().Serialize(writer, exception);

                                await writer.FlushAsync().ConfigureAwait(false);
                            }
                        });
        }
    }
}
