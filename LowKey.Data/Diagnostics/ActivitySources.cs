using System.Diagnostics;

namespace LowKey.Data.Diagnostics
{
    public static class ActivitySourceNames
    {
        internal const string SessionActivityName = "lowkey.data.session";
        public const string CommandSessionActivityName = SessionActivityName + ".command";
        public const string QuerySessionActivityName = SessionActivityName + ".query";
    }
    internal static class ActivitySources
    {
        public static readonly ActivitySource CommandSessionActivity = new ActivitySource(ActivitySourceNames.CommandSessionActivityName, typeof(ActivitySources).Assembly.GetName().Version?.ToString());
        public static readonly ActivitySource QuerySessionActivity = new ActivitySource(ActivitySourceNames.QuerySessionActivityName, typeof(ActivitySources).Assembly.GetName().Version?.ToString());
    }
}
