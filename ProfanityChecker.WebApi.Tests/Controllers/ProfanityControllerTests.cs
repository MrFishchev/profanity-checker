using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using ProfanityChecker.Domain;
using ProfanityChecker.Logic;
using ProfanityChecker.WebApi.Controllers;

namespace ProfanityChecker.WebApi.Tests.Controllers
{
    [TestFixture]
    public class ProfanityControllerTests
    {
        private readonly Mock<IProfanityService> _profanityService = new();
        private readonly Mock<IFileService> _fileService = new();
        private ProfanityController _profanityController;
        private readonly FormFile _file = new(Stream.Null, 0, 1, "", "");

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _profanityController = new ProfanityController(_profanityService.Object, _fileService.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _profanityService.Reset();
            _fileService.Reset();
        }

        [Test]
        public void FindAllAsync_FileIsNull_ThrowsArgumentNullException()
        {
            Action act = () => _profanityController.FindAllProfanity(null);

            act.Should().Throw<ArgumentNullException>();
        }
        
        [Test]
        public void FindAllAsync_FileLengthIsZero_ThrowsArgumentException()
        {
            var file = new FormFile(Stream.Null, 0, 0, "", "");
            
            Action act = () => _profanityController.FindAllProfanity(file);

            act.Should().Throw<ArgumentException>();
        }
        
        [Test]
        public async Task FindAllAsync_CannotSaveFile_ReturnsConflict()
        {
            _fileService.Setup(x => x.SaveAsTempFileAsync(It.IsAny<IFormFile>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => string.Empty);

            var response = await _profanityController.FindAllProfanity(_file);

            response.Result.Should().BeOfType<ConflictObjectResult>();
        }

        [Test]
        public async Task FinAllAsync_CannotGetFileContent_ReturnsConflict()
        {
            _fileService.Setup(x => x.SaveAsTempFileAsync(It.IsAny<IFormFile>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => "path_to_file");
            _fileService.Setup(x => x.GetWholeTextAsync(It.IsAny<string>()))
                .ReturnsAsync(() => string.Empty);

            var response = await _profanityController.FindAllProfanity(_file);

            response.Result.Should().BeOfType<ConflictObjectResult>();
        }
        
        [Test]
        public async Task FinAllAsync_WhenCalled_ReturnsScanResult()
        {
            _fileService.Setup(x => x.SaveAsTempFileAsync(It.IsAny<IFormFile>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => "path_to_file");
            _fileService.Setup(x => x.GetWholeTextAsync(It.IsAny<string>()))
                .ReturnsAsync(() => "hello");
            _profanityService.Setup(x => x.ScanAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => ProfanityScanResult.NoProfanity);

             _= await _profanityController.FindAllProfanity(_file);

            _fileService.Verify(x=> x.SaveAsTempFileAsync(It.IsAny<IFormFile>(), It.IsAny<CancellationToken>()));
            _fileService.Verify(x=> x.GetWholeTextAsync(It.IsAny<string>()));
            _profanityService.Verify(x=> x.ScanAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()));
            _fileService.Verify(x=> x.DeleteFileAsync(It.IsAny<string>()));
        }
    }
}