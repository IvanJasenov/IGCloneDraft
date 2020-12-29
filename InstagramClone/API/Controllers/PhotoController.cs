using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Authorize]
    public class PhotoController : BaseApiController
    {
        private readonly IPhotoService _photoService;

        public PhotoController(IPhotoService photoService)
        {
            _photoService = photoService;
        }

        [HttpGet("photos-with-comments")]
        public async Task<ActionResult<IEnumerable<PhotoCommentUser>>> PhotosWithComments()
        {
            List<PhotoCommentUser> photoCommets = await _photoService.GetPhotoCommentsAndUsers();
            //photoCommets.ForEach(el => {
            //    el.PhCommentCreators.Add(new PhCommentCreator { AppUser = new AppUser(), Comment = el.PhotoComments.First() });
            //});
            if (photoCommets != null)
            {
                return Ok(photoCommets);
            }
            return NotFound("No Photo Commets found");
        }

        //[Authorize(Roles = "Member")]
        [HttpGet("comments-for-photo/{photoId}")]
        public async Task<ActionResult<PhotoCommentUserDto>> GetPhotoComments(int photoId)
        {
            if (photoId == 0)
            {
                return BadRequest("Bad User Request");

            }
            var photoCommetsById = await _photoService.GetPhCommentsById(photoId);
            if (photoCommetsById == null)
            {
                return NotFound("No comments found");
            }

            return Ok(photoCommetsById);
        }

        [HttpGet("get-all-photos-with-com-urs")]
        public async Task<ActionResult<PhotoCommentUserDto>> GetAllPhotosWithCommetsAndUsers()
        {
            var result = await _photoService.GetAllPhotosWithComments();
            //result.ForEach(el => {
            //    el.PhCommentCreators.Add(new PhCommentCreator { AppUser = new AppUser(), Comment = el.PhotoComments.First() });
            //});

            if (result == null)
            {
                return NotFound("No data found");
            }
            return Ok(result);

        }

        #region PhotoComments


        [HttpPost("create-comment/{photoId}")]
        public async Task<ActionResult<PhCommentCreatorDto>> CreateComment(int photoId, PhtoCommentCreation photoCommentObject)
        {
            int userId = HttpContext.GetUserId();
            string comment = photoCommentObject.PhotoComment;
            PhotoComment photoComment = new PhotoComment(comment, userId, photoId);
            // save it to the dbs
            if (await _photoService.AddCommentForPhoto(photoComment))
            {
                var photoCommentCreatorDto = await _photoService.GetPhCommentAuthorById(photoId, userId, photoCommentObject.PhotoComment);
                //return Ok(photoWithComments);
                return Ok(photoCommentCreatorDto);
            }
            return BadRequest("Comment cannot be added");

        }

        [HttpPut("edit-photo-comment/{photoId}")]
        public async Task<ActionResult<PhCommentCreatorDto>> EditPhotoComment(int photoId, PhotoCommentObject photoCommentObject)
        {
            int userId = HttpContext.GetUserId();
            string commentEdited = photoCommentObject.PhotoCommentEdited;
            string commentOriginal = photoCommentObject.PhotoCommentOriginal;
            PhCommentCreatorDto photoComment = await _photoService.EditPhotoComment(photoId, commentEdited, commentOriginal);
            if (photoComment != null)
            {
                return Ok(photoComment);
            }
            return BadRequest("Error in updating comment");

        }

        [HttpDelete("delete-photo-comment/{photoId}")]
        public async Task<ActionResult<bool>> DeletePhotoComment(int photoId)
        {
            int userId = HttpContext.GetUserId();
            bool deleteComment = await _photoService.DeletePhotoComment(photoId, userId);
            if (deleteComment)
            {
                return true;
            }

            return BadRequest("Failed to delete Photo");
        }

        #endregion

        #region PhotoLikes

        // make this FromQuery, because it is more then one param and placing thise into object is jut overkill
        // this is my practice and solution
        // userId can be omitted in the request, the default value will be 0 and it's optional since it comes as query param
        // this is my solution
        [HttpPost("{add-photo-like}")]
        public async Task<ActionResult<PhotoLikeDto>> CreatePhotoLike([FromQuery] int photoId, int userId)
        {
            int loggedInUserId = HttpContext.GetUserId();
            if (userId != 0 && (userId != loggedInUserId))
            {
                return BadRequest("Cannot like!");
            }
            PhotoLikeDto photoLikeDto = await _photoService.LikePhoto(photoId, userId == 0 ? loggedInUserId : userId);

            if (photoLikeDto == null)
            {
                return BadRequest("cannot like photo");
            }
            return Ok(photoLikeDto);
        }

        // userId can be omitted in the request, the default value will be 0 and it's optional since it comes as query param
        // this is my solution
        [HttpDelete("{delete-photo-like}")]
        public async Task<ActionResult<bool>> DeletePhotoLike([FromQuery] int photoId, int userId)
        {
            int loggedInUserId = HttpContext.GetUserId();
            if (userId != 0 && (userId != loggedInUserId))
            {
                return BadRequest("Cannot delete like!");
            }
            bool deleteResult = await _photoService.UnlikePhoto(photoId, userId == 0 ? loggedInUserId : userId);
            if (deleteResult)
            {
                return Ok(true);
            }
            return BadRequest("Cannot remove like");
        }

        [HttpGet("get-photo-likes-by-photoId/{photoId}")]
        public async Task<ActionResult<List<PhotoLikeDto>>> GetPhotoLikesById(int photoId)
        {
            List<PhotoLikeDto> photoLikesDto = await _photoService.GetLikesByPhotoId(photoId);
            if (photoLikesDto != null)
            {
                return Ok(photoLikesDto);
            }
            return BadRequest("Nobody likes this photo");
        }
       
        [HttpGet("get-liked-photos-by-logedInUser")]
        public async Task<ActionResult<List<int>>> GetLikedPhotosByUsername()
        {
            // get list of ins, list of photo Ids, all photos liked by this currently logged in user
            int userId = HttpContext.GetUserId();
            List<int> photosIdLikedByUser = await _photoService.LikedPhotosByUser(userId);
            if (photosIdLikedByUser != null)
            {
                return Ok(new { likedPhotosIds = photosIdLikedByUser });
            }
            return BadRequest("User have't liked any photo yet");
        }

        #endregion
    }

}
