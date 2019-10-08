using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DualAuthCore.Models;
using Microsoft.AspNetCore.Identity;

namespace DualAuthCore.Data
{
  public class DataSeeder
  {
    private readonly DualAuthContext _ctx;
    private readonly UserManager<IdentityUser> _userManager;

    public DataSeeder(DualAuthContext ctx, UserManager<IdentityUser> userManager)
    {
      _ctx = ctx;
      _userManager = userManager;
    }

    public async Task SeedAsync()
    {
      _ctx.Database.EnsureCreated();

      if (!_ctx.Users.Any())
      {

        var user = new IdentityUser()
        {
          Email = "bob@aol.com",
          UserName = "bob@aol.com"
        };

        var result = await _userManager.CreateAsync(user, "P@ssw0rd!");
        if (result.Succeeded)
        {
          user.EmailConfirmed = true;
          await _userManager.UpdateAsync(user);
        }
      }
    }
  }
}
