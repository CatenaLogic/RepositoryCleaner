namespace RepositoryCleaner.Models
{
    using System.Collections.Generic;
    using System.IO;
    using Cleaners;
    using Catel.Data;
    using System;

    public class Repository : ModelBase
    {
        public Repository(string directory, IEnumerable<ICleaner> cleaners)
        {
            ArgumentNullException.ThrowIfNull(directory);

            var directoryInfo = new DirectoryInfo(directory);

            IsIncluded = true;
            Name = directoryInfo.Name;
            Directory = directoryInfo.FullName;
            Cleaners = new List<ICleaner>();

            if (cleaners is not null)
            {
                Cleaners.AddRange(cleaners);
            }
        }

        public bool IsIncluded { get; set; }

        public string Name { get; set; }

        public string Directory { get; set; }

        public List<ICleaner> Cleaners { get; private set; }

        public ulong? CleanableSize { get; set; }

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
