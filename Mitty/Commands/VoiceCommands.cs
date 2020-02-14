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

namespace Mitty.Commands
{
    class VoiceCommands : BaseCommandModule
    {
        [Command("join")]
        public async Task Join(CommandContext ctx)
        {
            var vnext = ctx.Client.GetVoiceNext();

            var vnc = vnext.GetConnection(ctx.Guild);
            if (vnc != null)
                throw new Exception("Already connected to this guild");

            var chn = ctx.Member?.VoiceState?.Channel;
            if (chn == null)
                throw new Exception("You are not in a voice channel");

            vnc = await vnext.ConnectAsync(chn);
        }

        [Command("leave")]
        public async Task Leave(CommandContext ctx)
        {
            var vnext = ctx.Client.GetVoiceNext();

            var vnc = vnext.GetConnection(ctx.Guild);
            if (vnc == null)
                throw new Exception("Not connected to this guild.");

            vnc.Disconnect();
        }

        [Command("derankers")]
        public async Task Derankers(CommandContext ctx)
        {
            await SendFile(ctx, "Sounds/derankers.mp3");
        }

        [Command("papa")]
        [Aliases("jekelmissed")]
        public async Task Papa(CommandContext ctx)
        {
            await SendFile(ctx, "Sounds/papa.mp3");
        }

        [Command("swerrorage")]
        public async Task SwerroRage(CommandContext ctx)
        {
            await SendFile(ctx, "Sounds/nwords.mp3");
        }

        [Command("nwords")]
        public async Task NWords(CommandContext ctx)
        {
            await SendFile(ctx, "Sounds/nwords.mp3");
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
                vnc.SendSpeaking(true);

                var psi = new ProcessStartInfo
                {
                    FileName = "/usr/bin/ffmpeg",
                    Arguments = $@"-i ""{filename}"" -ac 2 -f s16le -ar 48000 pipe:1",
                    RedirectStandardOutput = true,
                    UseShellExecute = false
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
