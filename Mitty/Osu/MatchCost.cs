using DSharpPlus.Entities;
using Mitty.Osu.Api;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mitty.Osu
{
    class MatchCost
    {
        private MatchData match;
        private Dictionary<int, List<double>> blueScores = new Dictionary<int, List<double>>();
        private Dictionary<int, List<double>> redScores = new Dictionary<int, List<double>>();
        private Dictionary<int, List<double>> totalScores = new Dictionary<int, List<double>>();
        private Dictionary<int, double[]> blueCosts = new Dictionary<int, double[]>();
        private Dictionary<int, double[]> redCosts = new Dictionary<int, double[]>();
        private Dictionary<int, double[]> totalCosts = new Dictionary<int, double[]>();

        private double blueTotal = 0;
        private double redTotal = 0;
        private double total = 0;

        private double blueAverage;
        private double redAverage;
        private double totalAverage;

        private double blueScoreCount;
        private double redScoreCount;
        private double totalScoreCount;

        private double mapCount;
        private int warmupCount;

        private bool isFfa = false;

        public DiscordEmbed Embed { get; private set; }

        public MatchCost(string matchLink, int warmupCount)
        {
            match = OsuApi.Match(matchLink.Substring(37)).Result;
            this.warmupCount = warmupCount;

            if (match.Games.Length == 0)
                throw new Exception("jij bent echt kankerdom");

            foreach (MultiPlayerGame game in match.Games)
            {
                if (warmupCount != 0)
                {
                    warmupCount--;
                    continue;
                }

                foreach (MultiPlayerScore score in game.Scores)
                {
                    if (score.Score < 5000)
                        continue;

                    if (score.Team == 0)
                    {
                        isFfa = true;

                        if (!totalScores.ContainsKey(score.UserId))
                            totalScores.Add(score.UserId, new List<double>());

                        totalScores[score.UserId].Add(score.Score);
                    }
                    else if (score.Team == 1)
                    {
                        blueTotal += score.Score;

                        if (!blueScores.ContainsKey(score.UserId))
                            blueScores.Add(score.UserId, new List<double>());

                        blueScores[score.UserId].Add(score.Score);
                        blueScoreCount++;
                    }
                    else if (score.Team == 2)
                    {
                        redTotal += score.Score;

                        if (!redScores.ContainsKey(score.UserId))
                            redScores.Add(score.UserId, new List<double>());

                        redScores[score.UserId].Add(score.Score);
                        redScoreCount++;
                    }

                    total += score.Score;
                    totalScoreCount++;
                }

                mapCount++;
            }

            totalAverage = total / totalScoreCount;
            blueAverage = blueTotal / blueScoreCount;
            redAverage = redTotal / redScoreCount;

            if (isFfa)
            {
                foreach (KeyValuePair<int, List<double>> playerScores in totalScores)
                {
                    double cost = 0;

                    foreach (int score in playerScores.Value)
                    {
                        cost += score / totalAverage;
                    }

                    totalCosts.Add(playerScores.Key, new double[] {
                        cost / playerScores.Value.Count * Math.Pow(1.2, Math.Pow(playerScores.Value.Count / mapCount, 0.4))
                    });
                }
            }
            else
            {
                foreach (KeyValuePair<int, List<double>> playerScores in blueScores)
                {
                    double cost = 0;
                    double teamcost = 0;

                    foreach (int score in playerScores.Value)
                    {
                        cost += score / totalAverage;
                        teamcost += score / blueAverage;
                    }

                    blueCosts.Add(playerScores.Key, new double[] {
                        cost / playerScores.Value.Count * Math.Pow(1.2, Math.Pow(playerScores.Value.Count / mapCount, 0.4)),
                        teamcost / playerScores.Value.Count * Math.Pow(1.2, Math.Pow(playerScores.Value.Count / mapCount, 0.4))
                    });
                }

                foreach (KeyValuePair<int, List<double>> playerScores in redScores)
                {
                    double cost = 0;
                    double teamcost = 0;

                    foreach (int score in playerScores.Value)
                    {
                        cost += score / totalAverage;
                        teamcost += score / redAverage;
                    }

                    redCosts.Add(playerScores.Key, new double[] {
                        cost / playerScores.Value.Count * Math.Pow(1.2, Math.Pow(playerScores.Value.Count / mapCount, 0.4)),
                        teamcost / playerScores.Value.Count * Math.Pow(1.2, Math.Pow(playerScores.Value.Count / mapCount, 0.4))
                    });
                }
            }
        }

        public DiscordEmbed CreateEmbed(string calcType)
        {
            var embed = CreateBaseEmbed();

            if (isFfa)
            {
                string NameList = "";
                string CostList = "";

                foreach (KeyValuePair<int, double[]> cost in totalCosts.OrderByDescending(key => key.Value[0]))
                {
                    UserData user = OsuApi.User(cost.Key).Result;
                    NameList += $"**{user.Username}:**\n";
                    CostList += $"{Math.Round(cost.Value[0], 2)}\n";
                }

                embed.AddField($"🔵__**Costs**__",
                    NameList,
                    true);

                embed.AddField("** **",
                    CostList,
                    true);
            }
            else
            {
                int valueIndex;

                if (calcType.Equals("All"))
                    valueIndex = 0;
                else
                    valueIndex = 1;

                string redNameList = "";
                string redCostList = "";
                string blueNameList = "";
                string blueCostList = "";

                foreach (KeyValuePair<int, double[]> cost in redCosts.OrderByDescending(key => key.Value[valueIndex]))
                {
                    UserData user = OsuApi.User(cost.Key).Result;
                    redNameList += $"**{user.Username}:**\n";
                    redCostList += $"{Math.Round(cost.Value[valueIndex], 2)}\n";
                }

                foreach (KeyValuePair<int, double[]> cost in blueCosts.OrderByDescending(key => key.Value[valueIndex]))
                {
                    UserData user = OsuApi.User(cost.Key).Result;
                    blueNameList += $"**{user.Username}:**\n";
                    blueCostList += $"{Math.Round(cost.Value[valueIndex], 2)}\n";
                }

                embed.Description += $"Calculated for: {calcType}";

                embed.AddField($"🔴__**Red Team**__",
                    redNameList,
                    true);

                embed.AddField("** **",
                    redCostList,
                    true);

                embed.AddField("** **",
                    "** **",
                    true);

                embed.AddField($"🔵__**Blue Team**__",
                    blueNameList,
                    true);

                embed.AddField("** **",
                    blueCostList,
                    true);

                embed.AddField("** **",
                    "** **",
                    true);
            }

            return embed;
        }

        private DiscordEmbedBuilder CreateBaseEmbed()
        {
            var embed = new DiscordEmbedBuilder();

            embed.WithAuthor($"{match.Match.Name}",
                $"https://osu.ppy.sh/community/matches/{match.Match.MatchId}");

            embed.WithDescription($"Warmups: {warmupCount}\n");

            return embed;
        }
    }
}
