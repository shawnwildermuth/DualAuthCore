using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using DualAuthCore.Services;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace DualAuthCore.Controllers
{
  [Authorize]
  [Route("[controller]/[action]")]
  public class TokensController : Controller
  {
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly ILogger _logger;
    private readonly IConfiguration _config;

    public TokensController(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        ILogger<TokensController> logger,
        IConfiguration config)
    {
      _userManager = userManager;
      _signInManager = signInManager;
      _logger = logger;
      _config = config;
    }

    [TempData]
    public string ErrorMessage { get; set; }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> GenerateToken([FromBody] TokenRequestViewModel model)
    {

      if (true) //ModelState.IsValid)
      {
        model = new TokenRequestViewModel()
        {
          Email = "bob@aol.com",
          Password = "P@ssw0rd!"
        };
        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user != null)
        {
          var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
          if (result.Succeeded)
          {

            var claims = new[]
            {
              new Claim(JwtRegisteredClaimNames.Sub, user.Email),
              new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Tokens:Issuer"],
              _config["Tokens:Issuer"],
              claims,
              expires: DateTime.Now.AddDays(30),
              signingCredentials: creds);

            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
          }
        }
      }

      return BadRequest("Could not create token");
    }


  }
}
