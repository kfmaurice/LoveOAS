namespace Dynamite.LoveOAS.Discovery
{
  public class Settings : ISettings
  {
    public ModeEnum Mode { get; set; }
    public bool CheckAuthorization { get; set; }
    public bool UseAbsoluteUrl { get; set; }
    public string AbsoluteBaseUrl { get; set; }
    public bool AllowOrphans { get; set; }
    public bool TreatCollectionAsPayload { get; set; }
    public bool HandleOnlyMarkedApis { get; set; }
  }
}
