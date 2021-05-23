using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using ProfanityChecker.Domain;
using ProfanityChecker.Infrastructure;
using ProfanityChecker.Logic;
using ProfanityChecker.Logic.DTO;
using ProfanityChecker.WebApi.Controllers;
using ProfanityChecker.WebApi.Requests;

namespace ProfanityChecker.WebApi.Tests.Controllers
{
    [TestFixture]
    public class BannedDictionaryControllerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWork = new();
        private readonly Mock<IFileService> _fileService = new();
        private BannedDictionaryController _controller;
        private readonly FormFile _file = new(Stream.Null, 0, 10, "", "");


        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _controller = new BannedDictionaryController(_unitOfWork.Object, _fileService.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _unitOfWork.Reset();
            _fileService.Reset();
        }

        [Test]
        public async Task GetAll_WhenCalled_CallsUnitOfWork()
        {
            _unitOfWork.Setup(x => x.BannedPhrases.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new List<BannedPhrase>());
            
            _ = await _controller.GetAll();

            _unitOfWork.Verify(x => x.BannedPhrases.GetAllAsync(It.IsAny<CancellationToken>()));
        }
        
        [Test]
        public async Task GetById_ItemNotFound_ReturnNotFound()
        {
            _unitOfWork.Setup(x => x.BannedPhrases.GetByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => null);
            
            var result = await _controller.GetById(1);

            result.Result.Should().BeOfType<NotFoundResult>();
        }
        
        [Test]
        public async Task GetById_WhenCalled_ReturnsResult()
        {
            var expected = new BannedPhraseDto {Id = 1, Name = "test"};
            _unitOfWork.Setup(x => x.BannedPhrases.GetByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new BannedPhrase(1, "test"));
            
            var result = await _controller.GetById(1);

            result.Value.Should().BeEquivalentTo(expected);
        }
        
        [TestCase("")]
        [TestCase(null)]
        [TestCase(" ")]
        public async Task Create_NameIsNullOrEmptyOrWhiteSpace_ThrowsArgumentException(string name)
        {
            var request = new AddBannedPhraseRequest {Name = name};

            Func<Task> act = async () => await _controller.Create(request);

            await act.Should().ThrowAsync<ArgumentException>();
        }
        
        [Test]
        public async Task Create_WhenCalled_ReturnsCreatedId()
        {
            var expected = 10;
            _unitOfWork.Setup(x => x.BannedPhrases.AddAsync(It.IsAny<BannedPhrase>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new BannedPhrase(10, "test"));

            var result = (await _controller.Create(new AddBannedPhraseRequest {Name = "test"}))
                .Result as ObjectResult;

            result.Should().NotBeNull();
            result!.Value.Should().Be(expected);
        }

        [Test]
        public async Task CreateRangeFromFile_FileIsNull_ThrowsArgumentNullException()
        {
            Func<Task> act = async () => await _controller.CreateRangeFromFile(null);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }
        
        [Test]
        public async Task CreateRangeFromFile_FileLengthIsZero_ThrowsArgumentException()
        {
            var file = new FormFile(Stream.Null, 0, 0, "", "");
            
            Func<Task> act = async () => await _controller.CreateRangeFromFile(file);

            await act.Should().ThrowAsync<ArgumentException>();
        }
        
        [Test]
        public async Task CreateRangeFromFile_CannotSaveFile_ReturnsConflict()
        {
            _fileService.Setup(x => x.SaveAsTempFileAsync(It.IsAny<IFormFile>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => string.Empty);

            var response = await _controller.CreateRangeFromFile(_file);

            response.Result.Should().BeOfType<ConflictObjectResult>();
        }

        [Test]
        public async Task CreateRangeFromFile_WhenCalled_ReturnsCountOfAddedItems()
        {
            _fileService.Setup(x => x.SaveAsTempFileAsync(It.IsAny<IFormFile>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => "path_to_file");
            _fileService.Setup(x => x.GetLinesAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<string> {"test", "test1", "test2"});
            _unitOfWork.Setup(x => x.BannedPhrases.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new List<BannedPhrase>
                {
                    new(1, "test")
                });
           
            var response = (await _controller.CreateRangeFromFile(_file)).Result as ObjectResult;

            response!.Value.Should().Be(2);
            _unitOfWork.Verify(x=> x.SaveChangesAsync(It.IsAny<CancellationToken>()));
            _fileService.Verify(x=> x.DeleteFileAsync(It.IsAny<string>()));
        }

    }
}