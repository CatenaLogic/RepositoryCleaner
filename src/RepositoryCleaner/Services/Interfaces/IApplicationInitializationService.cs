namespace RepositoryCleaner.Services
{
    using System.Threading.Tasks;

    public interface IApplicationInitializationService
    {
        Task InitializeAsync();
    }
}
