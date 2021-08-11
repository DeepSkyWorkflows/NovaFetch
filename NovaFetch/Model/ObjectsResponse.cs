// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System.Text.Json.Serialization;

namespace NovaFetch.Model
{
    /// <summary>
    /// The object response.
    /// </summary>
    public class ObjectsResponse
    {
        /// <summary>
        /// Gets or sets the objects in the field of view.
        /// </summary>
        [JsonPropertyName("objects_in_field")]
        public string[] Objects { get; set; }
    }
}
