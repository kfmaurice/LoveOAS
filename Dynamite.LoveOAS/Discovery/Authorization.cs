using System.Linq;
using System.Web.Http.Controllers;

using Dynamite.LoveOAS.Attributes;

namespace Dynamite.LoveOAS.Discovery
{
  public class Authorization : IAuthorization
  {
    private HttpActionContext actionContext;
    public ISettings Settings { get; set; }


    public Authorization(ISettings settings, HttpActionContext actionContext)
    {
      Settings = settings;
      this.actionContext = actionContext;
    }

    #region IAuthorization interface implementation
    /// <summary>
    /// Check whether user is authorized
    /// </summary>
    /// <param name="attributes">Metadata on authorize attributes</param>
    /// <returns></returns>
    public bool IsAuthorized(AuthorizeAttributes attributes)
    {
      if (!Settings.CheckAuthorization || attributes.AllowAnonymous != null || attributes.ClassAllowAnonymous != null)
      {
        return true;
      }
      else
      {
        if (attributes.MethodAuthorize.Count() > 0)
        {
          return attributes.MethodAuthorize.Select(x => x.ForwardIsAuthorized(actionContext)).All(x => x == true);
        }
        else if (attributes.ClassAuthorize.Count() > 0)
        {
          return attributes.ClassAuthorize.Select(x => x.ForwardIsAuthorized(actionContext)).All(x => x == true);
        }
        else
        {
          return true;
        }
      }
    } 
    #endregion
  }
}
