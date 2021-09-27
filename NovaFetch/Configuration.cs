// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System;
using System.Collections.Generic;
using System.IO;

namespace NovaFetch
{
    /// <summary>
    /// Represents the configuration for a run.
    /// </summary>
    public class Configuration
    {
        private const string SHORTHELP = "-h";
        private const string LONGHELP = "--help";
        private const string SHORTSUBMIT = "-s";
        private const string LONGSUBMIT = "--submit-only";
        private const string SHORTEXISTING = "-e";
        private const string LONGEXISTING = "--existing";

        private static readonly List<(string shortSwitch, string longSwitch, string description)>
            CommandLineSwitches = new ()
            {
                (SHORTHELP, LONGHELP,
                "Display help. Use <option> -h for context-specific help.~Usage: NovaFetch [name] [pathToFile] <targetDir>"),
                (SHORTSUBMIT, LONGSUBMIT, "Only submit. Don't wait on status.~Usage: NovaFetch -s [pathToFile] <targetDir>"),
                (SHORTEXISTING, LONGEXISTING, "Start with existing job.~Usage: NovaFetch -e [jobId] [name] [pathToFile] <targetDir>"),
            };

        /// <summary>
        /// Gets a value indicating whether the current run is just for help display.
        /// </summary>
        public bool HelpOnly { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the tool should just submit and stop.
        /// </summary>
        public bool SubmitOnly { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the tool should query an existing job.
        /// </summary>
        public bool ExistingJob { get; private set; }

        /// <summary>
        /// Gets or sets the job id for the job to query.
        /// </summary>
        public string JobId { get; set; }

        /// <summary>
        /// Gets the path to the file to plate solve.
        /// </summary>
        public string FilePath { get; private set; }

        /// <summary>
        /// Gets the root name for the target files.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the target directory to place results.
        /// </summary>
        public string TargetDirectory { get; private set; }

        /// <summary>
        /// Parses settings from the command line.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        public void Parse(string[] args)
        {
            if (args == null || args.Length < 1 || args[0] == null)
            {
                Console.WriteLine("Nothing to do!");
                Help();
                HelpOnly = true;
                return;
            }

            var pos = 0;

            if (args[0].StartsWith("-"))
            {
                var cmd = args[0];
                pos = 1;

                switch (cmd.ToLowerInvariant().Trim())
                {
                    case SHORTHELP:
                    case LONGHELP:
                        HelpOnly = true;
                        Help();
                        return;

                    case SHORTSUBMIT:
                    case LONGSUBMIT:
                        Console.WriteLine("Submission only. Will not stick around.");
                        SubmitOnly = true;
                        break;

                    case SHORTEXISTING:
                    case LONGEXISTING:
                        Console.WriteLine("Existing only. Will query existing job.");
                        ExistingJob = true;
                        break;

                    default:
                        Console.WriteLine($"Unknown command or switch: {cmd}");
                        HelpOnly = true;
                        Help();
                        return;
                }
            }

            while (pos < args.Length)
            {
                var cmd = args[pos].ToLowerInvariant().Trim();
                pos++;

                if (ExistingJob && string.IsNullOrWhiteSpace(JobId))
                {
                    JobId = cmd;
                    Console.WriteLine($"Will query existing job id: {JobId}");
                    continue;
                }

                if (!SubmitOnly && string.IsNullOrWhiteSpace(Name))
                {
                    Name = cmd;
                    Console.WriteLine($"Using target filename: {Name}");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(FilePath))
                {
                    if (!File.Exists(cmd))
                    {
                        throw new FileNotFoundException(cmd);
                    }

                    FilePath = cmd;
                    continue;
                }

                if (!Directory.Exists(cmd))
                {
                    throw new DirectoryNotFoundException(cmd);
                }

                TargetDirectory = cmd;
            }

            Validate();
        }

        /// <summary>
        /// Sets the job id.
        /// </summary>
        /// <param name="jobId">The job id.</param>
        public void SetJob(string jobId)
        {
            ExistingJob = true;
            JobId = jobId;
        }

        private static void Help()
        {
            Console.WriteLine("Abbreviated\tCommand\tDescription");
            foreach (var (shortCmd, longCmd, description) in CommandLineSwitches)
            {
                var desc = description.Replace("~", Environment.NewLine);
                Console.WriteLine($"{shortCmd}\t\t{longCmd}\t{desc}");
            }
        }

        private void Validate()
        {
            if (ExistingJob && string.IsNullOrWhiteSpace(JobId))
            {
                throw new ArgumentNullException("Job id is required for --existing options.");
            }

            if (string.IsNullOrWhiteSpace(FilePath))
            {
                throw new ArgumentNullException("File path is required for upload.");
            }

            if (string.IsNullOrWhiteSpace(TargetDirectory))
            {
                TargetDirectory = Environment.CurrentDirectory;
            }
        }
    }
}
