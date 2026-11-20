using GymApi.Dtos;
using GymApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace GymApi.Controllers;
[ApiController]
[Route("api/v1/auth")]
public class AuthController(IMemberService members, IAuthService auth) : ApiControllerBase
{
    [AllowAnonymous][HttpPost("register")] public Task<IActionResult> Register(RegisterDto d) => Execute(async () => { var m = await members.RegisterAsync(d); return StatusCode(201, new { message = "Account created. Your membership is active and you can log in now.", member = m }); });
    [AllowAnonymous][HttpPost("login")] public Task<IActionResult> Login(LoginDto d) => Execute(async () => { var a = await auth.LoginAsync(d); return a == null ? Unauthorized(new { error = "Invalid username or password." }) : Ok(a); });
}
