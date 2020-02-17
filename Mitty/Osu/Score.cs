using DSharpPlus.Entities;
using Mitty.Commands;
using Mitty.Osu.Api;
using Mitty.Osu.Calculators;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mitty.Osu
{
    class Score
    {
        private ScoreData score;
        private ScoreData[] scoreList;
        private ScoreData[] leaderboard;
        private UserData user;
        private BeatmapData beatmap;
        private Input input;

        private delegate Dictionary<string, string> GetModeDelegate(ScoreData score, BeatmapData beatmap);
        GetModeDelegate GetModeData;

        public DiscordEmbed Embed { get; private set; } = new DiscordEmbedBuilder();

        public Score(Input input, ScoreData[] scoreList)
        {
            this.input = input;
            this.scoreList = scoreList;

            switch (input.Gamemode)
            {
                case 0:
                    this.GetModeData = GetOsuData;
                    break;
                case 1:
                    this.GetModeData = GetTaikoData;
                    break;
                case 2:
                    this.GetModeData = GetCatchData;
                    break;
                case 3:
                    this.GetModeData = GetManiaData;
                    break;
            }

            if (!input.IsList)
            {
                if (scoreList.Length < input.Page)
                    throw new Exception($"No score found at position {input.Page}");

                this.score = scoreList[input.Page - 1];
                this.Embed = CreateScoreEmbed().Result;
            }
            else
            {
                if (scoreList.Length < input.Page * 5 - 4)
                    throw new Exception($"No score found on page {input.Page}");

                this.Embed = CreateListEmbed();
            }
        }

        public async Task<DiscordEmbedBuilder> CreateScoreEmbed()
        {
            int requestnumber = OsuHelper.GetModRequestNumber(score.EnabledMods);

            Task<UserData> userRequest = OsuApi.User(input);
            Task<BeatmapData[]> beatmapRequest = OsuApi.Beatmap(score.BeatmapId ?? input.BeatmapId, requestnumber, input.Gamemode);
            Task<ScoreData[]> leaderboardRequest = OsuApi.Leaderboard(score.BeatmapId, input.Gamemode);

            Task.WhenAll(
                userRequest,
                beatmapRequest,
                leaderboardRequest
            ).Wait();

            user = userRequest.Result;
            beatmap = beatmapRequest.Result[0];
            leaderboard = leaderboardRequest.Result;

            DiscordEmbedBuilder embed = CreateBaseEmbed();
            Dictionary<string, string> data = GetModeData(score, beatmap);
            string emoji = GetRankEmoji(score);
            string mods = OsuHelper.ParseMods(score.EnabledMods);

            ppData pp = new PerformanceCalculator(score, beatmap, input.Gamemode).Calculate();
            string achievedPp;

            if (score.Rank.Equals("F") || beatmap.Approved == 4)
                achievedPp = $"~~{Math.Round(score.PP > 0 && !input.IsDeltaT ? score.PP : pp.AchievedPp)}pp~~";
            else
                achievedPp = Math.Round(score.PP > 0 && !input.IsDeltaT ? score.PP : pp.AchievedPp).ToString() + "pp";

            var totalTimeSpan = TimeSpan.FromSeconds(beatmap.TotalLength);
            var hitTimeSpan = TimeSpan.FromSeconds(beatmap.HitLength);
            string totalLength = string.Format("{0:D2}:{1:D2}", totalTimeSpan.Minutes, totalTimeSpan.Seconds);
            string hitLength = string.Format("{0:D2}:{1:D2}", hitTimeSpan.Minutes, hitTimeSpan.Seconds);

            int retryCount = GetRetryCount();
            if (retryCount > 0)
                embed.Author.Name += $" | Try #{retryCount}";

            embed.WithDescription($"[{beatmap.Artist} - {beatmap.Title} [{beatmap.Version}]](https://osu.ppy.sh/b/{beatmap.BeatmapId})\n" +
                $"{mods} **{achievedPp}**");

            int leaderboardPosition = GetLeaderboardPosition();
            if (leaderboardPosition > 0)
                embed.Description += $" BM#{leaderboardPosition}";

            embed.AddField("**Score**",
                $"{emoji} {score.Score.ToString("#,##0")}",
                true);

            embed.AddField("**Combo**",
                data["combo"],
                true);

            embed.AddField("**Accuracy**",
                data["accuracy"],
                true);

            embed.AddField("**Hits**",
                data["hits"],
                true);

            if (pp.FullComboPp != 0)
            {
                embed.AddField("**PP if FC**",
                    Math.Round(pp.FullComboPp) + "pp",
                    true);
            }

            if (pp.ssPp != 0)
            {
                embed.AddField("**PP if SS**",
                    Math.Round(pp.ssPp) + "pp",
                    true);
            }

            embed.AddField("**Beatmap Values**",
                $"★{Math.Round(beatmap.DifficultyRating, 2)} **Length**: {totalLength} ({hitLength}) **BPM** {beatmap.bpm}\n" +
                data["stats"]);

            embed.WithFooter($"Beatmap by: {beatmap.Creator} | Played on {score.Date}");

            Database.SetLastMap(input.DiscordChannelId, beatmap.BeatmapId);
            return embed;
        }

        public DiscordEmbedBuilder CreateListEmbed()
        {
            this.user = OsuApi.User(input).Result;

            DiscordEmbedBuilder embed = CreateBaseEmbed();

            for (int i = input.Page * 5 - 5; i < input.Page * 5; i++)
            {
                if (i < scoreList.Length)
                {
                    ScoreData score = scoreList[i];

                    int modRequestNumber = OsuHelper.GetModRequestNumber(score.EnabledMods);
                    BeatmapData beatmap = OsuApi.Beatmap(score.BeatmapId, modRequestNumber, input.Gamemode).Result[0];

                    ppData pp = new PerformanceCalculator(score, beatmap, input.Gamemode).Calculate();
                    string achievedPp;

                    if (score.Rank.Equals("F") || beatmap.Approved == 4)
                        achievedPp = $"~~{Math.Round(score.PP > 0 && !input.IsDeltaT ? score.PP : pp.AchievedPp)}pp~~";
                    else
                        achievedPp = Math.Round(score.PP > 0 && !input.IsDeltaT ? score.PP : pp.AchievedPp).ToString() + "pp";

                    Dictionary<string, string> data = GetModeData(score, beatmap);

                    embed.AddField($"**{beatmap.Artist} - {beatmap.Title} [{beatmap.Version}]**",
                        $"{GetRankEmoji(score)}{OsuHelper.ParseMods(score.EnabledMods)} {data["accuracy"]} {achievedPp}\n" +
                        $"{data["combo"]} ({data["hits"]})");
                }
                else
                    break;
            }

            return embed;
        }

        private DiscordEmbedBuilder CreateBaseEmbed()
        {
            var embed = new DiscordEmbedBuilder();

            switch (beatmap.Approved)
            {
                case -2:
                case -1:
                case 0:
                    embed.Color = new DiscordColor("CC6666");
                    break;
                case 1:
                case 2:
                    embed.Color = new DiscordColor("38703e");
                    break;
                case 3:
                    embed.Color = new DiscordColor("463870");
                    break;
                case 4:
                    embed.Color = new DiscordColor("703860");
                    break;
            }

            embed.WithAuthor($"{user.Username}: {user.PPRaw}pp (#{user.PPRank} {user.Country}{user.PPCountryRank})",
                $"https://osu.ppy.sh/u/{user.UserId}",
                $"https://s.ppy.sh/a/{user.UserId}");

            embed.WithThumbnailUrl($"https://b.ppy.sh/thumb/{beatmap.BeatmapSetId}l.jpg");

            return embed;
        }

        private int GetRetryCount()
        {
            string beatmapid = score.BeatmapId;
            int count = 0;

            for (int i = input.Page; i < scoreList.Length; i++)
            {
                if (scoreList[i].BeatmapId == beatmapid)
                    count++;
                else
                    break;
            }

            return count;
        }

        private int GetLeaderboardPosition()
        {
            return Array.FindIndex(leaderboard, x => x.UserId == score.UserId && x.Score == score.Score) + 1;
        }

        private string GetRankEmoji(ScoreData score)
        {
            switch (score.Rank)
            {
                case "F":
                    return "<:whenlifegetsatyou:495914397659824128>";
                case "D":
                    return "<:D_:495911981694451723>";
                case "C":
                    return "<:C_:495911972810915840>";
                case "B":
                    return "<:B_:495911965739319316>";
                case "A":
                    return "<:A_:495911960710348820>";
                case "S":
                    return "<:S_:495911953584357377>";
                case "X":
                    return "<:X_:495911948207259648>";
                case "SH":
                    return "<:SH:495911942360137732>";
                case "XH":
                    return "<:XH:495911930238599169>";
                default:
                    return "<:whenlifegetsatyou:495914397659824128>";
            }
        }

        private Dictionary<string, string> GetOsuData(ScoreData score, BeatmapData beatmap)
        {
            var data = new Dictionary<string, string>();

            data.Add("accuracy", $"{Math.Round(OsuHelper.CalculateAccuracy(score, input.Gamemode) * 100, 2)}%");
            data.Add("hits", $"{ score.Count300 }/{ score.Count100}/{ score.Count50}/{ score.CountMiss}");
            data.Add("combo", $"{score.MaxCombo}x/{beatmap.MaxCombo}x");
            data.Add("stats", $"**CS** {Math.Round(beatmap.DiffSize, 2)} **AR** {Math.Round(beatmap.DiffApproach, 2)} **OD** {Math.Round(beatmap.DiffOverall, 2)} **HP** {Math.Round(beatmap.DiffDrain, 2)}");

            return data;
        }
        private Dictionary<string, string> GetTaikoData(ScoreData score, BeatmapData beatmap)
        {
            var data = new Dictionary<string, string>();

            data.Add("accuracy", $"{Math.Round(OsuHelper.CalculateAccuracy(score, input.Gamemode) * 100, 2)}%");
            data.Add("hits", $"{score.Count300 }/{score.Count100}/{score.CountMiss}");
            data.Add("combo", $"{score.MaxCombo}x");
            data.Add("stats", $"**OD** {Math.Round(beatmap.DiffOverall, 2)} **HP** {Math.Round(beatmap.DiffDrain, 2)}");

            return data;
        }
        private Dictionary<string, string> GetCatchData(ScoreData score, BeatmapData beatmap)
        {
            var data = new Dictionary<string, string>();

            data.Add("accuracy", $"{Math.Round(OsuHelper.CalculateAccuracy(score, input.Gamemode) * 100, 2)}%");
            data.Add("hits", $"{score.Count300 }/{score.Count100}/{score.Count50}/{score.CountMiss}");
            data.Add("combo", $"{score.MaxCombo}x/{beatmap.MaxCombo}x");
            data.Add("stats", $"**CS** {Math.Round(beatmap.DiffSize, 2)} **AR** {Math.Round(beatmap.DiffApproach, 2)} **OD** {Math.Round(beatmap.DiffOverall, 2)} **HP** {Math.Round(beatmap.DiffDrain, 2)}");

            return data;
        }
        private Dictionary<string, string> GetManiaData(ScoreData score, BeatmapData beatmap)
        {
            var data = new Dictionary<string, string>();

            data.Add("accuracy", $"{Math.Round(OsuHelper.CalculateAccuracy(score, input.Gamemode) * 100, 2)}%");
            data.Add("hits", $"{score.CountGeki}/{score.Count300 }/{score.CountKatu}/{score.Count100}/{score.Count50}/{score.CountMiss}");
            data.Add("combo", $"{score.MaxCombo}x");
            data.Add("stats", $"**Keys** {Math.Round(beatmap.DiffSize, 2)} **OD** {Math.Round(beatmap.DiffOverall, 2)} **HP** {Math.Round(beatmap.DiffDrain, 2)}");

            return data;
        }
    }
}
