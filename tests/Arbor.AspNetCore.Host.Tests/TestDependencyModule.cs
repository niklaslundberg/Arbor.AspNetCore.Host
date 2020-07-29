using Arbor.App.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Arbor.AspNetCore.Host.Tests
{
    public class TestDependencyModule : IModule
    {
        private readonly TestDependency _testDependency;

        public TestDependencyModule(TestDependency testDependency)
        {
            _testDependency = testDependency;
        }
        public IServiceCollection Register(IServiceCollection builder) => builder;
    }
}