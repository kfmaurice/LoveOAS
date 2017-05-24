using System.Collections.Generic;

namespace Dynamite.LoveOAS.Model
{
  public class Output
  {
    /// <summary>
    /// Endpoint result
    /// </summary>
    public object Payload { get; set; }

    /// <summary>
    /// Endpoint links
    /// </summary>
    public IEnumerable<Link> Links { get; set; }
  }
}
