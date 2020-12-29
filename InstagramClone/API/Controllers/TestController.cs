using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Interfaces;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace API.Controllers
{
    public class TestController : BaseApiController
    {
        private readonly IPhotoService _photoService;

        public TestController(IPhotoService photoService)
        {
            _photoService = photoService;
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("photos-with-comments")]
        public async Task<ActionResult<IEnumerable<PhotoCommentUser>>> PhotosWithComments()
        {
            List<PhotoCommentUser> photoCommets = await _photoService.GetPhotoCommentsAndUsers();

            if (photoCommets != null)
            {
                return Ok(photoCommets);
            }
            return NotFound("No Photo Commets found");
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("comments-for-photo/{photoId}")]
        public async Task<ActionResult<PhotoCommentUserDto>> GetPhotoComments(int photoId)
        {
            if (photoId == 0)
            {
                return BadRequest("Bad User Request");

            }
            PhotoCommentUserDto photoCommetsById = await _photoService.GetPhCommentsById(photoId);
            if (photoCommetsById == null)
            {
                return NotFound("No comments found");
            }

            return Ok(photoCommetsById);
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("get-all-photos-with-com-urs")]
        public async Task<ActionResult<PhotoCommentUserDto>> GetAllPhotosWithCommetsAndUsers()
        {
            List<PhotoCommentUserDto> result = await _photoService.GetAllPhotosWithComments();
            if (result == null)
            {
                return NotFound("No data found");
            }
            return Ok(result);

        }
    }
}
