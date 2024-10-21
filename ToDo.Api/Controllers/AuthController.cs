using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using ToDo.Api.DTOs;

namespace ToDo.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;

        public AuthController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        /// <summary>
        /// Login existing user
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        /// <response code="400">Model validation error</response>
        /// <response code="500">System error</response>
        [HttpGet]
        [Produces(MediaTypeNames.Text.Plain)]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserResponse>> Login(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                return NotFound(new ErrorResponse("User not found"));
            }

            if (await _userManager.CheckPasswordAsync(user, password))
            {
                return new UserResponse(user);
            }
            else
            {
                return BadRequest(new ErrorResponse("Wrong password"));
            }
        }

        /// <summary>
        /// Register new user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <respomse code="400">Model validation error</respomse>
        /// <response code="500">System error</response>
        [HttpPost]
        [Produces(MediaTypeNames.Application.Json)]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Register(UserForRegistration user)
        {
            if (await _userManager.FindByNameAsync(user.UserName) != null)
            {
                return BadRequest(new ErrorResponse($"User already exists with user name {user.UserName}"));
            }

            if (await _userManager.FindByEmailAsync(user.Email) != null)
            {
                return BadRequest(new ErrorResponse($"User already exists with email {user.Email}"));
            }

            var userIdentity = new IdentityUser(user.UserName);
            userIdentity.Email = user.Email;

            var rez = await _userManager.CreateAsync(userIdentity, user.Password);

            if (!rez.Succeeded)
            {
                return BadRequest(new ErrorResponse(string.Join(';', rez.Errors)));
            }
            else
            {
                return Ok();
            }
        }
    }
}
