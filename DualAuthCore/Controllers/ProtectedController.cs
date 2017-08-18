using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DualAuthCore.Controllers
{
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  [Route("/api/customers")]
  public class ProtectedController : Controller
  {
    public ProtectedController()
    {

    }

    public IActionResult Get()
    {
      return Ok(new[] { "One", "Two", "Three" });
    }

  }
}
