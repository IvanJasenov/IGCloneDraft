using API.Entities;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Data
{
    public class PhotoCommentsRepository : IPhotoCommentsRepository
    {
        private readonly DataContext _dataContext;

        public PhotoCommentsRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async void AddComment(PhotoComment photoComment)
        {
            await _dataContext.PhotoComments.AddAsync(photoComment);
           
        }

        public async Task<PhotoComment> GetPhCommentAndUser(int photoId, int userId, string commment)
        {
            var res = await _dataContext.PhotoComments
                                        .Where(p => p.PhotoId == photoId && p.AppUserId == userId && p.Comment == commment)
                                        .Include(el => el.AppUser)
                                        .FirstOrDefaultAsync();
            return res;
        }

        public async Task<PhotoComment> GetPhotoComment(int photoId, string commentOriginal)
        {
            PhotoComment photoComment = await _dataContext.PhotoComments
                                                          .Where(el => el.PhotoId == photoId && el.Comment == commentOriginal)
                                                          .Include(el => el.AppUser)
                                                          .FirstOrDefaultAsync();
            return photoComment;
        }

        public async Task<bool> DeletePhotoComment(int photoCommentId, int userId)
        {
            PhotoComment photoCommentToDelete = await _dataContext.PhotoComments
                                                                  .Where(el => el.PhotoId == photoCommentId && el.AppUserId == userId)
                                                                  .FirstOrDefaultAsync();
            if (photoCommentToDelete == null)
            {
                return false;
            }
            else
            {
                _dataContext.PhotoComments.Remove(photoCommentToDelete);
                return true;
            }
           
        }

        public void Update(PhotoComment photoComment)
        {
            _dataContext.Entry(photoComment).State = EntityState.Modified;
        }
    }
}
