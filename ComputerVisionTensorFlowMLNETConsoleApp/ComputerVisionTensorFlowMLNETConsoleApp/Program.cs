using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Hosting;

namespace ComputerVisionTensorFlowMLNETConsoleApp
{
    class Program
    {
        static void Error(string msg)
        {
            Console.WriteLine("Error: {0}", msg);
            Environment.Exit(1);
        }

        static bool jagged = true;

        static string dnnModelsFolder, dnnModelFile, labelsFile;
        static string imagesFolder;

        static AppSettings settings;
        static IHostingEnvironment environment;

       
        static void Main(string[] args)
        {
            bool downloadAndUpdatePreTrainedModel = false;

            dnnModelsFolder = "DNNModels";
            imagesFolder = "ImagesForInference";

            List<string> imageFileNamesToInfer = new List<string>() { "ImagesForInference/green-frisbee.jpg" };

            if (downloadAndUpdatePreTrainedModel)
                DownloadInceptionv3ModelFiles(dnnModelsFolder);

            //Get App Settings from appsettings.json and other App's configuration
            InitializeAppSettings();

            // Run inference on the image files
            foreach (var fileName in imageFileNamesToInfer)
            {
                IEnumerable<string> tagsForCurrentImage = null;

                //var filePathName = Path.Combine(imagesFolder, "/", fileName);
                var filePathName = fileName;

                var imageBytes = File.ReadAllBytes(filePathName);

                if (!imageBytes.IsValidImage())
                {
                    Console.WriteLine($"Error: UnsupportedMediaType");
                    Environment.Exit(1);
                }                

                //ITensorFlowPredictionStrategy predictionServices = new TensorFlowPredictionStrategy(settings,
                //                                                                                    environment);

                //tagsForCurrentImage = await predictionServices.ClassifyImageAsync(imageData);


            }
        }
        
        // If using Dependency Injection in an IHost this code would not be necessary
        static void InitializeAppSettings()
        {
            //Build Configuration Environment for Console App

            IConfiguration configuration = ConfigurationFactory.CreateConfiguration();

            settings = new AppSettings();
            settings.AIModelsPath = configuration["AIModelsPath"];
            settings.TensorFlowPredictionDefaultModel = configuration["TensorFlowPredictionDefaultModel"];


            environment = new HostingEnvironment();

            environment.EnvironmentName = "DEVELOPMENT";
            environment.ApplicationName = "ComputerVisionTensorFlowMLNETConsoleApp";
            environment.ContentRootPath = Directory.GetCurrentDirectory();
        }

        //
        // Downloads the inception graph and labels
        //
        static void DownloadInceptionv3ModelFiles(string dir)
        {
            string url = "https://storage.googleapis.com/download.tensorflow.org/models/inception5h.zip";

            dnnModelFile = Path.Combine(dnnModelsFolder, "tensorflow_inception_graph.pb");
            labelsFile = Path.Combine(dnnModelsFolder, "imagenet_comp_graph_label_strings.txt");
            var licenseFile = Path.Combine(dnnModelsFolder, "LICENSE");
            var zipfile = Path.Combine(dnnModelsFolder, "inception5h.zip");

            //Directory.CreateDirectory(dnnModelsFolder);

            //Delete existing files since we want to update with downloaded files
            if (File.Exists(dnnModelFile))
                File.Delete(dnnModelFile);

            if (File.Exists(labelsFile))
                File.Delete(labelsFile);

            if (File.Exists(licenseFile))
                File.Delete(licenseFile);

            if (File.Exists(zipfile))
                File.Delete(zipfile);

            //Download .ZIP file with model's related files
            var wc = new WebClient();
            wc.DownloadFile(url, zipfile);
            ZipFile.ExtractToDirectory(zipfile, dnnModelsFolder);
            File.Delete(zipfile);
        }

    }

    public static class ImageValidationExtensions
    {
        public static bool IsValidImage(this byte[] image)
        {
            var imageFormat = GetImageFormat(image);
            return imageFormat == ImageFormat.jpeg ||
                   imageFormat == ImageFormat.png;
        }

        public enum ImageFormat
        {
            bmp,
            jpeg,
            gif,
            tiff,
            png,
            unknown
        }

        private static ImageFormat GetImageFormat(byte[] bytes)
        {
            // see http://www.mikekunz.com/image_file_header.html  
            var bmp = Encoding.ASCII.GetBytes("BM");     // BMP
            var gif = Encoding.ASCII.GetBytes("GIF");    // GIF
            var png = new byte[] { 137, 80, 78, 71 };    // PNG
            var tiff = new byte[] { 73, 73, 42 };         // TIFF
            var tiff2 = new byte[] { 77, 77, 42 };         // TIFF
            var jpeg = new byte[] { 255, 216, 255, 224 }; // jpeg
            var jpeg2 = new byte[] { 255, 216, 255, 225 }; // jpeg canon

            if (bmp.SequenceEqual(bytes.Take(bmp.Length)))
                return ImageFormat.bmp;

            if (gif.SequenceEqual(bytes.Take(gif.Length)))
                return ImageFormat.gif;

            if (png.SequenceEqual(bytes.Take(png.Length)))
                return ImageFormat.png;

            if (tiff.SequenceEqual(bytes.Take(tiff.Length)))
                return ImageFormat.tiff;

            if (tiff2.SequenceEqual(bytes.Take(tiff2.Length)))
                return ImageFormat.tiff;

            if (jpeg.SequenceEqual(bytes.Take(jpeg.Length)))
                return ImageFormat.jpeg;

            if (jpeg2.SequenceEqual(bytes.Take(jpeg2.Length)))
                return ImageFormat.jpeg;

            return ImageFormat.unknown;
        }
    }
}
