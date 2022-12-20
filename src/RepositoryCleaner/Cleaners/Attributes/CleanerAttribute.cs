namespace RepositoryCleaner.Cleaners
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    internal class CleanerAttribute : Attribute
    {
        public CleanerAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        public string Description { get; set; }
    }
}
