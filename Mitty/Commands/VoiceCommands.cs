using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.VoiceNext;
using DSharpPlus.Lavalink;
using DSharpPlus.Net;

namespace Mitty.Commands
{
    [ModuleLifespan(ModuleLifespan.Transient)]
    class VoiceCommands : BaseCommandModule
    {
        //private readonly LavalinkConfiguration lavaConfig = new LavalinkConfiguration
        //{
        //    Password = Bot.configJson["LavaPass"],
        //    RestEndpoint = new ConnectionEndpoint { Hostname = Bot.configJson["LavaHost"], Port = 2333 },
        //    SocketEndpoint = new ConnectionEndpoint { Hostname = Bot.configJson["LavaHost"], Port = 80 }
        //};

        //[Command("join")]
        //public async Task Join(CommandContext ctx)
        //{
        //    var vnext = ctx.Client.GetVoiceNext();

        //    var vnc = vnext.GetConnection(ctx.Guild);
        //    if (vnc != null)
        //        throw new Exception("Already connected to this guild");

        //    var chn = ctx.Member?.VoiceState?.Channel;
        //    if (chn == null)
        //        throw new Exception("You are not in a voice channel");

        //    vnc = await vnext.ConnectAsync(chn);
        //}

        //[Command("leave")]
        //public async Task Leave(CommandContext ctx)
        //{
        //    var vnext = ctx.Client.GetVoiceNext();

        //    var vnc = vnext.GetConnection(ctx.Guild);
        //    if (vnc == null)
        //        throw new Exception("Not connected to this guild.");

        //    vnc.Disconnect();
        //}

        //[Command("Play")]
        //public async Task Leave(CommandContext ctx, Uri uri)
        //{
        //    var vnext = ctx.Client.GetVoiceNext();
        //    var lava = ctx.Client.GetLavalink();
        //    var nodeConnection = await lava.ConnectAsync(lavaConfig);

        //    var chn = ctx.Member?.VoiceState?.Channel;
        //    if (chn == null)
        //        throw new Exception("You are not in a voice channel");

        //    var guildConnection = nodeConnection.ConnectAsync(chn);

        //    var tracks = guildConnection;
        //}

        [Command("derankers")]
        public async Task Derankers(CommandContext ctx)
        {
            await SendFile(ctx, "Sounds/derankers.mp3");
        }

        private async Task SendFile(CommandContext ctx, string filename)
        {
            bool leaveOnComplete = false;
            var vnext = ctx.Client.GetVoiceNext();

            var vnc = vnext.GetConnection(ctx.Guild);
            if (vnc == null)
            {
                var chn = ctx.Member?.VoiceState?.Channel;
                if (chn == null)
                    throw new Exception("You are not in a voice channel");

                leaveOnComplete = true;
                vnc = await vnext.ConnectAsync(chn);
            }
            else if (ctx.Member?.VoiceState?.Channel != vnc.Channel)
                throw new Exception("You are not in this voice channel");

            while (vnc.IsPlaying)
                await vnc.WaitForPlaybackFinishAsync();

            Exception exc = null;
            
            try
            {
                vnc.SendSpeaking();

                var psi = new ProcessStartInfo
                {
                    FileName = "/usr/bin/ffmpeg",
                    Arguments = $@"-i ""{filename}"" -ac 2 -f s16le -ar 48000 pipe:1",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                var ffmpeg = Process.Start(psi);
                var ffout = ffmpeg.StandardOutput.BaseStream;

                var txStream = vnc.GetTransmitStream();
                await ffout.CopyToAsync(txStream);
                await txStream.FlushAsync();
            }
            catch (Exception ex) { exc = ex; }
            finally
            {
                vnc.SendSpeaking(false);

                if (leaveOnComplete)
                {
                    while (vnc.IsPlaying)
                        await vnc.WaitForPlaybackFinishAsync();

                    vnc.Disconnect();
                }
            }

            if (exc != null)
                await ctx.RespondAsync($"{exc.GetType()}: {exc.Message}");
        }
    }
}
