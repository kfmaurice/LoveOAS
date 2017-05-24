using System;

namespace Dynamite.LoveOAS.Attributes
{
  /// <summary>
  /// Attribute to mark methods which should return start packet
  /// </summary>
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
  public class BaseAttribute : Attribute
  {
  }
}
