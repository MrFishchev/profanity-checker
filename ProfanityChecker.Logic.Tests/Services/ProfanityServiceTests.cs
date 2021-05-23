using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using ProfanityChecker.Domain;
using ProfanityChecker.Infrastructure;

namespace ProfanityChecker.Logic.Tests.Services
{
    [TestFixture]
    public class ProfanityServiceTests
    {
        private readonly Mock<IAlgorithmFactory> _algorithmFactoryMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<ISearchingAlgorithm> _searchingAlgorithm = new();
        private IProfanityService _profanityService;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var logger = new Mock<ILogger<ProfanityService>>();
            _profanityService = new ProfanityService(_algorithmFactoryMock.Object,
                _unitOfWorkMock.Object, logger.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _algorithmFactoryMock.Reset();
            _unitOfWorkMock.Reset();
        }

        [Test]
        public async Task ScanAsync_DataIsNullOrWhiteSpace_ReturnsNoProfanityResult()
        {
            var result = await _profanityService.ScanAsync("");

            result.HasProfanity.Should().BeFalse();
            result.ProfanityItems.Should().BeNullOrEmpty();
        }
        
        [Test]
        public async Task ScanAsync_ProfanityNotFound_ReturnsNoProfanityResult()
        {
            _searchingAlgorithm.Setup(x => x.FindAll(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(() => new List<ProfanityItem>());
            SetUpDependencies();
            
            var result = await _profanityService.ScanAsync("some data");

            result.HasProfanity.Should().BeFalse();
            result.ProfanityItems.Should().BeNullOrEmpty();
        }
        
        [Test]
        public async Task ScanAsync_WhenCalled_ReturnsResult()
        {
            _searchingAlgorithm.Setup(x => x.FindAll(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(() => new List<ProfanityItem>
                {
                    new("1", "1", 1),
                    new("2", "2", 2)
                });
            SetUpDependencies();
            
            var result = await _profanityService.ScanAsync("some data");

            result.HasProfanity.Should().BeTrue();
            result.ProfanityItems.Should().HaveCount(2);
            _unitOfWorkMock.Verify(x=> x.BannedPhrases.GetAllAsync(It.IsAny<CancellationToken>()));
            _algorithmFactoryMock.Verify(x=> x.CreateAlgorithm<AhoCorasickAlgorithm>(It.IsAny<IEnumerable<string>>()));
            _searchingAlgorithm.Verify(x=> x.FindAll(It.IsAny<string>(), It.IsAny<CancellationToken>()));
        }
        
        private void SetUpDependencies()
        {
            _unitOfWorkMock.Setup(x => x.BannedPhrases.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new List<BannedPhrase>
                {
                    new(1, "word")
                });
            
            _algorithmFactoryMock.Setup(x => x.CreateAlgorithm<AhoCorasickAlgorithm>(It.IsAny<IEnumerable<string>>()))
                .Returns(() => _searchingAlgorithm.Object);
        }
    }
}