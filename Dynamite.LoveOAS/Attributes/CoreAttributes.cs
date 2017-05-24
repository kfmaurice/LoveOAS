using System.Collections.Generic;

using Dynamite.LoveOAS.Model;

namespace Dynamite.LoveOAS.Attributes
{
  /// <summary>
  /// Class to contain endpoint information
  /// </summary>
  public class CoreAttributes
  {
    public RouteAttributes Routes { get; set; }
    public IEnumerable<HttpVerbProxy> Verbs { get; set; }
    public AuthorizeAttributes Authorize { get; set; }
  }
}
