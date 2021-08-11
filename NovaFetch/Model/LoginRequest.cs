// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System.Text.Json.Serialization;

namespace NovaFetch.Model
{
    /// <summary>
    /// Login request.
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// Gets or sets the api key.
        /// </summary>
        [JsonPropertyName("apikey")]
        public string ApiKey { get; set; }
    }
}
