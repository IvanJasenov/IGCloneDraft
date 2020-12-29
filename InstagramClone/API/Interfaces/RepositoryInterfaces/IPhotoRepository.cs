using API.DTOs;
using API.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IPhotoRepository
    {
        Task<IEnumerable<Photo>> GetUnapprovedPhotos();

        Task<List<Photo>> GetAllPhotos();

        Task<List<Photo>> GetAllPhWithComAndUsr();

        Task<Photo> GetPhotoById(int photoId);

        Task<List<Photo>> PhotosWithComments();

        Task<Photo> GetPhotoComments(int photoId);

        void RemovePhoto(Photo photo);

        #region PhotoLikes

        Task LikePhoto(PhotoLikes photoLikes);

        void DeleteLike(PhotoLikes photoLikes);

        #endregion
    }
}