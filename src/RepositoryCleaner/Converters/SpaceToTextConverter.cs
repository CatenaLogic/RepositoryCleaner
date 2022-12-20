namespace RepositoryCleaner.Converters
{
    using System;
    using Catel.MVVM.Converters;
    using Humanizer;

    public class SpaceToTextConverter : ValueConverterBase<ulong?>
    {
        protected override object Convert(ulong? value, Type targetType, object parameter)
        {
            if (!value.HasValue)
            {
                return null;
            }

            var space = value.Value;
            if (space == 0L)
            {
                return "-";
            }

            return ((long)space).Bytes().Humanize("#.#");
        }
    }
}
