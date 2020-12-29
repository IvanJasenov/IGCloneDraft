using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    public class AdminController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPhotoService _photoService;
        private readonly DataContext _dataContext;

        public AdminController(UserManager<AppUser> userManager, DataContext dataContext, IMapper mapper,
                                IUnitOfWork unitOfWork, IPhotoService photoService)
        {
            _userManager = userManager;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _photoService = photoService;
            _dataContext = dataContext;
        }


        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("users-with-roles")]
        public async Task<ActionResult> GetUsersWithRoles()
        {
            // get all the users with their roles
            var users = await _userManager.Users
                                          .Include(el => el.UserRoles)
                                          .ThenInclude(e => e.Role)
                                          .OrderBy(u => u.UserName)
                                          .Select(u => new
                                          {
                                              u.Id,
                                              Username = u.UserName,
                                              Roles = u.UserRoles.Select(r => r.Role.Name).ToList()
                                          })
                                          .ToListAsync();

            return Ok(users);
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photos-to-moderate")]
        // {{url}}/admin/photos-to-moderate
        public ActionResult GetPhotosForModeration()
        {
            return Ok("Admins and Moderators can see this");
        }

        // {{url}}/admin/edit-roles/username?roles=role1,role2,role3
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("edit-roles/{username}")]
        public async Task<ActionResult> EditRoles(string username, [FromQuery] string roles)
        {
            var selecterRoles = roles.Split(",").ToArray();

            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return NotFound("Could not found user");
            }
            var userRoles = await _userManager.GetRolesAsync(user);
            // add new roles to the user
            var result = await _userManager.AddToRolesAsync(user, selecterRoles.Except(userRoles));
            if (!result.Succeeded)
            {
                return BadRequest("Failed to add roles");
            }
            // remove the old roles
            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selecterRoles));
            if (!result.Succeeded)
            {
                return BadRequest("failed to remove old roles");
            }

            return Ok(await _userManager.GetRolesAsync(user));
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("get-all-roles")]
        public async Task<ActionResult<IEnumerable<string>>> GetAllRoles()
        {
           
            IEnumerable<string> roles = await _dataContext.Roles
                                                              .Select(el => el.Name)
                                                              .ToListAsync();

            if (roles == null)
            {
                return NotFound("No roles found");
            }
            return Ok(roles);
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("get-unapproved-photos")]
        public async Task<ActionResult<IEnumerable<PhotosToApproveDto>>> GetUnapprovedPhotos()
        {
            IEnumerable<Photo> photos = await _unitOfWork.PhotoRepository.GetUnapprovedPhotos();

            IEnumerable<PhotosToApproveDto> photosToReturn = _mapper.Map<IEnumerable<Photo>, IEnumerable<PhotosToApproveDto>>(photos);

            if (photosToReturn == null)
            {
                return BadRequest("Error in fetching photos");
            }

            return Ok(photosToReturn);
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("roles-for-user/{username}")]
        public async Task<ActionResult<IEnumerable<string>>> RolesForUser(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            var roles = await _userManager.GetRolesAsync(user);

            if (roles != null)
            {
                return Ok(roles);
            }

            return NotFound("No roles found for the user");
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPut("approve-photo/{photoId}")]
        public async Task<ActionResult<PhotosToApproveDto>> ApprovePhoto(int photoId)
        {
            Photo photoToApprove = await _unitOfWork.PhotoRepository.GetPhotoById(photoId);
            if (photoToApprove == null)
            {
                return BadRequest("No photo was found");
            }
            photoToApprove.IsApproved = true;
            if (await _unitOfWork.Complete())
            {
                PhotosToApproveDto photoDto = _mapper.Map<PhotosToApproveDto>(photoToApprove);
                return Ok(photoDto);
            }
            return BadRequest("Failed to approve photo");

        }
        // for Instagram, mapping was never done so no results are returned
        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("get-all-photos")]
        public async Task<ActionResult<IEnumerable<Photo>>> GetAllPhotos()
        {
            var photos = await _photoService.GetPhotosAndUsers();
            var photosForReturn = _mapper.Map<List<Photo>, List<PhotoWithUserDto>>(photos);
            if (photos == null)
            {
                return BadRequest("No photos found");
            }

            return Ok(photosForReturn);
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