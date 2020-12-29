using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;
        private readonly IUnitOfWork _unitOfWork;

        public UsersController(IPhotoService photoService, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _photoService = photoService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // check for roles
        //[Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers([FromQuery] UserParams userParams)
        {
            var name = HttpContext.GetUsername();
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(name);
            userParams.CurrentUsername = user.UserName;
            // if there is notnig in the query params, use the gender of the loged in user
            if (string.IsNullOrEmpty(userParams.Gender))
            {              
                userParams.Gender = user.Gender;       
            }
            var users = await _unitOfWork.UserRepository.GetUsersAsync(userParams);
            if (users == null)
            {
                return NotFound("No items in dbs");
            }
            // automapper can figure it out for IEnumerable objects
            var usersDto = _mapper.Map<IEnumerable<AppUser>, IEnumerable<MemberDto>>(users.ListOfItems);
            // set the pagination header
            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

            return Ok(usersDto);
        }

        // api/users/2
        [HttpGet("{id:int}")]
        public async Task<ActionResult<MemberDto>> GetUserById(int id)
        {
            //try this here, to try
            // https://stackoverflow.com/a/45315377/2989167

            // this works!!
            // capture NameId claim with NameIdentifier ClaimType, googled this
            var name = HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var user = await _unitOfWork.UserRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound("There is no such item in the db");
            }
            var userDto = _mapper.Map<AppUser, MemberDto>(user);

            return Ok(userDto);
        }
        // api/users/ivan
        [Authorize(Roles = "Member")]
        [HttpGet("{username}", Name = "GetUser")] // you dont specify username:string, it'll raise an error
        public async Task<ActionResult<MemberDto>> GetUserByUsername(string username)
        {
            //try this here, to try
            // https://stackoverflow.com/a/45315377/2989167

            // this works!!
            // capture NameId claim with NameIdentifier ClaimType, googled this
            var name = HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);
            if (user == null)
            {
                return NotFound("There is no such item in the db");
            }
            var userDto = _mapper.Map<AppUser, MemberDto>(user);

            return Ok(userDto);
        }

        [HttpPut("update")]
        public async Task<ActionResult<MemberDto>> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            // extract the username from Bearer token send in the HttpContext header
            var username = HttpContext.GetUsername();
            var appUser = await _unitOfWork.UserRepository.GetUserByUsernameAsync(HttpContext.GetUsername());
            _mapper.Map(memberUpdateDto, appUser);
            _unitOfWork.UserRepository.Update(appUser);
            if (await _unitOfWork.Complete())
            {
                return Ok(appUser);
            }
            return BadRequest();
        }

        //{{url}}/users/add-photo
        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var username = HttpContext.GetUsername();
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);
            var result = await _photoService.AddPhotoAsync(file);

            if (result.Error != null)
            {
                return BadRequest(result.Error.Message);
            }

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };
            // if user got any photos as the moment and if not, set the first image he uploads to be the main
            if (user.Photos.Count == 0)
            {
                photo.IsMain = true;
            }
            user.Photos.Add(photo);
            
            if (await _unitOfWork.Complete())
            {
                var photoDto = _mapper.Map<Photo, PhotoDto>(photo);
                // this creates 201 code and adds "Localtion" header in the response from the API
                // 201 and Loacation is what we need to add from a method where we create a respurce
                return CreatedAtRoute("GetUser", new { username = user.UserName}, photoDto);
            }

            return BadRequest("Problem adding photo");
        }

        // {{url}}/users/set-main-photo/13
        [HttpPut("set-main-photo/{photoId}")]
        // will return the photo url from the mian photo
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var username = HttpContext.GetUsername();
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);
            var photo = user.Photos.FirstOrDefault(el => el.Id == photoId);
            if (photo.IsMain)
            {
                return BadRequest("This is already your main photo");
            }
            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
            if (currentMain != null) currentMain.IsMain = false;
            photo.IsMain = true;

            if (await _unitOfWork.Complete()) return Ok(new { photoUrl = photo.Url });

            return BadRequest("Failed to set main photo");

        }

        // this is mine helper method for when I user loges in it should fetch the main photo as well
        //{{url}}/users/get-main-photo/clare
        [HttpGet("get-main-photo/{username}")]
        public async Task<ActionResult<string>> MainPhotoUrlForUser(string username)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);
            string mainUrl = user.Photos.Find(el => el.IsMain == true).Url;

            if (mainUrl.Length > 0)
            {
                return Ok(new { main = mainUrl });
            }
            return  BadRequest("No main Photo for this user");

        }

        [HttpDelete("delete-photo/{photoId:int}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var username = HttpContext.GetUsername();
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);
            var photoDelete = user.Photos.FirstOrDefault(el => el.Id == photoId);
            if (photoDelete.IsMain)
            {
                BadRequest("You cannot delete the main photo");
            }
            // check for PublicId, because not all Photos have PublicId property
            // delete the photo from Cloudinary as well
            var result = await _photoService.DeletePhotoAsync(photoDelete.PublicId);
            if (result.Error != null)
            {
                BadRequest(result.Error.Message);
            }

            user.Photos.Remove(photoDelete);
            if (await _unitOfWork.Complete())
            {
                //MemberDto memberDto = _mapper.Map<AppUser, MemberDto>(user);
                return Ok(new { success = true });
            }
            return BadRequest("Cannot Delete image");
        }

    }
}
