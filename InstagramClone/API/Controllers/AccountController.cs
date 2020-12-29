using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
                                  ITokenService tokenService, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _mapper = mapper;
        }
        // api/register/object from the register
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            // if username does not exists, register the user
            if (await UserExists(registerDto.Username) == false)
            {
                // create a new AppUser from the mapper
                var user = _mapper.Map<RegisterDto, AppUser>(registerDto);
               
                user.UserName = registerDto.Username.ToLower();

                var result = await _userManager.CreateAsync(user, registerDto.Password);
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }
                // any new registered user will be put into the Member role
                var roleResult = await _userManager.AddToRoleAsync(user, "Member");
                if (!roleResult.Succeeded)
                {
                    return BadRequest(roleResult.Errors);
                }

                var userDto = new UserDto
                {
                    Username = user.UserName,
                    Token = await _tokenService.CreateToken(user),
                    KnownAs = user.KnownAs,
                    Gender = user.Gender
                };

                return Ok(userDto);
            }
            // if the username is already taken
            return BadRequest($"Username, '{registerDto.Username}' is taken");
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userManager.Users
                                          .Include(el => el.Photos)
                                          .SingleOrDefaultAsync(el => el.UserName == loginDto.Username.ToLower());

            if (user == null)
            {
                return Unauthorized("Invalid username");
            }

            // using SigninManager to signin the user
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded)
            {
                return Unauthorized();
;           }
            
            var userDto = new UserDto
            {
                Username = user.UserName,
                Token = await _tokenService.CreateToken(user),
                PhotoUrl = await MainPhotoUrlForUser(user.UserName),
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
            return Ok(userDto);

        }


        private async Task<bool> UserExists(string username)
        {
            return await _userManager.Users.AnyAsync(el => el.UserName == username.ToLower());
        }

        // i added this heper method
        private async Task<string> MainPhotoUrlForUser(string username)
        {
            //var user = await _userRepository.GetUserByUsernameAsync(username);
            var user = await _userManager.Users.FirstOrDefaultAsync(el => el.UserName == username);
            string mainUrl = user.Photos.FirstOrDefault(el => el.IsMain == true)?.Url;

            if (mainUrl != null)
            {
                return mainUrl;
            }
            return "";
        }

    }
}
