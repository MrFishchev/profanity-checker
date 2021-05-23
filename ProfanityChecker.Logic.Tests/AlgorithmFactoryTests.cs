using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace ProfanityChecker.Logic.Tests
{
    [TestFixture]
    public class AlgorithmFactoryTests
    {
        private IAlgorithmFactory _algorithmFactory;

        [OneTimeSetUp]
        public void SetUp()
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddTransient<IAlgorithmFactory, AlgorithmFactory>();
            var serviceProvider = services.BuildServiceProvider();

            _algorithmFactory = serviceProvider.GetService<IAlgorithmFactory>();
        }

        [Test]
        public void CreateAlgorithm_WhenCalled_ReturnsInstanceOfAnAlgorithm()
        {
            var instance = _algorithmFactory
                .CreateAlgorithm<AhoCorasickAlgorithm>(new List<string> {"abc"});

            instance.Should().NotBeNull();
        }
    }
}