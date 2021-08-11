// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System.Text.Json.Serialization;

namespace NovaFetch.Model
{
    /// <summary>
    /// Login response from the API.
    /// </summary>
    public class LoginResponse
    {
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the message from the login server.
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        [JsonPropertyName("errormessage")]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the session.
        /// </summary>
        [JsonPropertyName("session")]
        public string Session { get; set; }
    }
}
