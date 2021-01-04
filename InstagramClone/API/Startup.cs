using API.Data;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using API.Middleware;
using API.Services;
using API.SignalR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API
{
    public class Startup
    {
        public IConfiguration _configuration { get; }

        public Startup(IConfiguration config)
        {
            _configuration = config;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // this is refered as a Dipendency Injection Container
        // if we want to make a class or a sevice available to other areas of our application we add them inside this container
        // and .net gonna take controll of the creation and distruction of those classes when the're not used
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            #region extension methods
            services.AddApplicationServices(_configuration);
            services.AddIdentityServiceCollection(_configuration);
            services.AddSignalR();
            #endregion

            // enable CORS
            services.AddCors(option =>
            {
                option.AddPolicy("CorsPolicy", policy =>
                {
                    policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4209");
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            #region Exception Middleware 
            app.UseMiddleware<ExceptionMiddleware>();
            #endregion

            app.UseHttpsRedirection();

            app.UseRouting();

            // CORS middleware
            //app.UseCors("CorsPolicy");
            app.UseCors(x => 
                x.AllowAnyHeader()
                 .AllowAnyMethod()
                 .AllowCredentials()
                 .WithOrigins("https://localhost:4209"));

            // add Authentication middleware for tokens, add this at the same time with services.AddAuthentication
            // ordering i simportant, need to come before UseAuthorizatio
            app.UseAuthentication();
           
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                // for SignalE
                endpoints.MapHub<PresenceHub>("hubs/presence");
                endpoints.MapHub<MessageHub>("hubs/message");
            });
        }
    }
}
