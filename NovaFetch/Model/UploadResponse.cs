// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System.Text.Json.Serialization;

namespace NovaFetch.Model
{
    /// <summary>
    /// Response to upload from the server.
    /// </summary>
    public class UploadResponse
    {
        /// <summary>
        /// Gets or sets the status of the request.
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        [JsonPropertyName("errormessage")]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the submission id.
        /// </summary>
        [JsonPropertyName("subid")]
        public int JobId { get; set; }

        /// <summary>
        /// Gets or sets the hash.
        /// </summary>
        [JsonPropertyName("hash")]
        public string Hash { get; set; }
    }
}
