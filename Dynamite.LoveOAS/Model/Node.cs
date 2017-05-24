using System.Collections.Generic;

namespace Dynamite.LoveOAS.Model
{
  public class Node
  {
    public Node()
    {
      Exits = new List<Node>();
    }

    /// <summary>
    /// Endpoint unique identifier
    /// </summary>
    public string Key { get; set; }

    public string Relation { get; set; }

    /// <summary>
    /// Whether endpoint has no predecessor
    /// </summary>
    public bool IsIsolated { get; set; }

    /// <summary>
    /// Endpoint metadata obtained through reflection
    /// </summary>
    public Endpoint Metadata { get; set; }

    /// <summary>
    /// Endpoint exits
    /// </summary>
    public List<Node> Exits { get; set; }
  }
}
