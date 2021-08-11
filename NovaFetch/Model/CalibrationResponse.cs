// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System.Text.Json.Serialization;

namespace NovaFetch.Model
{
    /// <summary>
    /// The calibration response.
    /// </summary>
    public class CalibrationResponse
    {
        /// <summary>
        /// Gets or sets the right ascension.
        /// </summary>
        [JsonPropertyName("ra")]
        public double RightAscension { get; set; }

        /// <summary>
        /// Gets or sets the declination.
        /// </summary>
        [JsonPropertyName("dec")]
        public double Declination { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        [JsonPropertyName("width_arcsec")]
        public double Width { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        [JsonPropertyName("height_arcsec")]
        public double Height { get; set; }

        /// <summary>
        /// Gets or sets the radius.
        /// </summary>
        [JsonPropertyName("radius")]
        public double Radius { get; set; }

        /// <summary>
        /// Gets or sets the pixel scale.
        /// </summary>
        [JsonPropertyName("pixscale")]
        public double PixelScale { get; set; }

        /// <summary>
        /// Gets or sets the orientation.
        /// </summary>
        [JsonPropertyName("orientation")]
        public double Orientation { get; set; }
    }
}
