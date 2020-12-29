using API.Entities;
using API.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IUserRepository
    {
        void Update(AppUser appUser);

        //Task<bool> SaveAllAsync();

        Task<PagedList<AppUser>> GetUsersAsync(UserParams userParams);

        Task<AppUser> GetUserByIdAsync(int id);

        Task<AppUser> GetUserByIdWithoutPhotosAsync(int id);

        Task<AppUser> GetUserByUsernameAsync(string username);

        Task<AppUser> GetUserByUsernameWithoutPhotos(string username);
    }
}
