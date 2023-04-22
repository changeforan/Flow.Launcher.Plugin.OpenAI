namespace Flow.Launcher.Plugin.OpenAI
{
    public class Settings
    {
        public string ApiKey { get; set; }
        public AIProvider Provider { get; set; } = AIProvider.OpenAI;
        public string ApiBase { get; set; }
    }

    public enum AIProvider
    {
        OpenAI,
        Azure
    }
}