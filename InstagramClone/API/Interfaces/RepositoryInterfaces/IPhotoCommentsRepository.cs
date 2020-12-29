using API.Entities;
using API.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IPhotoCommentsRepository
    {
        void AddComment(PhotoComment photoComment);

        void Update(PhotoComment photoComment);

        Task<PhotoComment> GetPhCommentAndUser(int photoId, int userId, string commment);

        Task<PhotoComment> GetPhotoComment(int photoId, string commentOriginal);

        Task<bool> DeletePhotoComment(int photoCommentId, int userId);
    }
}
