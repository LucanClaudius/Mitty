using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mitty.Commands
{
    class ImageCommands : BaseCommandModule
    {
        [Command("cat")]
        public async Task Cat(CommandContext ctx)
        {
            await SendEmbed(ctx, "http://shibe.online/api/cats?count=1", false, null);
        }

        [Command("shiba")]
        public async Task Shiba(CommandContext ctx)
        {
            await SendEmbed(ctx, "http://shibe.online/api/shibes?count=1", false, null);
        }

        [Command("bird")]
        public async Task Bird(CommandContext ctx)
        {
            await SendEmbed(ctx, "http://shibe.online/api/birds?count=1", false, null);
        }

        [Command("fox")]
        public async Task Fox(CommandContext ctx)
        {
            await SendEmbed(ctx, "https://randomfox.ca/floof/", true, "image");
        }

        [Command("dog")]
        public async Task Dog(CommandContext ctx)
        {
            await SendEmbed(ctx, "https://random.dog/woof.json", true, "url");
        }

        private async Task SendEmbed(CommandContext ctx, string url, bool isDictionary, string key)
        {
            string html = await Api.Call(url);
            string imageUrl;

            if (isDictionary)
                imageUrl = JsonConvert.DeserializeObject<Dictionary<string, string>>(html)[key];
            else
                imageUrl = JsonConvert.DeserializeObject<string[]>(html)[0];


            var embed = new DiscordEmbedBuilder()
                .WithImageUrl(imageUrl);

            await ctx.Channel.SendMessageAsync(embed: embed);
        }
    }
}
