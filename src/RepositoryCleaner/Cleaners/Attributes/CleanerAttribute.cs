// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CleanerAttribute.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2015 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


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