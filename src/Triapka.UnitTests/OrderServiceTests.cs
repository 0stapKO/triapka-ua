using System.Security.Claims;

using FluentAssertions;

using Microsoft.AspNetCore.Http;

using Moq;

using Triapka.Application.DTOs;
using Triapka.Application.Interfaces;
using Triapka.Application.Services;
using Triapka.Domain.Entities;

using Xunit;

namespace Triapka.UnitTests;

public class OrderServiceTests
{
    private const string FakeUserId = "user-abc-123";

    private readonly Mock<ICartRepository> _cartRepositoryMock = new();
    private readonly Mock<IOrderRepository> _orderRepositoryMock = new();
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock = new();

    public OrderServiceTests()
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, FakeUserId)
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = principal };
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);
    }

    [Fact]
    public async Task CreateOrderAsync_WhenCartHasItems_ShouldCreateOrderAndClearCart()
    {
        var product = new Product
        {
            ProductId = 10,
            Name = "Моторна олива",
            Price = 500m,
            CategoryId = 1,
            Category = new ProductCategory { CategoryId = 1, Name = "Тест" },
            Images = []
        };

        var cart = new Cart
        {
            CartId = 1,
            UserId = FakeUserId,
            CartItems =
            [
                new CartItem { CartItemId = 1, ProductId = 10, Quantity = 2, Product = product }
            ]
        };

        _cartRepositoryMock.Setup(r => r.GetCartByUserIdAsync(FakeUserId)).ReturnsAsync(cart);

        Order? createdOrder = null;
        _orderRepositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<Order>()))
            .Callback<Order>(o =>
            {
                o.OrderId = 44;
                createdOrder = o;
            })
            .ReturnsAsync((Order o) => o);

        var sut = new OrderService(_cartRepositoryMock.Object, _orderRepositoryMock.Object, _httpContextAccessorMock.Object);

        var result = await sut.CreateOrderAsync(new CheckoutDto
        {
            ShippingAddress = "м. Львів, вул. Тестова, 1",
            Phone = "+380991234567"
        });

        result.OrderId.Should().Be(44);
        result.TotalPrice.Should().Be(1000m);
        result.TotalItems.Should().Be(2);
        createdOrder.Should().NotBeNull();
        createdOrder!.OrderItems.Should().HaveCount(1);
        createdOrder.OrderItems.First().PriceAtPurchase.Should().Be(500m);
        _cartRepositoryMock.Verify(r => r.ClearCartAsync(FakeUserId), Times.Once);
    }

    [Fact]
    public async Task CreateOrderAsync_WhenCartIsEmpty_ShouldThrowInvalidOperationException()
    {
        _cartRepositoryMock.Setup(r => r.GetCartByUserIdAsync(FakeUserId)).ReturnsAsync(new Cart
        {
            CartId = 1,
            UserId = FakeUserId,
            CartItems = []
        });

        var sut = new OrderService(_cartRepositoryMock.Object, _orderRepositoryMock.Object, _httpContextAccessorMock.Object);

        Func<Task> act = async () => await sut.CreateOrderAsync(new CheckoutDto
        {
            ShippingAddress = "test",
            Phone = "123"
        });

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Кошик порожній.");
    }

    [Fact]
    public async Task CreateOrderAsync_WhenUserNotAuthenticated_ShouldThrowUnauthorizedAccessException()
    {
        var anonHttpContextAccessorMock = new Mock<IHttpContextAccessor>();
        anonHttpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());

        var sut = new OrderService(_cartRepositoryMock.Object, _orderRepositoryMock.Object, anonHttpContextAccessorMock.Object);

        Func<Task> act = async () => await sut.CreateOrderAsync(new CheckoutDto
        {
            ShippingAddress = "test",
            Phone = "123"
        });

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }
}
