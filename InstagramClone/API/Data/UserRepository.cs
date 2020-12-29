using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;

        public UserRepository(DataContext context)
        {
            _context = context;
        }

        public UserRepository(DataContext context, IMapper mapper) : this(context)
        {
            Mapper = mapper;
        }

        public IMapper Mapper { get; }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.Users.Include(el => el.Photos).FirstOrDefaultAsync(el => el.Id == id);
        }

        public async Task<AppUser> GetUserByIdWithoutPhotosAsync(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(el => el.Id == id);
        }

        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
            //return await _context.Users.Include(el => el.Photos).SingleOrDefaultAsync(el => el.UserName == username);
            // this one laods the related photo commements and likes. Mapping is tricky -TODO

            // hoover over the labda expression and it'll show you the type of the expression
            // might fetch PhotoLikes this way...see it ater how to concat those two queris

            //var getPhotoLikes = await _context.Users
            //                                    .Include(el => el.Photos)
            //                                    .ThenInclude(p => p.PhotoLikes).ThenInclude(u => u.AppUser)
            //                                    .SingleOrDefaultAsync(el => el.UserName == username);

            return await _context.Users
                                    // first branch
                                    .Include(el => el.Photos)
                                    // once in photos, from there include PhotoComments and
                                    // from PhtotComments include PhotoLikes
                                    .ThenInclude(p => p.PhotoComments).ThenInclude(u => u.AppUser)
                                    // second branch
                                    .Include(c => c.PhotoLikes).ThenInclude(u => u.AppUser)
                                    .SingleOrDefaultAsync(el => el.UserName == username);
        }

        public async Task<AppUser> GetUserByUsernameWithoutPhotos(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(el => el.UserName == username);
        }

        public async Task<PagedList<AppUser>> GetUsersAsync(UserParams userParams)
        {
            var query = _context.Users
                                .Include(el => el.Photos)
                                .Where(el => el.Gender == userParams.Gender && el.UserName != userParams.CurrentUsername)
                                .AsNoTracking();

            var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
            var maxDob = DateTime.Today.AddYears(-userParams.MinAge);

            query = query.Where(el => el.DateOfBirth >= minDob && el.DateOfBirth <= maxDob).OrderByDescending(el => el.DateOfBirth);

            // make use of the new switch statement
            query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(el => el.DateCreated),
                _ => query.OrderByDescending(el => el.LastActive) // the default will be Last Active
            };

            return await PagedList<AppUser>.CreateAsync(query, userParams.PageNumber, userParams.ItemsPerPage);
        }

        //public async Task<bool> SaveAllAsync()
        //{
        //    return await _context.SaveChangesAsync() > 0 ? true : false;
        //}

        public void Update(AppUser appUser)
        {
            _context.Entry(appUser).State = EntityState.Modified;
        }
    }
}
