using System.Collections.Generic;

namespace Flow.Launcher.Plugin.OpenAI
{
    public class Settings
    {
        public string ApiKey { get; set; }
        public AIProvider Provider { get; set; } = AIProvider.OpenAI;
        public string ApiBase { get; set; }
        public string ModelName { get; set; }
        public string PromptStop { get; set; } = "##";
        public Dictionary<string, string> SystemPrompts { get; set; }
    }
}