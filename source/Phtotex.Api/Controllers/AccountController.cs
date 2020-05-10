using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Photex.Core.Contracts.Requests;
using Photex.Core.Interfaces;
using Phtotex.Api.Extensions;

namespace Phtotex.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(
            IAccountService accountService)
        {
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(
            [FromBody]RegistrationRequest request)
        {
            try
            {
                var model = await _accountService.Register(request);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody]LoginRequest request)
        {
            var claimsIdentity = await _accountService.Authenticate(request);
            if (claimsIdentity != null)
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("KeyPhotexKeyPhotexKeyPhotexKeyPhotexKeyPhotexKeyPhotex"));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                                            issuer: "IssuerPhotex",
                                            audience: "IssuerPhotex",
                                            claims: claimsIdentity.Claims,
                                            expires: DateTime.Now.AddMinutes(30),
                                            signingCredentials: creds);

                var handler = new JwtSecurityTokenHandler();
                return Ok(new
                {
                    token = handler.WriteToken(token)
                });
            }

            return BadRequest("Unknown user");
        }

        [HttpGet("user")]
        [Authorize]
        public async Task<IActionResult> GetUser()
        {
            var userId = HttpContext.GetUserId();
            if (!userId.HasValue)
            {
                return BadRequest(new
                {
                    message = "Invalid cookie"
                });
            }

            return Ok(await _accountService.GetUserData(userId.Value));
        }
    }
}