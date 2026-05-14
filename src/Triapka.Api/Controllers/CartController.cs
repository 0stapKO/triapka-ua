using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Triapka.Application.DTOs;
using Triapka.Application.Interfaces;

namespace Triapka.Api.Controllers;

[Authorize]
public class CartController : Controller
{
    private readonly ICartService _cartService;
    private readonly IOrderService _orderService;
    private readonly ILogger<CartController> _logger;

    public CartController(
        ICartService cartService,
        IOrderService orderService,
        ILogger<CartController> logger)
    {
        _cartService = cartService;
        _orderService = orderService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        CartDto dto = await _cartService.GetCartAsync();
        return View(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Add(int productId)
    {
        await _cartService.AddToCartAsync(productId);
        _logger.LogInformation("Product {ProductId} was added to the cart", productId);
        return Json(new { success = true, message = "Товар додано в кошик" });
    }

    [HttpPost]
    public async Task<IActionResult> Remove(int itemId)
    {
        await _cartService.RemoveFromCartAsync(itemId);
        _logger.LogInformation("Item {ItemId} was removed from the cart", itemId);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> UpdateQuantity(int itemId, int newQuantity)
    {
        await _cartService.UpdateQuantityAsync(itemId, newQuantity);
        _logger.LogInformation("Item {ItemId} quantity updated to {NewQuantity}", itemId, newQuantity);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Checkout()
    {
        CartDto cart = await _cartService.GetCartAsync();
        if (cart.CartItems.Count == 0)
        {
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Cart = cart;
        return View(new CheckoutDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(CheckoutDto dto)
    {
        CartDto cart = await _cartService.GetCartAsync();
        if (!ModelState.IsValid)
        {
            ViewBag.Cart = cart;
            return View(dto);
        }

        try
        {
            var result = await _orderService.CreateOrderAsync(dto);
            _logger.LogInformation("Order {OrderId} was created successfully", result.OrderId);
            return View("CheckoutSuccess", result);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            ViewBag.Cart = cart;
            return View(dto);
        }
    }
}
