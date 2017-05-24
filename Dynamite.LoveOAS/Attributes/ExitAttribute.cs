using System;

namespace Dynamite.LoveOAS.Attributes
{
  /// <summary>
  /// Attribute to enable non first entry endpoint
  /// </summary>
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
  public class ExitAttribute : Attribute
  {
    public virtual string Method { get; private set; }
    public virtual Type Parent { get; private set; }
    public virtual Type[] Parameters { get; private set; }
    public virtual string Relation { get; private set; }

    public ExitAttribute(string Method, Type Parent, string Relation, Type[] Parameters = null)
    {
      this.Method = Method;
      this.Parent = Parent;
      this.Relation = Relation;
      this.Parameters = Parameters;
    }
  }
}
