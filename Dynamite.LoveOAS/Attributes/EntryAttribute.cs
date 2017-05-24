using System;

namespace Dynamite.LoveOAS.Attributes
{
  /// <summary>
  /// Attribute to enable first entry endpoint
  /// </summary>
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
  public class EntryAttribute: Attribute
  {
     public virtual string Relation { get; private set; }

    public EntryAttribute(string Relation)
    {
      this.Relation = Relation;
    }
  }
}
