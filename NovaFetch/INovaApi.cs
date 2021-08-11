// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System.Drawing;
using System.Threading.Tasks;
using NovaFetch.Model;

namespace NovaFetch
{
    /// <summary>
    /// API interface.
    /// </summary>
    public interface INovaApi
    {
        /// <summary>
        /// Gets an image from the service.
        /// </summary>
        /// <param name="jobId">The job id.</param>
        /// <param name="image">The image to fetch.</param>
        /// <param name="downloadPath">The path to write the file to.</param>
        /// <returns>The image buffer.</returns>
        Task DownloadImageAsync(string jobId, string image, string downloadPath);

        /// <summary>
        /// Login request.
        /// </summary>
        /// <returns>A value indicating whether the login was succcessful.</returns>
        Task<bool> LoginAsync();

        /// <summary>
        /// Uploads a file to the server.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <returns>A value indicating whether the upload was successful.</returns>
        Task<bool> UploadFileAsync(Configuration config);

        /// <summary>
        /// Gets the calibration data.
        /// </summary>
        /// <param name="jobId">The job id.</param>
        /// <returns>The calibration response.</returns>
        Task<CalibrationResponse> GetCalibrationDataAsync(string jobId);

        /// <summary>
        /// Gets the related objects data.
        /// </summary>
        /// <param name="jobId">The job id.</param>
        /// <returns>The related objects response.</returns>
        Task<ObjectsResponse> GetObjectsAsync(string jobId);

        /// <summary>
        /// Checks the status of a job.
        /// </summary>
        /// <param name="subId">The submission id.</param>
        /// <returns>The <see cref="StatusResponse"/>.</returns>
        Task<StatusResponse> CheckStatusAsync(string subId);
    }
}
