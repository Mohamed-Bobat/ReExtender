/*
    ReExtender extension replacer tool
    Copyright (C) 2021  Mohamed Bobat

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using CommandLine;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;

namespace ReExtender
{
    /// <summary>
    /// the class for the command line options
    /// </summary>
    class Options
    {
        /// <summary>
        /// the argument for the path to the files 
        /// </summary>
        [Option(longName: "path",shortName: 'p', HelpText = "path to the directory", Required = true)]
        public string Path { get; set; }

        /// <summary>
        /// the argument for the regEx filter
        /// </summary>
        [Option(longName: "regEx", shortName: 'r', SetName ="filter",  HelpText = "RegEx filter string. Mutually exclusive with --filter.",Required = false)]
        public string RegEx { get; set; }

        /// <summary>
        /// the argument for the filter
        /// </summary>
        [Option(longName: "filter", shortName:'f', SetName = "filter", HelpText = "Filter string. Mutally exclusive with --regEx.", Required = false)]
        public string Filter { get; set; }
    }

    /// <summary>
    /// the main class
    /// </summary>
    class ReExtender
    {
        /// <summary>
        /// the main method
        /// </summary>
        /// <param name="args">the command line arguments</param>
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed(RunOptions);
        }

        /// <summary>
        /// handles the parsed options
        /// </summary>
        /// <param name="opts"> list of options</param>
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
                throw new DirectoryNotFoundException(opts.Path + " not found");
            }
        }

        /// <summary>
        /// filters list of file paths using the provided regEx expressions
        /// </summary>
        /// <param name="paths">list of file paths</param>
        /// <param name="regex">regex expression</param>
        /// <returns></returns>
        private static string[] RegexFilter(string[] paths, Regex regex)
        {
            List<string> FilteredNames = new List<string>();
            foreach (string path in paths)
            {
                if (regex.IsMatch(path))
                {
                    FilteredNames.Add(path);
                }
            }
            return FilteredNames.ToArray();
        }
    }

}
