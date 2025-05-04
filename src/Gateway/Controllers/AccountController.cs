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
    
    [HttpGet("public")]
    public IActionResult Public()
    {
        return Ok("Welcome to API Gateway");
    }

    [AllowAnonymous]
    [HttpGet("login")]
    public IActionResult Login()
    {
        // Challenge triggers the OIDC redirect flow
        return Challenge(new AuthenticationProperties
        {
            RedirectUri = "/account/info" // or wherever you want to land after login
        }, OpenIdConnectDefaults.AuthenticationScheme);
    }
    
    [Authorize]
    [HttpGet("info")]
    public IActionResult GetInfo()
    {
        var claims = HttpContext.User.Claims
            .Select(x => new { x.Type, x.Value })
            .ToList();

        return Ok(claims);
    }
    
    [Authorize]
    [HttpGet("logout")]
    public async Task Logout()
    {
        var prop = new AuthenticationProperties
        {
            RedirectUri = "/account/public"
        };
        await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme, prop);

        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }
}