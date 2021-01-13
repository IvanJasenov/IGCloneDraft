using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Data
{
    public class LikesRepository : ILikesRepository
    {
        private readonly DataContext _context;

        public LikesRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<UserLike> GetUserLike(int sourceId, int likedUserId)
        {
        
            return await _context.Likes.FirstOrDefaultAsync(item => item.SourceUserId == sourceId && item.LikedUserId == likedUserId);
        }

        public async Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams)
        {
            var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();
            var likes = _context.Likes.AsQueryable();
            // who I like
            if (likesParams.Predicates == "liked")
            {
                likes = likes.Where(like => like.SourceUserId == likesParams.UserId);
                users = likes.Select(like => like.LikedUser);
            }
            // who likes me
            if (likesParams.Predicates == "likedBy")
            {
                likes = likes.Where(like => like.LikedUserId == likesParams.UserId);
                users = likes.Select(like => like.SourceUser);
            }

            // used projection, could use AutoMapper but the amount of code would be the same
            var likedUsers = users.Select(user => new LikeDto
            {
                Id = user.Id,
                Username = user.UserName,
                KnownAs = user.KnownAs,
                Age = user.DateOfBirth.CalculateAge(),
                PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain).Url,
                City = user.City
            }).AsQueryable();

            return await PagedList<LikeDto>.CreateAsync(likedUsers, likesParams.PageNumber, likesParams.ItemsPerPage);
        }

        public async Task<List<LikeDto>> GetUsersNotLiked(int userId)
        {
            var likes = _context.Likes.AsQueryable();
            var users_I_LikeIds = likes.Where(like => like.SourceUserId == userId).Select(el => el.LikedUserId).AsQueryable();
            List<int> LikedIds = await users_I_LikeIds.ToListAsync();

            var notLikedusers = _context.Users.AsQueryable();
            LikedIds.ForEach(el =>
            {
                notLikedusers = notLikedusers.Where(u => u.Id != el && u.Id != userId).AsQueryable();
            });
            // projection
            var notlikedUsers = notLikedusers.OrderByDescending(i => i.LastActive)
                .Take(5)
                .Where(u => u.UserName.ToLower() != "admin")
                .Select(user => new LikeDto
                {
                    Id = user.Id,
                    Username = user.UserName,
                    KnownAs = user.KnownAs,
                    Age = user.DateOfBirth.CalculateAge(),
                    PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain).Url,
                    City = user.City
                }).AsQueryable();

            List<LikeDto> notLikedUsersList = await notlikedUsers.ToListAsync();

            return notLikedUsersList;
        }

        // list of users that this user has liked
        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            return await _context.Users
                                 .Include(l => l.LikedUsers)
                                 .FirstOrDefaultAsync(el => el.Id == userId);
        }
    }
}
