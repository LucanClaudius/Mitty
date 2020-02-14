using osu.Framework.Audio.Track;
using osu.Framework.Graphics.Textures;
using osu.Framework.Graphics.Video;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.Formats;
using osu.Game.IO;
using System.IO;
using System.Net;

namespace Mitty.Osu.Calculators
{
    class ProcessorWorkingBeatmap : WorkingBeatmap
    {
        private readonly osu.Game.Beatmaps.Beatmap beatmap;

        public ProcessorWorkingBeatmap(string beatmapId) : this(readBeatmap(beatmapId), int.Parse(beatmapId))
        {
        }

        private ProcessorWorkingBeatmap(osu.Game.Beatmaps.Beatmap beatmap, int? beatmapId = null) : base(beatmap.BeatmapInfo, null)
        {
            this.beatmap = beatmap;

            if (beatmapId.HasValue)
                beatmap.BeatmapInfo.OnlineBeatmapID = beatmapId;
        }

        private static osu.Game.Beatmaps.Beatmap readBeatmap(string beatmapId)
        {
            //if beatmap is not downloaded
            byte[] data = new WebClient().DownloadData($"https://osu.ppy.sh/osu/{beatmapId}");
            var stream = new MemoryStream(data, false);

            /*
                if beatmap is downloaded
                using (var stream = File.OpenRead(filename))
                using (var streamReader = new LineBufferedReader(stream))
            */

            var streamReader = new LineBufferedReader(stream);
            return Decoder.GetDecoder<osu.Game.Beatmaps.Beatmap>(streamReader).Decode(streamReader);
        }

        protected override IBeatmap GetBeatmap() => beatmap;
        protected override Texture GetBackground() => null;
        protected override Track GetTrack() => null;
        protected override VideoSprite GetVideo() => null;
    }
}
