using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace ProfanityChecker.Logic.Tests
{
    [TestFixture]
    public class AhoCorasickAlgorithmTests
    {
        private readonly IAlgorithmFactory _algorithmFactory;
        private ISearchingAlgorithm _searchingAlgorithm;
        private HashSet<string> _dictionary;

        public AhoCorasickAlgorithmTests()
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddTransient<IAlgorithmFactory, AlgorithmFactory>();
            var serviceProvider = services.BuildServiceProvider();

            _algorithmFactory = serviceProvider.GetService<IAlgorithmFactory>();
        }

        [SetUp]
        public void SetUp()
        {
            _dictionary = new HashSet<string>
            {
                "a b"
            };
            _searchingAlgorithm = _algorithmFactory.CreateAlgorithm<AhoCorasickAlgorithm>(_dictionary);
        }

        #region Search Tests

        [TestCase("a a b b")]
        [TestCase("aa bb")]
        [TestCase("a a bb")]
        [TestCase("aa b b")]
        public void FindAll_WhenCalled_ReturnsResult(string text)
        {
            var result = _searchingAlgorithm.FindAll(text).ToList();

            result.Should().HaveCount(1);
            result.First().Data.Should().Be("a b");
            result.First().Indexes.Should().HaveCount(1);
        }

        [Test]
        public void FindAll_WhenCalled_ReturnsCorrectIndexes()
        {
            var text = "aa bb aa bb";
            var expected = new List<int> {1, 7};

            var result = _searchingAlgorithm.FindAll(text).ToList();

            result.Should().HaveCount(1);
            result.First().Indexes.Should().BeEquivalentTo(expected);
        }
        
        [TestCase("aa bb", "aa bb")]
        [TestCase("aaa bbb", "aaa bbb")]
        [TestCase("a a b", "a b")]
        [TestCase("a b b", "a b")]
        [TestCase("aa aa cc a b", "a b")]
        [TestCase("cc a b cc", "a b")]
        public void FindAll_WhenCalled_ReturnsCorrectBounds(string text, string expectedBound)
        {
            var result = _searchingAlgorithm.FindAll(text).ToList();

            result.Should().HaveCount(1);
            result.First().FullBounds.Should().Be(expectedBound);
        }

        [TestCase("aa bb", 1, 1)]
        [TestCase("ab a b a a b b cc a b cc a b cccc a b", 1, 5)]
        [TestCase("aabb", 0, 0)]
        public void FindAll_WhenCalled_ReturnsCorrectCount(string text, int expectedItems, int expectedCount)
        {
            var result = _searchingAlgorithm.FindAll(text).ToList();

            result.Should().HaveCount(expectedItems);
            result.FirstOrDefault()?.Indexes.Count.Should().Be(expectedCount);
        }

        [Test]
        public void FindAll_WhenTextDoesNotContainPattern_ReturnsEmptyList()
        {
            var text = "ccc";

            var result = _searchingAlgorithm.FindAll(text).ToList();

            result.Should().BeEmpty();
        }

        [TestCase("Some A btext is Here", "A btext")]
        [TestCase("Some A B text is Here", "A B")]
        public void FindAll_TextHasDifferentStyles_ReturnsCorrectResult(string text, string expectedBounds)
        {
            var result = _searchingAlgorithm.FindAll(text).ToList();

            result.Should().HaveCount(1);
            result.First().FullBounds.Should().Be(expectedBounds);
            result.First().Data.Should().Be("a b");
            result.First().Indexes.Should().HaveCount(1);
            result.First().Indexes.First().Should().Be(5);
        }
        
        [TestCase("!a b")]
        [TestCase("!a b!")]
        [TestCase("a b!")]
        [TestCase("a b !")]
        [TestCase(",a b.")]
        public void FindAll_TextHasPunctuation_ReturnsCorrectResult(string text)
        {
            var result = _searchingAlgorithm.FindAll(text).ToList();

            result.Should().HaveCount(1);
            result.First().Data.Should().Be("a b");
            result.First().Indexes.Should().HaveCount(1);
        }
        
        #endregion

        #region Contains Tests

        [TestCase("Some text is a b here")]
        [TestCase("Some text is ccc here")]
        public void Contains_WhenCalled_ReturnsTrue(string text)
        {
            _dictionary.Add("ccc");
            _searchingAlgorithm = _algorithmFactory.CreateAlgorithm<AhoCorasickAlgorithm>(_dictionary);

            var result = _searchingAlgorithm.ContainsAny(text);

            result.Should().BeTrue();
        }

        [Test]
        public void Contains_TextDoesNotContainPatter_ReturnsFalse()
        {
            var result = _searchingAlgorithm.ContainsAny("ababab");

            result.Should().BeFalse();
        }

        [TestCase("a B")]
        [TestCase("A B")]
        [TestCase("aaA bBb")]
        public void Contains_TextHasDifferentStyleAndContainsPattern_ReturnsTrue(string text)
        {
            var result = _searchingAlgorithm.ContainsAny(text);

            result.Should().BeTrue();
        }

        [TestCase("!a b")]
        [TestCase("!a b!")]
        [TestCase("a b!")]
        [TestCase("a b !")]
        [TestCase(",a b.")]
        public void Contains_TextHasPunctuation_ReturnsTrue(string text)
        {
            var result = _searchingAlgorithm.ContainsAny(text);

            result.Should().BeTrue();
        }
        
        #endregion
    }
}