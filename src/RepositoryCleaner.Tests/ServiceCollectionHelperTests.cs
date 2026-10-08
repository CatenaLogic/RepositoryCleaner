namespace RepositoryCleaner.Tests
{
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;

    [TestFixture]
    public class ServiceCollectionHelperTests
    {
        [Test]
        public void CreateServiceCollection_ProducesIsolatedProviders()
        {
            var firstServices = ServiceCollectionHelper.CreateServiceCollection();
            var secondServices = ServiceCollectionHelper.CreateServiceCollection();
            firstServices.AddSingleton(new object());
            secondServices.AddSingleton(new object());

            using var firstProvider = firstServices.BuildServiceProvider();
            using var secondProvider = secondServices.BuildServiceProvider();

            Assert.That(firstProvider.GetRequiredService<object>(), Is.Not.SameAs(secondProvider.GetRequiredService<object>()));
        }
    }
}
