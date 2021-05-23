using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using ProfanityChecker.Domain;

namespace ProfanityChecker.Infrastructure.Tests.Repositories
{
    [TestFixture]
    public class BannedPhraseRepositoryTests
    {
        private IUnitOfWork _unitOfWork;
        private DataContext _context;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            var services = new ServiceCollection();
            services.AddDbContext<DataContext>(o=> o.UseInMemoryDatabase(Guid.NewGuid().ToString()));
            services.AddTransient<IBannedPhraseRepository, BannedPhraseRepository>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            var serviceProvider = services.BuildServiceProvider();
            _unitOfWork = serviceProvider.GetService<IUnitOfWork>();
            _context = serviceProvider.GetService<DataContext>();
        }

        [SetUp]
        public async Task SetUp()
        {
            // ReSharper disable once PossibleNullReferenceException
            await _unitOfWork.BannedPhrases.AddAsync(new BannedPhrase("first"));
            await _unitOfWork.BannedPhrases.AddAsync(new BannedPhrase("second"));
            await _unitOfWork.BannedPhrases.AddAsync(new BannedPhrase("third"));
            await _unitOfWork.SaveChangesAsync();
        }

        [TearDown]
        public void TearDown()
        {
            _context.BannedPhrases.RemoveRange(_context.BannedPhrases);
        }

        [Test]
        public async Task GetAllAsync_WhenCalled_ReturnsListOfBannedPhrases()
        {
            var result = await _unitOfWork.BannedPhrases.GetAllAsync();

            result.Should().HaveCount(3);
        }


        [NonParallelizable]
        [Test]
        public void AddAsync_EntityIsNull_ThrowsArgumentNullException()
        {
            Func<Task> act = async () =>
            {
                await _unitOfWork.BannedPhrases.AddAsync(null);
            };

            act.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public async Task AddAsync_WhenCalled_ReturnsAddedEntity()
        {
            var result = await _unitOfWork.BannedPhrases
                .AddAsync(new BannedPhrase(nameof(AddAsync_WhenCalled_ReturnsAddedEntity)));
            await _unitOfWork.SaveChangesAsync();

            result.Id.Should().NotBe(0);
            result.Name.Should().Be(nameof(AddAsync_WhenCalled_ReturnsAddedEntity));
        }
        
        [Test]
        public void UpdateAsync_EntityIsNull_ThrowsArgumentNullException()
        {
            Func<Task> act = async () =>
            {
                await _unitOfWork.BannedPhrases.UpdateAsync(null);
            };

            act.Should().Throw<ArgumentNullException>();
        }
        
        [Test]
        public async Task UpdateAsync_WhenCalled_ReturnsUpdatedEntity()
        {
            var added = await _unitOfWork.BannedPhrases.AddAsync(new BannedPhrase("Added"));
            await _unitOfWork.SaveChangesAsync();

            added.Name = "Updated";
            var updated = await _unitOfWork.BannedPhrases.UpdateAsync(added);
            await _unitOfWork.SaveChangesAsync();
            
            updated.Id.Should().Be(added.Id);
            updated.Name.Should().Be(added.Name);
        }
        
        [Test]
        public void DeleteAsync_EntityIsNull_ThrowsArgumentNullException()
        {
            Func<Task> act = async () =>
            {
                await _unitOfWork.BannedPhrases.DeleteAsync(null);
            };

            act.Should().Throw<ArgumentNullException>();
        }
        
        [Test]
        public async Task DeleteAsync_WhenCalled_DeletesEntity()
        {
            var added = await _unitOfWork.BannedPhrases.AddAsync(new BannedPhrase("Added"));
            await _unitOfWork.SaveChangesAsync();

            await _unitOfWork.BannedPhrases.DeleteAsync(added);
            await _unitOfWork.SaveChangesAsync();

            var deleted = await _unitOfWork.BannedPhrases.GetByIdAsync(added.Id);
            deleted.Should().BeNull();
        }
    }
}