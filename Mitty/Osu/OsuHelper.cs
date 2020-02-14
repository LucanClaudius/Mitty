using Mitty.Commands;
using Mitty.Osu.Api;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Catch;
using osu.Game.Rulesets.Mania;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Osu;
using osu.Game.Rulesets.Taiko;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mitty.Osu
{
    public static class OsuHelper
    {
        public static int GetModRequestNumber(int modNumber)
        {
            string[] mods = ((Mods)modNumber).ToString().Split(", ");
            int modRequestNumber = 0;

            foreach (string mod in mods)
            {
                if (Enum.TryParse(mod, out Mods _mod))
                {
                    if (Mods.StarChangeMods.HasFlag(_mod))
                        modRequestNumber += (int)_mod;
                }
            }

            return modRequestNumber;
        }

        public static string ParseMods(int modNumber)
        {
            if (modNumber > 0)
            {
                string mods = $"+{((ModsShort)modNumber).ToString("G").Replace(" ", "")}";
                return mods.Replace("DT,NC", "NC").Replace("SD,PF", "PF");
            }
            else
                return "";
        }

        public static Mod[] GetModObjects(int modNumber, Ruleset ruleset)
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

        public static Ruleset GetRulesetFromID(int rulesetId)
        {
            switch (rulesetId)
            {
                case 0:
                    return new OsuRuleset();
                case 1:
                    return new TaikoRuleset();
                case 2:
                    return new CatchRuleset();
                case 3:
                    return new ManiaRuleset();
                default:
                    throw new ArgumentException("Invalid ruleset ID provided.");
            }
        }

        //public static double GetNewTop(int newPlay, string osuName)
        //{
        //    var input = new Input(osuName);

        //    Task<UserData> userRequest = OsuApi.User(osuName);
        //    Task<ScoreData[]> topRequest = OsuApi.UserTop(input);

        //    Task.WhenAll(
        //        userRequest,
        //        topRequest
        //    ).Wait();

        //    UserData user = userRequest.Result;
        //    ScoreData[] top = topRequest.Result;

        //    double netPpGain = 0;
        //    double ppLost = 0;

        //    for (int i = 0; i < top.Length; i++)
        //    {
        //        if (newPlay != 0 && top[i].PP < newPlay)
        //        {
        //            netPpGain = (double)newPlay * Math.Pow(0.95, i);
        //            newPlay = 0;

        //            ppLost += (double)top[i].PP * Math.Pow(0.95, i) - (double)top[i].PP * Math.Pow(0.95, i + 1);
        //        }
        //        else if (newPlay == 0)
        //        {
        //            ppLost += (double)top[i].PP * Math.Pow(0.95, i) - (double)top[i].PP * Math.Pow(0.95, i + 1);
        //        }

        //    }

        //    return user.PPRaw + netPpGain - ppLost;
        //}

        public static double CalculateAccuracy(ScoreData score, int gamemode)
        {
            switch (gamemode)
            {
                case 0:
                    return (score.Count50 * 50 + score.Count100 * 100 + score.Count300 * 300) / (double)(score.CountMiss * 300 + score.Count50 * 300 + score.Count100 * 300 + score.Count300 * 300);
                case 1:
                    return (score.Count100 * 0.5 + score.Count300) / (double)(score.CountMiss + score.Count100 + score.Count300);
                case 2:
                    return (score.Count50 + score.Count100 + score.Count300) / (double)(score.CountMiss + score.CountKatu + score.Count50 + score.Count100 + score.Count300);
                case 3:
                    return (score.Count50 * 50 + score.Count100 * 100 + score.CountKatu * 200 + (score.Count300 + score.CountGeki) * 300) / (double)(score.CountMiss * 300 + score.Count50 * 300 + score.Count100 * 300 + score.CountKatu * 300 + score.Count300 * 300 + score.CountGeki * 300);
                default:
                    return 0;
            }
        }
    }
}