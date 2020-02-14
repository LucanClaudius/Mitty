using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity;
using System;
using System.Threading.Tasks;

namespace Mitty.Commands
{
    class UtilityCommands : BaseCommandModule
    {
        [Command("help")]
        public async Task Help(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("Commands can be found here: https://justlucan.xyz/mitty/");
        }

        [Command("roll")]
        public async Task Roll(CommandContext ctx, int max)
        {
            int roll = new Random().Next(1, max + 1);
            await ctx.Channel.SendMessageAsync(roll.ToString()).ConfigureAwait(false);
        }

        [Command("response")]
        public async Task Response(CommandContext ctx)
        {
            var interactivity = ctx.Client.GetInteractivity();

            var message = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel).ConfigureAwait(false);

            await ctx.Channel.SendMessageAsync(message.Result.Content).ConfigureAwait(false);
        }
    }
}
