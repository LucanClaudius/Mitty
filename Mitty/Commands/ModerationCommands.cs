using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mitty.Commands
{
    class ModerationCommands : BaseCommandModule
    {
        [Command("kick")]
        [RequirePermissions(Permissions.KickMembers)]
        public async Task Kick(CommandContext ctx, DiscordUser user)
        {
            DiscordMember member = await ctx.Guild.GetMemberAsync(user.Id);
            await member.RemoveAsync();
            await ctx.Channel.SendMessageAsync($"{user.Mention} has been kicked by {ctx.Member.Nickname}");
        }

        [Command("ban")]
        [Aliases("yeet")]
        [RequirePermissions(Permissions.BanMembers)]
        public async Task Ban(CommandContext ctx, DiscordUser user, string reason)
        {
            DiscordMember member = await ctx.Guild.GetMemberAsync(user.Id);
            await member.BanAsync(0, reason);
            await ctx.Channel.SendMessageAsync($"{user.Mention} has been banned by {ctx.Member.Nickname}");
        }

        [Command("purge")]
        [RequirePermissions(Permissions.ManageMessages)]
        public async Task Purge(CommandContext ctx, int amount)
        {
            await ctx.Message.DeleteAsync();
            IReadOnlyList<DiscordMessage> messages = await ctx.Channel.GetMessagesAsync(amount);
            await ctx.Channel.DeleteMessagesAsync(messages);
        }
    }
}
