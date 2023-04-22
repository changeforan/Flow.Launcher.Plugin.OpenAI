using System;
using System.Collections.Generic;
using Azure.AI.OpenAI;
using Flow.Launcher.Plugin;

namespace Flow.Launcher.Plugin.OpenAI
{
    public class OpenAI : IPlugin
    {
        private PluginInitContext _context;
        private OpenAIClient _client;
        private Settings _settings;

        public void Init(PluginInitContext context)
        {
            _context = context;
            _settings = _context.API.LoadSettingJsonStorage<Settings>();
        }

        public List<Result> Query(Query query)
        {
            return new List<Result>();
        }
    }
}