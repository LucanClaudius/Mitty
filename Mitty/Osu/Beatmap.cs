using DSharpPlus.Entities;
using Mitty.Commands;
using Mitty.Osu.Api;
using Mitty.Osu.Calculators;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mitty.Osu
{
    class Beatmap
    {
        private ScoreData[] leaderboard;
        private BeatmapData beatmap;
        private DifficultyCalculator difficultyCalculator;
        private BeatmapStats difficulty;
        private string leaderBoardFullCombos;

        public DiscordEmbed Embed { get; private set; }

        public Beatmap(Input input)
        {
            if (input.BeatmapId == null)
                throw new Exception("No beatmap link provided");

            Task<BeatmapData[]> beatmapRequest = OsuApi.Beatmap(input.BeatmapId, input.ModRequestNumber ?? 0, input.Gamemode);
            Task<ScoreData[]> leaderboardRequest = OsuApi.Leaderboard(input.BeatmapId, input.Gamemode);

            Task.WhenAll(
                beatmapRequest,
                leaderboardRequest
            ).Wait();

            beatmap = beatmapRequest.Result[0];
            leaderboard = leaderboardRequest.Result;

            CountLeaderboardFullCombo();

            difficultyCalculator = new DifficultyCalculator(input.BeatmapId, input.Gamemode, input.ModNumber);
            difficulty = difficultyCalculator.Calculate();

            Embed = CreateEmbed();
        }

        private DiscordEmbed CreateEmbed()
        {
            var embed = new DiscordEmbedBuilder();

            embed.WithAuthor($"{beatmap.Artist} - {beatmap.Title} [{beatmap.Version}]");

            embed.WithThumbnailUrl($"https://b.ppy.sh/thumb/{beatmap.BeatmapSetId}.jpg");

            embed.WithDescription(beatmap.DownloadUnavailable || beatmap.AudioUnavailable ?
                    "[Bloodcat](https://bloodcat.com/osu/s/{beatmap.beatmapSetId})" :
                    $"[Download](https://osu.ppy.sh/beatmapsets/{beatmap.BeatmapSetId}/download) - [No Video](https://osu.ppy.sh/beatmapsets/{beatmap.BeatmapSetId}?noVideo=1)\n" +
                $"★`{Math.Round(difficulty.DiffTotal, 2)} `**CS:**`{beatmap.DiffSize} `**AR:**`{beatmap.DiffApproach} `**OD:**`{beatmap.DiffOverall} `**HP:**`{beatmap.DiffDrain}`");

            embed.AddField("**Length:**",
                $"{TimeSpan.FromSeconds(beatmap.TotalLength).ToString(@"mm\:ss")} ({TimeSpan.FromSeconds(beatmap.HitLength).ToString(@"mm\:ss")})",
                true);

            embed.AddField("**BPM:**",
                $"{beatmap.bpm}",
                true);

            embed.AddField("**Combo:**",
                $"{beatmap.MaxCombo}",
                true);

            embed.AddField("**Objects:**",
                $"{beatmap.CountNormal + beatmap.CountSlider + beatmap.CountSpinner}",
                true);

            embed.AddField("**Playcount:**",
                $"{beatmap.Playcount}",
                true);

            embed.AddField("**Consistency:**",
                $"temp",
                true);

            embed.AddField("**FC's in top 100:**",
                $"{leaderBoardFullCombos}");

            return embed;
        }

        private void CountLeaderboardFullCombo()
        {
            var fullComboCount = new Dictionary<string, int>
            {
                { "DTHR", 0 },
                { "DT", 0 },
                { "HR", 0 },
                { "HD", 0 },
                { "NM", 0 }
            };

            foreach (ScoreData score in leaderboard)
            {
                if (score.CountMiss == 0 && score.MaxCombo > Math.Min(beatmap.MaxCombo - 10, beatmap.MaxCombo * 0.98))
                {
                    var mods = (Mods)score.EnabledMods;

                    if (mods.HasFlag(Mods.DoubleTime) && mods.HasFlag(Mods.HardRock))
                        fullComboCount["DTHR"] += 1;
                    else if (mods.HasFlag(Mods.DoubleTime))
                        fullComboCount["DT"] += 1;
                    else if (mods.HasFlag(Mods.HardRock))
                        fullComboCount["HR"] += 1;
                    else if (mods.HasFlag(Mods.Hidden))
                        fullComboCount["HD"] += 1;
                    else
                        fullComboCount["NM"] += 1;
                }
            }

            string fullComboList = "";

            foreach (KeyValuePair<string, int> pair in fullComboCount)
            {
                if (pair.Value > 0)
                {
                    fullComboList += $"{pair.Key}: `{pair.Value}`\n";
                }
            }

            leaderBoardFullCombos = fullComboList;
        }
    }
}
