using System.Collections.Generic;

namespace Flow.Launcher.Plugin.OpenAI
{
    public class Settings
    {
        public string ApiKey { get; set; }
        public AIProvider Provider { get; set; } = AIProvider.OpenAI;
        public string ApiBase { get; set; }
        public string ModelName { get; set; }
        public string PromptStop { get; set; } = "||";
        public Dictionary<string, string> SystemPrompts { get; set; } = new Dictionary<string, string>()
        {
            { "Default", "You are an AI assistant that helps people find information."},
            { "Short", "You are an AI assistant that helps people find information. All your answers are short, to the point and don't give any additional context." },
            { "Trans", "You are an AI assistant that helps transfer user message to English. If the user message is already in English, you help correct and polish the message." }
        };
    }
}