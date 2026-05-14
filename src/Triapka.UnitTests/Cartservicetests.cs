using System.Security.Claims;

using FluentAssertions;

using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;

using Moq;


using Triapka.Application.Interfaces;
using Triapka.Application.Services;
using Triapka.Domain.Entities;

using Xunit;

namespace Triapka.UnitTests;

public class CartServiceTests
{
    private readonly Mock<ICartRepository> _cartRepoMock;
    private readonly Mock<IProductRepository> _productRepoMock;
    private readonly Mock<IHttpContextAccessor> _httpContextMock;

    private const string FakeUserId = "user-abc-123";

    public CartServiceTests()
    {
        _cartRepoMock = new Mock<ICartRepository>();
        _productRepoMock = new Mock<IProductRepository>();
        _httpContextMock = new Mock<IHttpContextAccessor>();

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, FakeUserId)
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = principal };

        _httpContextMock.Setup(x => x.HttpContext).Returns(httpContext);
    }

    private CartService CreateSut() =>
        new(_cartRepoMock.Object, _productRepoMock.Object, _httpContextMock.Object);

    [Fact]
    public async Task GetCartAsync_WhenUserHasNoCart_ShouldReturnEmptyCartDto()
    {
        _cartRepoMock
            .Setup(r => r.GetCartByUserIdAsync(FakeUserId))
            .ReturnsAsync((Cart?)null);

        var result = await CreateSut().GetCartAsync();

        result.CartItems.Should().BeEmpty();
        result.TotalAmount.Should().Be(0);
    }

    [Fact]
    public async Task GetCartAsync_WhenUserHasCart_ShouldReturnMappedCartDto()
    {
        var product = BuildProduct(10, "Моторна олива", 500m);
        var cart = new Cart
        {
            CartId = 1,
            UserId = FakeUserId,
            CartItems =
            [
                new CartItem { CartItemId = 1, CartId = 1, ProductId = 10, Quantity = 2, Product = product }
            ]
        };

        _cartRepoMock
            .Setup(r => r.GetCartByUserIdAsync(FakeUserId))
            .ReturnsAsync(cart);

        var result = await CreateSut().GetCartAsync();

        result.CartId.Should().Be(1);
        result.CartItems.Should().HaveCount(1);
        result.CartItems[0].Quantity.Should().Be(2);
        result.TotalAmount.Should().Be(1000m);
    }

    [Fact]
    public async Task AddToCartAsync_WhenCartIsEmpty_ShouldAddNewItemWithQuantityOne()
    {
        var product = BuildProduct(7, "Гальмівний диск", 950m);

        _productRepoMock.Setup(r => r.GetByIdAsync(7)).ReturnsAsync(product);
        _cartRepoMock.Setup(r => r.GetCartByUserIdAsync(FakeUserId)).ReturnsAsync((Cart?)null);

        Cart? savedCart = null;
        _cartRepoMock
            .Setup(r => r.UpdateCartAsync(It.IsAny<Cart>()))
            .Callback<Cart>(c => savedCart = c)
            .ReturnsAsync((Cart c) =>
            {
                c.CartId = 1;
                foreach (var item in c.CartItems) item.Product = product;
                return c;
            });

        var result = await CreateSut().AddToCartAsync(7);

        result.CartItems.Should().HaveCount(1, "в порожній кошик додається рівно один запис");
        result.CartItems[0].Quantity.Should().Be(1);
        result.CartItems[0].ProductName.Should().Be("Гальмівний диск");
        savedCart!.CartItems.Should().HaveCount(1);
    }

    [Fact]
    public async Task AddToCartAsync_WhenSameProductAddedTwice_ShouldIncrementQuantityNotAddNewItem()
    {
        var product = BuildProduct(7, "Гальмівний диск", 950m);

        _productRepoMock.Setup(r => r.GetByIdAsync(7)).ReturnsAsync(product);

        var existingCart = new Cart
        {
            CartId = 1,
            UserId = FakeUserId,
            CartItems =
            [
                new CartItem { CartItemId = 5, CartId = 1, ProductId = 7, Quantity = 1, Product = product }
            ]
        };

        _cartRepoMock.Setup(r => r.GetCartByUserIdAsync(FakeUserId)).ReturnsAsync(existingCart);

        Cart? savedCart = null;
        _cartRepoMock
            .Setup(r => r.UpdateCartAsync(It.IsAny<Cart>()))
            .Callback<Cart>(c => savedCart = c)
            .ReturnsAsync((Cart c) =>
            {
                foreach (var item in c.CartItems) item.Product = product;
                return c;
            });

        var result = await CreateSut().AddToCartAsync(7);

        result.CartItems.Should().HaveCount(1, "новий запис не додається — товар вже є");
        result.CartItems[0].Quantity.Should().Be(2, "Quantity збільшується з 1 до 2");
        savedCart!.CartItems.First().Quantity.Should().Be(2);
    }

    [Fact]
    public async Task AddToCartAsync_WhenProductDoesNotExist_ShouldThrowInvalidOperationException()
    {
        _productRepoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((Product?)null);

        Func<Task> act = async () => await CreateSut().AddToCartAsync(999);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Товар не існує.");
    }

    [Fact]
    public async Task AddToCartAsync_WhenUserIsNotAuthenticated_ShouldThrowUnauthorizedAccessException()
    {
        var anonMock = new Mock<IHttpContextAccessor>();
        anonMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());

        var sut = new CartService(_cartRepoMock.Object, _productRepoMock.Object, anonMock.Object);

        Func<Task> act = async () => await sut.AddToCartAsync(1);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task RemoveFromCartAsync_WhenItemExists_ShouldReturnUpdatedCart()
    {
        var emptyCart = new Cart { CartId = 1, UserId = FakeUserId, CartItems = [] };

        _cartRepoMock.Setup(r => r.RemoveItemAsync(55)).ReturnsAsync(emptyCart);

        var result = await CreateSut().RemoveFromCartAsync(55);

        result.CartItems.Should().BeEmpty();
        _cartRepoMock.Verify(r => r.RemoveItemAsync(55), Times.Once);
    }

    [Fact]
    public async Task RemoveFromCartAsync_WhenItemDoesNotExist_ShouldReturnEmptyCartDto()
    {
        _cartRepoMock
            .Setup(r => r.RemoveItemAsync(It.IsAny<int>()))
            .ReturnsAsync((Cart?)null);

        var result = await CreateSut().RemoveFromCartAsync(999);

        result.CartItems.Should().BeEmpty();
        result.TotalAmount.Should().Be(0);
    }

    private static Product BuildProduct(int productId, string name, decimal price) => new()
    {
        ProductId = productId,
        Name = name,
        Price = price,
        CategoryId = 1,
        Category = new ProductCategory { CategoryId = 1, Name = "Тест" },
        Images = []
    };
}