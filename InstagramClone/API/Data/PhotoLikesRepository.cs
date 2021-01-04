using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Data
{
    public class PhotoLikesRepository : IPhotoLikesRepository
    {
        private readonly DataContext _dataContext;

        public PhotoLikesRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<List<PhotoLikes>> GetPhotoLikesByPhotoId(int photoId)
        {
            var photoLikes = await _dataContext.PhotoLikes
                                                .Include(p => p.AppUser).ThenInclude(u => u.Photos)
                                                .Where(el => el.PhotoId == photoId)
                                                .ToListAsync();
      
            return photoLikes;
        }
        
        public async Task<List<int>> GetPhotoLikesByUserId(int userId)
        {
            var photoLikes = await _dataContext.PhotoLikes
                                                .Include(p => p.Photo)
                                                .Where(el => el.AppUserId == userId)
                                                .Select(el => el.Photo.Id)
                                                .ToListAsync();
      
            return photoLikes;
        }
    }
}
