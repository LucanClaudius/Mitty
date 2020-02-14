using osu.Framework.Audio.Track;
using osu.Framework.Graphics.Textures;
using osu.Framework.Graphics.Video;
using osuDeltaT.Game.Beatmaps;
using osuDeltaT.Game.Beatmaps.Formats;
using osuDeltaT.Game.IO;
using System.IO;
using System.Net;

namespace Mitty.Osu.Calculators.DeltaT
{
    class ProcessorWorkingBeatmap : WorkingBeatmap
    {
        private readonly osuDeltaT.Game.Beatmaps.Beatmap beatmap;

        public ProcessorWorkingBeatmap(string beatmapId) : this(readBeatmap(beatmapId), int.Parse(beatmapId))
        {
        }

        private ProcessorWorkingBeatmap(osuDeltaT.Game.Beatmaps.Beatmap beatmap, int? beatmapId = null) : base(beatmap.BeatmapInfo, null)
        {
            this.beatmap = beatmap;

            if (beatmapId.HasValue)
                beatmap.BeatmapInfo.OnlineBeatmapID = beatmapId;
        }

        private static osuDeltaT.Game.Beatmaps.Beatmap readBeatmap(string beatmapId)
        {
            //if beatmap is not downloaded
            byte[] data = new WebClient().DownloadData($"https://osu.ppy.sh/osu/{beatmapId}");
            var stream = new MemoryStream(data, false);

            /*
                if beatmap is downloaded
                using (var stream = File.OpenRead(filename))
                using (var streamReader = new LineBufferedReader(stream))
            */

            var streamReader = new StreamReader(stream);
            return Decoder.GetDecoder<osuDeltaT.Game.Beatmaps.Beatmap>(streamReader).Decode(streamReader);
        }

        protected override IBeatmap GetBeatmap() => beatmap;
        protected override Texture GetBackground() => null;
        protected override Track GetTrack() => null;
    }
}
