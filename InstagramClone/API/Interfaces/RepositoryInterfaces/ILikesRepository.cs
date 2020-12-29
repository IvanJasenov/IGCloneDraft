using API.DTOs;
using API.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Helpers;

namespace API.Interfaces
{
    public interface ILikesRepository
    {
        Task<UserLike> GetUserLike(int sourceId, int likedUserId);

        Task<AppUser> GetUserWithLikes(int userId);

        // list of user that have been liked or liked by some user
        Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams);
    }
}
