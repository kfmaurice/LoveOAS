namespace Dynamite.LoveOAS
{
  public enum ModeEnum
  {
    /// <summary>
    /// Default mode => network is built once the web application starts and cached for later retrieval
    /// </summary>
    Boot = 0,

    /// <summary>
    /// The links are newly retrieved on each request (not supported yet)
    /// </summary>
    Runtime = 2
  }
}
