using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Net7.Dtos.User;

namespace Net7.Controllers
{
    // AuthController
    [ApiController] // This is an API controller
    [Route("[controller]")] // This is the route
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepo;
        public AuthController(IAuthRepository authRepo)
        {
           _authRepo = authRepo;
            
        }
        // Register
        [HttpPost("Register")]
        public async Task<ActionResult<ServiceResponse<int>>> Register(UserRegisterDtos request)
        {
            var response = await _authRepo.Register(
                new User { Username = request.Username }, request.Password);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        // Login
        [HttpPost("Login")]
        public async Task<ActionResult<ServiceResponse<int>>> Login(UserLoginDtos request)
        {
            var response = await _authRepo.Login(request.Username, request.Password);
               
            if (!response.Success)
            {
                return BadRequest(response);
            } 
            return Ok(response);
        }
    }
}