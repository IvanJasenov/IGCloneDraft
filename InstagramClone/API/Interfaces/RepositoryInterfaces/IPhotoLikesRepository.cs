using API.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IPhotoLikesRepository
    {
        Task<List<PhotoLikes>> GetPhotoLikesByPhotoId(int photoId);

        Task<List<int>> GetPhotoLikesByUserId(int userId);
    }
}
