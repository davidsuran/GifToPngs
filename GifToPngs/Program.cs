using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace GifToPngs
{
    internal class Program
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        static void Main(string[] args)
        {
            //System.Diagnostics.Debugger.Launch();

            if (args.Length == 0)
            {
                return;
            }

            try
            {
                foreach (string path in args)
                {
                    if (path.StartsWith("-"))
                    {
                        continue;
                    }

                    Convert(path);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Converts the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        private static void Convert(string path)
        {
            Image img = Image.FromFile(path);
            FrameDimension dimension = new FrameDimension(img.FrameDimensionsList[0]);
            int frameCount = img.GetFrameCount(dimension);

            path = FormattableString.Invariant($"{Path.GetDirectoryName(path)}/{Sanitize(Path.GetFileNameWithoutExtension(path), Path.GetInvalidFileNameChars())}");
            Directory.CreateDirectory(path);

            for (int i = 0; i < frameCount; i++)
            {
                img.SelectActiveFrame(dimension, i);
                string outputPathWithFileName = FormattableString.Invariant($"{path}/frame{i}.png");
                using (MemoryStream memory = new MemoryStream())
                {
                    img.Save(memory, ImageFormat.Png);
                    File.WriteAllBytes(outputPathWithFileName, memory.ToArray());
                }
            }
        }

        /// <summary>
        /// Sanitizes the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="invalidChars">The invalid chars.</param>
        /// <returns></returns>
        public static string Sanitize(string input, char[] invalidChars)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (char c in input)
            {
                stringBuilder.Append(invalidChars.Contains(c) ? '_' : c);
            }

            return stringBuilder.ToString();
        }
    }
}
