// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System.Text.Json.Serialization;

namespace NovaFetch.Model
{
    /// <summary>
    /// The response message for status query.
    /// </summary>
    public class StatusResponse
    {
        /// <summary>
        /// Gets or sets the time the processing started.
        /// </summary>
        [JsonPropertyName("processing_started")]
        public string Started { get; set; }

        /// <summary>
        /// Gets or sets the time the processing ended.
        /// </summary>
        [JsonPropertyName("processing_finished")]
        public string Finished { get; set; }

        /// <summary>
        /// Gets a value indicating whether processing is done.
        /// </summary>
        [JsonIgnore]
        public bool Done => Jobs.Length > 0 && Jobs[0] != null;

        /// <summary>
        /// Gets a value indicating whether plate-solving was successful.
        /// </summary>
        [JsonIgnore]
        public bool Success => Jobs.Length > 0;

        /// <summary>
        /// Gets or sets the list of calibration jobs.
        /// </summary>
        [JsonPropertyName("job_calibrations")]
        public int[][] JobCalibrations { get; set; }

        /// <summary>
        /// Gets or sets thet list of jobs.
        /// </summary>
        [JsonPropertyName("jobs")]
        public int?[] Jobs { get; set; }
    }
}
