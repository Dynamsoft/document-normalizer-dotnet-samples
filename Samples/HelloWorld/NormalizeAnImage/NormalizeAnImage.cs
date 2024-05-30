using Dynamsoft.CVR;
using Dynamsoft.License;
using Dynamsoft.Core;
using Dynamsoft.Utility;
using Dynamsoft.DDN;

namespace NormalizeAnImage
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int errorCode = 1;
            string errorMsg;
            errorCode = LicenseManager.InitLicense("DLS2eyJvcmdhbml6YXRpb25JRCI6IjIwMDAwMSJ9", out errorMsg);
            if (errorCode != (int)EnumErrorCode.EC_OK)
                Console.WriteLine("License initialization error: " + errorMsg);
            using (CaptureVisionRouter cvr = new CaptureVisionRouter())
            {
                string imageFile = "../../../../../../Images/sample-image.png";
                CapturedResult? result = cvr.Capture(imageFile, PresetTemplate.PT_DETECT_AND_NORMALIZE_DOCUMENT);
                if (result == null)
                {
                    Console.WriteLine("No normalized documents.");
                }
                else if (result.GetErrorCode() != 0)
                {
                    Console.WriteLine("Error: " + result.GetErrorCode() + "," + result.GetErrorString());
                }
                else
                {
                    NormalizedImagesResult? normalizedImagesResult = result.GetNormalizedImagesResult();
                    if (normalizedImagesResult != null)
                    {
                        NormalizedImageResultItem[] items = normalizedImagesResult.GetItems();
                        Console.WriteLine("Normalized " + items.Length + " documents");
                        foreach (NormalizedImageResultItem normalizedItem in items)
                        {
                            string outPath = "normalizedResult_" + Array.IndexOf(items, normalizedItem) + ".png";
                            ImageManager imageManager = new ImageManager();
                            var image = normalizedItem.GetImageData();
                            if (image != null)
                            {
                                errorCode = imageManager.SaveToFile(image, outPath);
                                if (errorCode == 0)
                                {
                                    Console.WriteLine("Document " + Array.IndexOf(items, normalizedItem) + " file: " + outPath);
                                }
                            }
                        }
                    }
                }
            }
            Console.WriteLine("Press any key to quit...");
            Console.Read();
        }
    }
}