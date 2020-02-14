using osu.Game.Rulesets.Catch.Difficulty;
using osu.Game.Rulesets.Mania.Difficulty;
using osu.Game.Rulesets.Osu.Difficulty;
using osu.Game.Rulesets.Taiko.Difficulty;
using System;
using System.Linq;

namespace Mitty.Osu.Calculators
{
    class DifficultyCalculator
    {
        private const double difficultyMultiplier = 0.0675;

        public string BeatmapId { get; private set; }
        public int Gamemode { get; private set; }
        public int Mods { get; private set; }
        public ProcessorWorkingBeatmap ProcessedBeatmap { get; private set; }

        public DifficultyCalculator(string beatmapId, int gamedmode, int mods)
        {
            BeatmapId = beatmapId;
            Gamemode = gamedmode;
            Mods = mods;
        }

        public BeatmapStats Calculate()
        {
            if (!string.IsNullOrEmpty(BeatmapId))
            {
                this.ProcessedBeatmap = new ProcessorWorkingBeatmap(BeatmapId);

                ProcessedBeatmap.BeatmapInfo.Ruleset = OsuHelper.GetRulesetFromID(ProcessedBeatmap.BeatmapInfo.RulesetID).RulesetInfo;

                var ruleset = OsuHelper.GetRulesetFromID(Gamemode);
                var attributes = ruleset.CreateDifficultyCalculator(ProcessedBeatmap).Calculate(OsuHelper.GetModObjects(Mods, ruleset));

                switch (attributes)
                {
                    case OsuDifficultyAttributes osu:
                        double[] strainPeakTotal = new double[osu.Skills[0].StrainPeaks.Count];
                        double[] strainPeakAim = osu.Skills[0].StrainPeaks.ToArray();
                        double[] strainPeakSpeed = osu.Skills[1].StrainPeaks.ToArray();

                        for (int i = 0; i < osu.Skills[0].StrainPeaks.Count; i++)
                        {
                            strainPeakAim[i] = Math.Sqrt(strainPeakAim[i] * 9.999) * difficultyMultiplier;
                            strainPeakSpeed[i] = Math.Sqrt(strainPeakSpeed[i] * 9.999) * difficultyMultiplier;

                            strainPeakTotal[i] = strainPeakAim[i] + strainPeakSpeed[i] + Math.Abs(strainPeakAim[i] - strainPeakSpeed[i]) / 2;
                        }

                        return new BeatmapStats
                        {
                            DiffAim = osu.AimStrain,
                            DiffSpeed = osu.SpeedStrain,
                            DiffTotal = osu.StarRating,
                            StrainPeakAim = strainPeakAim,
                            StrainPeakSpeed = strainPeakSpeed,
                            StrainPeakTotal = strainPeakTotal
                        };

                    case TaikoDifficultyAttributes taiko:
                        return new BeatmapStats
                        {
                            DiffTotal = taiko.StarRating
                        };

                    case CatchDifficultyAttributes @catch:
                        return new BeatmapStats
                        {
                            DiffTotal = @catch.StarRating
                        };

                    case ManiaDifficultyAttributes mania:
                        return new BeatmapStats
                        {
                            DiffTotal = mania.StarRating
                        };

                    default:
                        return new BeatmapStats { };
                }
            }
            else
                throw new ArgumentNullException("No beatmap ID provided");
        }
    }
}
