using System;
using System.IO;
using System.Net;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Autofac;
using HeatingApi.DependencyResolution;
using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Swagger;

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
            services.AddMvc();

            services.AddSwaggerGen(c =>
                                   {
                                       c.SwaggerDoc("v1", new Info { Title = "HeatingAPI", Version = "v1" });

                                       var xmlFile = $"{Assembly.GetEntryAssembly().GetName().Name}.xml";
                                       var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                                       c.IncludeXmlComments(xmlPath);
                                   });

            services.AddSingleton(Configuration);
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            Modules.Register(builder);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseSwagger();

            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "HeatingAPI v1"); });

            app.UseExceptionHandler(ExceptionHandler);

            app.UseMvc();
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
