using DSharpPlus.Entities;
using Mitty.Commands;
using Mitty.Osu.Api;
using System;
using System.Text.RegularExpressions;

namespace Mitty.Osu
{
    class User
    {
        private UserData user;
        private Input input;

        public DiscordEmbed Embed { get; private set; } = new DiscordEmbedBuilder();

        public User(Input input, UserData user)
        {
            this.user = user;
            this.input = input;

            if (input.IsList)
                Embed = CreateFullEmbed();
            else if (input.IsEvent)
                Embed = CreateEventEmbed();
            else
                Embed = CreateEmbed();
        }

        private DiscordEmbed CreateEmbed()
        {
            var embed = CreateBaseEmbed();

            embed.WithDescription($"**Rank:** {user.PPRank} ({user.PPCountryRank})\n" +
                $"**PP:** {user.PPRaw}\n" +
                $"**Accuracy:** {Math.Round(user.Accuracy, 2)}%\n" +
                $"**Playcount:** {user.Playcount}\n" +
                $"**Playtime:** {TimeSpan.FromSeconds(user.TotalSecondsPlayed).ToString(@"hh\:mm\:ss")}");

            return embed;
        }

        private DiscordEmbed CreateFullEmbed()
        {
            var embed = CreateBaseEmbed();

            double scoreToNextLevel = GetScoreToNextLevel();

            embed.WithDescription($"**Rank:** {user.PPRank.ToString("N0")} ({user.PPCountryRank.ToString("N0")})\n" +
                $"**PP:** {user.PPRaw.ToString("N0")}\n" +
                $"**Accuracy:** {Math.Round(user.Accuracy, 2)}%\n" +
                $"**Level:** {user.Level}\n" +
                $"**Score to next level:** {scoreToNextLevel.ToString("N0")}\n" +
                $"**Ranked Score:** {user.RankedScore.ToString("N0")}\n" +
                $"**Total Score:** {user.TotalScore.ToString("N0")}\n" +
                $"**Playcount:** {user.Playcount.ToString("N0")}\n" +
                $"**Playtime:** {TimeSpan.FromSeconds(user.TotalSecondsPlayed).ToString(@"hh\:mm\:ss")}");

            embed.AddField("**Hits**",
                $"{user.Count300.ToString("N0")} / {user.Count100.ToString("N0")} / {user.Count50.ToString("N0")}");

            embed.AddField("**Ranks**",
                $"<:XH:495911930238599169>{user.CountRankSSH.ToString("N0")} <:X_:495911948207259648>{user.CountRankSS.ToString("N0")} <:SH:495911942360137732>{user.CountRankSH.ToString("N0")} <:S_:495911953584357377>{user.CountRankS.ToString("N0")} <:A_:495911960710348820>{user.CountRankA.ToString("N0")}");

            embed.WithFooter($"Joined on {user.JoinDate}");

            return embed;
        }

        private DiscordEmbed CreateEventEmbed()
        {
            var embed = CreateBaseEmbed();

            embed.WithDescription($"");

            if (input.Page * 10 - 9 > user.Events.Length)
                throw new Exception($"No events on page {input.Page}");

            for (int i = input.Page; i < (input.Page * 10 > user.Events.Length ? user.Events.Length : input.Page * 10); i++)
            {
                string eventString = Regex.Replace(user.Events[i]["display_html"], @"(\b[A-z\s]+\b(?=\<))|(\<[^>]*\>)|(\([^>]*\))", string.Empty);

                embed.Description += "**" + (i + 1) + ":**" + char.ToUpper(eventString[0]) + eventString.Substring(1) + "\n";
            }

            return embed;
        }

        private DiscordEmbedBuilder CreateBaseEmbed()
        {
            var embed = new DiscordEmbedBuilder();

            switch (input.Gamemode)
            {
                case 0:
                    embed.WithColor(new DiscordColor("ff4dc3"));
                    break;
                case 1:
                    embed.WithColor(new DiscordColor("4dff79"));
                    break;
                case 2:
                    embed.WithColor(new DiscordColor("ff4d7f"));
                    break;
                case 3:
                    embed.WithColor(new DiscordColor("4dc1ff"));
                    break;
            }

            embed.WithAuthor($"{user.Username}'s {(Gamemodes)input.Gamemode} profile",
                $"https://osu.ppy.sh/u/{user.UserId}",
                $"https://osu.ppy.sh/images/flags/{user.Country}.png");

            embed.WithThumbnailUrl($"https://s.ppy.sh/a/{user.UserId}");

            return embed;
        }

        private double GetScoreToNextLevel()
        {
            float level = user.Level;

            if (level < 100)
                return 5000 / 3 * (Math.Pow(4 * level, 3) - Math.Pow(3 * level, 2) - level) + 1.25 * Math.Pow(1.8, level - 60);
            else
                return 26931190827 + 99999999999 * (level - 100);
        }
    }
}
