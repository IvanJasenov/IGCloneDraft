using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Authorize]
    public class LikesController : BaseApiController
    {
        
        private readonly IUnitOfWork _unitOfWork;

        public LikesController(IUnitOfWork unitOfWork)
        {
           
            _unitOfWork = unitOfWork;
        }

        // {{url}}/likes/username
        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
            var sourceUserId = HttpContext.GetUserId();
            var likedUser = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);
            var sourceUser = await _unitOfWork.LikesRepository.GetUserWithLikes(sourceUserId);
            if (likedUser == null)
            {
                return NotFound();
            }
            // prevent self likes
            if (sourceUser.UserName == username)
            {
                return BadRequest("You cannot like yourself");
            }
            var userLike = await _unitOfWork.LikesRepository.GetUserLike(sourceUserId, likedUser.Id);
            if (userLike != null)
            {
                return BadRequest("You already liked this user");
            }

            userLike = new UserLike
            {
                SourceUserId = sourceUserId,
                LikedUserId = likedUser.Id
            };

            sourceUser.LikedUsers.Add(userLike);

            if (await _unitOfWork.Complete())
            {
                return Ok(new { success = true });
            }
            return BadRequest("Failed to like user");
        }

        [HttpDelete("deleteLike/{username}")]
        public async Task<ActionResult> DeleteLike(string username)
        {
            int sourceUserId = HttpContext.GetUserId();
            AppUser likedUser = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);
            int likedUserId = likedUser.Id;
            AppUser sourceUser = await _unitOfWork.LikesRepository.GetUserWithLikes(sourceUserId);
            // delete
            UserLike userLike = await _unitOfWork.LikesRepository.GetUserLike(sourceUserId, likedUserId);
            sourceUser.LikedUsers.Remove(userLike);
            if (await _unitOfWork.Complete())
            {
                return Ok(new { success = true });
            }
            return BadRequest(new { success = false });
        }

        // {{url}}/likes?predicates=liked
        // {{url}}/likes?predicates=likedBy
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LikeDto>>> GetUserLikes([FromQuery] LikesParams likesParams)
        {
            var userId = HttpContext.GetUserId();
            likesParams.UserId = userId;
            var userLikes = await _unitOfWork.LikesRepository.GetUserLikes(likesParams);
            if (userLikes == null)
            {
                return BadRequest("No likes found");
            }
            // set the pagination header
            Response.AddPaginationHeader(userLikes.CurrentPage, userLikes.PageSize, userLikes.TotalCount, userLikes.TotalPages);

            return Ok(userLikes);
        }

        [HttpGet("users-not-followed")]
        public async Task<ActionResult<List<LikeDto>>> GetNotLikedUsers()
        {
            int userId = HttpContext.GetUserId();
            List<LikeDto> users = await _unitOfWork.LikesRepository.GetUsersNotLiked(userId);
            if (users.Count > 0)
            {
                return Ok(users);
            }
            return BadRequest("You like all users");
        }


    }
}
