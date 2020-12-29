using API.Data;
using API.Helpers;
using API.Interfaces;
using API.Services;
using API.SignalR;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // register the presence tracker
            // Singleton objects are the same for every object and every request.
            services.AddSingleton<PresenceTracker>();
            // register services
            // Scoped objects are the same within a request, but different across different requests.
            services.AddScoped<ITokenService, TokenService>();
            services.AddDbContext<DataContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            //services.AddScoped<IUserRepository, UserRepository>();
            //services.AddScoped<ILikesRepository, LikesRepository>();
            //services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            // register AutoMapper as a service
            services.AddAutoMapper(typeof(AutoMapperProfiles), typeof(PhotoMappingProfiles), typeof(UserMapperProfiles));
            // register Cloudinary, we read from json and write into class CloudinarySettings an object from which we can access 
            // the data.
            services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));
            // register PhotoService
            services.AddScoped<IPhotoService, PhotoService>();
            // add the ActionFilter as a service
            services.AddScoped<LogUserActivity>();

            return services;
        }
    }
}
