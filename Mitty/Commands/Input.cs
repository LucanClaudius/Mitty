using DSharpPlus.Entities;
using Mitty.Osu;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Mitty.Commands
{
    class Input
    {
        private Regex beatmapLinkRegex = new Regex(@"o(?:su|ld)\.ppy\.sh\/\D+\/((?<beatmapsetid>(?<=beatmapsets\/)\d+(?=$|\#))|(?<=b\/|beatmaps\/))(?:\#\D+)?(?<beatmapid>\d+)?");
        private bool osuNameDefined = false;

        public ulong DiscordUserId { get; private set; }
        public string DiscordUserName { get; private set; }
        public ulong DiscordChannelId { get; private set; }
        public string AnyString { get; private set; }
        public string BeatmapId { get; private set; }
        public string BeatmapSetId { get; private set; }
        public string Command { get; set; }
        public char[] CommandModifiers { get; private set; }
        public int ModNumber { get; private set; }
        public int? ModRequestNumber { get; private set; } = null;
        public int Page { get; private set; } = 1;
        public int Gamemode { get; private set; } = 0;
        public bool IsList { get; private set; } = false;
        public bool IsPassOnly { get; private set; } = false;
        public bool IsRecentSorting { get; private set; } = false;
        public bool IsEvent { get; private set; } = false;
        public bool IsDeltaT { get; private set; } = false;

        public Input(DiscordMessage message)
        {
            DiscordUserId = message.Author.Id;
            DiscordUserName = message.Author.Username;
            DiscordChannelId = message.ChannelId;

            string messageContent = message.Content;
            var anyStringList = new List<string>();

            if (messageContent.Split('"', '"').Length >= 2)
            {
                AnyString = messageContent.Split('"', '"')[1];
                messageContent = messageContent.Replace($"\"{AnyString}\"", "").Trim();
                osuNameDefined = true;
            }

            if (messageContent.Contains("/deltat"))
            {
                IsDeltaT = true;
                messageContent = messageContent.Replace("/deltat", "").Trim();
            }

            Command = messageContent.Split(" ")[0].Remove(0, 1);
            messageContent = messageContent.Replace($"{Bot.configJson["Prefix"]}{Command}", "").Trim();

            if (Command.Equals("compare") || Command.Equals("c"))
                BeatmapId = Database.GetLastMapId(message.ChannelId).Result;

            string[] messageList = messageContent.Split(" ");

            for (int i = 0; i < messageList.Length; i++)
            {
                if (int.TryParse(messageList[i], out int page))
                {
                    Page = page;
                }
                else if (messageList[i].StartsWith("="))
                {
                    char[] commandModifiers = messageList[i].Remove(0, 1).ToCharArray();
                    CommandModifiers = commandModifiers;

                    foreach (char commandModifier in commandModifiers)
                    {
                        switch (commandModifier)
                        {
                            case 'l':
                                IsList = true;
                                break;
                            case 'p':
                                IsPassOnly = true;
                                break;
                            case 'r':
                                IsRecentSorting = true;
                                break;
                            case 'e':
                                IsEvent = true;
                                break;
                            default:
                                if (Enum.IsDefined(typeof(GamemodesShort), commandModifier.ToString()))
                                    Gamemode = (int)Enum.Parse(typeof(GamemodesShort), commandModifier.ToString());
                                break;
                        }
                    }
                }
                else if (messageList[i].StartsWith("+"))
                {
                    ModRequestNumber = 0;

                    for (int j = 1; j < messageList[i].Length; j += 2)
                    {
                        string mod = messageList[i].Substring(j, 2);

                        if (Enum.IsDefined(typeof(ModsShort), mod))
                        {
                            var modShort = (ModsShort)Enum.Parse(typeof(ModsShort), mod);

                            ModNumber += (int)modShort;
                            ModRequestNumber += ModsShort.StarChangeMods.HasFlag(modShort) ? (int)modShort : 0;
                        }
                    }
                }
                else if (BeatmapId == null && beatmapLinkRegex.IsMatch(messageList[i]))
                {
                    Match match = beatmapLinkRegex.Match(messageList[i]);
                    Group beatmapGroup = match.Groups["beatmapid"];
                    Group beatmapSetGroup = match.Groups["beatmapsetid"];

                    BeatmapId = beatmapGroup.Success ? beatmapGroup.Value : null;
                    BeatmapSetId = beatmapSetGroup.Success ? beatmapSetGroup.Value : null;
                }
                else if (!osuNameDefined)
                {
                    anyStringList.Add(messageList[i]);
                }
            }

            if (!osuNameDefined && anyStringList.Count > 0)
                AnyString = string.Join(" ", anyStringList);

            if (Command.Equals("top") && (Page > 100 || (IsList && Page > 20)))
                throw new Exception("Cannot find more than 100 scores");
            else if (!Command.Equals("top") && (Page > 50 || (IsList && Page > 10)))
                throw new Exception("Cannot find more than 50 scores");
        }
    }
}
