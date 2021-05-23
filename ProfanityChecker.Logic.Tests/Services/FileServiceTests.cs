using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace ProfanityChecker.Logic.Tests.Services
{
    public class FileServiceTests
    {
        private const string TestFile = "testdata/file_service.txt";
        private IFileService _fileService;

        [OneTimeSetUp]
        public void SetUp()
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddTransient<IFileService, FileService>();
            var serviceProvider = services.BuildServiceProvider();

            _fileService = serviceProvider.GetService<IFileService>();
        }

        [Test]
        public void DeleteFileAsync_WhenCalled_ReturnsWithoutError()
        {
            var fileName = Path.GetTempFileName();

            Action act = () => _fileService.DeleteFileAsync(fileName);
            
            act.Should().NotThrow();
        }
        
        [Test]
        public async Task DeleteFileAsync_WhenCalled_DeletesFile        ()
        {
            var fileName = Path.GetTempFileName();
            
            await _fileService.DeleteFileAsync(fileName);
            
            File.Exists(fileName).Should().BeFalse();
        }

        [Test]
        public async Task SaveAsTempFileAsync_FileIsEmpty_ReturnsNull()
        {
            var file = new FormFile(Stream.Null, 0, 0, "", "");

            var result = await _fileService.SaveAsTempFileAsync(file);
            
            result.Should().BeNull();
        }
        
        [Test]
        public async Task SaveAsTempFileAsync_FileIsCorrect_ReturnsFilePath()
        {
            var file = new FormFile(Stream.Null, 0, 10, "", "");

            var result = await _fileService.SaveAsTempFileAsync(file);
            var exist = File.Exists(result);
            
            result.Should().NotBeNullOrWhiteSpace();
            exist.Should().BeTrue();
        }

        [Test]
        public async Task GetWholeTextAsync_FileDoesNotExist_ReturnsNull()
        {
            var result = await _fileService.GetWholeTextAsync("");

            result.Should().BeNull();
        }
        
        [Test]
        public async Task GetWholeTextAsync_FileContainsTwoLines_ReturnsAllText()
        {
            var result = await _fileService.GetWholeTextAsync(TestFile);

            result.Should().NotBeNullOrWhiteSpace();
        }
        
        [Test]
        public async Task GetLinesAsync_FileDoesNotExist_ReturnsEmptyList()
        {
            var result = await _fileService.GetLinesAsync("");

            result.Should().BeEmpty();
        }
        
        [Test]
        public async Task GetLinesAsync_OperationWasCancelled_ReturnsEmptyList()
        {
            var ct = new CancellationToken(true);
            
            var result = await _fileService.GetLinesAsync(TestFile, ct);

            result.Should().BeEmpty();
        }
        
        [Test]
        public async Task GetLinesAsync_FileContainsTwoLines_ReturnsLines()
        {
            var result = await _fileService.GetLinesAsync(TestFile);

            result.Should().HaveCount(2);
        }
    }
}