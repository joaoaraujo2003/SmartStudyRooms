using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CoreWCF;
using CoreWCF.Configuration;
using CoreWCF.Description;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SmartStudyRooms.API.Services;
using SmartStudyRooms.API.Services.SOAP;
using SmartStudyRooms.Data.Repositories;

namespace SmartStudyRooms.API
{
    public class Startup
    {

        public IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("connection.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSingleton<IConfiguration>(Configuration);

            // Registrar SalaRepository com DI
            services.AddSingleton<SalaRepository>();
            services.AddScoped<ReservaRepository>();
            services.AddHostedService<SalaCleanUpService>();
            services.AddScoped<SensorRepository>();
            services.AddServiceModelServices();
            services.AddServiceModelMetadata(); 

            services.AddScoped<ISalaSoapService, SalaSoapService>();
            services.AddScoped<SalaSoapService>();


            services.AddScoped<IReservaSoapService, ReservaSoapService>();
            services.AddScoped<ReservaSoapService>();

            // Swagger
            services.AddSwaggerGen();

            // Permitir CORS se precisares (ex: frontend local)
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                });
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors("AllowAll");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SmartStudyRooms API V1");
            });

            app.UseServiceModel(builder =>
            {
                builder.AddService<SalaSoapService>();
                builder.AddServiceEndpoint<SalaSoapService, ISalaSoapService>(
                    new BasicHttpBinding(),
                    "/soap/salas"
                );

                builder.AddService<ReservaSoapService>();
                builder.AddServiceEndpoint<ReservaSoapService, IReservaSoapService>(
                    new BasicHttpBinding(),
                    "/soap/reservas"
                );
            });

            app.UseMiddleware<ApiKeyMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            

            var serviceMetadataBehavior = app.ApplicationServices
                .GetRequiredService<ServiceMetadataBehavior>();

            serviceMetadataBehavior.HttpGetEnabled = true;
        }
    }
}
