using AutoMapper;
using MediPorta.API.DTO;
using MediPorta.Application.Interfaces;
using MediPorta.Domain.Entites;
using MediPorta.Infrastructure.Interfaces;
using MediPorta.Infrastructure.Models;
using Moq;
using Xunit;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace MediPorta.UnitTests.Services
{
    public class TagServiceTests
    {
        private readonly Mock<ITagRepository> _repoMock;
        private readonly Mock<IStackOverflowClient> _stackClientMock;
        private readonly Mock<ITagProcessor> _processorMock;
        private readonly IMapper _mapper;

        public TagServiceTests()
        {
            _repoMock = new Mock<ITagRepository>();
            _stackClientMock = new Mock<IStackOverflowClient>();
            _processorMock = new Mock<ITagProcessor>();

            // AutoMapper configuration
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Tag, TagDto>();
            });
            _mapper = config.CreateMapper();
        }

        [Fact(DisplayName = "GetTagsAsync zwraca poprawnie zmapowane tagi")]
        public async Task GetTagsAsync_ReturnsMappedTags()
        {
            // Arrange
            var tagsFromRepo = new List<Tag>
            {
                new Tag { Name = "csharp", Count = 100, Percentage = 1.5m },
                new Tag { Name = "dotnet", Count = 50, Percentage = 0.75m }
            };

            _repoMock.Setup(r => r.GetPagedAsync(1, 10, "name", true))
                     .ReturnsAsync(tagsFromRepo);

            var service = new TagService(
                _repoMock.Object,
                _stackClientMock.Object,
                _processorMock.Object,
                _mapper);

            // Act
            var result = await service.GetTagsAsync(1, 10, "name", true);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("csharp", result[0].Name);
            Assert.Equal("dotnet", result[1].Name);
        }

        [Theory(DisplayName = "GetTagsAsync obsługuje różne parametry paginacji i sortowania")]
        [InlineData(1, 2, "name", true)]
        [InlineData(2, 1, "count", false)]
        [InlineData(1, 10, "percentage", true)]
        public async Task GetTagsAsync_VariousPaginationAndSorting_ReturnsCorrectCount(
            int pageNumber, int pageSize, string sortBy, bool ascending)
        {
            // Arrange
            var tagsFromRepo = new List<Tag>
            {
                new Tag { Name = "csharp", Count = 100, Percentage = 50m },
                new Tag { Name = "dotnet", Count = 50, Percentage = 25m },
                new Tag { Name = "aspnet", Count = 50, Percentage = 25m }
            };

            _repoMock.Setup(r => r.GetPagedAsync(pageNumber, pageSize, sortBy, ascending))
                     .ReturnsAsync(tagsFromRepo.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList());

            var service = new TagService(_repoMock.Object, _stackClientMock.Object, _processorMock.Object, _mapper);

            // Act
            var result = await service.GetTagsAsync(pageNumber, pageSize, sortBy, ascending);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Count <= pageSize);
        }

        [Fact(DisplayName = "RefreshTagsFromStackOverflowAsync wywołuje pobranie tagów i zapis do repozytorium")]
        public async Task RefreshTagsFromStackOverflowAsync_CallsRepository()
        {
            // Arrange
            var rawTags = new List<StackOverflowTag>
            {
                new StackOverflowTag { Name = "csharp", Count = 100 },
                new StackOverflowTag { Name = "dotnet", Count = 50 }
            };
            _stackClientMock.Setup(s => s.GetTagsAsync(1000))
                            .ReturnsAsync(rawTags);

            var processedTags = new List<Tag>
            {
                new Tag { Name = "csharp", Count = 100, Percentage = 66.66m },
                new Tag { Name = "dotnet", Count = 50, Percentage = 33.33m }
            };
            _processorMock.Setup(p => p.Process(rawTags))
                          .Returns(processedTags);

            var service = new TagService(
                _repoMock.Object,
                _stackClientMock.Object,
                _processorMock.Object,
                _mapper);

            // Act
            await service.RefreshTagsFromStackOverflowAsync();

            // Assert
            _stackClientMock.Verify(s => s.GetTagsAsync(1000), Times.Once);
            _processorMock.Verify(p => p.Process(rawTags), Times.Once);
            _repoMock.Verify(r => r.ClearAsync(), Times.Once);
            _repoMock.Verify(r => r.AddOrUpdateRangeAsync(processedTags), Times.Once);
        }

        [Fact(DisplayName = "RefreshTagsFromStackOverflowAsync obsługuje brak tagów bez błędów")]
        public async Task RefreshTagsFromStackOverflowAsync_EmptyRawTags_DoesNotCallAddOrUpdate()
        {
            // Arrange
            var rawTags = new List<StackOverflowTag>(); // brak tagów
            _stackClientMock.Setup(s => s.GetTagsAsync(1000))
                            .ReturnsAsync(rawTags);

            _processorMock.Setup(p => p.Process(rawTags))
                          .Returns(new List<Tag>());

            var service = new TagService(
                _repoMock.Object,
                _stackClientMock.Object,
                _processorMock.Object,
                _mapper);

            // Act
            await service.RefreshTagsFromStackOverflowAsync();

            // Assert
            _repoMock.Verify(r => r.ClearAsync(), Times.Once);
            _repoMock.Verify(r => r.AddOrUpdateRangeAsync(It.IsAny<List<Tag>>()), Times.Once);
        }

        [Fact(DisplayName = "RefreshTagsFromStackOverflowAsync obsługuje null zwrócone przez klienta SO")]
        public async Task RefreshTagsFromStackOverflowAsync_NullRawTags_HandledGracefully()
        {
            // Arrange
            _stackClientMock.Setup(c => c.GetTagsAsync(1000)).ReturnsAsync((List<StackOverflowTag>?)null);
            _processorMock.Setup(p => p.Process(It.IsAny<List<StackOverflowTag>>())).Returns(new List<Tag>());

            var service = new TagService(_repoMock.Object, _stackClientMock.Object, _processorMock.Object, _mapper);

            // Act & Assert
            await service.RefreshTagsFromStackOverflowAsync(); // nie powinno rzucić wyjątku
            _repoMock.Verify(r => r.ClearAsync(), Times.Once);
            _repoMock.Verify(r => r.AddOrUpdateRangeAsync(It.IsAny<List<Tag>>()), Times.Once);
        }

        [Fact(DisplayName = "RefreshTagsFromStackOverflowAsync przelicza poprawnie procenty")]
        public async Task RefreshTagsFromStackOverflowAsync_CalculatesPercentagesCorrectly()
        {
            // Arrange
            var rawTags = new List<StackOverflowTag>
            {
                new StackOverflowTag { Name = "csharp", Count = 100 },
                new StackOverflowTag { Name = "dotnet", Count = 50 }
            };

            _stackClientMock.Setup(c => c.GetTagsAsync(1000)).ReturnsAsync(rawTags);
            _processorMock.Setup(p => p.Process(rawTags)).Returns(rawTags.Select(t => new Tag
            {
                Name = t.Name,
                Count = t.Count,
                Percentage = Math.Round((decimal)t.Count / rawTags.Sum(x => x.Count) * 100, 4)
            }).ToList());

            var service = new TagService(_repoMock.Object, _stackClientMock.Object, _processorMock.Object, _mapper);

            // Act
            await service.RefreshTagsFromStackOverflowAsync();

            // Assert
            _repoMock.Verify(r => r.AddOrUpdateRangeAsync(It.Is<List<Tag>>(tags =>
                tags.First(t => t.Name == "csharp").Percentage == 66.6667m &&
                tags.First(t => t.Name == "dotnet").Percentage == 33.3333m
            )), Times.Once);
        }

        [Fact(DisplayName = "RefreshTagsFromStackOverflowAsync rzuca wyjątek jeśli klient zwraca błąd")]
        public async Task RefreshTagsFromStackOverflowAsync_ClientThrows_ExceptionPropagated()
        {
            // Arrange
            _stackClientMock.Setup(c => c.GetTagsAsync(1000)).ThrowsAsync(new Exception("SO API Error"));

            var service = new TagService(_repoMock.Object, _stackClientMock.Object, _processorMock.Object, _mapper);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => service.RefreshTagsFromStackOverflowAsync());
            Assert.Equal("SO API Error", ex.Message);
        }
    }
}
