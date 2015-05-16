// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Repository.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2015 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace RepositoryCleaner.Models
{
    using System.Collections.Generic;
    using System.IO;
    using Cleaners;
    using System.Linq;
    using Catel;

    public class Repository
    {
        public Repository(string directory, IEnumerable<ICleaner> cleaners)
        {
            Argument.IsNotNull(() => directory);

            var directoryInfo = new DirectoryInfo(directory);

            IsIncluded = true;
            Name = directoryInfo.Name;
            Directory = directoryInfo.FullName;
            Cleaners = new List<ICleaner>();

            if (cleaners != null)
            {
                Cleaners.AddRange(cleaners);
            }
        }

        public bool IsIncluded { get; set; }

        public string Name { get; set; }

        public string Directory { get; set; }

        public List<ICleaner> Cleaners { get; private set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return Name;
        }
    }
}