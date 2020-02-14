using osu.Game.Beatmaps;
using osu.Game.Scoring;

namespace Mitty.Osu.Calculators
{
    class ProcessorScoreParser
    {
        private readonly WorkingBeatmap beatmap;

        public ProcessorScoreParser(WorkingBeatmap beatmap)
        {
            this.beatmap = beatmap;
        }

        public osu.Game.Scoring.Score Parse(ScoreInfo scoreInfo)
        {
            var score = new osu.Game.Scoring.Score { ScoreInfo = scoreInfo };
            return score;
        }
    }
}
