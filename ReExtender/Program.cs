using CommandLine;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;

namespace ReExtender
{

    class Options
    {
        [Option("path", HelpText = "path to the directory", Required = true)]
        public string Path { get; set; }

        [Option("regEx", HelpText = "regEx filter string", Required = false)]
        public string RegEx { get; set; }

        [Option("filter", HelpText = "filter string", Required = false)]
        public string Filter { get; set; }
    }

    class ReExtender
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(RunOptions)
                .WithNotParsed(HandleParseError);
        }

        private static void RunOptions(Options opts)
        {
            if (Directory.Exists(opts.Path))
            {
                string[] FileNames;
                if (opts.Filter != null)
                {
                    FileNames = Directory.GetFiles(opts.Path, opts.Filter);
                }
                else if (opts.RegEx != null)
                {
                    FileNames = Directory.GetFiles(opts.Path);
                    Regex searchTerm = new Regex(opts.RegEx);
                    FileNames = RegexFilter(FileNames, searchTerm);
                }
                else
                {
                    FileNames = Directory.GetFiles(opts.Path);
                }
                var ReplacementDictionary = ConfigurationManager.AppSettings;
                foreach (string FileName in FileNames)
                {
                    string NewFileName = FileName;
                    foreach (var Key in ReplacementDictionary)
                    {
                        NewFileName = NewFileName.Replace(Key.ToString(), ReplacementDictionary.Get(Key.ToString()));
                    }
                    File.Move(FileName, NewFileName);
                }
            }
            else
            {
                throw new Exception("Incorrect Path");
            }
        }

        private static void HandleParseError(IEnumerable<Error> errs)
        {
            //handle errors
        }

        private static string[] RegexFilter(string[] names, Regex regex)
        {
            List<string> FilteredNames = new List<string>();
            foreach (string name in names)
            {
                if (regex.IsMatch(name))
                {
                    FilteredNames.Add(name);
                }
            }
            return FilteredNames.ToArray();
        }
    }

}
