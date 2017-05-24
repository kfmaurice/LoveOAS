using System.Linq;
using System.Collections.Generic;

using Dynamite.LoveOAS.Attributes;

namespace Dynamite.LoveOAS.Model
{
  public class Endpoint
  {
    public bool IsEntry
    {
      get
      {
        return Entry != null;
      }
    }

    public bool IsExit
    {
      get
      {
        return Exits != null && Exits.Count() > 0;
      }
    }

    public bool IsLeaf
    {
      get
      {
        return Exits != null && Exits.Count() == 0;
      }
    }

    public bool IsBase
    {
      get
      {
        return Base != null;
      }
    }

    public CoreAttributes Core { get; set; }
    public BaseAttribute Base { get; set; }
    public EntryAttribute Entry { get; set; }
    public IEnumerable<ExitAttribute> Exits { get; set; }
  }
}
