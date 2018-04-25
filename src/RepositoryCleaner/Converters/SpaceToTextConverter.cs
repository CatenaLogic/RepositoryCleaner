// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpaceToTextConverter.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2015 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace RepositoryCleaner.Converters
{
    using System;
    using Catel.MVVM.Converters;
    using Humanizer;

    public class SpaceToTextConverter : ValueConverterBase<long?>
    {
        protected override object Convert(long? value, Type targetType, object parameter)
        {
            if (!value.HasValue)
            {
                return null;
            }

            if (value.Value == 0L)
            {
                return "-";
            }

            return value.Value.Bytes().Humanize("#.#");
        }
    }
}