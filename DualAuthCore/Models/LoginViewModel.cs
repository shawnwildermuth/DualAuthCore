using System;

namespace DualAuthCore.Controllers
{
  [Serializable]
  public class TokenRequestViewModel
  {
    public string Email { get; set; }
    public string Password { get; set; }
  }
}