// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System.Text.Json.Serialization;

namespace NovaFetch.Model
{
    /// <summary>
    /// Request to upload a file for plate solving.
    /// </summary>
    public class UploadRequest
    {
        /// <summary>
        /// Gets or sets the session identifier.
        /// </summary>
        [JsonPropertyName("session")]
        public string Session { get; set; }

        /// <summary>
        /// Gets or sets whether commercial use is allowed.
        /// </summary>
        [JsonPropertyName("allow_commercial_use")]
        public string AllowCommercialUse { get; set; } = "n";

        /// <summary>
        /// Gets or sets whether modifications are allowed.
        /// </summary>
        [JsonPropertyName("allow_modifications")]
        public string AllowModifications { get; set; } = "n";

        /// <summary>
        /// Gets or sets whether the submission is visible to the public.
        /// </summary>
        [JsonPropertyName("publicly_visible")]
        public string Public { get; set; } = "y";
    }
}
