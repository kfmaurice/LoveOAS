using Microsoft.AspNetCore.Mvc;

namespace Dynamite.LoveOAS.NetCore.Filters
{
  public class LoveOASFilterAttribute : TypeFilterAttribute
  {
    public LoveOASFilterAttribute() : base(typeof(LoveOASFilterAttributeImplementation))
    {
    }
  }
}
