using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Mitty.Osu;
using Mitty.Osu.Api;
using System;
using System.Threading.Tasks;

namespace Mitty.Commands
{
    [Cooldown(40, 60, CooldownBucketType.Global)]
    class OsuCommands : BaseCommandModule
    {
        [Command("recent")]
        [Aliases("r", "rs")]
        public async Task Recent(CommandContext ctx)
        {
            ctx.TriggerTypingAsync();
            var input = new Input(ctx.Message);
            ScoreData[] scoreData = await OsuApi.Recent(input);

            var score = new Score(input, scoreData);
            await ctx.Channel.SendMessageAsync(embed: score.Embed);
        }

        [Command("top")]
        public async Task Top(CommandContext ctx)
        {
            ctx.TriggerTypingAsync();
            var input = new Input(ctx.Message);
            ScoreData[] scoreData = await OsuApi.Best(input);

            var score = new Score(input, scoreData);
            await ctx.Channel.SendMessageAsync(embed: score.Embed);
        }

        [Command("best")]
        [Aliases("b", "compare", "c")]
        public async Task Best(CommandContext ctx)
        {
            ctx.TriggerTypingAsync();
            var input = new Input(ctx.Message);
            ScoreData[] scoreData = await OsuApi.Score(input);

            var score = new Score(input, scoreData);
            await ctx.Channel.SendMessageAsync(embed: score.Embed);
        }

        [Command("user")]
        [Aliases("u")]
        public async Task User(CommandContext ctx)
        {
            ctx.TriggerTypingAsync();
            //add mod data in top plays
            var input = new Input(ctx.Message);
            UserData userData = await OsuApi.User(input);

            var user = new User(input, userData);
            await ctx.Channel.SendMessageAsync(embed: user.Embed);
        }

        [Command("beatmap")]
        [Aliases("bm")]
        public async Task Beatmap(CommandContext ctx)
        {
            ctx.TriggerTypingAsync();
            await ctx.TriggerTypingAsync();
            var input = new Input(ctx.Message);

            var beatmap = new Beatmap(input);
            await ctx.Channel.SendMessageAsync(embed: beatmap.Embed);
        }

        [Command("diffgraph")]
        [Aliases("dg")]
        public async Task DiffGraph(CommandContext ctx)
        {
            ctx.TriggerTypingAsync();
            var input = new Input(ctx.Message);

            BeatmapData[] beatmapData = await OsuApi.Beatmap(input.BeatmapId, input.ModRequestNumber ?? 0, input.Gamemode);

            if (beatmapData.Length == 0)
                throw new Exception("Beatmap not found");

            var graph = new DifficultyGraph(input, beatmapData[0]);
            await ctx.Channel.SendFileAsync("graph.png", graph.ToStream());
        }

        [Command("link")]
        public async Task Link(CommandContext ctx, params string[] nameArgs)
        {
            ctx.TriggerTypingAsync();
            string osuName = string.Join(" ", nameArgs);

            if (string.IsNullOrEmpty(osuName))
                throw new Exception("No name provided");

            UserData osuUser = await OsuApi.User(osuName);

            await Database.SetOsuUser(ctx.Message.Author.Id, osuUser.UserId.ToString());
            await ctx.Channel.SendMessageAsync($"{ctx.Message.Author.Mention} has been linked to `{osuName}`");
        }

        [Command("osu")]
        public async Task Osu(CommandContext ctx)
        {
            string id = await Database.GetOsuUser(ctx.Message.MentionedUsers[0].Id);
            if (string.IsNullOrEmpty(id))
                throw new Exception($"{ctx.Message.MentionedUsers[0].Username} is not linked to an osu account");

            await ctx.Channel.SendMessageAsync($"{ctx.Message.MentionedUsers[0].Username} is linked to https://osu.ppy.sh/users/{id}");
        }

        [Command("matchcosts")]
        [Aliases("mc")]
        public async Task MatchCosts(CommandContext ctx, string matchLink, int warmupCount)
        {
            ctx.TriggerTypingAsync();
            await ctx.TriggerTypingAsync();
            var matchCost = new MatchCost(matchLink, warmupCount);
            await ctx.Channel.SendMessageAsync(embed: matchCost.CreateEmbed("All"));
        }

        [Command("teamcsots")]
        [Aliases("tc")]
        public async Task TeamCosts(CommandContext ctx, string matchLink, int warmupCount)
        {
            ctx.TriggerTypingAsync();
            await ctx.TriggerTypingAsync();
            var matchCost = new MatchCost(matchLink, warmupCount);
            await ctx.Channel.SendMessageAsync(embed: matchCost.CreateEmbed("Team"));
        }

        [Command("bws")]
        public async Task BadgeWeightedSeedingByUser(CommandContext ctx, int badgeCount, params string[] nameArgs)
        {
            int rank = 0;
            string osuName;

            if (nameArgs.Length == 1)
            {
                int.TryParse(nameArgs[0], out rank);
            }

            if (rank == 0)
            {
                UserData osuUser;

                if (nameArgs.Length > 0)
                {
                    osuName = string.Join(" ", nameArgs).Replace("\"", "");
                    osuUser = await OsuApi.User(osuName);
                }
                else
                {
                    if (ctx.Message.MentionedUsers.Count > 0)
                        osuName = await Database.GetOsuUser(ctx.Message.MentionedUsers[0].Id);
                    else
                        osuName = await Database.GetOsuUser(ctx.Message.Author.Id);

                    osuUser = await OsuApi.User(int.Parse(osuName));
                }

                if (string.IsNullOrEmpty(osuName))
                    throw new Exception("No username or rank provided");

                await ctx.Channel.SendMessageAsync($"BWS rank for `{osuUser.Username}` with {badgeCount} badges is {Math.Round(Math.Pow(osuUser.PPRank, Math.Pow(0.9937, Math.Pow(badgeCount, 2))))}");
            }
            else
            {
                await ctx.Channel.SendMessageAsync($"BWS at rank `{rank}` with {badgeCount} badges is {Math.Round(Math.Pow(rank, Math.Pow(0.9937, Math.Pow(badgeCount, 2))))}");
            }
        }
    }
}
