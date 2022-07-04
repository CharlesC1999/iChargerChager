using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using backend.util;
using Microsoft.AspNetCore.StaticFiles;
namespace backend
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
            services.AddHttpContextAccessor();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            string[] corsOrigins = Configuration["Cors:AllowOrigin"].Split(',', StringSplitOptions.RemoveEmptyEntries);
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        if (corsOrigins.Contains("*"))
                        {
                            builder.SetIsOriginAllowed(_ => true);
                        }
                        else
                        {
                            builder.WithOrigins(corsOrigins);
                        }
                        builder.AllowAnyMethod();
                        builder.AllowAnyHeader();
                        builder.AllowCredentials();
                    }
                    );
            });

            #region api
            // Service
            // services.AddScoped<Services.AuthService>();
            // Dao
            // services.AddScoped<dao.UserDao>();
            #endregion

            services.AddControllers()
                .AddJsonOptions(options =>
                    options.JsonSerializerOptions.PropertyNamingPolicy = null);
            services.Configure<appSettings>(Configuration.GetSection("appSettings"));
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "backend", Version = "v1" });
            });
            services.AddMvc();
            services.AddSingleton<FileExtensionContentTypeProvider>();
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = "";
            });
            // app.UseHttpsRedirection();
            app.UseStaticFiles();
            // app.UseStaticFiles(new StaticFileOptions{
            //     FileProviders = new PhysicalFileProvider(
            //         Path.Combine(env.ContentRootPath, "Files")
            //     ),
            //     RequestPath = "/Files"
            // });
            app.UseRouting();
            // app.UseAuthorization();
            // app.UseMiddleware<jwtMiddleware>();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            // 排程
            // var provider = app.ApplicationServices;
            // provider.UseScheduler(scheduler =>
            // {
            //     scheduler.Schedule<analysisSchedule>()
            //         // .EveryMinute();
            //         .DailyAtHour(2);
            //         // .PreventOverlapping("analysisSchedule");
            //         // .Sunday();
            // });
        }
    }
}