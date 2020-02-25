using Mitty.Commands;
using Mitty.Osu.Api;
using Mitty.Osu.Calculators;
using ScottPlot;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;

namespace Mitty.Osu
{
    class DifficultyGraph : DataGraph
    {
        private BeatmapStats difficulty;
        private BeatmapData beatmap;

        public DifficultyGraph(Input input, BeatmapData beatmap)
        {
            this.beatmap = beatmap;

            var difficultyCalculator = new DifficultyCalculator(input.BeatmapId, input.Gamemode, input.ModNumber);
            difficulty = difficultyCalculator.Calculate();

            double maxDifficulty = (double)beatmap.DifficultyRating > difficulty.StrainPeakTotal.Max() ? (double)beatmap.DifficultyRating : difficulty.StrainPeakTotal.Max();

            var webClient = new WebClient();
            Image background = null;
            Graphics graphic = null;

            try
            {
                byte[] backgroundBytes = webClient.DownloadData($"https://assets.ppy.sh/beatmaps/{beatmap.BeatmapSetId}/covers/cover.jpg");
                var backgroundStream = new MemoryStream(backgroundBytes);
                background = Image.FromStream(backgroundStream);
                graphic = Graphics.FromImage(background);
            }
            catch (Exception e) { }

            double[] dataX = CreateXRange();

            var plt = new Plot(900, 250);
            plt.Axis(0, beatmap.TotalLength, 0, maxDifficulty + 0.5);
            plt.PlotScatter(dataX, difficulty.StrainPeakTotal, markerSize: 0, label: "Strain", lineWidth: 3, color: Color.DodgerBlue);
            plt.PlotScatter(dataX, difficulty.StrainPeakAim, markerShape: MarkerShape.none, label: "Aim", lineWidth: 1, color: Color.MediumAquamarine, lineStyle: LineStyle.Solid);
            plt.PlotScatter(dataX, difficulty.StrainPeakSpeed, markerShape: MarkerShape.none, label: "Speed", lineWidth: 1, color: Color.Firebrick);
            plt.Title($"{beatmap.Artist} - {beatmap.Title} [{beatmap.Version}]", fontSize: 24);
            plt.Ticks(displayTicksXminor: false, displayTicksYminor: false, fontSize: 16);
            plt.PlotHLine((double)beatmap.DifficultyRating, lineWidth: 1, label: "SR", color: Color.Gold);
            plt.Legend(location: legendLocation.upperLeft, fontSize: 10, fixedLineWidth: true, bold: true);
            plt.Style(figBg: Color.FromArgb(150, 35, 39, 42), dataBg: Color.FromArgb(150, 35, 39, 42), tick: Color.White, title: Color.White, label: Color.White);
            plt.Layout(xLabelHeight: 5, yLabelWidth: -10);

            if (graphic != null && background != null)
            {
                graphic.DrawImage(plt.GetBitmap(), new Point(0, 0));
                Graph = background;
            }
            else
            {
                Graph = plt.GetBitmap();
            }
            
            //background.Save("output.png", ImageFormat.Png);
        }

        private double[] CreateXRange()
        {
            double[] ticks = new double[difficulty.StrainPeakTotal.Length];
            double tickIncrement = (double)beatmap.TotalLength / (double)difficulty.StrainPeakTotal.Length;

            for (int i = 0; i < difficulty.StrainPeakTotal.Length; i++)
            {
                ticks[i] = i * tickIncrement;
            }

            return ticks;
        }
    }
}
