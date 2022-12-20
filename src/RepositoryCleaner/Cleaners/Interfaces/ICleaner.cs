namespace RepositoryCleaner.Cleaners
{
    using System.Threading.Tasks;
    using Models;

    public interface ICleaner
    {
        #region Properties
        string Name { get; }
        string Description { get; }
        #endregion

        #region Methods
        bool CanClean(CleanContext context);

        ulong CalculateCleanableSpace(CleanContext context);

        void Clean(CleanContext context);
        #endregion
    }
}
