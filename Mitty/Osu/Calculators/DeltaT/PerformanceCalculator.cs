using Mitty.Osu.Api;
using osuDeltaT.Game.Beatmaps.Legacy;
using osuDeltaT.Game.Rulesets.Mods;
using osuDeltaT.Game.Rulesets.Osu;
using osuDeltaT.Game.Rulesets.Scoring;
using osuDeltaT.Game.Scoring;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mitty.Osu.Calculators.DeltaT
{
    class PerformanceCalculator
    {
        private int gamemode;
        private BeatmapData beatmap;
        private ScoreData score;

        public PerformanceCalculator(ScoreData score, BeatmapData beatmap, int gamemode)
        {
            this.score = score;
            this.beatmap = beatmap;
            this.gamemode = gamemode;
        }

        public ppData Calculate()
        {
            var ruleset = new OsuRuleset();
            Mod[] mods = ruleset.ConvertLegacyMods((LegacyMods)score.EnabledMods).ToArray();
            double objectCount;
            double hitCount = score.CountGeki + score.Count300 + score.Count100 + score.CountKatu + score.Count50 + score.CountMiss;

            if (gamemode == 1 || gamemode == 2)
                objectCount = score.CountGeki + score.Count300 + score.Count100 + score.CountKatu + score.Count50 + score.CountMiss;
            else
                objectCount = beatmap.CountNormal + beatmap.CountSlider + beatmap.CountSpinner;

            double hitMultiplier = objectCount / hitCount;

            var workingBeatmap = new ProcessorWorkingBeatmap(beatmap.BeatmapId.ToString());
            workingBeatmap.BeatmapInfo.Ruleset = ruleset.RulesetInfo;

            var result = new ppData();

            var  parsedScore = new ProcessorScoreParser(workingBeatmap).Parse(new ScoreInfo
            {
                Ruleset = ruleset.RulesetInfo,
                MaxCombo = score.MaxCombo,
                TotalScore = score.Score,
                Mods = mods,
                Accuracy = OsuHelper.CalculateAccuracy(score, gamemode),
                Statistics = new Dictionary<HitResult, int>
                {
                    { HitResult.Perfect, score.CountGeki },
                    { HitResult.Great, score.Count300 },
                    { HitResult.Good, score.Count100 },
                    { HitResult.Ok, score.CountKatu },
                    { HitResult.Meh, score.Count50 },
                    { HitResult.Miss, score.CountMiss }
                }
            });

            result.AchievedPp = ruleset.CreatePerformanceCalculator(workingBeatmap, parsedScore.ScoreInfo).Calculate();

            if (!score.Perfect && gamemode != 1 && gamemode != 3)
            {
                var fullComboScore = new ProcessorScoreParser(workingBeatmap).Parse(new ScoreInfo
                {
                    Ruleset = ruleset.RulesetInfo,
                    MaxCombo = beatmap.MaxCombo,
                    TotalScore = score.Score,
                    Mods = mods,
                    Accuracy = OsuHelper.CalculateAccuracy(score, gamemode),
                    Statistics = new Dictionary<HitResult, int>
                    {
                        { HitResult.Perfect, (int)Math.Round(hitMultiplier * score.CountGeki) },
                        { HitResult.Great, (int)Math.Round(hitMultiplier * score.Count300) },
                        { HitResult.Good, (int)Math.Round(hitMultiplier * score.Count100) },
                        { HitResult.Ok, (int)Math.Round(hitMultiplier * score.CountKatu) },
                        { HitResult.Meh, (int)Math.Round(hitMultiplier * score.Count50 + score.CountMiss) },
                        { HitResult.Miss, 0 }
                    }
                });

                result.FullComboPp = ruleset.CreatePerformanceCalculator(workingBeatmap, fullComboScore.ScoreInfo).Calculate();
            }

            if (OsuHelper.CalculateAccuracy(score, gamemode) != 1)
            {
                var ssScore = new ProcessorScoreParser(workingBeatmap).Parse(new ScoreInfo
                {
                    Ruleset = ruleset.RulesetInfo,
                    MaxCombo = beatmap.MaxCombo,
                    Mods = mods,
                    Accuracy = 1,
                    Statistics = new Dictionary<HitResult, int>
                    {
                        { HitResult.Perfect, 0 },
                        { HitResult.Great, (int)objectCount },
                        { HitResult.Good, 0 },
                        { HitResult.Ok, 0 },
                        { HitResult.Meh, 0 },
                        { HitResult.Miss, 0 }
                    }
                });

                result.ssPp = ruleset.CreatePerformanceCalculator(workingBeatmap, ssScore.ScoreInfo).Calculate();
            }

            return result;
        }
    }
}
