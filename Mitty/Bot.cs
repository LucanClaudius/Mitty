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
using System.Linq;
using DSharpPlus.Entities;

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
            Client.MessageDeleted += MessageLogger;
            Client.MessageUpdated += MessageLogger;

            Client.UseInteractivity(new InteractivityConfiguration { });

            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new string[] { configJson["Prefix"] },
                CaseSensitive = false,
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
            System.Console.WriteLine("yuh");
            return Task.CompletedTask;
        }

        private Task OnCommandError(CommandErrorEventArgs e)
        {
            e.Context.Channel.SendMessageAsync(e.Exception.Message);
            return Task.CompletedTask;
        }

        private Task MessageLogger(MessageDeleteEventArgs e)
        {
            var chn = e.Guild.Channels[e.Guild.Channels.FirstOrDefault(x => x.Value.Name == "message-log").Key];

            if (chn != null)
            {
                var embed = CreateLogEmbed(e.Message).Result;
                embed.WithDescription($"🗑 **Message deleted in {e.Message.Channel.Mention}**");
            }

            return Task.CompletedTask;
        }

        private Task MessageLogger(MessageUpdateEventArgs e)
        {
            var chn = e.Guild.Channels[e.Guild.Channels.FirstOrDefault(x => x.Value.Name == "message-log").Key];

            if (chn != null)
            {
                var embed = CreateLogEmbed(e.Message).Result;
                embed.WithDescription($"🗑 **Message edited in {e.Message.Channel.Mention}**");
                embed.AddField("New message:", $"{e.Message.JumpLink}");
            }

            return Task.CompletedTask;
        }

        private Task<DiscordEmbedBuilder> CreateLogEmbed(DiscordMessage message)
        {
            var embed = new DiscordEmbedBuilder();

            if (message.Content.Length > 0)
                embed.AddField("Old content:", message.Content);

            if (message.Attachments.Count > 0)
            {
                foreach (DiscordAttachment attachment in message.Attachments)
                {
                    embed.AddField("OAttachment:", attachment.ProxyUrl);
                }
            }

            embed.WithFooter($"Original message sent at {message.CreationTimestamp}, edited at {message.EditedTimestamp}");

            return Task.FromResult(embed);
        }
    }
}
