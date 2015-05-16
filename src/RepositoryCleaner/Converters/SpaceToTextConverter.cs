// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpaceToTextConverter.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2015 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace RepositoryCleaner.Converters
{
    using System;
    using Catel.MVVM.Converters;

    public class SpaceToTextConverter : ValueConverterBase<long>
    {
        protected override object Convert(long value, Type targetType, object parameter)
        {
            if (value > 0)
            {
                value = value / 1024;
            }

            return string.Format("{0} Kb", value);
        }
    }
}