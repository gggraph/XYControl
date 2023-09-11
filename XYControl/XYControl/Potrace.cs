using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Drawing;

namespace XYControl
{
    public class PoTrace
    {
        /*
    We need additionnal Options here
         SVG options:

   --group                    - group related paths together
   --flat                     - whole image as a single path

  Frontend options:
   -k, --blacklevel <n>       - black/white cutoff in input file (default 0.5)

  Algorithm options:
   -z, --turnpolicy <policy>  - how to resolve ambiguities in path decomposition
   -t, --turdsize <n>         - suppress speckles of up to this size (default 2)
   -a, --alphamax <n>         - corner threshold parameter (default 1) !!!!! 
   -n, --longcurve            - turn off curve optimization
   -O, --opttolerance <n>     - curve optimization tolerance (default 0.2)
   -u, --unit <n>             - quantize output to 1/unit pixels (default 10)

         */

        public static void CreateShellExecution(List<string> commands)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.CreateNoWindow = true;

            process.Start();
            StreamWriter sw = process.StandardInput;
            foreach (string s in commands)
            {
                sw.WriteLine(s);
            }
            sw.WriteLine("exit");

            process.Close();

        }

        public static string BuildSVGFromImage
            (
            string potraceDirectory,
            string imageFilePath,
            string outputDirectory
            )
        {
            string bmpFilePath = imageFilePath;
            if (!imageFilePath.EndsWith(".bmp"))
            {
                using (Image inputImage = Image.FromFile(imageFilePath))
                {
                    // Create a Bitmap from the input image to ensure it's in a format that can be saved as BMP
                    Bitmap bmpImage = new Bitmap(inputImage);

                    // Save the Bitmap as a BMP image
                    bmpImage.Save(
                        Path.Combine(outputDirectory, new FileInfo(imageFilePath).Name + ".bmp"),
                        System.Drawing.Imaging.ImageFormat.Bmp);

                    // Dispose of the Bitmap
                    bmpImage.Dispose();
                    bmpFilePath = Path.Combine(outputDirectory, new FileInfo(imageFilePath).Name + ".bmp");
                }
            }
            return BuildSVGFromBitmap(potraceDirectory, bmpFilePath, outputDirectory);

        }

        public static string BuildSVGFromBitmap(string potraceDirectory, string bmpFilePath, string outputDirectory)
        {
            FileInfo fInfo = new FileInfo(bmpFilePath);
            int fileIndex = 0;
            string outputFileName = fInfo.Name + ".svg";
            while (File.Exists(Path.Combine(outputDirectory, outputFileName)))
            {
                outputFileName = fInfo.Name + "_" + fileIndex;
                fileIndex++;
            }
            string outputFilePath = Path.Combine(outputDirectory, outputFileName);
            List<string> commands = new List<string>()
            {
                "cd "+ potraceDirectory,
                $"potrace -s \"{bmpFilePath}\" -o \"{outputFilePath}\"",
                ":quit"
            };
            CreateShellExecution(commands);
            bool fileReady = false;
            while (!fileReady)
            {
                try
                {
                    using (FileStream fileStream = new FileStream(outputFilePath,
                        FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                    {
                        Console.WriteLine("SVG is created...");
                    }
                    fileReady = true;
                    break;
                }
                catch (System.Exception e) { }
                Thread.Sleep(10);
            }
            // @ Remove doc type node in xml 
            string svgText = File.ReadAllText(outputFilePath);
            int index = svgText.IndexOf("<!DOCTYPE");
            if (index >= 0)
            {
                string part = svgText.Substring(index);
                index = part.IndexOf(">");
                string def = part.Substring(0, index + 1);
                Console.WriteLine(def);
                svgText = svgText.Replace(def, string.Empty);
                File.WriteAllText(outputFilePath, svgText);
            }
            return outputFilePath;
        }
    }
}
