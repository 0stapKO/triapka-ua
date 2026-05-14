using FluentAssertions;

using Moq;

using Triapka.Application.Interfaces;
using Triapka.Application.Services;
using Triapka.Domain.Entities;

using Xunit;

namespace Triapka.UnitTests;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _repositoryMock;
    private readonly ProductService _sut;

    private readonly Mock<IReviewRepository> _reviewRepositoryMock;

    public ProductServiceTests()
    {
        _repositoryMock = new Mock<IProductRepository>();
        _reviewRepositoryMock = new Mock<IReviewRepository>();
        _sut = new ProductService(_repositoryMock.Object, _reviewRepositoryMock.Object);
    }


    [Fact]
    public async Task GetAllProductsAsync_WhenRepositoryReturnsProducts_ShouldReturnMappedDtos()
    {
        var fakeProducts = new List<Product>
        {
            new()
            {
                ProductId  = 1,
                Name       = "Моторна олива",
                Price      = 450.00m,
                Rating     = 4.5m,
                CategoryId = 1,
                Category   = new ProductCategory { CategoryId = 1, Name = "Оливи" },
                Images     = [new ProductImage { ImageId = 1, ImageUrl = "https://example.com/oil.jpg", ProductId = 1 }]
            },
            new()
            {
                ProductId  = 2,
                Name       = "Гальмівні колодки",
                Price      = 780.00m,
                CategoryId = 2,
                Category   = new ProductCategory { CategoryId = 2, Name = "Гальма" },
                Images     = []
            }
        };

        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(fakeProducts);

        var result = (await _sut.GetAllProductsAsync()).ToList();

        _repositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
        result.Should().HaveCount(2);
        result[0].ProductId.Should().Be(1);
        result[0].Name.Should().Be("Моторна олива");
        result[0].MainImageUrl.Should().Be("https://example.com/oil.jpg");
        result[1].MainImageUrl.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllProductsAsync_WhenRepositoryReturnsEmptyList_ShouldReturnEmptyCollection()
    {
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync([]);

        var result = await _sut.GetAllProductsAsync();

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetProductByIdAsync_WhenProductExists_ShouldReturnCorrectDetailsDto()
    {
        var fakeProduct = new Product
        {
            ProductId = 42,
            Name = "Свічки NGK",
            Description = "Ірідієві свічки",
            Price = 620.00m,
            CategoryId = 3,
            Category = new ProductCategory { CategoryId = 3, Name = "Запалювання" },
            Images =
            [
                new ProductImage { ImageId = 10, ImageUrl = "https://example.com/ngk1.jpg", ProductId = 42 },
                new ProductImage { ImageId = 11, ImageUrl = "https://example.com/ngk2.jpg", ProductId = 42 }
            ]
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(42)).ReturnsAsync(fakeProduct);

        var result = await _sut.GetProductByIdAsync(42);

        result.Should().NotBeNull();
        result!.ProductId.Should().Be(42);
        result.Name.Should().Be("Свічки NGK");
        result.CategoryName.Should().Be("Запалювання");
        result.ImageUrls.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetProductByIdAsync_WhenProductDoesNotExist_ShouldReturnNull()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Product?)null);

        var result = await _sut.GetProductByIdAsync(999);

        result.Should().BeNull();
        _repositoryMock.Verify(r => r.GetByIdAsync(999), Times.Once);
    }

    [Fact]
    public async Task SearchProductsByNameAsync_WhenQueryMatches_ShouldReturnFilteredDtos()
    {
        var fakeProducts = new List<Product>
        {
            new()
            {
                ProductId  = 5,
                Name       = "Фільтр масляний Bosch",
                Price      = 210.00m,
                CategoryId = 4,
                Category   = new ProductCategory { CategoryId = 4, Name = "Фільтри" },
                Images     = []
            }
        };

        _repositoryMock.Setup(r => r.SearchByNameAsync("фільтр")).ReturnsAsync(fakeProducts);

        var result = (await _sut.SearchProductsByNameAsync("фільтр")).ToList();

        result.Should().HaveCount(1);
        result[0].Name.Should().Be("Фільтр масляний Bosch");
    }

    [Fact]
    public async Task SearchProductsByNameAsync_WhenQueryMatchesNothing_ShouldReturnEmptyCollection()
    {
        _repositoryMock.Setup(r => r.SearchByNameAsync(It.IsAny<string>())).ReturnsAsync([]);

        var result = await _sut.SearchProductsByNameAsync("qwerty");

        result.Should().BeEmpty();
    }
}