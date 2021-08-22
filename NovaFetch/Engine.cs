// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NovaFetch.Api;
using NovaFetch.Model;

namespace NovaFetch
{
    /// <summary>
    /// Main engine to drive the process.
    /// </summary>
    public class Engine
    {
        private readonly TokenManager tokenManager;
        private readonly Configuration config;
        private readonly INovaApi api;

        /// <summary>
        /// Initializes a new instance of the <see cref="Engine"/> class.
        /// </summary>
        /// <param name="manager">The <see cref="TokenManager"/>.</param>
        /// <param name="configuration">The configuration.</param>
        public Engine(TokenManager manager, Configuration configuration)
        {
            tokenManager = manager;
            config = configuration;
            api = new NovaApi(manager);
        }

        /// <summary>
        /// Run the engine.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task RunAsync()
        {
            try
            {
                if (await LoginAsync())
                {
                    ShowConfig();

                    if (!config.ExistingJob)
                    {
                        await UploadAsync();
                    }

                    if (!config.SubmitOnly)
                    {
                        var result = await JobStatusAsync();
                        if (result.Stage != Stages.Calibrated)
                        {
                            Console.WriteLine("Failed to plate solve.");
                            return;
                        }

                        var jobId = result.Jobs[0].Value;

                        await DownloadFilesAsync(jobId);
                        await DownloadCalibrationAsync(jobId);

                        Console.WriteLine("Final tasks...");

                        Console.WriteLine("Copying original file...");
                        var tgtPath = Path.Combine(config.TargetDirectory, $"{config.Name}.jpg");
                        File.Copy(config.FilePath, tgtPath);
                        Console.WriteLine("Creating thumbnail...");
                        var image = Image.FromFile(tgtPath);
                        var ratio = image.Width / 256;
                        int height = image.Height / ratio;
                        var thumb = image.GetThumbnailImage(256, height, null, IntPtr.Zero);
                        var thumbPath = Path.Combine(config.TargetDirectory, "thumb.jpg");
                        thumb.Save(thumbPath);
                    }

                    Console.WriteLine("Done.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Oops! Something unexpected happened.");
                Console.WriteLine($"The error: {ex.Message}.");
            }
        }

        private static string DegreesToRA(double deg)
        {
            var h = 0;
            double v = 360.0 / 24;
            while (deg >= v)
            {
                h++;
                deg -= v;
            }

            var m = 0;
            v = 15.0 / 60;
            while (deg >= v)
            {
                m++;
                deg -= v;
            }

            var s = 0;
            v = 0.25 / 60;
            while (deg >= v)
            {
                s++;
                deg -= v;
            }

            var ds = 0;
            v = 0.25 / 60 / 10;
            while (deg >= v)
            {
                ds++;
                deg -= v;
            }

            return $"{h}h {m}m {s}.{ds}s";
        }

        private static string DecToDegrees(double dec)
        {
            var sign = dec < 0 ? "-" : "+";
            dec = Math.Abs(dec);
            var degrees = Math.Floor(dec);
            var remainder = dec - degrees;
            var arcmin = remainder * 60;
            var arcminValue = Math.Floor(arcmin);
            var arcminDisplay = arcminValue < 10 ? $"0{(int)arcminValue}" : ((int)arcminValue).ToString();
            remainder = arcmin - arcminValue;
            var arcseconds = Math.Round(remainder * 60, 3);
            return $"{sign}{(int)degrees}° {arcminDisplay}' {arcseconds}";
        }

        private async Task DownloadCalibrationAsync(int jobId)
        {
            var calibration = await api.GetCalibrationDataAsync(jobId.ToString());
            var objects = await api.GetObjectsAsync(jobId.ToString());
            var dataFile = new List<string>
            {
                "---",
                $"title: \"{config.Name}\"",
                "type:",
                "tags: [" +
                string.Join(",", objects.Objects.Select(o => $"\"{o}\"").ToArray()) +
                "]",
                "description:",
                $"image: /assets/images/gallery/{config.Name}/thumb.jpg",
                "telescope: Stellina",
                "length: \"400mm\"",
                "aperture: \"80mm\"",
                $"folder: {config.TargetDirectory.Split(Path.DirectorySeparatorChar)[^1]}",
                "exposure: ",
                "lights: ",
                "sessions: ",
                "firstCapture: ",
                "lastCapture:",
                $"ra: \"{DegreesToRA(calibration.RightAscension)}\"",
                $"dec: \"{DecToDegrees(calibration.Declination)}\"",
                $"size: \"{Math.Round(calibration.Width / 60, 3)} x {Math.Round(calibration.Height / 60, 3)} arcmin\"",
                $"radius: \"{Math.Round(calibration.Radius, 3)} deg\"",
                $"scale: \"{Math.Round(calibration.PixelScale, 3)} arcsec/pixel\"",
                "---",
            };
            var fileName = Path.Combine(config.TargetDirectory, $"{config.Name}.md");
            Console.WriteLine(string.Join(Environment.NewLine, dataFile));
            Console.WriteLine($"Writing data to {fileName}");
            await File.WriteAllLinesAsync(fileName, dataFile);
        }

        private async Task DownloadFilesAsync(int jobId)
        {
            Console.WriteLine("Downloading result files...");
            var files = new (string src, string tgt)[]
            {
                ("annotated_display", "-annotated"),
                ("grid_display", "-grid"),
                ("annotated_full", "-annotated-fs"),
            };

            foreach (var (src, tgt) in files)
            {
                var tgtFileName = $"{config.Name}{tgt}.jpg";
                var tgtPath = Path.Combine(config.TargetDirectory, tgtFileName);
                Console.WriteLine($"Downloading {tgtFileName}...");
                await api.DownloadImageAsync(jobId.ToString(), src, tgtPath);
            }
        }

        private async Task<StatusResponse> JobStatusAsync()
        {
            Console.WriteLine($"Getting status for job {config.JobId}...");
            var done = false;
            StatusResponse status = null;
            Stages stage = Stages.None;
            var timeOut = DateTime.Now.AddMinutes(15);
            while (!done && DateTime.Now < timeOut)
            {
                status = await api.CheckStatusAsync(config.JobId);
                done = status.Stage == Stages.Calibrated;
                if ((int)status.Stage > (int)stage)
                {
                    stage = status.Stage;
                    Console.WriteLine($"{Environment.NewLine}{stage}");
                }

                Thread.Sleep(1000);
                Console.Write(".");
            }

            // wind-down an extra 10 seconds
            Thread.Sleep(10000);

            return status;
        }

        private async Task UploadAsync()
        {
            Console.WriteLine($"Uploading {config.FilePath} to Nova...");
            await api.UploadFileAsync(config);
        }

        /// <summary>
        /// Perform the login to retrieve the session.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        private async Task<bool> LoginAsync()
        {
            var result = await api.LoginAsync();
            if (result)
            {
                Console.WriteLine($"Established session with id {tokenManager.Session}.");
            }

            return result;
        }

        private void ShowConfig()
        {
            if (!config.ExistingJob)
            {
                Console.WriteLine($"Target file: {Path.GetFileName(config.FilePath)}");
            }

            if (!config.SubmitOnly)
            {
                Console.WriteLine($"Target directory: {config.TargetDirectory}");
            }
        }
    }
}
