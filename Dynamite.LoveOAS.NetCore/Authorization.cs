using Dynamite.LoveOAS.Attributes;

namespace Dynamite.LoveOAS.NetCore
{
  public class Authorization : IAuthorization
  {
    public ISettings Settings { get; set; }


    public Authorization(ISettings settings)
    {
      Settings = settings;
    }

    #region IAuthorization interface implemetion
    public bool IsAuthorized(AuthorizeAttributes attributes)
    {
      return true;
    } 
    #endregion
  }
}
