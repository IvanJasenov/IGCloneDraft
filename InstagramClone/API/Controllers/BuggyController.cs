using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    public class BuggyController : BaseApiController
    {
        private readonly DataContext _context;

        public BuggyController(DataContext context)
        {
            _context = context;
        }

        // api/buggy/auth
        [HttpGet("auth")]
        [Authorize]
        public ActionResult<string> GetSecret()
        {
            // this will test the 401, unauthorized responses
            return "secret text";
        }

        // api/buggy/autherror
        [HttpGet("authError")]
        public ActionResult<string> GetSecretError()
        {
            // this will test the 401, unauthorized responses
            return Unauthorized("You're not allowed");

        }
        
        // api/buggy/not-found
        [HttpGet("not-found")]
        public ActionResult<AppUser> GetNotFound()
        {
            var thing = _context.Users.Find(-1);

            if (thing == null)
            {
                return NotFound("No item found...");
            }
            return Ok(thing);
        }
        // this error throws null reference exception, so in order to handle it, we createated ExceptionMiddleware
        [HttpGet("server-error")]
        public ActionResult<string> GetServerError()
        {
            var thing = _context.Users.Find(-1);
            // null toString(), null reference exception error
            var thingToReturn = thing.ToString();

            return thingToReturn;
        }

        [HttpGet("bad-request")]
        public ActionResult<string> GetBadRequest()
        {
            return BadRequest("this was not a good request");
        }

    }
}
