using Dynamsoft.Core;
using Dynamsoft.CVR;
using Dynamsoft.Utility;
using Dynamsoft.DDN;
using Dynamsoft.License;
namespace NormalizeMultipleImages
{
    class MyCapturedResultReceiver : CapturedResultReceiver
    {
        private static int resultCount = 0;

        public override void OnNormalizedImagesReceived(NormalizedImagesResult result)
        {
            FileImageTag? tag = (FileImageTag?)result.GetOriginalImageTag();
            Console.WriteLine("File: " + tag.GetFilePath());
            string fileName = tag.GetFilePath();
            int pos = fileName.LastIndexOf('\\');
            if (pos < 0)
                pos = fileName.LastIndexOf('/');
            if (pos < 0)
            {
                fileName = fileName.Substring(pos + 1);
            }
            else
            {
                fileName = resultCount.ToString();
                resultCount++;
            }

            if (result.GetErrorCode() != (int)EnumErrorCode.EC_OK)
            {
                Console.WriteLine("Error: " + result.GetErrorString());
            }
            else
            {
                NormalizedImageResultItem[] items = result.GetItems();
                Console.WriteLine("Normalized " + items.Length + " documents");
                ImageManager imageManager = new ImageManager();
                foreach (NormalizedImageResultItem item in items)
                {
                    string outPath = "normalizeImage_" + fileName + "_" + Array.IndexOf(items, item) + ".png";
                    var image = item.GetImageData();
                    if (image != null)
                    {
                        int errorCode = imageManager.SaveToFile(image, outPath);
                        if (errorCode == 0)
                        {
                            Console.WriteLine("Document " + Array.IndexOf(items, item) + " file: " + outPath);
                        }
                    }
                }
                resultCount++;
            }
            Console.WriteLine();
        }
    }
    class MyImageSourceStateListener : IImageSourceStateListener
    {
        private CaptureVisionRouter? cvr = null;
        public MyImageSourceStateListener(CaptureVisionRouter cvr)
        {
            this.cvr = cvr;
        }

        public void OnImageSourceStateReceived(EnumImageSourceState state)
        {
            if (state == EnumImageSourceState.ISS_EXHAUSTED)
            {
                if (cvr != null)
                {
                    cvr.StopCapturing();
                }
            }
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            int errorCode = 1;
            string errorMsg;
            errorCode = LicenseManager.InitLicense("DLS2eyJvcmdhbml6YXRpb25JRCI6IjIwMDAwMSJ9", out errorMsg);
            if (errorCode != (int)EnumErrorCode.EC_OK)
            {
                Console.WriteLine("License initialization error: " + errorMsg);
            }
            using (CaptureVisionRouter cvr = new CaptureVisionRouter())
            using (DirectoryFetcher fetcher = new DirectoryFetcher())
            {
                fetcher.SetDirectory("../../../../../../Images");
                cvr.SetInput(fetcher);

                CapturedResultReceiver receiver = new MyCapturedResultReceiver();
                cvr.AddResultReceiver(receiver);

                MyImageSourceStateListener listener = new MyImageSourceStateListener(cvr);
                cvr.AddImageSourceStateListener(listener);

                errorCode = cvr.StartCapturing(PresetTemplate.PT_DETECT_AND_NORMALIZE_DOCUMENT, true, out errorMsg);
                if (errorCode != (int)EnumErrorCode.EC_OK)
                {
                    Console.WriteLine("error: " + errorMsg);
                }
            }
            Console.WriteLine("Press any key to quit...");
            Console.Read();
        }
    }
}