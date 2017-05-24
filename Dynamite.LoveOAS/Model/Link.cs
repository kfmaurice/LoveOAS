namespace Dynamite.LoveOAS.Model
{
  public class Link
  {
    /// <summary>
    /// Endpoint route
    /// </summary>
    public string Href { get; set; }

    /// <summary>
    /// Endpoint relation to the link container
    /// </summary>
    public string Rel { get; set; }

    /// <summary>
    /// HTTP verb
    /// </summary>
    public string Method { get; set; }

    public Link()
    {

    }

    /// <summary>
    /// Copy contructor
    /// </summary>
    /// <param name="link">Link to copy from</param>
    public Link(Link link)
    {
      Href = link.Href;
      Rel = link.Rel;
      Method = link.Method;
    }
  }
}
