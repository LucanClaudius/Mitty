using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using Mitty.Commands;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.VoiceNext;

namespace Mitty
{
    public class Bot
    {
        public static Dictionary<string, string> configJson;
        public DiscordClient Client { get; set; }
        public InteractivityExtension Interactivity { get; set; }
        public CommandsNextExtension Commands { get; set; }
        public VoiceNextExtension Voice { get; set; }

        public async Task RunAsync()
        {
            var json = string.Empty;
            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync().ConfigureAwait(false);

            configJson = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

            var config = new DiscordConfiguration
            {
                Token = configJson["Token"],
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                LogLevel = LogLevel.Debug,
                UseInternalLogHandler = true
            };

            Client = new DiscordClient(config);
            Client.Ready += OnClientReady;

            Client.UseInteractivity(new InteractivityConfiguration { });

            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new string[] { configJson["Prefix"] },
                EnableDms = false,
                EnableMentionPrefix = true,
                EnableDefaultHelp = false,
                IgnoreExtraArguments = true
            };

            Commands = Client.UseCommandsNext(commandsConfig);

            Commands.RegisterCommands<ModerationCommands>();
            Commands.RegisterCommands<OsuCommands>();
            Commands.RegisterCommands<UtilityCommands>();
            Commands.RegisterCommands<ImageCommands>();
            Commands.RegisterCommands<VoiceCommands>();

            Commands.CommandErrored += OnCommandError;

            Voice = Client.UseVoiceNext();

            await Client.ConnectAsync();
            await Task.Delay(-1);
        }

        private Task OnClientReady(ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }

        private Task OnCommandError(CommandErrorEventArgs e)
        {
            e.Context.Channel.SendMessageAsync(e.Exception.Message);
            return Task.CompletedTask;
        }
    }
}
