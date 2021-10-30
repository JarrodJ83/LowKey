using System.Diagnostics;

namespace LowKey.Data.Diagnostics
{
    public static class ActivitySourceNames
    {
        public const string SessionActivityName = "lowkey.data.session";
    }
    internal static class ActivitySources
    {
        
        public static readonly ActivitySource SessionActivity = new ActivitySource(ActivitySourceNames.SessionActivityName, typeof(ActivitySources).Assembly.GetName().Version.ToString());
    }
}
