using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Triapka.Application.DTOs;
using Triapka.Application.Interfaces;

namespace Triapka.Api.Controllers;

[Authorize]
public class CartController : Controller
{
    private readonly ICartService _cartService;
    private readonly ILogger<CartController> _logger;

    public CartController(
        ICartService cartService,
        ILogger<CartController> logger)
    {
        _cartService = cartService;
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
        return RedirectToAction(nameof(Index));
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
}