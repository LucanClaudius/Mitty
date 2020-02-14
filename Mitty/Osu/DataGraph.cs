using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Mitty.Osu
{
    abstract class DataGraph
    {

        public Image Graph;

        public Stream ToStream()
        {
            var stream = new MemoryStream();

            Graph.Save(stream, ImageFormat.Png);
            stream.Position = 0;

            return stream;
        }
    }
}
