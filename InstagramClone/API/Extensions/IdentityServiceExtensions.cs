using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Extensions
{
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddIdentityServiceCollection(this IServiceCollection services, IConfiguration configuration)
        {
            #region Identity setup
            services.AddIdentityCore<AppUser>(opt => {
                // if want to make use of weak password, turn off all the password requirements
                opt.Password.RequireNonAlphanumeric = false;
            })
                .AddRoles<AppRole>()
                .AddRoleManager<RoleManager<AppRole>>() // bound the RoleManager to AppRole
                .AddSignInManager<SignInManager<AppUser>>() // Bound the SignInManager to AppUser
                .AddRoleValidator<RoleValidator<AppRole>>() // bound the RoleValidator to AppUser
                .AddEntityFrameworkStores<DataContext>();

            // I added this. make RoleManager<IdentityRole<int>> roleManager as injectable service
            // if this added I cannot use Policy AUthorization 
            //services.AddIdentity<AppUser, IdentityRole<int>>()
            //        .AddEntityFrameworkStores<DataContext>();

            #endregion

            // Authentication service for token, Client -> API
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(options =>
               {
                   options.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateIssuerSigningKey = true,
                       IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Token:Key"])),
                       // accept tokens only issued by this server
                       //ValidIssuer = configuration["Token:Issuer"],
                       ValidateIssuer = false,
                       // to who the token is issued to
                       ValidateAudience = false
                   };
                   // Setting up SingnalR Authorization, because SignalR and WebSockets cannot send Authentication header
                   options.Events = new JwtBearerEvents
                   {
                       OnMessageReceived = context =>
                       {
                           var accessToken = context.Request.Query["access_token"];
                           var path = context.HttpContext.Request.Path;
                           if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs")) 
                           {
                               context.Token = accessToken;
                           }

                           return Task.CompletedTask;
                       }
                   };
               });
            // Policy based Authorization
            services.AddAuthorization(opt => {
                opt.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));               
                opt.AddPolicy("ModeratePhotoRole", policy => policy.RequireRole("Admin", "Moderator"));               
            });

            return services;
        }
    }
}
