// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System;

namespace NovaFetch
{
    /// <summary>
    /// Manages the secret token to work with the session.
    /// </summary>
    public class TokenManager
    {
        /// <summary>
        /// The name of the environment variable holding the token.
        /// </summary>
        public const string TOKENVARIABLENAME = "novatoken";

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenManager"/> class.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when the environment variable is not set.</exception>
        public TokenManager()
        {
            FetchAndValidateToken();
        }

        /// <summary>
        /// Gets the token.
        /// </summary>
        public string Token { get; private set; }

        /// <summary>
        /// Gets the session identifier.
        /// </summary>
        public string Session { get; private set; }

        /// <summary>
        /// Ses the session.
        /// </summary>
        /// <param name="session">The session identifier.</param>
        public void SetSession(string session) => Session = session;

        /// <summary>
        /// Gets the token from the environment.
        /// </summary>
        private void FetchAndValidateToken() =>
            Token = Environment.GetEnvironmentVariable(TOKENVARIABLENAME) ??
                throw new ArgumentNullException($"Environment variable '{TOKENVARIABLENAME}' not found.");
    }
}
