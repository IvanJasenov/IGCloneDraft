using API.Data;
using API.Data.SeedData;
using API.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            //CreateHostBuilder(args).Build().Run();
            // create local scope for where you will get services
            var host = CreateHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                try
                {
                    // get the DataContext service
                    var dataContext = services.GetRequiredService<DataContext>();
                    // applies migrations nad if dont have/deleted dbs start the application and it'll create dbs
                    await dataContext.Database.MigrateAsync();
                    // get the Role Manager
                    var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
                    // seed the json data to the dbs
                    var userManager = services.GetRequiredService<UserManager<AppUser>>();
                    await DataContextSeed.SeedUserData(userManager, roleManager);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Error occured during migration");
                    throw ex;
                }
                // run the Program now
                await host.RunAsync();
            }
           
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
