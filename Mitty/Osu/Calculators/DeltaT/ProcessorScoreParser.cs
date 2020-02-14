using osuDeltaT.Game.Beatmaps;
using osuDeltaT.Game.Scoring;

namespace Mitty.Osu.Calculators.DeltaT
{
    class ProcessorScoreParser
    {
        private readonly WorkingBeatmap beatmap;

        public ProcessorScoreParser(WorkingBeatmap beatmap)
        {
            this.beatmap = beatmap;
        }

        public osuDeltaT.Game.Scoring.Score Parse(ScoreInfo scoreInfo)
        {
            var score = new osuDeltaT.Game.Scoring.Score { ScoreInfo = scoreInfo };
            return score;
        }
    }
}
