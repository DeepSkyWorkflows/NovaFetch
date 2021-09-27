// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using NovaFetch.Model;

namespace NovaFetch.Api
{
    /// <summary>
    /// Implementation of the Nova API (http://astrometry.net/doc/net/api.html).
    /// </summary>
    public class NovaApi : INovaApi
    {
        /// <summary>
        /// Success value in login status.
        /// </summary>
        private const string SUCCESS = "success";

        /// <summary>
        /// The HTTP client.
        /// </summary>
        private readonly HttpClient client;

        /// <summary>
        /// The token manager.
        /// </summary>
        private readonly TokenManager tokenManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="NovaApi"/> class.
        /// </summary>
        /// <param name="manager">The <see cref="TokenManager"/>.</param>
        public NovaApi(TokenManager manager)
        {
            tokenManager = manager;
            client = new HttpClient { BaseAddress = new Uri("http://nova.astrometry.net/api/") };
        }

        /// <summary>
        /// Checks the status of a submission.
        /// </summary>
        /// <param name="subId">The submission id.</param>
        /// <returns>The result.</returns>
        public async Task<StatusResponse> CheckStatusAsync(string subId)
        {
            var result = await client.GetAsync($"submissions/{subId}");
            result.EnsureSuccessStatusCode();
            var jsonResult = await result.Content.ReadAsStringAsync();
            try
            {
                var response = JsonSerializer.Deserialize<StatusResponse>(jsonResult);
                return response;
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to deserialize:");
                Console.WriteLine(jsonResult);
                throw;
            }
        }

        /// <summary>
        /// Gets an image.
        /// </summary>
        /// <param name="jobId">The job id.</param>
        /// <param name="image">The image name.</param>
        /// <param name="downloadPath">The path to save the image to.</param>
        /// <returns>The task.</returns>
        public async Task DownloadImageAsync(string jobId, string image, string downloadPath)
        {
            var imageUrl = $"http://nova.astrometry.net/{image}/{jobId}";
            var retries = 1;
            while (retries > 0)
            {
                var result = await client.GetAsync(imageUrl);
                try
                {
                    result.EnsureSuccessStatusCode();
                    var bytes = await result.Content.ReadAsByteArrayAsync();
                    await File.WriteAllBytesAsync(downloadPath, bytes);
                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unexpected error: {ex.Message}");
                    if (retries == 0)
                    {
                        throw;
                    }
                    else
                    {
                        retries--;
                        Console.WriteLine("Retrying in 1 second...");
                        Thread.Sleep(1000);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the calibration data.
        /// </summary>
        /// <param name="jobId">The job id.</param>
        /// <returns>The data.</returns>
        public async Task<CalibrationResponse> GetCalibrationDataAsync(string jobId)
        {
            var result = await client.GetAsync($"jobs/{jobId}/calibration/");
            result.EnsureSuccessStatusCode();
            var json = await result.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<CalibrationResponse>(json);
        }

        /// <summary>
        /// Get the related objects.
        /// </summary>
        /// <param name="jobId">The job id.</param>
        /// <returns>The related objects.</returns>
        public async Task<ObjectsResponse> GetObjectsAsync(string jobId)
        {
            var result = await client.GetAsync($"jobs/{jobId}/objects_in_field/");
            result.EnsureSuccessStatusCode();
            var json = await result.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ObjectsResponse>(json);
        }

        /// <summary>
        /// Login.
        /// </summary>
        /// <returns>A value indicating whether the login was successful.</returns>
        public async Task<bool> LoginAsync()
        {
            var request = new LoginRequest
            {
                ApiKey = tokenManager.Token,
            };

            var response = await ApiCallAsync<LoginResponse, LoginRequest>("login", request);
            if (response == null)
            {
                throw new ArgumentNullException("The login response was empty or unrecognizable.");
            }

            if (response.Status != SUCCESS)
            {
                throw new InvalidOperationException($"Unable to login. Status: {response.Status} Message: {response.ErrorMessage}");
            }

            tokenManager.SetSession(response.Session);
            return true;
        }

        /// <summary>
        /// Uploads a file to the server.
        /// </summary>
        /// <param name="config">The current <see cref="Configuration"/>.</param>
        /// <returns>A value indicating whether the upload was successful.</returns>
        public async Task<bool> UploadFileAsync(Configuration config)
        {
            var request = new UploadRequest
            {
                Session = tokenManager.Session,
            };

            var result = await ApiCallAsync<UploadResponse, UploadRequest>("upload", request, config);

            if (result.Status != SUCCESS)
            {
                throw new InvalidOperationException($"Uploaded failed with status: {result.Status}");
            }

            config.JobId = result.JobId.ToString();
            Console.WriteLine($"Success! Submission id is {config.JobId}");
            return true;
        }

        /// <summary>
        /// Makes the api call.
        /// </summary>
        /// <typeparam name="TResponse">The type of the expected return.</typeparam>
        /// <typeparam name="TRequest">The type of the request.</typeparam>
        /// <param name="action">The action, like "login".</param>
        /// <param name="value">The value of the request.</param>
        /// <param name="config">Optional config for file path to upload.</param>
        /// <returns>The result.</returns>
        private async Task<TResponse> ApiCallAsync<TResponse, TRequest>(string action, TRequest value, Configuration config = null)
        {
            var json = JsonSerializer.Serialize(value);

            HttpResponseMessage result = null;

            if (config == null)
            {
                var formVariables = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("request-json", json),
                };

                var formContent = new FormUrlEncodedContent(formVariables);

                result = await client.PostAsync(action, formContent);
            }
            else
            {
                if (!File.Exists(config.FilePath))
                {
                    throw new FileNotFoundException(config.FilePath);
                }

                var formContent = new StringContent(json);
                formContent.Headers.ContentType = MediaTypeHeaderValue.Parse("text/plain");
                formContent.Headers.TryAddWithoutValidation("Content-Disposition", "form-data; name=\"request-json\"");

                using var fileContent = new ByteArrayContent(await File.ReadAllBytesAsync(config.FilePath));
                fileContent.Headers.TryAddWithoutValidation("Content-Type", "octet-stream");
                fileContent.Headers.TryAddWithoutValidation("Content-Disposition", $"form-data; name=\"file\"; filename=\"{config.Name}.jpg\"");

                using var form = new MultipartFormDataContent
                {
                    formContent,
                    fileContent,
                };

                result = await client.PostAsync(action, form);
            }

            result.EnsureSuccessStatusCode();

            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var jsonResult = await result.Content.ReadAsStringAsync();
                try
                {
                    return JsonSerializer.Deserialize<TResponse>(jsonResult);
                }
                catch (Exception)
                {
                    Console.WriteLine("Failed to deserialize:");
                    Console.WriteLine(jsonResult);
                    throw;
                }
            }

            return default;
        }
    }
}
