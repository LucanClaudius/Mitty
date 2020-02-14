using osuDeltaT.Game.Rulesets;
using osuDeltaT.Game.Rulesets.Mods;
using osuDeltaT.Game.Rulesets.Osu;
using osuDeltaT.Game.Rulesets.Osu.Difficulty;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mitty.Osu.Calculators.DeltaT
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
                ProcessedBeatmap = new ProcessorWorkingBeatmap(BeatmapId);
                var ruleset = new OsuRuleset();

                ProcessedBeatmap.BeatmapInfo.Ruleset = ruleset.RulesetInfo;
                
                var attributes = ruleset.CreateDifficultyCalculator(ProcessedBeatmap).Calculate(GetModObjects(Mods, ruleset));

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
                            DiffAim = osu.AimSR,
                            DiffSpeed = osu.TapSR,
                            DiffTotal = osu.StarRating,
                            StrainPeakAim = strainPeakAim,
                            StrainPeakSpeed = strainPeakSpeed,
                            StrainPeakTotal = strainPeakTotal
                        };

                    default:
                        return new BeatmapStats { };
                }
            }
            else
                throw new ArgumentNullException("No beatmap ID provided");
        }

        private static Mod[] GetModObjects(int modNumber, Ruleset ruleset)
        {
            if (modNumber > 0)
            {
                var mods = new List<Mod>();

                string[] modAbbreviations = ((ModsShort)modNumber).ToString().Split(" ,");
                var availableMods = ruleset.GetAllMods().ToList();

                foreach (var modString in modAbbreviations)
                {
                    Mod newMod = availableMods.FirstOrDefault(m => string.Equals(m.Acronym, modString, StringComparison.CurrentCultureIgnoreCase));
                    if (newMod == null)
                        throw new ArgumentException($"Invalid mod provided: {modString}");

                    mods.Add(newMod);
                }

                return mods.ToArray();
            }
            else
                return new List<Mod>().ToArray();
        }
    }
}
