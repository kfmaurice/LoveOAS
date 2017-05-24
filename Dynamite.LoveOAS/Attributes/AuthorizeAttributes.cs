using System.Collections.Generic;
using System.Web.Http;

namespace Dynamite.LoveOAS.Attributes
{
  public class AuthorizeAttributes
  {
    public AuthorizeAttributes()
    {
      MethodAuthorize = new List<ExtendedAuthorizeAttribute>();
      ClassAuthorize = new List<ExtendedAuthorizeAttribute>();
    }

    public AllowAnonymousAttribute AllowAnonymous { get; set; }
    public IEnumerable<ExtendedAuthorizeAttribute> MethodAuthorize { get; set; }
    public AllowAnonymousAttribute ClassAllowAnonymous { get; set; }
    public IEnumerable<ExtendedAuthorizeAttribute> ClassAuthorize { get; set; }
  }
}
