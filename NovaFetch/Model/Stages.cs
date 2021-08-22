// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

namespace NovaFetch.Model
{
    /// <summary>
    /// Stages for plate-solving.
    /// </summary>
    public enum Stages
    {
        /// <summary>
        /// No stage.
        /// </summary>
        None = -1,

        /// <summary>
        /// The request was successfully submitted.
        /// </summary>
        RequestSubmitted = 0,

        /// <summary>
        /// The images were processed.
        /// </summary>
        ImageAccepted = 1,

        /// <summary>
        /// The images are being plate-solved.
        /// </summary>
        JobProcessing = 2,

        /// <summary>
        /// Successfully calibrated.
        /// </summary>
        Calibrated = 3,
    }
}
