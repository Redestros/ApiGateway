using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{

    [Authorize]
    [HttpGet("/login")]
    public IActionResult Login()
    {
        return RedirectToAction(nameof(GetInfo));
    }
    
    [HttpGet("/public")]
    public IActionResult Public()
    {
        return Ok("Welcome to API Gateway");
    }
    
    [HttpGet("/logout")]
    public async Task Logout()
    {
        var prop = new AuthenticationProperties
        {
            RedirectUri = "/public"
        };
        await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme, prop);

        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }

    [HttpGet("/info")]
    public IActionResult GetInfo()
    {
        var claims = HttpContext.User.Claims.Select(x => new { x.Type, x.Value}).ToList();
        return Ok(claims);
    }
}