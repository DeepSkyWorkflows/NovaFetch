// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System;
using NovaFetch;

var version = typeof(TokenManager).Assembly.GetName().Version.ToString();
Console.WriteLine($"NovaFetch v{version}");

var tokenManager = new TokenManager();
var configuration = new Configuration();
configuration.Parse(args);

if (configuration.HelpOnly)
{
    return;
}

var engine = new Engine(tokenManager, configuration);
await engine.RunAsync();
