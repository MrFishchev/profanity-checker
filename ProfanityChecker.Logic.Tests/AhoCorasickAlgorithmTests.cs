using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace ProfanityChecker.Logic.Tests
{
    [TestFixture]
    public class AhoCorasickAlgorithmTests
    {
        private ISearchingAlgorithm _searchingAlgorithm;
        private HashSet<string> _dictionary;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _dictionary = new HashSet<string>
            {
                "a b"
            };
        }

        [SetUp]
        public void SetUp()
        {
            _searchingAlgorithm = new AhoCorasickAlgorithm(_dictionary);
        }
        
        [TestCase("a a b b")]
        [TestCase("aa bb")]
        [TestCase("a a bb")]
        [TestCase("aa b b")]
        public void Search_WhenCalled_ReturnsResult(string text)
        {
            var result = _searchingAlgorithm.FindAll(text).ToList();

            result.Should().HaveCount(1);
            result.First().Data.Should().Be("a b");
            result.First().Count.Should().Be(1);
            result.First().Indexes.Should().HaveCount(1);
        }

        [Test]
        public void Search_WhenCalled_ReturnsCorrectIndexes()
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
        public void Search_WhenCalled_ReturnsCorrectBounds(string text, string expectedBound)
        {
            var result = _searchingAlgorithm.FindAll(text).ToList();

            result.Should().HaveCount(1);
            result.First().FullBounds.Should().Be(expectedBound);
        }

        [TestCase("aa bb", 1, 1)]
        [TestCase("ab a b a a b b cc a b cc a b cccc a b", 1, 5)]
        [TestCase("aabb", 0, 0)]
        public void Search_WhenCalled_ReturnsCorrectCount(string text, int expectedItems, int expectedCount)
        {
            var result = _searchingAlgorithm.FindAll(text).ToList();

            result.Should().HaveCount(expectedItems);
            result.FirstOrDefault()?.Count.Should().Be(expectedCount);
        }

        [Test]
        public void Search_WhenTextDoesNotContainPattern_ReturnsEmptyList()
        {
            var text = "ccc";

            var result = _searchingAlgorithm.FindAll(text).ToList();

            result.Should().BeEmpty();
        }
        
        // TODO: ignore case
        // TODO: ignore punctuation
        // TODO: ignore new lines and tabs
        // TODO: tests for Contains method
    }
}