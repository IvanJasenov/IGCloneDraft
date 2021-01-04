using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Data
{
    public class PhotoRepository : IPhotoRepository
    {
        private readonly DataContext _dataContext;

        public PhotoRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<Photo> GetPhotoById(int photoId)
        {
            return await _dataContext.Photos.IgnoreQueryFilters().FirstOrDefaultAsync(el => el.Id == photoId); ;
        }

        // intentionally left this as Photo return type and not PhotoDto 
        public async Task<IEnumerable<Photo>> GetUnapprovedPhotos()
        {
            IEnumerable<Photo> photos = await _dataContext.Photos.Include(el => el.AppUser).IgnoreQueryFilters().Where(p => p.IsApproved == false).ToListAsync();

            return photos;
        }

        public async Task<List<Photo>> GetAllPhotos()
        {
            var photos = await _dataContext.Photos.Include(el => el.AppUser).ToListAsync();

            return photos;
        }

        public void RemovePhoto(Photo photo)
        {
            //var res = _dataContext.Photos.Remove(photo);
            _dataContext.Photos.Remove(photo);
        }

        public async Task<List<Photo>> PhotosWithComments()
        {
            // this query will load the AppUser if there exists a comment for the specific photo
            var photosWithComments = await _dataContext.Photos.Where(el => el.PhotoComments.Count > 0)
                                                        .Include(e => e.AppUser)
                                                        .Include(el => el.PhotoComments)
                                                        .ThenInclude(e => e.AppUser) // load the comment creator
                                                        .ToListAsync();
       
            return photosWithComments;
        }

        public async Task<Photo> GetPhotoComments(int photoId)
        {
            var phComments = await _dataContext.Photos.IgnoreQueryFilters()
                                                        .Where(p => p.Id == photoId)
                                                        .Include(el => el.AppUser).ThenInclude(p => p.Photos.Where(el => el.IsMain)) // include the photo owner
                                                        .Include(el => el.PhotoComments) // keep an eye on ThenInclude, important
                                                        .ThenInclude(el => el.AppUser) //  include the photo commentot
                                                        .ThenInclude(p => p.Photos.Where(el => el.IsMain)) // load the main photo for the creator
                                                        .SingleOrDefaultAsync();
            // if querying the photo comments
            //var photoComments = await _dataContext.PhotoComments
            //                                       .Where(p => p.Id == photoId)
            //                                       .Include(el => el.AppUser)
            //                                       .SingleOrDefaultAsync();

            return phComments;
        }

        public async Task<List<Photo>> GetAllPhWithComAndUsr()
        {
            // ignore query filters for now, see whether you should handle each image from admin
            var result = await _dataContext.Photos
                                                  .IgnoreQueryFilters()
                                                  .Include(el => el.AppUser)
                                                  .Include(el => el.PhotoComments)  // keep an eye on ThenInclude, important
                                                  .ThenInclude(el => el.AppUser)
                                                  .ToListAsync();

            return result;
        }

        public async Task LikePhoto(PhotoLikes photoLikes)
        {
            await _dataContext.PhotoLikes.AddAsync(photoLikes);
        }

        public void DeleteLike(PhotoLikes photoLikes)
        {
            _dataContext.PhotoLikes.Remove(photoLikes);
        }
    }
}
