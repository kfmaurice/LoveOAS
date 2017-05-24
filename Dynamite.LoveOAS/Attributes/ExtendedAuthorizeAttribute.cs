using System.Web.Http;
using System.Web.Http.Controllers;

namespace Dynamite.LoveOAS.Attributes
{
  public class ExtendedAuthorizeAttribute: AuthorizeAttribute
  {
    /// <summary>
    /// Check whether user is authorized
    /// </summary>
    /// <param name="actionContext">Context</param>
    /// <returns></returns>
    public virtual bool ForwardIsAuthorized(HttpActionContext actionContext)
    {
      return IsAuthorized(actionContext);
    }

  }
}
