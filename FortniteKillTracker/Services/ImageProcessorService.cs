using Microsoft.Extensions.Logging.Abstractions;
using OpenCvSharp;
using Tesseract;

namespace FortniteKillTracker.Services
{
    public class ImageProcessorService
    {
        public List<string> ExtractTextFromImage(Mat image)
        {
            List<string> extractedText = new List<string>();

            using var pix = Pix.LoadFromMemory(image.ToMemoryStream().ToArray());


            using (TesseractEngine engine = new TesseractEngine(@"", "eng", EngineMode.Default))
            {
                using var page = engine.Process(pix);
                var text = page.GetText();

                // Split the extracted text by newlines and add each line to the list
                string[] lines = text.Split('\n');
                foreach (string line in lines)
                {
                    if (line.Contains("knocked out") || line.Contains("eliminated"))
                    {
                        extractedText.Add(line);
                    }
                }
            }
            Console.Clear();
            foreach (var item in extractedText)
            {
                Console.WriteLine(item);
            }
            Task.Delay(500).Wait();
            return extractedText;
        }

    }
}