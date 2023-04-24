using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Windows;
using Azure;
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
            // load settings from json file
            var settingsPath = Path.Combine(context.CurrentPluginMetadata.PluginDirectory, "settings.json");
            if (File.Exists(settingsPath))
            {
                _settings = JsonSerializer.Deserialize<Settings>(File.ReadAllText(settingsPath));
            }
            else
            {
                _settings = new Settings();
                File.WriteAllText(settingsPath, JsonSerializer.Serialize(_settings));
            }
            InitClient();
        }

        public List<Result> Query(Query query)
        {
            var search = query.Search.Trim();
            var assistant = GetActiveAssistant(search);
            if (!CanGenerateResponse(query))
            {
                
                var results = new List<Result>() {
                    new Result()
                    {
                        Title = $"TAB to start the query",
                        SubTitle = $"Debug info: Active assisant {assistant}, query {search}, active key {query.ActionKeyword}",
                        AutoCompleteText = $"{query.ActionKeyword} {search} {_settings.PromptStop}",
                        IcoPath = "Images/app.png"
                    }
                };
                return results;
            }

            if (query.Search.StartsWith(assistant, StringComparison.OrdinalIgnoreCase))
            {
                search = search.Substring(assistant.Length).Trim();
            }

            var response = GenerateResponse(_settings.SystemPrompts[assistant], search);

            var result = new Result
            {
                Title = response,
                CopyText = response,
                SubTitle = "Click to copy",
                Action = c =>
                {
                    try
                    {
                        Clipboard.SetDataObject(response);
                        return true;
                    }
                    catch (ExternalException)
                    {
                        MessageBox.Show("Copy failed, please try later");
                        return false;
                    }
                },
                IcoPath = "Images/app.png"
            };
            return new List<Result> { result };
        }

        private bool CanGenerateResponse(Query query)
        {
            return query.Search.Length > 0 && query.Search.EndsWith(_settings.PromptStop);
        }

        private string GenerateResponse(string systemPrompt, string userPrompt)
        {
            var chatCompletionsOptions = new ChatCompletionsOptions()
            {
                Messages =
                {
                    new ChatMessage(ChatRole.System, systemPrompt),
                    new ChatMessage(ChatRole.User, userPrompt),
                }
            };
            var response = _client.GetChatCompletions(
                _settings.ModelName,
                chatCompletionsOptions
                );
            return response.Value.Choices[0].Message.Content;
        }

        private string GetActiveAssistant(string userInput)
        {
            var name = userInput.Split(' ')[0];
            foreach (var key in _settings.SystemPrompts.Keys)
            {
                if (string.Equals(key, name, StringComparison.OrdinalIgnoreCase))
                {
                    return key;
                }
            }
            return _settings.SystemPrompts.Keys.First();
        }

        // Create and init an OpenAIClient
        private void InitClient()
        {
            if (_client == null)
            {
                _client = _settings.Provider switch
                {
                    AIProvider.Azure => new OpenAIClient(new Uri(_settings.ApiBase), new AzureKeyCredential(_settings.ApiKey)),
                    AIProvider.OpenAI => new OpenAIClient(_settings.ApiKey),
                    _ => throw new NotImplementedException()
                };
            }
        }
    }
}