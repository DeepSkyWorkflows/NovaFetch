// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System.Linq;
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
        /// Gets the stage of the process.
        /// </summary>
        [JsonIgnore]
        public Stages Stage
        {
            get
            {
                if (Jobs != null && Jobs.Length > 0)
                {
                    return JobCalibrations.SelectMany(jc => jc)
                        .Any(jc => Jobs.Any(j => j == jc)) ?
                        Stages.Calibrated : Stages.JobProcessing;
                }

                if (Images.Length > 0)
                {
                    return Stages.ImageAccepted;
                }

                return Stages.RequestSubmitted;
            }
        }

        /// <summary>
        /// Gets or sets the image list.
        /// </summary>
        [JsonPropertyName("images")]
        public int[] Images { get; set; }

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
